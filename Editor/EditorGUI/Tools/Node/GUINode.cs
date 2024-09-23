//****************** 代码文件申明 ***********************
//* 文件：GUINode
//* 作者：wheat
//* 创建时间：2024/04/29 08:34:56 星期一
//* 描述：GUI节点
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
    public class GUINode
    {
        #region 字段

        /// <summary>
        /// 节点父级
        /// </summary>
        public GUINode Parent;
        /// <summary>
        /// 节点子集
        /// </summary>
        public List<GUINode> Children = new List<GUINode>();
        /// <summary>
        /// 点击节点事件
        /// </summary>
        public Action OnClick;
        /// <summary>
        /// GUI显示内容
        /// </summary>
        public GUIContent GUIContent;
        /// <summary>
        /// GUI的样式
        /// </summary>
        public GUIStyle GUIStyle;
        /// <summary>
        /// 节点Rect
        /// </summary>
        public Rect NodeRect;
        /// <summary>
        /// 正上方位置
        /// </summary>
        public Vector2 RectTop => new Vector2(NodeRect.center.x, NodeRect.yMin);
        /// <summary>
        /// 正下方位置
        /// </summary>
        public Vector2 RectBottom => new Vector2(NodeRect.center.x, NodeRect.yMax);

        #endregion

        #region 构造函数
        public GUINode(Rect rect, GUIContent content, GUIStyle style) 
        {
            this.NodeRect = rect;
            this.GUIContent = content;
            this.GUIStyle = style;
        }
        public GUINode(GUINode parent, Rect rect, GUIContent content, GUIStyle style) : this(rect, content, style)
        {
            SetParent(parent);
        }


        #endregion
        /// <summary>
        /// 绑定新的父级
        /// </summary>
        /// <param name="parent"></param>
        public void SetParent(GUINode parent)
        {
            //如果当前已经有父级了，那就先解绑
            if(Parent != null)
            {
                Parent.Children.Remove(this);
            }

            //设置父级
            Parent = parent;
            //如果不为空那就添加
            if(Parent != null)
            {
                Parent.Children.Add(this);
            }
        }

    }
}

