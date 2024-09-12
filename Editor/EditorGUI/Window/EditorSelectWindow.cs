//****************** 代码文件申明 ************************
//* 文件：EditorSelectWindow                                       
//* 作者：wheat
//* 创建时间：2024/09/05 07:21:45 星期四
//* 描述：用于选择的窗口
//*****************************************************

using UnityEngine;
using System;
using UnityEditor;

namespace KFrame.Editor
{
    public class EditorSelectWindow : EditorWindow
    {
        #region GUI显示相关

                
        /// <summary>
        /// 标准化统一当前编辑器的绘制Style
        /// </summary>
        private static class MStyle
        {
#if UNITY_2018_3_OR_NEWER
            internal static readonly float spacing = EditorGUIUtility.standardVerticalSpacing;
#else
            internal static readonly float spacing = 2.0f;
#endif
            internal static readonly float btnWidth = 80f;
            internal static readonly float btnSpace= 10f;

            internal static readonly GUIStyle messageStyle = new GUIStyle("label")
            {
                alignment = TextAnchor.UpperLeft,
                
            };

        }

        private GUIContent okGUI;
        private GUIContent cancelGUI;
        private GUIContent textGUI;
        private Action<ConfirmType> confirmAction;
        
        #endregion
        /// <summary>
        /// 确认类型
        /// </summary>
        public enum ConfirmType
        {
            /// <summary>
            /// 无
            /// </summary>
            None = 0,
            /// <summary>
            /// 取消
            /// </summary>
            Cancel = 1,
            /// <summary>
            /// 确认
            /// </summary>
            Confirm = 2,
        }

        #region 初始化相关

        /// <summary>
        ///   <para>显示选择窗口</para>
        /// </summary>
        /// <param name="title">窗口标题</param>
        /// <param name="message">显示信息</param>
        /// <param name="ok">确认的文本</param>
        /// <param name="cancel">取消的文本</param>
        /// <param name="callback">回调函数</param>
        /// <returns>
        ///   <para>点击确认就返回Confirm，点击取消返回Cancel，直接关掉返回None</para>
        /// </returns>
        public static void DisplayDialog(string title, string message, string ok, string cancel, Action<ConfirmType> callback)
        {
            EditorSelectWindow window = EditorWindow.GetWindow<EditorSelectWindow>();
            window.Init(title, message, ok, cancel, callback);
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="title">窗口标题</param>
        /// <param name="message">显示信息</param>
        /// <param name="ok">确认的文本</param>
        /// <param name="cancel">取消的文本</param>
        /// <param name="callback">回调函数</param>
        public void Init(string title, string message, string ok, string cancel, Action<ConfirmType> callback)
        {
            titleContent.text = title;
            textGUI = new GUIContent(message);
            okGUI = new GUIContent(ok);
            cancelGUI = new GUIContent(cancel);
            confirmAction = callback;
        }

        private void OnDisable()
        {
            confirmAction?.Invoke(ConfirmType.None);
        }

        #endregion

        #region 绘制GUI

        private void OnGUI()
        {
            float textWidth = position.width - (MStyle.btnSpace * 3f + MStyle.btnWidth * 2f);
            
            GUI.Label(new Rect(0, 0, position.width, position.height), textGUI, MStyle.messageStyle);
            if (GUI.Button(new Rect(textWidth + MStyle.btnSpace, position.height - 35f, MStyle.btnWidth, 20f),okGUI))
            {
                confirmAction?.Invoke(ConfirmType.Confirm);
                confirmAction = null;
                Close();
            }
            if (GUI.Button(new Rect(textWidth + MStyle.btnSpace + MStyle.btnWidth + MStyle.btnSpace, position.height - 35f, MStyle.btnWidth, 20f),cancelGUI))
            {
                confirmAction?.Invoke(ConfirmType.Cancel);
                confirmAction = null;
                Close();
            }
        }

        #endregion

    }
}
