//****************** 代码文件申明 ***********************
//* 文件：KGUIGroup
//* 作者：wheat
//* 创建时间：2024/04/30 20:07:11 星期二
//* 描述：GUIGroup的基类
//*******************************************************

using KFrame.Attributes;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace KFrame.Editor
{
    public class KGUIGroup
    {
        #region GUIStyle相关参数
        /// <summary>
        /// TabGroup的一些标准Style
        /// 所有值来自 <see cref="GUIStyle"/>s, paddings, widths, heights, etc.
        /// </summary>
        protected static class Style
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
#if UNITY_2019_3_OR_NEWER
            internal static readonly float footerPadding = 0.0f;
#else
            internal static readonly float footerPadding = 3.0f;
#endif
            internal static readonly float handleWidth = 15.0f;
            internal static readonly float handleHeight = 7.0f;
            internal static readonly float dragAreaWidth = 40.0f;
            internal static readonly float sizeAreaWidth = 19.0f;
            internal static readonly float minEmptyHeight = 8.0f;

            internal static readonly Color selectionColor = new Color(0.3f, 0.47f, 0.75f);
            internal static readonly Color unselectedColor = new Color(0.6f, 0.6f, 0.6f);
            internal static readonly Color selectedColor = Color.white;

            internal static readonly GUIStyle namePropertyStyle;
            internal static readonly GUIStyle foldoutLabelStyle;
            internal static readonly GUIStyle contentGroupStyle;
            internal static readonly GUIStyle footerButtonStyle;
            internal static readonly GUIStyle dragHandleButtonStyle;
            internal static readonly GUIStyle headerBackgroundStyle;
            internal static readonly GUIStyle middleBackgroundStyle;
            internal static readonly GUIStyle elementBackgroundStyle;

            static Style()
            {
                namePropertyStyle = new GUIStyle(EditorStyles.label)
                {
                    alignment = TextAnchor.MiddleCenter
                };
                foldoutLabelStyle = new GUIStyle(EditorStyles.foldout)
                {
                    alignment = TextAnchor.MiddleLeft
                };
                contentGroupStyle = new GUIStyle(EditorStyles.inspectorFullWidthMargins);

                //创建一些Style提供绘制使用
                footerButtonStyle = new GUIStyle("RL FooterButton");
                dragHandleButtonStyle = new GUIStyle("RL DragHandle");
                headerBackgroundStyle = new GUIStyle("RL Header");
                middleBackgroundStyle = new GUIStyle("RL Background");
                elementBackgroundStyle = new GUIStyle("RL Element");
            }
        }
#if UNITY_2020_1_OR_NEWER
        public virtual float HeaderHeight { get; set; } = 20.0f;
#else
        public virtual float HeaderHeight { get; set; } = 18.0f;
#endif
        /// <summary>
        /// 每个元素之间的空隙
        /// </summary>
        public virtual float ElementSpacing { get; set; } = 5.0f;
        public int Count =>SerializedProperties.Count;
        /// <summary>
        /// Group中部元素高度
        /// </summary>
        public virtual float MiddleHeight
        {
            get
            {
                var arraySize = Count;
                var middleHeight = Style.padding * 2;
                if (arraySize != 0)
                {
                    middleHeight += GetElementYOffset(arraySize - 1) + GetRowHeight(arraySize - 1);
                }

                return middleHeight;
            }
        }

        #endregion

        #region Group相关

        /// <summary>
        /// 优先度
        /// </summary>
        public int Order;
        /// <summary>
        /// 组名
        /// </summary>
        public string GroupName;
        /// <summary>
        /// 被选择了
        /// </summary>
        protected bool selected;
        /// <summary>
        /// 这个Group所包含的SP
        /// </summary>
        public List<KSerializedProperty> SerializedProperties;

        #endregion

        #region 构造

        public KGUIGroup(int order, string groupName)
        {
            Order = order;
            GroupName = groupName;
            selected = false;
            SerializedProperties = new List<KSerializedProperty>();
        }

        #endregion

        #region Group方法

        /// <summary>
        /// 选择Group
        /// </summary>
        public virtual void Select()
        {
            selected = true;
        }
        /// <summary>
        /// 取消选择Group
        /// </summary>
        public virtual void Deselect()
        {
            selected = false;
        }
        /// <summary>
        /// 绘制Group
        /// </summary>
        public virtual void DrawGroup()
        {

        }

        #endregion

        #region GUI绘制相关方法
        /// <summary>
        /// 获取第i个元素的Y偏移
        /// </summary>
        /// <param name="index">下标</param>
        /// <returns>Rect的Y坐标偏移</returns>
        protected float GetElementYOffset(int index)
        {
            return GetElementYOffset(index, -1);
        }
        /// <summary>
        /// 获得第i个元素的Y偏移
        /// </summary>
        /// <param name="index">下标</param>
        /// <param name="skipIndex">无视的选项</param>
        protected float GetElementYOffset(int index, int skipIndex)
        {
            //到这一个元素的偏移量等于到这一个元素之前所有元素的高度之和
            var offset = 0.0f;
            for (var i = 0; i < index; i++)
            {
                if (i != skipIndex)
                {
                    offset += GetRowHeight(i);
                }
            }

            return offset;
        }
        /// <summary>
        /// 获得某一行元素的高度
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected float GetRowHeight(int index)
        {
            return GetElementHeight(index) + ElementSpacing;
        }
        /// <summary>
        /// 获取元素的高度
        /// </summary>
        /// <param name="index">下标</param>
        /// <param name="includeChildren">是否包括Children</param>
        /// <returns>元素的高度</returns>
        protected float GetElementHeight(int index, bool includeChildren = true)
        {
            return SerializedProperties[index].GetPropertyHeight(includeChildren);
        }
        /// <summary>
        /// 绘制元素
        /// </summary>
        public virtual void DrawStandardElement(Rect rect, KSerializedProperty property)
        {
            property.DrawProperty(rect);
        }
        #endregion

    }
}

