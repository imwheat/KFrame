using KFrame.Attributes;
using UnityEngine;
using Sirenix.OdinInspector;
using KFrame.Systems;
using KFrame.Utilities;
using KFrame.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace KFrame
{
    /// <summary>
    /// 资源系统类型
    /// </summary>
    public enum ResourcesSystemType
    {
        Resources,
        Addressables
    }

    /// <summary>
    /// 存档系统类型
    /// </summary>
    public enum SaveSystemType
    {
        Binary,
        Json
    }

    #if UNITY_EDITOR
    [InitializeOnLoad]
    #endif
    /// <summary>
    /// 框架的设置SO文件
    /// </summary>
    [KGlobalConfigPath(GlobalPathType.Frame, typeof(FrameSettings), true)]
    public class FrameSettings : GlobalConfigBase<FrameSettings>
    {

        /// <summary>
        /// 是否使用2D物理
        /// </summary>
        public bool Use2DPhysics = true;
        
        #region 资源管理

        [SerializeField, LabelText("资源管理方式")]
#if UNITY_EDITOR
        [OnValueChanged(nameof(SetResourcesSystemType))]
#endif

        private ResourcesSystemType resourcesSystemType = ResourcesSystemType.Resources;

        #endregion

        #region 存档

#if UNITY_EDITOR
        [LabelText("存档方式"), Tooltip("修改类型会导致之前的存档丢失"), OnValueChanged(nameof(SetSaveSystemType))]
#endif
        public SaveSystemType SaveSystemType = SaveSystemType.Binary;

		[LabelText("二进制序列化器，仅用于二进制方式存档")]
		public IBinarySerializer binarySerializer;


		#endregion

		public static int DefaultMaxGOPCount = -1; //默认初始化的对象池上限


#if UNITY_EDITOR
        
        public static void CreateFrameSetting()
        {
            FrameSettings setting = ScriptableObject.CreateInstance<FrameSettings>();
            string[] selectedGUIDs = Selection.assetGUIDs;
            string assetPath = "Assets/KFrame";
            foreach (string guid in selectedGUIDs)
            {
                assetPath = AssetDatabase.GUIDToAssetPath(guid);
            }

            string fileName = "FrameSetting.asset";

            if (!AssetDatabase.IsValidFolder(assetPath))
                AssetDatabase.CreateFolder("Assets", "CustomHierarchyConfigs");

            AssetDatabase.CreateAsset(setting, $"{assetPath}/{fileName}");
            AssetDatabase.SaveAssets();

            Debug.Log($"FrameSetting配置创建在: {assetPath}/{fileName}");
        }

        [Button("重置设定")]
        public void Reset()
        {
            SetSaveSystemType();
            InitOnEditor();
        }

        [InitializeOnLoadMethod]
        private static void InitEditor()
        {
            //如果在运行的话那就返回
            if(Application.isPlaying) return;
            Instance.InitOnEditor();
        }
        public void InitOnEditor()
        {
            SetResourcesSystemType();
            UIGlobalConfig.Instance.InitUIDataOnEditor();
            KFramePathSearch.Instance.InitGlobalConfig();
        }


        /// <summary>
        /// 修改资源管理系统的类型
        /// </summary>
        public void SetResourcesSystemType()
        {
            switch (resourcesSystemType)
            {
                case ResourcesSystemType.Resources:
                    RemoveScriptCompilationSymbol("ENABLE_ADDRESSABLES");
                    break;
                case ResourcesSystemType.Addressables:
                    AddScriptCompilationSymbol("ENABLE_ADDRESSABLES");
                    break;
            }
        }

        /// <summary>
        /// 修改存档系统的类型
        /// </summary>
        private void SetSaveSystemType()
        {
            // 清空存档
            SaveSystem.DeleteAll();
        }
        
        /// <summary>
        /// 增加编辑器宏
        /// </summary>
        public static void AddScriptCompilationSymbol(string name)
        {
            BuildTargetGroup buildTargetGroup = UnityEditor.EditorUserBuildSettings.selectedBuildTargetGroup;
            string group = UnityEditor.PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            if (!group.Contains(name))
            {
                UnityEditor.PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, group + ";" + name);
            }
        }
        /// <summary>
        /// 移除编辑器宏
        /// </summary>
        public static void RemoveScriptCompilationSymbol(string name)
        {
            BuildTargetGroup buildTargetGroup = UnityEditor.EditorUserBuildSettings.selectedBuildTargetGroup;
            string group = UnityEditor.PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            if (group.Contains(name))
            {
                UnityEditor.PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup,
                    group.Replace(";" + name, string.Empty));
            }
        }

#endif
    }


}