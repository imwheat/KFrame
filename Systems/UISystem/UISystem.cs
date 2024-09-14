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
            activeWindowsDic = new Dictionary<string, List<UI_WindowBase>>();
            
            //加载UI配置
            LoadUISettings();
        }

        private void OnDestroy()
        {
            //摧毁前保存设置
            SaveUISettings();
        }

        private static Dictionary<string, UIWindowData> UIWindowDataDic => FrameRoot.Setting.UIWindowDataDic;
        private static Dictionary<string, List<UI_WindowBase>> activeWindowsDic;

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
        /// <param name="windowKey">自定义的名称，可以是资源路径或类型名称或其他自定义</param>
        /// <param name="windowData">窗口的重要数据</param>
        public static void AddUIWindowData(string windowKey, UIWindowData windowData)
        {
            UIWindowDataDic.TryAdd(windowKey, windowData);
        }

        /// <summary>
        /// 初始化UI元素数据
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <param name="windowData">UI数据</param>
        public static void AddUIWindowData(Type type, UIWindowData windowData)
        {
            AddUIWindowData(type.GetNiceName(), windowData);
        }

        /// <summary>
        /// 初始化UI元素数据
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="windowData">UI数据</param>
        public static void AddUIWindowData<T>(UIWindowData windowData)
        {
            AddUIWindowData(typeof(T), windowData);
        }

        /// <summary>
        /// 获取UI窗口数据
        /// </summary>
        /// <param name="windowKey"></param>
        /// <returns>可能为Null</returns>
        public static UIWindowData GetUIWindowData(string windowKey)
        {
            if (UIWindowDataDic.TryGetValue(windowKey, out UIWindowData windowData))
            {
                return windowData;
            }

            return null;
        }

        public static UIWindowData GetUIWindowData(Type windowType)
        {
            return GetUIWindowData(windowType.GetNiceName());
        }

        public static UIWindowData GetUIWindowData<T>()
        {
            return GetUIWindowData(typeof(T));
        }

        /// <summary>
        /// 尝试获取UI窗口数据
        /// </summary>
        /// <param name="windowKey"></param>
        public static bool TryGetUIWindowData(string windowKey, out UIWindowData windowData)
        {
            return UIWindowDataDic.TryGetValue(windowKey, out windowData);
        }

        /// <summary>
        /// 移除UI窗口数据,
        /// </summary>
        /// <param name="windowKey"></param>
        /// <returns></returns>
        public static bool RemoveUIWindowData(string windowKey)
        {
            return UIWindowDataDic.Remove(windowKey);
        }

        /// <summary>
        /// 清除所有UI窗口数据
        /// </summary>
        public static void ClearUIWindowData()
        {
            UIWindowDataDic.Clear();
        }

        #endregion

        #region UI窗口生命周期管理

        /// <summary>
        /// 显示窗口
        /// </summary>
        /// <typeparam name="T">窗口类型</typeparam>
        /// <param name="layer">层级 -1等于不设置</param>
        public static T Show<T>(int layer = -1) where T : UI_WindowBase
        {
            return Show(typeof(T), layer) as T;
        }

        /// <summary>
        /// 显示窗口 异步
        /// </summary>
        /// <typeparam name="T">窗口类型</typeparam>
        /// <param name="layer">层级 -1等于不设置</param>
        public static void ShowAsync<T>(Action<T> callback = null, int layer = -1) where T : UI_WindowBase
        {
            ShowAsync(typeof(T), (window) => { callback?.Invoke((T)window); }, layer);
        }

        /// <summary>
        /// 显示窗口
        /// </summary>
        /// <typeparam name="T">要返回的窗口类型</typeparam>
        /// <param name="windowKey">窗口的Key</param>
        /// <param name="layer">层级 -1等于不设置</param>
        public static T Show<T>(string windowKey, int layer = -1) where T : UI_WindowBase
        {
            return Show(windowKey, layer) as T;
        }

        /// <summary>
        /// 显示窗口 异步
        /// </summary>
        /// <typeparam name="T">要返回的窗口类型</typeparam>
        /// <param name="windowKey">窗口的Key</param>
        /// <param name="layer">层级 -1等于不设置</param>
        public static T ShowAsync<T>(string windowKey, Action<T> callback = null, int layer = -1)
            where T : UI_WindowBase
        {
            return ShowAsync(windowKey, callback, layer) as T;
        }

        /// <summary>
        /// 显示窗口
        /// </summary>
        /// <param name="type">窗口类型</param>
        /// <param name="layer">层级 -1等于不设置</param>
        public static UI_WindowBase Show(Type type, int layer = -1)
        {
            return Show(type.FullName, layer);
        }

        /// <summary>
        /// 显示窗口
        /// </summary>
        /// <param name="type">窗口类型</param>
        /// <param name="layer">层级 -1等于不设置</param>
        public static void ShowAsync(Type type, Action<UI_WindowBase> callback = null, int layer = -1)
        {
            ShowAsync(type.FullName, callback, layer);
        }

        /// <summary>
        /// 显示窗口
        /// </summary>
        /// <param name="windowKey">窗口的key</param>
        /// <param name="layer">层级 -1等于不设置</param>
        public static UI_WindowBase Show(string windowKey, int layer = -1)
        {
            if (UIWindowDataDic.TryGetValue(windowKey, out UIWindowData windowData))
            {
                return ShowWindow(windowData, windowKey, layer);
            }

            // 资源库中没有意味着不允许显示
            Debug.Log($"GameFrame:不存在{windowKey}的UIWindowData");
            return null;
        }

        /// <summary>
        /// 异步显示窗口
        /// </summary>
        /// <param name="windowKey">窗口的key</param>
        /// <param name="layer">层级 -1等于不设置</param>
        public static void ShowAsync(string windowKey, Action<UI_WindowBase> callback = null, int layer = -1)
        {
            if (UIWindowDataDic.TryGetValue(windowKey, out UIWindowData windowData))
            {
                //ShowAsync(windowData, windowKey, callback, layer);
            }
            else Debug.Log($"JKFrame:不存在{windowKey}的UIWindowData"); // 资源库中没有意味着不允许显示
        }

        private static UI_WindowBase ShowWindow(UIWindowData windowData, string windowKey, int layer = -1)
        {
            //获取layer
            int layerNum = layer == -1 ? windowData.LayerNum : layer;
            //获取预制体
            if (windowData.Prefab == null)
            {
                windowData.Prefab = ResSystem.LoadAsset<GameObject>(windowData.AssetPath);
            }
            
            //尝试从对象池里面获取Gameobject，并放到对应的层级
            GameObject windowObj = PoolSystem.GetOrNewGameObject(windowData.Prefab, UILayers[layerNum].Root);
            //然后获取UI组件，再进行初始化
            UI_WindowBase window = windowObj.GetComponent<UI_WindowBase>();
            windowObj.transform.SetAsLastSibling();
            window.Init();
            window.ShowGeneralLogic(layerNum);
            //然后把window放入Active的列表里面
            if (!activeWindowsDic.TryGetValue(windowKey, out List<UI_WindowBase> windowList))
            {
                windowList = new List<UI_WindowBase>();
                activeWindowsDic[windowKey] = windowList;
            }
            windowList.Add(window);
            
            //更新层级然后显示
            windowData.LayerNum = layerNum;
            UILayers[layerNum].OnWindowShow();
            return window;
        }

        #endregion

        #region 获取与销毁窗口

        /// <summary>
        /// 获取窗口(单个)
        /// </summary>
        /// <param name="windowKey">窗口Key</param>
        /// <returns>没找到会为Null</returns>
        public static UI_WindowBase GetWindow(string windowKey)
        {
            if (activeWindowsDic.TryGetValue(windowKey, out List<UI_WindowBase> windowList))
            {
                return windowList[0];
            }

            return null;
        }

        /// <summary>
        /// 获取窗口(单个)
        /// </summary>
        /// <param name="windowKey">窗口Key</param>
        /// <returns>没找到会为Null</returns>
        public static T GetWindow<T>(string windowKey) where T : UI_WindowBase
        {
            return GetWindow(windowKey) as T;
        }

        /// <summary>
        /// 获取窗口(单个)
        /// </summary>
        /// <returns>没找到会为Null</returns>
        public static UI_WindowBase GetWindow(Type windowType)
        {
            return GetWindow(windowType.GetNiceName());
        }
        
        /// <summary>
        /// 获取窗口(单个)
        /// </summary>
        /// <returns>没找到会为Null</returns>
        public static T GetWindow<T>() where T : UI_WindowBase
        {
            return GetWindow(typeof(T)) as T;
        }
        
        /// <summary>
        /// 获取窗口(所有)
        /// </summary>
        /// <param name="windowKey">窗口Key</param>
        /// <returns>没找到会为Null</returns>
        public static List<UI_WindowBase> GetWindowAll(string windowKey)
        {
            if (activeWindowsDic.TryGetValue(windowKey, out List<UI_WindowBase> windowList))
            {
                return windowList;
            }

            return null;
        }
        /// <summary>
        /// 获取窗口(所有)
        /// </summary>
        /// <returns>没找到会为Null</returns>
        public static List<UI_WindowBase> GetWindowAll(Type windowType)
        {
            return GetWindowAll(windowType.GetNiceName());
        }
        /// <summary>
        /// 获取窗口(所有)
        /// </summary>
        /// <param name="windowKey">窗口Key</param>
        /// <returns>没找到会为Null</returns>
        public static List<T> GetWindowAll<T>(string windowKey) where T : UI_WindowBase
        {
            return GetWindowAll(windowKey) as List<T>;
        }
        /// <summary>
        /// 获取窗口(所有)
        /// </summary>
        /// <returns>没找到会为Null</returns>
        public static List<T> GetWindowAll<T>() where T : UI_WindowBase
        {
            return GetWindowAll(typeof(T)) as List<T>;
        }


        /// <summary>
        /// 尝试获取窗口
        /// </summary>
        /// <param name="windowKey"></param>
        public static bool TryGetWindow(string windowKey, out UI_WindowBase window)
        {
            window = GetWindow(windowKey);
            return window != null;
        }

        /// <summary>
        /// 尝试获取窗口
        /// </summary>
        /// <param name="windowKey"></param>
        public static bool TryGetWindow<T>(string windowKey, out T window) where T : UI_WindowBase
        {
            window = GetWindow<T>(windowKey);
            return window != null;
        }

        /// <summary>
        /// 销毁窗口(单个)
        /// </summary>
        public static void DestroyWindow(string windowKey)
        {
            if (activeWindowsDic.TryGetValue(windowKey, out var windowList))
            {
                UI_WindowBase window = windowList[0];
                windowList.RemoveAt(0);
                Destroy(window.gameObject);
            }
        }
        /// <summary>
        /// 销毁窗口(所有)
        /// </summary>
        public static void DestroyWindowAll(string windowKey)
        {
            if (activeWindowsDic.TryGetValue(windowKey, out var windowList))
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

        #region 关闭窗口

        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="windowBase">要关闭的窗口</param>
        public static void Close(UI_WindowBase windowBase)
        {
            //从Active列表里面去除，然后推进对象池
            List<UI_WindowBase> windowList = GetWindowAll(windowBase.Type);
            windowList.Remove(windowBase);
            PoolSystem.PushGameObject(windowBase.gameObject);
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="windowKey">窗口key</param>
        public static void Close(string windowKey)
        {
            if (TryGetWindow(windowKey, out UI_WindowBase windowBase))
            {
                Close(windowBase);
            }
            else Debug.Log($"GameFrame:找不到激活的{windowKey}");
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
        public static void CloseAllWindow(string windowKey)
        {
            List<UI_WindowBase> windowList = GetWindowAll(windowKey);
            
            if(windowList == null) Debug.Log($"GameFrame:找不到激活的{windowKey}");
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
        public static void CloseAllWindow(Type type)
        {
            CloseAllWindow(type.GetNiceName());
        }
        /// <summary>
        /// 关闭全部窗口
        /// </summary>
        public static void CloseAllWindow<T>()
        {
            CloseAllWindow(typeof(T));
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