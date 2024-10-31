using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using KFrame;
using KFrame.Systems;
using KFrame.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;

namespace KFrame.UI
{
    /// <summary>
    /// UI根节点
    /// </summary>
    public class UISystem : MonoBehaviour
    {
        private static UISystem instance;
        public static UISettingsSave Settings;

        private static Dictionary<string, UIData> uiDataDic;
        private static Dictionary<string, List<UIBase>> activeUIsDic;

        [SerializeField, ShowInInspector] private UILayerBase[] uiLayers;
        [SerializeField] private RectTransform dragLayer;

        private static UILayerBase[] UILayers => instance.uiLayers;

        #region 初始化操作

        public static void Init()
        {
            //获取实例
            instance = FrameRoot.RootTransform.GetComponentInChildren<UISystem>();
            
            //加载UI配置
            LoadUISettings();
            
            //初始化字典
            instance.InitDic();
            
        }
        /// <summary>
        /// 初始化字典
        /// </summary>
        private void InitDic()
        {
            //新建字典
            uiDataDic = new Dictionary<string, UIData>();
            activeUIsDic = new Dictionary<string, List<UIBase>>();
            
            //注册字典
            foreach (UIData uiData in UIGlobalConfig.Instance.UIDatas)
            {
                uiDataDic[uiData.UIKey] = uiData;
                //缓存处理
                if (uiData.IsCache)
                {
                    Debug.Log(uiData.UIKey);
                    var ui = Show(uiData.UIKey);
                    ui.Close();
                }
            }
        }

        private void OnDestroy()
        {
            //摧毁前保存设置
            SaveUISettings();
        }

        #endregion

        #region 动态加载/移除窗口数据

        // UI系统的窗口数据中主要包含：预制体路径、是否缓存、当前窗口对象实例等重要信息
        // 为了方便使用，所以窗口数据必须先存放于UIWindowDataDic中，才能通过UI系统显示、关闭等

        /// <summary>
        /// 初始化UI元素数据
        /// </summary>
        /// <param name="uiKey">自定义的名称，可以是资源路径或类型名称或其他自定义</param>
        /// <param name="data">窗口的重要数据</param>
        public static void AddUIData(string uiKey, UIData data)
        {
            uiDataDic.TryAdd(uiKey, data);
        }

        /// <summary>
        /// 初始化UI元素数据
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <param name="data">UI数据</param>
        public static void AddUIData(Type type, UIData data)
        {
            AddUIData(type.GetNiceName(), data);
        }

        /// <summary>
        /// 初始化UI元素数据
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="data">UI数据</param>
        public static void AddUIData<T>(UIData data)
        {
            AddUIData(typeof(T), data);
        }

        /// <summary>
        /// 获取UI窗口数据
        /// </summary>
        /// <param name="uiKey"></param>
        /// <returns>可能为Null</returns>
        public static UIData GetUIData(string uiKey)
        {
            if (uiDataDic.TryGetValue(uiKey, out UIData windowData))
            {
                return windowData;
            }

            return null;
        }

        public static UIData GetUIData(Type Type)
        {
            return GetUIData(Type.GetNiceName());
        }

        public static UIData GetUIData<T>()
        {
            return GetUIData(typeof(T));
        }

        /// <summary>
        /// 尝试获取UI窗口数据
        /// </summary>
        /// <param name="uiKey"></param>
        public static bool TryGetUIData(string uiKey, out UIData data)
        {
            return uiDataDic.TryGetValue(uiKey, out data);
        }

        /// <summary>
        /// 移除UI窗口数据,
        /// </summary>
        /// <param name="uiKey"></param>
        /// <returns></returns>
        public static bool RemoveUIData(string uiKey)
        {
            return uiDataDic.Remove(uiKey);
        }

        /// <summary>
        /// 清除所有UI窗口数据
        /// </summary>
        public static void ClearUIData()
        {
            uiDataDic.Clear();
        }

        #endregion

        #region 打开UI
        
        /// <summary>
        /// 显示UI
        /// </summary>
        /// <param name="ui">要显示的UI</param>
        public static void Show(UIBase ui)
        {
            //设置父级，然后激活GameObject
            ui.transform.SetParent(UILayers[ui.CurrentLayer].Root);
            ui.transform.localScale = Vector3.one;
            ui.transform.SetAsLastSibling();
            ui.gameObject.SetActive(true);
            
            //放入激活的UI列表
            if (!activeUIsDic.TryGetValue(ui.UIKey, out var list))
            {
                list = new List<UIBase>();
                activeUIsDic[ui.UIKey] = list;
            }
            list.Add(ui);
        }
        /// <summary>
        /// 显示UI
        /// </summary>
        /// <param name="data">UI数据</param>
        /// <returns>UI</returns>
        private static UIBase ShowUI(UIData data)
        {
            //获取layer和UIKey
            int layerNum = data.LayerNum;
            string uiKey = data.UIKey;
            
            //获取预制体
            if (data.Prefab == null)
            {
                data.Prefab = ResSystem.LoadAsset<GameObject>(data.AssetPath);
                if (data.Prefab == null)
                {
                    throw new Exception($"错误：不存在路径为{data.AssetPath}的UI预制体");
                }
            }
            
            //尝试从对象池里面获取GameObject，并放到对应的层级
            GameObject uiObj = PoolSystem.GetOrNewGameObject(data.Prefab, UILayers[layerNum].Root);
            uiObj.transform.localScale = Vector3.one;
            //然后获取UI组件，再进行初始化
            UIBase ui = uiObj.GetComponent<UIBase>();
            uiObj.transform.SetAsLastSibling();
            ui.Init(data);
            
            //然后把window放入Active的列表里面
            if (!activeUIsDic.TryGetValue(uiKey, out List<UIBase> uiList))
            {
                uiList = new List<UIBase>();
                activeUIsDic[uiKey] = uiList;
            }
            uiList.Add(ui);
            
            //更新层级然后显示
            return ui;
        }

        /// <summary>
        /// 显示窗口
        /// </summary>
        /// <param name="uiKey">窗口的key</param>
        public static UIBase Show(string uiKey)
        {
            //获取数据，然后显示UI
            if (uiDataDic.TryGetValue(uiKey, out UIData uiData))
            {
                return ShowUI(uiData);
            }

            // 资源库中没有意味着不允许显示
            Debug.Log($"GameFrame:不存在{uiKey}的UIData");
            return null;
        }
        /// <summary>
        /// 显示窗口
        /// </summary>
        /// <param name="type">窗口类型</param>
        public static UIBase Show(Type type)
        {
            return Show(type.GetNiceName());
        }
        /// <summary>
        /// 显示窗口
        /// </summary>
        /// <typeparam name="T">要返回的窗口类型</typeparam>
        /// <param name="uiKey">窗口的Key</param>
        public static T Show<T>(string uiKey) where T : UIBase
        {
            return Show(uiKey) as T;
        }
        /// <summary>
        /// 显示窗口
        /// </summary>
        /// <typeparam name="T">窗口类型</typeparam>
        /// <param name="layer">层级 -1等于不设置</param>
        public static T Show<T>() where T : UIBase
        {
            return Show(typeof(T)) as T;
        }
        
        #endregion

        #region 获取与销毁UI

        /// <summary>
        /// 获取窗口(单个)
        /// </summary>
        /// <param name="uiKey">窗口Key</param>
        /// <returns>没找到会为Null</returns>
        public static UIBase GetUI(string uiKey)
        {
            if (activeUIsDic.TryGetValue(uiKey, out List<UIBase> activeUIList) && activeUIList.Count > 0)
            {
                return activeUIList[0];
            }

            return null;
        }

        /// <summary>
        /// 获取窗口(单个)
        /// </summary>
        /// <param name="uiKey">窗口Key</param>
        /// <returns>没找到会为Null</returns>
        public static T GetUI<T>(string uiKey) where T : UIBase
        {
            return GetUI(uiKey) as T;
        }

        /// <summary>
        /// 获取窗口(单个)
        /// </summary>
        /// <returns>没找到会为Null</returns>
        public static UIBase GetUI(Type uiType)
        {
            return GetUI(uiType.GetNiceName());
        }
        
        /// <summary>
        /// 获取窗口(单个)
        /// </summary>
        /// <returns>没找到会为Null</returns>
        public static T GetUI<T>() where T : UIBase
        {
            return GetUI(typeof(T)) as T;
        }
        
        /// <summary>
        /// 获取窗口(所有)
        /// </summary>
        /// <param name="uiKey">窗口Key</param>
        /// <returns>没找到会为Null</returns>
        public static List<UIBase> GetUIAll(string uiKey)
        {
            if (activeUIsDic.TryGetValue(uiKey, out List<UIBase> activeUIList))
            {
                return activeUIList;
            }

            return null;
        }
        /// <summary>
        /// 获取窗口(所有)
        /// </summary>
        /// <returns>没找到会为Null</returns>
        public static List<UIBase> GetUIAll(Type uiType)
        {
            return GetUIAll(uiType.GetNiceName());
        }
        /// <summary>
        /// 获取窗口(所有)
        /// </summary>
        /// <param name="uiKey">窗口Key</param>
        /// <returns>没找到会为Null</returns>
        public static List<T> GetUIAll<T>(string uiKey) where T : UIBase
        {
            return GetUIAll(uiKey) as List<T>;
        }
        /// <summary>
        /// 获取窗口(所有)
        /// </summary>
        /// <returns>没找到会为Null</returns>
        public static List<T> GetUIAll<T>() where T : UIBase
        {
            return GetUIAll(typeof(T)) as List<T>;
        }


        /// <summary>
        /// 尝试获取窗口
        /// </summary>
        /// <param name="uiKey"></param>
        public static bool TryGetUI(string uiKey, out UIBase ui)
        {
            ui = GetUI(uiKey);
            return ui != null;
        }

        /// <summary>
        /// 尝试获取窗口
        /// </summary>
        /// <param name="uiKey"></param>
        public static bool TryGetUI<T>(string uiKey, out T ui) where T : UIBase
        {
            ui = GetUI<T>(uiKey);
            return ui != null;
        }

        /// <summary>
        /// 销毁窗口(单个)
        /// </summary>
        public static void DestroyUI(string uiKey)
        {
            if (activeUIsDic.TryGetValue(uiKey, out var activeUIList))
            {
                UIBase window = activeUIList[0];
                activeUIList.RemoveAt(0);
                Destroy(window.gameObject);
            }
        }
        /// <summary>
        /// 销毁窗口(所有)
        /// </summary>
        public static void DestroyAllUI(string uiKey)
        {
            if (activeUIsDic.TryGetValue(uiKey, out var activeUIList))
            {
                //遍历删除每个窗口
                for (int i = activeUIList.Count - 1; i >= 0; i--)
                {
                    Destroy(activeUIList[i].gameObject);
                }
                activeUIList.Clear();
            }
        }

        #endregion

        #region 关闭UI

        /// <summary>
        /// 关闭UI
        /// </summary>
        /// <param name="ui">要关闭的UI</param>
        public static void Close(UIBase ui)
        {
            //从Active列表里面去除，然后推进对象池
            List<UIBase> windowList = GetUIAll(ui.Type);
            windowList.Remove(ui);
            PoolSystem.PushGameObject(ui.gameObject);
        }

        /// <summary>
        /// 关闭UI
        /// </summary>
        /// <param name="uiKey">UIkey</param>
        public static void Close(string uiKey)
        {
            if (TryGetUI(uiKey, out UIBase ui))
            {
                Close(ui);
            }
            else Debug.Log($"GameFrame:找不到激活的{uiKey}");
        }
        /// <summary>
        /// 关闭UI
        /// </summary>
        /// <typeparam name="type">UI类型</typeparam>
        public static void Close(Type type)
        {
            Close(type.GetNiceName());
        }
        /// <summary>
        /// 关闭UI
        /// </summary>
        /// <typeparam name="T">UI类型</typeparam>
        public static void Close<T>() where T : UIBase
        {
            Close(typeof(T));
        }

        /// <summary>
        /// 关闭全部UI
        /// </summary>
        public static void CloseAllUI(string uiKey)
        {
            List<UIBase> windowList = GetUIAll(uiKey);
            
            if(windowList == null) Debug.Log($"GameFrame:找不到激活的{uiKey}");
            else
            {
                for (int i = windowList.Count - 1; i >= 0; i--)
                {
                    Close(windowList[i]);
                }
            }
        }
        /// <summary>
        /// 关闭全部UI
        /// </summary>
        public static void CloseAllUI(Type type)
        {
            CloseAllUI(type.GetNiceName());
        }
        /// <summary>
        /// 关闭全部UI
        /// </summary>
        public static void CloseAllUI<T>()
        {
            CloseAllUI(typeof(T));
        }
        
        #endregion

        #region 工具

        private static List<RaycastResult> raycastResultList = new List<RaycastResult>();

        /// <summary>
        /// 保存UI配置
        /// </summary>
        public static void SaveUISettings()
        {
            SaveSystem.SaveGlobalSave(Settings);
        }

        /// <summary>
        /// 加载UI配置
        /// </summary>
        public static void LoadUISettings()
        {
            //加载UI配置参数，没有就新建一个
            Settings = SaveSystem.LoadGlobalSave<UISettingsSave>();
            if (Settings == null)
            {
                Settings = new UISettingsSave();
                return;
            }
            
            AudioSetHelper.LoadAudioSettings();
            GameSetHelper.LoadGameSettings();
            GraphicSetHelper.LoadGraphicSettings();
        }

        /// <summary>
        /// 检查鼠标是否在UI上,会屏蔽名称为Mask的物体
        /// </summary>
        public static bool CheckMouseOnUI()
        {
#if ENABLE_LEGACY_INPUT_MANAGER
            return CheckPositionOnUI(Input.mousePosition);
#else
            return CheckPositoinOnUI(UnityEngine.InputSystem.Mouse.current.position.ReadValue());
#endif
        }

        private static UnityEngine.EventSystems.EventSystem eventSystem;
        private static PointerEventData pointerEventData;

        /// <summary>
        /// 检查一个坐标是否在UI上,会屏蔽名称为Mask的物体
        /// </summary>
        public static bool CheckPositionOnUI(Vector2 pos)
        {
            if (eventSystem == null)
            {
                eventSystem = UnityEngine.EventSystems.EventSystem.current;
                pointerEventData = new PointerEventData(eventSystem);
            }

            pointerEventData.position = pos;
            // 射线去检测有没有除了Mask以外的任何UI物体
            eventSystem.RaycastAll(pointerEventData, raycastResultList);
            for (int i = 0; i < raycastResultList.Count; i++)
            {
                // 是UI，同时还不是Mask作用的物体
                if (raycastResultList[i].gameObject.name != "Mask")
                {
                    raycastResultList.Clear();
                    return true;
                }
            }

            raycastResultList.Clear();
            return false;
        }



        #endregion

        #region 本地化相关

        /// <summary>
        /// 获取UI开关的UI本地化文本key
        /// </summary>
        /// <param name="value">开关状态</param>
        /// <returns>UI本地化文本key</returns>
        public static string GetOnOffUIKey(bool value)
        {
            return value ? "ui_On" : "ui_Off";
        }

        #endregion
    }
}