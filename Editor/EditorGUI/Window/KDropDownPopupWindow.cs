//****************** 代码文件申明 ***********************
//* 文件：KDropDownPopupWindow
//* 作者：wheat
//* 创建时间：2024/05/28 14:51:56 星期二
//* 描述：
//*******************************************************

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KFrame.Editor
{
    public class KDropDownPopupWindow : KPopupWindowContent
    {
        /// <summary>
        /// 上一个搜索的文本
        /// </summary>
        protected string prevSearchText;
        /// <summary>
        /// 搜索文本
        /// </summary>
        protected string searchText;
        /// <summary>
        /// gui的值
        /// </summary>
        protected string[] guiValues;
        /// <summary>
        /// gui的Content
        /// </summary>
        protected string[] guiContents;
        /// <summary>
        /// gui的激活情况
        /// </summary>
        protected bool[] guiActives;
        /// <summary>
        /// 每个elment高度
        /// </summary>
        protected virtual float elementHeight => 18f;
        /// <summary>
        /// 每个elment之间的空隙
        /// </summary>
        protected virtual float padding => 3f;
        /// <summary>
        /// 元素的总高度
        /// </summary>
        protected float elementsHeight;
        /// <summary>
        /// 滚动条位置
        /// </summary>
        protected float scrollPosition;
        /// <summary>
        /// 回调函数
        /// </summary>
        protected Action<string> callback;
        public KDropDownPopupWindow()
        {

        }
        public KDropDownPopupWindow(IReadOnlyCollection<string> strings, Action<string> callback) 
        {
            InitGUI(strings, callback);
        }
        protected virtual void InitGUI(IReadOnlyCollection<string> strings, Action<string> callback)
        {
            guiValues = strings.ToArray<string>();
            guiContents = new string[guiValues.Length];
            for (int i = 0; i < guiValues.Length; i++)
            {
                guiContents[i] = guiValues[i];
            }
            guiActives = new bool[guiValues.Length];
            this.callback = callback;
            RefreshGUI();
        }
        public override void OnGUI(Rect rect)
        {
            base.OnGUI(rect);

            rect.xMin += padding;
            rect.xMax -= padding;

            //绘制搜索框
            Rect searchRect = rect;
            searchRect.height = elementHeight;
            rect.yMin += elementHeight + padding;

            //查询是否需要重绘
            if (prevSearchText != searchText)
            {
                prevSearchText = searchText;
                RefreshGUI();
            }

            //绘制scrollview
            float newScrollPosition = KEditorGUI.DrawScrollVertical(ref rect, scrollPosition, elementsHeight);
            if(newScrollPosition != scrollPosition)
            {
                scrollPosition = newScrollPosition;
                editorWindow.Repaint();
            }
            rect.y -= scrollPosition;

            //逐个绘制每个元素
            for (int i = 0; i < guiValues.Length; i++)
            {
                //如果没有激活那就不画出来
                if (guiActives[i] == false) continue;

                //计算rect
                Rect curRect = rect;
                curRect.height = elementHeight;

                //显示选项
                if(GUI.Button(curRect, guiContents[i], KGUIStyles.Popup))
                {
                    //点击以后调用回调函数，然后关闭窗口
                    callback?.Invoke(guiValues[i]);
                    editorWindow.Close();
                }

                //更新rect
                rect.yMin += elementHeight + padding;
            }

            //绘制搜索栏
            searchText = KEditorGUI.SearchTextField(searchRect, searchText);

        }
        /// <summary>
        /// 刷新GUI的显示情况
        /// </summary>
        protected virtual void RefreshGUI()
        {
            //如果为空，那就全都显示
            if(string.IsNullOrEmpty(prevSearchText))
            {
                Array.Fill<bool>(guiActives, true);
                elementsHeight = guiActives.Length * (elementHeight + padding);
            }
            //不为空
            else
            {
                //更新元素高度
                elementsHeight = 0;

                //逐个遍历看看有没有包含
                for (int i = 0; i < guiValues.Length; i++)
                {
                    bool value = guiValues[i].Contains(prevSearchText);
                    guiActives[i] = value;

                    //如果显示的话计算高度
                    if(value)
                    {
                        elementsHeight += elementHeight + padding;
                    }
                }
            }

            //滚动条位置清零
            scrollPosition = 0f;
        }
    }
}

