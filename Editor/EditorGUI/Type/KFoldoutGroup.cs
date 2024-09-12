//****************** 代码文件声明 ***********************
//* 文件：KFoldoutGroup
//* 作者：wheat
//* 创建时间：2024/04/30 20:11:47 星期二
//* 描述：GUI绘制的FoldoutGroup
//*******************************************************

using KFrame.Attributes;
using UnityEditor;
using UnityEngine;

namespace KFrame.Editor
{
    public class KFoldoutGroup : KGUIGroup
    {

        #region 字段/属性

        /// <summary>
        /// 是否展开了
        /// </summary>
        private bool isExpanded;

        /// <summary>
        /// 如果展开来了，那么MiddleHeight就正常计算
        /// 如果没展开，MiddleHeight为0
        /// </summary>
        public override float MiddleHeight
        {
            get
            {
                if(isExpanded)
                {
                    return base.MiddleHeight;
                }
                else
                {
                    return 0;
                }
            }
        }

        #endregion

        #region 构造函数

        public KFoldoutGroup(int order, string groupName) : base(order, groupName)
        {
        }
        public KFoldoutGroup(KFoldoutGroupAttribute attribute) : this(attribute.Order, attribute.GroupName)
        {
        }

        #endregion

        #region 绘制GUI

        public override void DrawGroup()
        {
            base.DrawGroup();

            DrawHeader();
            DrawMiddle();
        }
        internal void DrawHeader()
        {
            var rect = GUILayoutUtility.GetRect(0, HeaderHeight, GUILayout.ExpandWidth(true));
            DrawHeader(rect);
        }
        internal void DrawHeader(Rect headerRect)
        {
            //在Repaint的时候再绘制背景
            if (Event.current.type == EventType.Repaint)
            {
                GUI.color = selected ? Style.selectedColor : Style.unselectedColor;

                DrawStandardHeaderBackground(headerRect);
                GUI.color = Color.white;
            }

            //设置Padding
            headerRect.xMin += Style.padding;
            headerRect.xMax -= Style.padding;
            headerRect.yMin += Style.spacing / 2;
            headerRect.yMax -= Style.spacing / 2;

            //绘制Header
            DrawStandardHeader(headerRect);
        }
        /// <summary>
        /// 绘制顶部背景
        /// </summary>
        /// <param name="rect"></param>
        public virtual void DrawStandardHeaderBackground(Rect rect)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Style.headerBackgroundStyle.Draw(rect, false, false, false, false);
            }
        }
        /// <summary>
        /// 绘制默认顶部GUI
        /// </summary>
        public virtual void DrawStandardHeader(Rect rect)
        {
            //显示名称和展开选项
            isExpanded = EditorGUI.Foldout(rect, isExpanded, GroupName, true, KGUIStyle.foldoutLabelStyle);

        }
        /// <summary>
        /// 绘制Tab的元素
        /// </summary>
        internal void DrawMiddle()
        {
            var rect = GUILayoutUtility.GetRect(0, 0, GUILayout.ExpandWidth(true));

            DrawMiddle(rect);
        }
        /// <summary>
        /// 绘制Tab的元素
        /// </summary>
        internal void DrawMiddle(Rect rect)
        {
            //如果被折叠了，那就不绘制
            if (isExpanded == false) return;
            //更新高度
            rect.height = MiddleHeight;
            //绘制背景
            DrawStandardMiddleBackground(rect);
            //更新padding
            rect.xMin += KGUIStyle.foldoutLabelStyle.padding.left;
            rect.xMax -= KGUIStyle.foldoutLabelStyle.padding.right;
            //绘制元素
            DrawStandardMiddleElements(rect);
        }
        /// <summary>
        /// 绘制中间背景
        /// </summary>
        /// <param name="rect"></param>
        public virtual void DrawStandardMiddleBackground(Rect rect)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Style.middleBackgroundStyle.Draw(rect, false, false, false, false);
            }
        }
        /// <summary>
        /// 绘制中间元素
        /// </summary>
        /// <param name="rect"></param>
        public virtual void DrawStandardMiddleElements(Rect rect)
        {
            //更新一下rect的padding
            rect.yMin += Style.padding;
            rect.yMax -= Style.padding;
            rect.xMin += Style.spacing;
            rect.xMax -= Style.spacing;

            //元素的Rect
            Rect itemElementRect = rect;

            //遍历绘制每一个Property
            for (int i = 0; i < SerializedProperties.Count; i++)
            {
                //获取当前元素的Rect高度和y偏移
                float height = GetElementHeight(i, false);
                itemElementRect.height = height;
                itemElementRect.y = rect.y + GetElementYOffset(i);

                //绘制元素
                DrawStandardElement(itemElementRect, SerializedProperties[i]);
            }
        }

        #endregion
    }
}

