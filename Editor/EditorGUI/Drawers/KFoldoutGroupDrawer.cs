//****************** 代码文件申明 ***********************
//* 文件：KFoldoutGroupDrawer
//* 作者：wheat
//* 创建时间：2024/05/09 10:17:38 星期四
//* 描述：用来绘制FoldoutGroup
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
    public class KFoldoutGroupDrawer
    {

#if UNITY_2020_1_OR_NEWER
        public virtual float HeaderHeight { get; set; } = 20.0f;
#else
        public virtual float HeaderHeight { get; set; } = 18.0f;
#endif
        /// <summary>
        /// 所拥有的FoldoutGroup
        /// </summary>
        private readonly KFoldoutGroup[] groups;

        public KFoldoutGroupDrawer(List<KFoldoutGroup> tabs)
        {
            this.groups = new KFoldoutGroup[tabs.Count];
            //先将group按照优先度排序，然后赋值
            tabs.Sort((a, b) => b.Order - a.Order);
            for (int i = 0; i < tabs.Count; i++)
            {
                this.groups[i] = tabs[i];
            }
        }

        #region 绘制相关

        /// <summary>
        /// 获取FoldoutGroup高度
        /// </summary>
        /// <returns></returns>
        public float GetGroupHeight()
        {
            //计算总高度，为header高度加空格的数量，加上每个foldout的高度
            float totalHeight = (HeaderHeight + KGUIStyle.spacing) * groups.Length;
            foreach (var group in groups)
            {
                totalHeight += group.MiddleHeight;
            }
            return totalHeight;
        }
        /// <summary>
        /// 创建FoldoutGroup
        /// </summary>
        public void DoFoldoutGroup()
        {
            Rect rect = GUILayoutUtility.GetRect(0, GetGroupHeight(), GUILayout.ExpandWidth(true));
            DoFoldoutGroup(rect);
        }
        /// <summary>
        /// 创建FoldoutGroup
        /// </summary>
        /// <param name="rect">指定区间</param>
        public void DoFoldoutGroup(Rect rect)
        {
            //绘制FoldoutGroup
            Draw(rect);
        }
        /// <summary>
        /// 绘制FoldoutGroup
        /// </summary>
        /// <param name="rect">指定区域的Rect</param>
        private void Draw(Rect rect)
        {
            //遍历绘制每个foldoutGroup
            for (int i = 0;i < groups.Length;i++)
            {
                DrawHeader(groups[i], ref rect);
                DrawMiddle(groups[i], ref rect);
                rect.y += KGUIStyle.spacing;
            }
        }
        /// <summary>
        /// 绘制顶部GUI
        /// </summary>
        private void DrawHeader(KFoldoutGroup group ,ref Rect rect)
        {
            rect.height = HeaderHeight;
            group.DrawHeader(rect);
            rect.y += rect.height;
        }
        /// <summary>
        /// 绘制内容GUI
        /// </summary>
        /// <param name="rect"></param>
        private void DrawMiddle(KFoldoutGroup group, ref Rect rect)
        {
            rect.height = group.MiddleHeight;
            group.DrawMiddle(rect);
            rect.y += rect.height;
        }


        #endregion

    }
}

