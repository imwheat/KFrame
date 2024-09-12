using UnityEngine;

namespace KFrame
{
    using UnityEngine.SceneManagement;
    using KFrame.Tools;
    using KFrame.Systems;
    


#if UNITY_EDITOR
    using UnityEditor;

    [InitializeOnLoad]
#endif

    [DefaultExecutionOrder(-50)]
    public class FrameRoot : MonoBehaviour
    {
        private FrameRoot() { }

        private static FrameRoot instance;

        public static FrameRoot Instance => instance;

        public static Transform RootTransform { get; private set; }


        public static FrameSettings Setting
        {
            get => instance.frameSetting;
        }

        // 框架层面的配置文件
        [SerializeField] private FrameSettings frameSetting;


        public GameObject eventSystem;

        private void Awake()
        {
            // 防止Editor下的Instance已经存在，并且是自身
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            RootTransform = transform;
            DontDestroyOnLoad(gameObject);
            Init();
#if UNITY_EDITOR
            //如果是UI测试场景
            if (SceneManager.GetActiveScene().name == "UIFrameTest")
            {
                if (EditorApplication.isPlaying)
                {
                    //执行一次初始化
                    UI_WindowBase[] window = instance.transform.GetComponentsInChildren<UI_WindowBase>();
                    foreach (UI_WindowBase win in window)
                    {
                        win.Init();
                    }
                }
            }

#endif
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            InitSystems();
            InitManagers();
            "Root初始化完毕".Log();
        }


        private void InitSystems()
        {
            PoolSystem.Init();
            EventBroadCastSystem.Init();
            MonoSystem.Init();
            SaveSystem.Init();
            GameSaveSystem.Init();
            LocalizationSystem.Init();
            InputMgrSystem.Init();
            UISystem.Init();
            AudioSystem.Init();
        }

        #region GamePlayer

        private void InitManagers()
        {

        }

        #endregion


        #region Editor

#if UNITY_EDITOR
        // 编辑器专属事件系统
        public static EventModule EditorEventModule;

        static FrameRoot()
        {
            EditorEventModule = new EventModule();
            EditorApplication.update += () => { InitForEditor(); };
        }

        [InitializeOnLoadMethod]
        public static void InitForEditor()
        {
            // 当前是否要进行播放或准备播放中
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<FrameRoot>();
                if (instance == null) return;


                instance.frameSetting.InitOnEditor();

                //如果是UI测试场景
                if (SceneManager.GetActiveScene().name == "UIFrameTest")
                {
                    return;
                }

                // 场景的所有窗口都进行一次Show
                UI_WindowBase[] window = instance.transform.GetComponentsInChildren<UI_WindowBase>();
                foreach (UI_WindowBase win in window)
                {
                    win.ShowGeneralLogic(-1);
                }
            }
        }
#endif

        #endregion
    }
}