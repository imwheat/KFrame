//****************** 代码文件申明 ***********************
//* 文件：KEditorWindow
//* 作者：wheat
//* 创建时间：2024/05/28 14:28:35 星期二
//* 描述：框架编辑器窗口的基类
//*******************************************************

using UnityEditor;

namespace KFrame.Editor
{
    /// <summary>
    /// 框架编辑器窗口的基类
    /// </summary>
    public class KEditorWindow : EditorWindow
    {
        protected virtual void OnGUI()
        {
            //如果有请求Repaint那就Repaint
            this.RepaintIfRequested();
        }
    }
}

