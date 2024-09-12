//****************** 代码文件申明 ***********************
//* 文件：KTabGroupDrawer
//* 作者：wheat
//* 创建时间：2024/05/02 14:06:39 星期四
//* 描述：用来绘制TabGroup
//*******************************************************

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace KFrame.Editor
{
    public class KTabGroupDrawer
    {

#if UNITY_2020_1_OR_NEWER
        public virtual float HeaderHeight { get; set; } = 20.0f;
#else
        public virtual float HeaderHeight { get; set; } = 18.0f;
#endif
        /// <summary>
        /// 所拥有的TabGroup
        /// </summary>
        private readonly KTabGroup[] tabs;
        /// <summary>
        /// 每个tab的Header的Rect
        /// </summary>
        private readonly Rect[] tabHeaderRect;
        /// <summary>
        /// 总的顶部Rect区域
        /// </summary>
        private Rect headerRect;
        /// <summary>
        /// 当前选择的TabGroup的下标
        /// </summary>
        private int curSelectIndex;
        /// <summary>
        /// 当前选择的TabGroup
        /// </summary>
        private KTabGroup curSelectTab => tabs[curSelectIndex];
        public KTabGroupDrawer(List<KTabGroup> tabs)
        {
            this.tabs = new KTabGroup[tabs.Count];
            this.tabHeaderRect = new Rect[tabs.Count];
            //先将group按照优先度排序，然后赋值
            tabs.Sort((a, b)=> b.Order-a.Order);
            for (int i = 0; i < tabs.Count; i++)
            {
                this.tabs[i] = tabs[i];
            }

            //默认选择第0个tab
            tabs[0].Select();
            curSelectIndex = 0;
        }

        #region 绘制相关

        /// <summary>
        /// 获取TabGroup高度
        /// </summary>
        /// <returns></returns>
        public float GetTabGroupHeight()
        {
            return HeaderHeight + curSelectTab.MiddleHeight + KGUIStyle.spacing;
        }
        /// <summary>
        /// 创建TabGroup
        /// </summary>
        public void DoTabGroup()
        {
            Rect rect = GUILayoutUtility.GetRect(0, GetTabGroupHeight(), GUILayout.ExpandWidth(true));
            DoTabGroup(rect);
        }
        /// <summary>
        /// 创建TabGroup
        /// </summary>
        /// <param name="rect">指定区间</param>
        public void DoTabGroup(Rect rect)
        {
            //绘制TabGroup
            Draw(rect);

            //处理鼠标操作
            DoMouseEvent();
        }
        /// <summary>
        /// 绘制TabGroup
        /// </summary>
        /// <param name="rect">指定区域的Rect</param>
        private void Draw(Rect rect)
        {
            var headerRect = rect;
            headerRect.height = HeaderHeight;
            DrawHeader(ref headerRect);
            rect.y = headerRect.y;
            DrawMiddle(rect);
        }
        /// <summary>
        /// 绘制顶部GUI
        /// </summary>
        private void DrawHeader(ref Rect rect)
        {
            //更新顶部Rect总区域
            headerRect = rect;
            //先获取tab数量
            int count = tabs.Length;
            //计算每个tab的宽度
            float width = rect.width;
            float tabWidth = width / count;
            rect.width = tabWidth;

            for (int i = 0; i < tabs.Length; i++)
            {
                tabHeaderRect[i] = rect;
                tabs[i].DrawHeader(rect);
                rect.x += tabWidth;
            }

            rect.y += rect.height;
        }
        /// <summary>
        /// 绘制内容GUI
        /// </summary>
        /// <param name="rect"></param>
        private void DrawMiddle(Rect rect)
        {
            curSelectTab.DrawMiddle(rect);
        }


        #endregion

        #region 操作相关方法

        /// <summary>
        /// 获取鼠标点击到的Group的下标
        /// </summary>
        /// <param name="mousePosition">鼠标位置</param>
        /// <returns></returns>
        public int GetCoveredGroupIndex(Vector2 mousePosition)
        {
            //如果总区域里面没有，那就返回-1
            if (headerRect.Contains(mousePosition) == false)
            {
                return -1;
            }

            //遍历每一个区域进行判断
            for (int i = 0; i < tabHeaderRect.Length; i++)
            {
                if (tabHeaderRect[i].Contains(mousePosition))
                {
                    return i;
                }
            }

            return -1;
        }
        /// <summary>
        /// 处理鼠标的一些操作Event
        /// </summary>
        public void DoMouseEvent()
        {
            var currentEvent = Event.current;

            switch (currentEvent.type)
            {
                case EventType.MouseDown:
                    {
                        //如果点的不是左键的话就不用管
                        if (currentEvent.button != 0)
                        {
                            break;
                        }

                        //获取点击到的鼠标瞄准的
                        int index = GetCoveredGroupIndex(currentEvent.mousePosition);

                        //如果没有点击到或者和当前选择的一样，那就返回
                        if (index == -1 || index == curSelectIndex)
                        {
                            break;
                        }

                        //选择新的Tab
                        curSelectTab.Deselect();
                        tabs[index].Select();
                        curSelectIndex = index;

                        //点在TabGroup的Header上面那就关闭正在编辑的输入栏
                        EditorGUIUtility.editingTextField = false;

                        currentEvent.Use();
                    }
                    break;

                case EventType.MouseDrag:
                    {

                        //currentEvent.Use();
                    }
                    break;

                case EventType.MouseUp:
                    {


                        //currentEvent.Use();
                    }
                    break;
            }

        }

        #endregion

    }
}

