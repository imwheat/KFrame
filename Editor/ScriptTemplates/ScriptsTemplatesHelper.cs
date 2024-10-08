using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace KFrame.Editor.ScriptTemplates
{
    /// <summary>
    /// 帮忙创建脚本模版
    /// </summary>
    public static class ScriptsTemplatesHelper
    {
        /// <summary>
        /// 通过脚本模版创建脚本
        /// </summary>
        /// <param name="scriptName">新的脚本名</param>
        /// <param name="templateName">模版名称</param>
        public static void CreateMyScript(string scriptName, string templateName)
        {
            //获取路径
            string locationPath = GetSelectedPathOrFallback();
            //创建脚本
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
                ScriptableObject.CreateInstance<MyDoCreateScriptAsset_Mono>(),
                locationPath + "/" + scriptName, null, ScriptTemplateConfig.GetTemplateFullPath(templateName));
        }
        /// <summary>
        /// 获取当前创建脚本选择的路径
        /// </summary>
        /// <returns></returns>
        public static string GetSelectedPathOrFallback()
        {
            string path = "Assets";
            foreach (Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    path = Path.GetDirectoryName(path);
                    break;
                }
            }

            return path;
        }

        internal class MyDoCreateScriptAsset_Mono : EndNameEditAction
        {
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                Object o = CreateScriptAssetFromTemplate(pathName, resourceFile);
                ProjectWindowUtil.ShowCreatedAsset(o);
            }

            internal static UnityEngine.Object CreateScriptAssetFromTemplate(string pathName, string resourceFile)
            {
                string fullPath = Path.GetFullPath(pathName);
                StreamReader streamReader = new StreamReader(resourceFile);
                string text = streamReader.ReadToEnd();
                streamReader.Close();
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pathName);
                //替换文件名
                text = Regex.Replace(text, ScriptTemplateConfig.ScriptName, fileNameWithoutExtension);
                bool encoderShouldEmitUTF8Identifier = true;
                bool throwOnInvalidBytes = false;
                UTF8Encoding encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier, throwOnInvalidBytes);
                bool append = false;
                StreamWriter streamWriter = new StreamWriter(fullPath, append, encoding);
                streamWriter.Write(text);
                streamWriter.Close();
                AssetDatabase.ImportAsset(pathName);
                return AssetDatabase.LoadAssetAtPath(pathName, typeof(UnityEngine.Object));
            }
        }
    }
}
