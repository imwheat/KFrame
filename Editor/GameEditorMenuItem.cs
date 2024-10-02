//****************** 代码文件申明 ***********************
//* 文件：GameEditorMenuItem
//* 作者：wheat
//* 创建时间：2024/04/28 19:32:06 星期日
//* 描述：游戏项目内使用的一些编辑器窗口在这里打开
//*******************************************************

using UnityEditor;
using KFrame.Systems;
using KFrame.UI.Editor;

namespace KFrame.Editor
{
    public static class GameEditorMenuItem
    {
        [MenuItem("项目工具/音效编辑器")]
        public static void ShowAudioEditorWindow()
        {
            AudioEditor.ShowWindow();
        }
        [MenuItem("项目工具/本地化编辑器")]
        public static void ShowLocalizationEditorWindow()
        {
            LocalizationEditorWindow.ShowWindow();
        }
    }
}

