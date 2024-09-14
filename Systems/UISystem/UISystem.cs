using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using KFrame;
using KFrame.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;

namespace KFrame.Systems
{
    /// <summary>
    /// UI根节点
    /// </summary>
    public class UISystem : MonoBehaviour
    {
        private static UISystem instance;
        public static UISettingsSave Settings;

        public static void Init()
        {
            //获取实例
            instance = FrameRoot.RootTransform.GetComponentInChildren<UISystem>();
            
            //初始化参数
            activeWindowsDic = new Dictionary<string, List<UI_Base>>();
            
            //加载UI配置
            LoadUISettings();
        }

        private void OnDestroy()
        {
            //摧毁前保存设置
            SaveUISettings();
        }

        private static Dictionary<string, UIData> UIDataDic => FrameRoot.Setting.UIDataDic;
        private static Dictionary<string, List<UI_Base>> activeWindowsDic;

        [SerializeField, ShowInInspector] private UILayerBase[] uiLayers;
        [SerializeField] private RectTransform dragLayer;

        /// <summary>
        /// 拖拽层，位于所有UI的最上层
        /// </summary>
        public static RectTransform DragLayer => instance.dragLayer;
        private static UILayerBase[] UILayers => instance.uiLayers;

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
            UIDataDic.TryAdd(uiKey, data);
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
            if (UIDataDic.TryGetValue(uiKey, out UIData windowData))
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
            return UIDataDic.TryGetValue(uiKey, out data);
        }

        /// <summary>
        /// 移除UI窗口数据,
        /// </summary>
        /// <param name="uiKey"></param>
        /// <returns></returns>
        public static bool RemoveUIData(string uiKey)
        {
            return UIDataDic.Remove(uiKey);
        }

        /// <summary>
        /// 清除所有UI窗口数据
        /// </summary>
        public static void ClearUIData()
        {
            UIDataDic.Clear();
        }

        #endregion

        #region 打开UI
        
        /// <summary>
        /// 显示UI
        /// </summary>
        /// <param name="base">要显示的UI</param>
        public static void Show(UI_Base @base)
        {
            //设置父级，然后激活Gameobject
            @base.transform.SetParent(UILayers[@base.CurrentLayer].Root);
            @base.transform.SetAsLastSibling();
            @base.gameObject.SetActive(true);
            
            //放入激活的UI列表
            if (!activeWindowsDic.TryGetValue(@base.UIKey, out var list))
            {
                list = new List<UI_Base>();
                activeWindowsDic[@base.UIKey] = list;
            }
            list.Add(@base);
        }
        /// <summary>
        /// 显示UI
        /// </summary>
        /// <param name="data">UI数据</param>
        /// <param name="uiKey">UI的key</param>
        /// <param name="layer">层级</param>
        /// <returns>UI</returns>
        private static UI_Base ShowWindow(UIData data, string uiKey, int layer = -1)
        {
            //获取layer
            int layerNum = layer == -1 ? data.LayerNum : layer;
            //获取预制体
            if (data.Prefab == null)
            {
                data.Prefab = ResSystem.LoadAsset<GameObject>(data.AssetPath);
            }
            
            //尝试从对象池里面获取Gameobject，并放到对应的层级
            GameObject windowObj = PoolSystem.GetOrNewGameObject(data.Prefab, UILayers[layerNum].Root);
            //然后获取UI组件，再进行初始化
            UI_Base window = windowObj.GetComponent<UI_Base>();
            windowObj.transform.SetAsLastSibling();
            window.Init();
            window.ShowGeneralLogic(layerNum);
            //然后把window放入Active的列表里面
            if (!activeWindowsDic.TryGetValue(uiKey, out List<UI_Base> windowList))
            {
                windowList = new List<UI_Base>();
                activeWindowsDic[uiKey] = windowList;
            }
            windowList.Add(window);
            
            //更新层级然后显示
            data.LayerNum = layerNum;
            UILayers[layerNum].OnWindowShow();
            return window;
        }

        /// <summary>
        /// 显示窗口
        /// </summary>
        /// <param name="uiKey">窗口的key</param>
        /// <param name="layer">层级 -1等于不设置</param>
        public static UI_Base Show(string uiKey, int layer = -1)
        {
            if (UIDataDic.TryGetValue(uiKey, out UIData windowData))
            {
                return ShowWindow(windowData, uiKey, layer);
            }

            // 资源库中没有意味着不允许显示
            Debug.Log($"GameFrame:不存在{uiKey}的UIData");
            return null;
        }
        /// <summary>
        /// 显示窗口
        /// </summary>
        /// <typeparam name="T">要返回的窗口类型</typeparam>
        /// <param name="uiKey">窗口的Key</param>
        /// <param name="layer">层级 -1等于不设置</param>
        public static T Show<T>(string uiKey, int layer = -1) where T : UI_Base
        {
            return Show(uiKey, layer) as T;
        }
        /// <summary>
        /// 显示窗口
        /// </summary>
        /// <typeparam name="T">窗口类型</typeparam>
        /// <param name="layer">层级 -1等于不设置</param>
        public static T Show<T>(int layer = -1) where T : UI_Base
        {
            return Show(typeof(T), layer) as T;
        }

        /// <summary>
        /// 显示窗口 异步
        /// </summary>
        /// <typeparam name="T">窗口类型</typeparam>
        /// <param name="layer">层级 -1等于不设置</param>
        public static void ShowAsync<T>(Action<T> callback = null, int layer = -1) where T : UI_Base
        {
            ShowAsync(typeof(T), (window) => { callback?.Invoke((T)window); }, layer);
        }



        /// <summary>
        /// 显示窗口 异步
        /// </summary>
        /// <typeparam name="T">要返回的窗口类型</typeparam>
        /// <param name="uiKey">窗口的Key</param>
        /// <param name="layer">层级 -1等于不设置</param>
        public static T ShowAsync<T>(string uiKey, Action<T> callback = null, int layer = -1)
            where T : UI_Base
        {
            return ShowAsync(uiKey, callback, layer) as T;
        }

        /// <summary>
        /// 显示窗口
        /// </summary>
        /// <param name="type">窗口类型</param>
        /// <param name="layer">层级 -1等于不设置</param>
        public static UI_Base Show(Type type, int layer = -1)
        {
            return Show(type.GetNiceName(), layer);
        }

        /// <summary>
        /// 显示窗口
        /// </summary>
        /// <param name="type">窗口类型</param>
        /// <param name="layer">层级 -1等于不设置</param>
        public static void ShowAsync(Type type, Action<UI_Base> callback = null, int layer = -1)
        {
            ShowAsync(type.FullName, callback, layer);
        }



        /// <summary>
        /// 异步显示窗口
        /// </summary>
        /// <param name="uiKey">窗口的key</param>
        /// <param name="layer">层级 -1等于不设置</param>
        public static void ShowAsync(string uiKey, Action<UI_Base> callback = null, int layer = -1)
        {
            if (UIDataDic.TryGetValue(uiKey, out UIData windowData))
            {
                //ShowAsync(windowData, uiKey, callback, layer);
            }
            else Debug.Log($"JKFrame:不存在{uiKey}的UIWindowData"); // 资源库中没有意味着不允许显示
        }
        
        #endregion

        #region 获取与销毁UI

        /// <summary>
        /// 获取窗口(单个)
        /// </summary>
        /// <param name="uiKey">窗口Key</param>
        /// <returns>没找到会为Null</returns>
        public static UI_Base GetUI(string uiKey)
        {
            if (activeWindowsDic.TryGetValue(uiKey, out List<UI_Base> windowList) && windowList.Count > 0)
            {
                return windowList[0];
            }

            return null;
        }

        /// <summary>
        /// 获取窗口(单个)
        /// </summary>
        /// <param name="uiKey">窗口Key</param>
        /// <returns>没找到会为Null</returns>
        public static T GetUI<T>(string uiKey) where T : UI_Base
        {
            return GetUI(uiKey) as T;
        }

        /// <summary>
        /// 获取窗口(单个)
        /// </summary>
        /// <returns>没找到会为Null</returns>
        public static UI_Base GetUI(Type windowType)
        {
            return GetUI(windowType.GetNiceName());
        }
        
        /// <summary>
        /// 获取窗口(单个)
        /// </summary>
        /// <returns>没找到会为Null</returns>
        public static T GetUI<T>() where T : UI_Base
        {
            return GetUI(typeof(T)) as T;
        }
        
        /// <summary>
        /// 获取窗口(所有)
        /// </summary>
        /// <param name="uiKey">窗口Key</param>
        /// <returns>没找到会为Null</returns>
        public static List<UI_Base> GetUIAll(string uiKey)
        {
            if (activeWindowsDic.TryGetValue(uiKey, out List<UI_Base> windowList))
            {
                return windowList;
            }

            return null;
        }
        /// <summary>
        /// 获取窗口(所有)
        /// </summary>
        /// <returns>没找到会为Null</returns>
        public static List<UI_Base> GetUIAll(Type windowType)
        {
            return GetUIAll(windowType.GetNiceName());
        }
        /// <summary>
        /// 获取窗口(所有)
        /// </summary>
        /// <param name="uiKey">窗口Key</param>
        /// <returns>没找到会为Null</returns>
        public static List<T> GetUIAll<T>(string uiKey) where T : UI_Base
        {
            return GetUIAll(uiKey) as List<T>;
        }
        /// <summary>
        /// 获取窗口(所有)
        /// </summary>
        /// <returns>没找到会为Null</returns>
        public static List<T> GetUIAll<T>() where T : UI_Base
        {
            return GetUIAll(typeof(T)) as List<T>;
        }


        /// <summary>
        /// 尝试获取窗口
        /// </summary>
        /// <param name="uiKey"></param>
        public static bool TryGetUI(string uiKey, out UI_Base window)
        {
            window = GetUI(uiKey);
            return window != null;
        }

        /// <summary>
        /// 尝试获取窗口
        /// </summary>
        /// <param name="uiKey"></param>
        public static bool TryGetUI<T>(string uiKey, out T window) where T : UI_Base
        {
            window = GetUI<T>(uiKey);
            return window != null;
        }

        /// <summary>
        /// 销毁窗口(单个)
        /// </summary>
        public static void DestroyUI(string uiKey)
        {
            if (activeWindowsDic.TryGetValue(uiKey, out var windowList))
            {
                UI_Base window = windowList[0];
                windowList.RemoveAt(0);
                Destroy(window.gameObject);
            }
        }
        /// <summary>
        /// 销毁窗口(所有)
        /// </summary>
        public static void DestroyAllUI(string uiKey)
        {
            if (activeWindowsDic.TryGetValue(uiKey, out var windowList))
            {
                //遍历删除每个窗口
                for (int i = windowList.Count - 1; i >= 0; i--)
                {
                    Destroy(windowList[i].gameObject);
                }
                windowList.Clear();
            }
        }

        #endregion

        #region 关闭UI

        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="base">要关闭的窗口</param>
        public static void Close(UI_Base @base)
        {
            //从Active列表里面去除，然后推进对象池
            List<UI_Base> windowList = GetUIAll(@base.Type);
            windowList.Remove(@base);
            PoolSystem.PushGameObject(@base.gameObject);
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="uiKey">窗口key</param>
        public static void Close(string uiKey)
        {
            if (TryGetUI(uiKey, out UI_Base windowBase))
            {
                Close(windowBase);
            }
            else Debug.Log($"GameFrame:找不到激活的{uiKey}");
        }
        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <typeparam name="Type">窗口类型</typeparam>
        public static void Close(Type type)
        {
            Close(type.GetNiceName());
        }
        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <typeparam name="T">窗口类型</typeparam>
        public static void Close<T>()
        {
            Close(typeof(T));
        }

        /// <summary>
        /// 关闭全部窗口
        /// </summary>
        public static void CloseAllUI(string uiKey)
        {
            List<UI_Base> windowList = GetUIAll(uiKey);
            
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
        /// 关闭全部窗口
        /// </summary>
        public static void CloseAllUI(Type type)
        {
            CloseAllUI(type.GetNiceName());
        }
        /// <summary>
        /// 关闭全部窗口
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
            SaveSystem.SaveSetting(Settings);
        }

        /// <summary>
        /// 加载UI配置
        /// </summary>
        public static void LoadUISettings()
        {
            //加载UI配置参数，没有就新建一个
            Settings = SaveSystem.LoadSetting<UISettingsSave>();
            if (Settings == null)
            {
                Settings = new UISettingsSave();
                return;
            }
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
    }
}