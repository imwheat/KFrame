//****************** 代码文件申明 ***********************
//* 文件：ScriptTemplateConfig
//* 作者：wheat
//* 创建时间：2024/06/05 09:37:31 星期三
//* 描述：脚本模版配置
//*******************************************************

using UnityEngine;
using UnityEditor;
using KFrame.Utilities;
using System.Collections.Generic;
using KFrame.Attributes;

namespace KFrame.Editor
{
    [KGlobalConfigPath(GlobalPathType.Editor, typeof(ScriptTemplateConfig), true)]
    public class ScriptTemplateConfig : GlobalConfigBase<ScriptTemplateConfig>
    {
        /// <summary>
        /// 模版中的脚本名
        /// </summary>
        public const string SCRIPTNAME = "#SCRIPTNAME#";
        /// <summary>
        /// 脚本模版路径
        /// </summary>
        public static string FrameScriptTemplatesPath => KFrameAssetsPath.FrameEditorAssetsPath + "ScriptTemplates/Templates/";
        /// <summary>
        /// 脚本模版列表
        /// </summary>
        public static List<TextAsset> Templates
        {
            get
            {
                return Instance._templates;
            }
            private set
            {
                Instance._templates = value;
            }
        }
        /// <summary>
        /// 模版字典
        /// </summary>
        private static Dictionary<string, TextAsset> _templateDic;
        /// <summary>
        /// 模版字典
        /// </summary>
        public static Dictionary<string, TextAsset> TemplateDic
        {
            get
            {
                if (_templateDic == null)
                {
                    InitTemplateDic();
                }

                return _templateDic;
            }
            set
            {
                _templateDic = value;
            }
        }
        [SerializeField]
        private List<TextAsset> _templates;

        #region 脚本模版

        /// <summary>
        /// 添加模版
        /// </summary>
        /// <param name="path"></param>
        public static void AddTemplate(string path)
        {
            AddTemplate(AssetDatabase.LoadAssetAtPath<TextAsset>(path));
        }
        /// <summary>
        /// 添加模版
        /// </summary>
        /// <param name="template"></param>
        public static void AddTemplate(TextAsset template)
        {
            //不能添加空的
            if (template == null) return;

            //防空
            if (Templates == null)
            {
                Templates = new List<TextAsset>();
            }

            Templates.Add(template);
            TemplateDic[template.name] = template;

            //保存
            EditorUtility.SetDirty(Instance);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }
        /// <summary>
        /// 获取模版路径
        /// </summary>
        /// <param name="templateName">模版名称</param>
        /// <returns>如果找不到的话返回""</returns>
        public static string GetTemplatePath(string templateName)
        {
            //如果字典里面有的话那就从字典里面获取
            if (TemplateDic.ContainsKey(templateName))
            {
                return FileExtensions.ConvertAssetPathToSystemPath(AssetDatabase.GetAssetPath(TemplateDic[templateName]));
            }

            Debug.LogWarning($"找不到模版，模版名称:{templateName}");
            return "";
        }
        /// <summary>
        /// 初始化模版字典
        /// </summary>
        private static void InitTemplateDic()
        {
            //初始化字典
            TemplateDic = new Dictionary<string, TextAsset>();
            //遍历赋值
            for (int i = 0; i < Templates.Count; i++)
            {
                if (Templates[i] != null)
                {
                    TemplateDic[Templates[i].name] = Templates[i];
                }
            }
        }

        #endregion

    }
}

