//****************** 代码文件申明 ***********************
//* 文件：KGUIHelper
//* 作者：wheat
//* 创建时间：2024/05/28 18:39:15 星期二
//* 描述：
//*******************************************************

using UnityEngine;
using UnityEditor;
using KFrame;
using KFrame.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;

namespace KFrame.Editor
{
    public static class KGUIHelper
    {
        private static readonly GUIContent tmpContent;
        private static bool repaintRequest;
        //
        // 摘要:
        //     Gets a temporary GUIContent with the specified text.
        //
        // 参数:
        //   t:
        //     The text for the GUIContent.
        //
        // 返回结果:
        //     Temporary GUIContent instance.
        public static GUIContent TempContent(string t)
        {
            tmpContent.image = null;
            tmpContent.text = t;
            tmpContent.tooltip = null;
            return tmpContent;
        }

        //
        // 摘要:
        //     Gets a temporary GUIContent with the specified text and tooltip.
        //
        // 参数:
        //   t:
        //     The text for the GUIContent.
        //
        //   tooltip:
        //     The tooltip for the GUIContent.
        //
        // 返回结果:
        //     Temporary GUIContent instance.
        public static GUIContent TempContent(string t, string tooltip)
        {
            tmpContent.image = null;
            tmpContent.text = t;
            tmpContent.tooltip = tooltip;
            return tmpContent;
        }

        //
        // 摘要:
        //     Gets a temporary GUIContent with the specified image and tooltip.
        //
        // 参数:
        //   image:
        //     The image for the GUIContent.
        //
        //   tooltip:
        //     The tooltip for the GUIContent.
        //
        // 返回结果:
        //     Temporary GUIContent instance.
        public static GUIContent TempContent(Texture image, string tooltip = null)
        {
            tmpContent.image = image;
            tmpContent.text = null;
            tmpContent.tooltip = tooltip;
            return tmpContent;
        }

        //
        // 摘要:
        //     Gets a temporary GUIContent with the specified text, image and tooltip.
        //
        // 参数:
        //   text:
        //     The text for the GUIContent.
        //
        //   image:
        //     The image for the GUIContent.
        //
        //   tooltip:
        //     The tooltip for the GUIContent.
        //
        // 返回结果:
        //     Temporary GUIContent instance.
        public static GUIContent TempContent(string text, Texture image, string tooltip = null)
        {
            tmpContent.image = image;
            tmpContent.text = text;
            tmpContent.tooltip = tooltip;
            return tmpContent;
        }

        /// <summary>
        /// 一个替代GUI.FocusControl(null)的方法
        /// <see cref="GUI.FocusControl(string)"/>
        /// 它不会从当前的GUI.Window中取走焦点。
        /// </summary>
        public static void RemoveFocusControl()
        {
            GUIUtility.hotControl = 0;
            DragAndDrop.activeControlID = 0;
            GUIUtility.keyboardControl = 0;
        }
        /// <summary>
        /// 取消当前对TextInput的Focus
        /// </summary>
        public static void RemoveFocusInput()
        {
            GUIUtility.keyboardControl = 0;
            EditorGUIUtility.editingTextField = false;
        }
        /// <summary>
        /// 请求Repaint
        /// </summary>
        public static void RequestRepaint()
        {
            repaintRequest = true;
        }
        /// <summary>
        /// 如果有Repaint请求的话，那就Repaint
        /// </summary>
        public static void RepaintIfRequested(this EditorWindow window)
        {
            if(repaintRequest && (bool)window)
            {
                window.Repaint();
                ClearRepaintRequest();
            }
        }
        /// <summary>
        /// 清空Repaint请求
        /// </summary>
        private static void ClearRepaintRequest()
        {
            repaintRequest = false;
        }

        static KGUIHelper()
        {
            tmpContent = new GUIContent("");
        }
    }
}

