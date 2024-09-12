//****************** 代码文件申明 ***********************
//* 文件：FrameEditorMenuItem
//* 作者：wheat
//* 创建时间：2024/04/26 19:32:06 星期五
//* 描述：框架内使用的一些编辑器窗口在这里打开
//*******************************************************
using System.Collections;
using System.Collections.Generic;
using KFrame.StarTable;
using UnityEngine;
using UnityEditor;

namespace KFrame.Editor
{
    public static class FrameEditorMenuItem
    {
        [MenuItem("框架工具/脚本编辑器")]
        public static void ShowScriptsTemplateEditorWindow()
        {
            ScriptsTemplateEditorWindow.ShowWindow();
        }
        
        [MenuItem("框架工具/星标桌面")]
        public static void ShowStarTableWindow()
        {
            StarTableWindow.ShowWindow();
        }
    }
}

