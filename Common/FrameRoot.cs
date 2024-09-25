using KFrame.UI;
using UnityEngine;

namespace KFrame
{
    using UnityEngine.SceneManagement;
    using Utilities;
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
        }
#endif

        #endregion
    }
}