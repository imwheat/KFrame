using KFrame.Utilities;
using UnityEditor;

namespace KFrame.Editor.ScriptTemplates
{
    public class ScriptsTemplatesMenuItem
    {
        /// <summary>
        /// 模版路径
        /// </summary>
        public static string TemplatesPath => FileExtensions.ConvertAssetPathToSystemPath(ScriptTemplateConfig.FrameScriptTemplatesPath);

        #region 代码生成开始标识




        
        //代码生成开始标识Mono

        [MenuItem("Assets/Frame C#/Mono", false, 0)]
        public static void CreatMono()
        {
            ScriptsTemplatesHelper.CreateMyScript("Mono.cs",
                "Mono.cs");
        }

        //代码生成结束标识Mono

        
        //代码生成开始标识UIBasePanel

        [MenuItem("Assets/Frame C#/UIBasePanel", false, 0)]
        public static void CreatUIBasePanel()
        {
            ScriptsTemplatesHelper.CreateMyScript("UIBasePanel.cs","UIBasePanel.cs");
        }

        //代码生成结束标识UIBasePanel

        
        //代码生成开始标识FrameEditor

        [MenuItem("Assets/Frame C#/Editor/FrameEditor", false, 0)]
        public static void CreatFrameEditor()
        {
            ScriptsTemplatesHelper.CreateMyScript("FrameEditor.cs", "FrameEditor.cs");
        }

        //代码生成结束标识FrameEditor

        
        //代码生成开始标识FrameEditorWindow

        [MenuItem("Assets/Frame C#/Editor/FrameEditorWindow", false, 0)]
        public static void CreatFrameEditorWindowEditor()
        {
            ScriptsTemplatesHelper.CreateMyScript("FrameEditorWindow.cs", "FrameEditorWindow.cs");
        }

        //代码生成结束标识FrameEditorWindow

        
        //代码生成开始标识GUIAttributeEditor

        [MenuItem("Assets/Frame C#/Editor/GUIAttributeEditor", false, 0)]
        public static void CreatGUIAttributeEditor()
        {
            ScriptsTemplatesHelper.CreateMyScript("GUIAttributeEditor.cs", "GUIAttributeEditor.cs");
        }

        //代码生成结束标识GUIAttributeEditor

        
        //代码生成开始标识FrameClass

        [MenuItem("Assets/Frame C#/Frame/FrameClass", false, 0)]
        public static void CreatFrameClass()
        {
            ScriptsTemplatesHelper.CreateMyScript("FrameClass.cs", "FrameClass.cs");
        }

        //代码生成结束标识FrameClass


        //代码生成开始标识UISystem

        [MenuItem("Assets/Frame C#/SystemMono", false, 0)]
        public static void CreatUISystem()
        {
            ScriptsTemplatesHelper.CreateMyScript("SystemMono.cs", "SystemMono.cs");
        }

        //代码生成结束标识UISystem

        
        //代码生成开始标识KUI

        [MenuItem("Assets/Frame C#/KUI", false, 0)]
        public static void CreatKSelectable()
        {
            ScriptsTemplatesHelper.CreateMyScript("KUI.cs", "KUI.cs");
        }

        //代码生成结束标识KUI

		#endregion 代码生成结束标识
    }
}