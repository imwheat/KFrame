//****************** 代码文件申明 ***********************
//* 文件：KGUIStyle
//* 作者：wheat
//* 创建时间：2024/05/01 10:34:07 星期三
//* 描述：提供Unity的一些标准的GUISytle
//*******************************************************

using UnityEngine;
using UnityEditor;

namespace KFrame.Editor
{
    /// <summary>
    /// Unity的一些标准的GUISytle
    /// 所有值来自 <see cref="GUIStyle"/>s, paddings, widths, heights, etc.
    /// </summary>
    public static class KGUIStyle
    {
#if UNITY_2018_3_OR_NEWER
        internal static readonly float spacing = EditorGUIUtility.standardVerticalSpacing;
#else
        internal static readonly float spacing = 2.0f;
#endif
        internal static readonly float padding = 6.0f;

#if UNITY_2018_3_OR_NEWER
        internal static readonly float lineHeight = EditorGUIUtility.singleLineHeight;
#else
            internal static readonly float lineHeight = 16.0f;
#endif
        internal static readonly float footerWidth = 60.0f;
        internal static readonly float footerButtonWidth = 25.0f;
        internal static readonly float footerButtonHeight = 13.0f;
        internal static readonly float footerMargin = 4.0f;
#if UNITY_2019_3_OR_NEWER
        internal static readonly float footerPadding = 0.0f;
#else
            internal static readonly float footerPadding = 3.0f;
#endif
        /// <summary>
        /// Property的子Property的Padding
        /// </summary>
        internal static readonly float propertyChildPadding = 15.0f;
        internal static readonly float handleWidth = 15.0f;
        internal static readonly float handleHeight = 7.0f;
        internal static readonly float dragAreaWidth = 40.0f;
        internal static readonly float sizeAreaWidth = 19.0f;
        internal static readonly float minEmptyHeight = 8.0f;

        internal static readonly Color selectionColor = new Color(0.3f, 0.47f, 0.75f);

        internal static readonly GUIContent sizePropertyContent;
        internal static readonly GUIContent iconToolbarAddContent;
        internal static readonly GUIContent iconToolbarDropContent;
        internal static readonly GUIContent iconToolbarRemoveContent;
        internal static readonly GUIContent emptyOrInvalidListContent;

        internal static readonly GUIStyle namePropertyStyle;
        internal static readonly GUIStyle foldoutLabelStyle;
        internal static readonly GUIStyle sizePropertyStyle;
        internal static readonly GUIStyle contentGroupStyle;
        internal static readonly GUIStyle footerButtonStyle;
        internal static readonly GUIStyle dragHandleButtonStyle;
        internal static readonly GUIStyle headerBackgroundStyle;
        internal static readonly GUIStyle middleBackgroundStyle;
        internal static readonly GUIStyle footerBackgroundStyle;
        internal static readonly GUIStyle elementBackgroundStyle;

        static KGUIStyle()
        {
            //获取Unity自身的一些GUIContent
            sizePropertyContent = EditorGUIUtility.TrTextContent("", "List size");
            iconToolbarAddContent = EditorGUIUtility.TrIconContent("Toolbar Plus", "Add to list");
            iconToolbarDropContent = EditorGUIUtility.TrIconContent("Toolbar Plus More", "Choose to add to list");
            iconToolbarRemoveContent = EditorGUIUtility.TrIconContent("Toolbar Minus", "Remove selection from list");
            emptyOrInvalidListContent = EditorGUIUtility.TrTextContent("Collection is Empty");

            namePropertyStyle = new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleLeft
            };
            foldoutLabelStyle = new GUIStyle(EditorStyles.foldout)
            {
                alignment = TextAnchor.MiddleLeft
            };
            sizePropertyStyle = new GUIStyle(EditorStyles.miniTextField)
            {
                alignment = TextAnchor.MiddleRight,
                //NOTE: the font size has to be adjusted in newer releases
#if UNITY_2019_3_OR_NEWER
                fixedHeight = 14.0f,
                fontSize = 10
#endif
            };
            contentGroupStyle = new GUIStyle(EditorStyles.inspectorFullWidthMargins);

            //创建一些Style提供绘制使用
            footerButtonStyle = new GUIStyle("RL FooterButton");
            dragHandleButtonStyle = new GUIStyle("RL DragHandle");
            headerBackgroundStyle = new GUIStyle("RL Header");
            middleBackgroundStyle = new GUIStyle("RL Background");
            footerBackgroundStyle = new GUIStyle("RL Footer");
            elementBackgroundStyle = new GUIStyle("RL Element");
        }
    }
}

