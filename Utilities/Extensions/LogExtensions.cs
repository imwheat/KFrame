//****************** 代码文件申明 ************************
//* 文件：LogTool                                       
//* 作者：Koo
//* 创建时间：2024/02/03 15:32:34 星期六
//* 功能：Log的静态拓展
//*****************************************************

using System;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace KFrame.Utilities
{
    public static class LogExtensions
    {
        public static void Log(this string logContent)
        {
            
            
            Debug.Log(logContent);
        }

        public static void Log(this int logContent)
        {
            Debug.Log(logContent);
        }

        public static void Log(this float logContent)
        {
            Debug.Log(logContent);
        }

        public static void Log<T>(this T logObject)
        {
            Debug.Log(logObject);
        }
    }

    #if UNITY_EDITOR

    /// <summary>
    /// 重定位 封装过后的Log信息所在的代码文本位置
    /// 参考: https://zhuanlan.zhihu.com/p/92291084
    /// </summary>
    public static class OpenAssetLogLine
    {
        /// <summary>
        /// 用来跟踪是否强制启用的编辑器脚本 防止在处理Asset打开回调期间无限递归
        /// </summary>
        private static bool m_hasForceMono = false;

        // 处理asset打开的callback函数
        // 用于打开Unity中某个资源的回调属性 此属性添加给静态方法后 系统会在Unity打开资源的时候调用此方法
        [UnityEditor.Callbacks.OnOpenAssetAttribute(-1)]
        static bool OnOpenAsset(int instance, int line)
        {
            if (m_hasForceMono) return false;

            // 自定义函数，用来获取log中的stacktrace，定义在后面。
            string stack_trace = GetStackTrace();
            // 通过stacktrace来定位是否是自定义的log，log中有LogTool/LogTool.cs
            if (!string.IsNullOrEmpty(stack_trace) && stack_trace.Contains("Log_Tool/LogTool.cs"))
            {
                // 正则匹配at xxx，在第几行
                Match matches = Regex.Match(stack_trace, @"\(at (.+)\)", RegexOptions.IgnoreCase);
                string pathline = "";
                while (matches.Success)
                {
                    pathline = matches.Groups[1].Value;

                    // 找到不是我们自定义log文件的那行，重新整理文件路径，手动打开
                    if (!pathline.Contains("Log_Tool/LogTool.cs") && !string.IsNullOrEmpty(pathline))
                    {
                        int split_index = pathline.LastIndexOf(":");
                        string path = pathline.Substring(0, split_index);
                        line = Convert.ToInt32(pathline.Substring(split_index + 1));
                        //这个操作可能会触发其他资源打开又再次进入 OnOpenAsset 资源打开后再设置为false
                        m_hasForceMono = true;
                        //方式一  
                        AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path), line);
                        m_hasForceMono = false;
                        //方式二
                        //string fullpath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("Assets"));
                        // fullpath = fullpath + path;
                        //  UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(fullpath.Replace('/', '\\'), line);
                        return true;
                    }

                    matches = matches.NextMatch();
                }

                return true;
            }

            return false;
        }

        static string GetStackTrace()
        {
            // 找到类UnityEditor.ConsoleWindow
            var type_console_window = typeof(EditorWindow).Assembly.GetType("UnityEditor.ConsoleWindow");
            // 找到UnityEditor.ConsoleWindow中的成员ms_ConsoleWindow
            var filedInfo =
                type_console_window.GetField("ms_ConsoleWindow", BindingFlags.Static | BindingFlags.NonPublic);
            // 获取ms_ConsoleWindow的值
            var ConsoleWindowInstance = filedInfo.GetValue(null);
            if (ConsoleWindowInstance != null)
            {
                if ((object)EditorWindow.focusedWindow == ConsoleWindowInstance)
                {
                    // 找到类UnityEditor.ConsoleWindow中的成员m_ActiveText
                    filedInfo = type_console_window.GetField("m_ActiveText",
                        BindingFlags.Instance | BindingFlags.NonPublic);
                    string activeText = filedInfo.GetValue(ConsoleWindowInstance).ToString();
                    return activeText;
                }
            }

            return null;
        }
    }
#endif
}