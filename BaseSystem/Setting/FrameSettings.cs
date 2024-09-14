using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using KFrame.Systems;
using System;
using System.Reflection;
using KFrame.Attributes;
using System.IO;
using KFrame.Tools;

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

    /// <summary>
    /// 框架的设置SO文件
    /// </summary>
    public partial class FrameSettings : SerializedScriptableObject
    {


        #region UI

        [LabelText("UI窗口数据(无需手动填写)")]
        public Dictionary<string, UIData> UIDataDic = new Dictionary<string, UIData>();

        #endregion

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
            SetResourcesSystemType();
            SetSaveSystemType();
            InitUIWindowDataDicOnEditor();
        }

        public void InitOnEditor()
        {
            SetResourcesSystemType();
            InitUIWindowDataDicOnEditor();
        }


        /// <summary>
        /// 增加预处理指令
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
        /// 移除预处理指令
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

        private void InitUIWindowDataDicOnEditor()
        {
            UIDataDic.Clear();
            // 获取所有程序集
            System.Reflection.Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();
            Type baseType = typeof(UI_Base);
            // 遍历程序集
            foreach (System.Reflection.Assembly assembly in asms)
            {
                // 遍历程序集下的每一个类型
                Type[] types = assembly.GetTypes();
                foreach (Type type in types)
                {
                    if (baseType.IsAssignableFrom(type)
                        && !type.IsAbstract)
                    {
                        var attributes = type.GetCustomAttributes<UIDataAttribute>();
                        foreach (var attribute in attributes)
                        {
                            UIDataDic.Add(attribute.UIKey,
                                new UIData(attribute.UIKey, attribute.AssetPath, attribute.IsCache, attribute.LayerNum));
                        }
                    }
                }
            }
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
                    // 查找资源R.cs，如果有需要删除
                    string path = Application.dataPath + "KFrame/Systems/ResSystem/R.cs";
                    if (System.IO.File.Exists(path))
                        AssetDatabase.DeleteAsset("Assets/KFrame/Systems/ResSystem/R.cs");
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

#endif
    }

#if UNITY_EDITOR

    /// <summary>
    /// 框架的设置SO文件
    /// </summary>
    public partial class FrameSettings : SerializedScriptableObject
    {
        private static FrameSettings _instance;
        public static FrameSettings Instance
        {
            get
            {
                //如果_instance为空，那就自动加载
                if (_instance == null)
                {
                    _instance = AssetDatabase.LoadAssetAtPath<FrameSettings>(FrameSettingsPath);
                }

                //如果还是为空那就在Assets里面搜索
                if (_instance == null)
                {
                    Debug.LogWarning("找不到FrameSettings，请刷新路径");
                    _instance = AssetDatabase.LoadAssetAtPath<FrameSettings>(FindFrameSettings());
                }

                return _instance;
            }
        }

        /// <summary>
        /// 框架配置文件夹
        /// </summary>
        public static readonly string FrameSettingsPath = "Assets/KFrame/FrameSettings.asset";
        /// <summary>
        /// 框架文件夹路径
        /// </summary>
        public static string FrameAssetPath => Instance._frameAssetPath;
        /// <summary>
        /// 框架编辑器路径
        /// </summary>
        public static string FrameEditorPath => Instance._frameEditorPath;
        /// <summary>
        /// 脚本模版路径
        /// </summary>
        [SerializeField, DisplayName("框架文件夹路径"), TabGroup("路径设置")]
        private string _frameAssetPath = "Assets/KFrame";
        [SerializeField, DisplayName("框架编辑器路径"), TabGroup("路径设置")]
        private string _frameEditorPath = "Assets/KFrame/Editor";
        [SerializeField, DisplayName("脚本模版路径"), TabGroup("资源设置")]
        public AudioLibrary AudioLibrary;
    
        #region 框架路径设置

        /// <summary>
        /// 路径检查
        /// </summary>
        [SerializeField, Button("路径检查",30),PropertySpace(5f,5f), FoldoutGroup("路径设置")]
        public static void PathCheck()
        {
            //先获取FrameSettings
            string newPath = "";
            FrameSettings fs = AssetDatabase.LoadAssetAtPath<FrameSettings>(FrameSettingsPath);

            if(fs == null)
            {
                newPath = FindFrameSettings();
                fs = AssetDatabase.LoadAssetAtPath<FrameSettings>(newPath);
            }


            //如果路径更新了，那就更新
            if(string.IsNullOrEmpty(newPath) == false)
            {
                UpdateFrameSettingsPath(newPath);
            }
        }
        /// <summary>
        /// 查找FrameSettings
        /// </summary>
        private static string FindFrameSettings()
        {
            string[] assetGUIDs = AssetDatabase.FindAssets("t:FrameSettings", new string[] { "Assets" });

            foreach (var assetGUID in assetGUIDs)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(assetGUID);
                FrameSettings settings = AssetDatabase.LoadAssetAtPath<FrameSettings>(assetPath);

                //如果找到了
                if (settings != null)
                {
                    //赋值并更新路径
                    _instance = settings;
                    return assetPath;
                }
            }

            return "";
        }
        /// <summary>
        /// 更新框架设置路径
        /// </summary>
        /// <param name="newPath">新的路径</param>
        private static void UpdateFrameSettingsPath(string newPath)
        {
            //如果和原来的路径一样就不用更新
            if (FrameSettingsPath == newPath) return;

            //先获取一下，如果没获取到就返回
            FrameSettings asset = AssetDatabase.LoadAssetAtPath<FrameSettings>(newPath);
            if (asset == null)
            {
                Debug.Log("找不到settings");
                return;
            }

            //获取到了，那就更新
            bool findPath = false;//ScriptTools.TryGetCSFilePath<FrameSettings>(out string settingsPath);

            if(findPath == false)
            {
                Debug.Log("找不到cs文件");
            }
            return;

            /*
            //根据方法名称确定位置    
            string csText = File.ReadAllText(settingsPath);

            string startText = "public static readonly string FrameSettingsPath = ";

            //找到新的标记位置
            int startIndex = csText.IndexOf(startText, StringComparison.Ordinal) +
                             startText.Length;
            //找到末尾的';'位置
            int endIndex = startIndex + 1;
            for (int i = endIndex; i < csText.Length; i++)
            {
                if (csText[endIndex] == ';')
                {
                    break;
                }
                else
                {
                    endIndex++;
                }
            }

            //先将start和end之间的代码删除干净，然后写入新的
            string newContents = csText.Remove(startIndex, endIndex - startIndex);
            newContents = newContents.Insert(startIndex, '\"'+newPath+'\"');

            //写入然后更新
            File.WriteAllText(settingsPath, newContents);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            */
        }

        #endregion

    }

#endif

}