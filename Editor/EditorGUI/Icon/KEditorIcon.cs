//****************** 代码文件申明 ***********************
//* 文件：KEditorIcon
//* 作者：wheat
//* 创建时间：2024/05/27 19:04:22 星期一
//* 描述：编辑器绘制使用的GUI的图片
//*******************************************************

using KFrame.Utilities;
using UnityEngine;

namespace KFrame.Editor
{
    /// <summary>
    /// 编辑器绘制使用的GUI的图片
    /// </summary>
    public abstract class KEditorIcon
    {
        private GUIContent inactiveGUIContent;

        private GUIContent highlightedGUIContent;

        private GUIContent activeGUIContent;


        /// <summary>
        /// 未加工的GUI材质纹理
        /// </summary>
        public abstract Texture2D Raw { get; }

        /// <summary>
        /// 高亮显示时的GUI材质纹理
        /// </summary>
        public abstract Texture Highlighted { get; }

        /// <summary>
        /// 激活时的GUI材质纹理纹理
        /// </summary>
        public abstract Texture Active { get; }

        /// <summary>
        /// 没激活时的GUI材质纹理
        /// </summary>
        public abstract Texture Inactive { get; }


        /// <summary>
        /// 激活状态下的GUI
        /// </summary>
        public GUIContent ActiveGUIContent
        {
            get
            {
                if (activeGUIContent == null || activeGUIContent.image == null)
                {
                    activeGUIContent = new GUIContent(Inactive);
                }

                return activeGUIContent;
            }
        }

        /// <summary>
        /// 没激活的状态下的GUI
        /// </summary>
        public GUIContent InactiveGUIContent
        {
            get
            {
                if (inactiveGUIContent == null || inactiveGUIContent.image == null)
                {
                    inactiveGUIContent = new GUIContent(Inactive);
                }

                return inactiveGUIContent;
            }
        }

        /// <summary>
        /// 高亮显示的GUI
        /// </summary>
        public GUIContent HighlightedGUIContent
        {
            get
            {
                if (highlightedGUIContent == null || highlightedGUIContent.image == null)
                {
                    highlightedGUIContent = new GUIContent(Inactive);
                }

                return highlightedGUIContent;
            }
        }

        /// <summary>
        /// 绘制图片在Rect里
        /// </summary>
        public void Draw(Rect rect)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Texture texture;
                if (!GUI.enabled)
                {
                    texture = Inactive;
                }
                else if (rect.Contains(Event.current.mousePosition))
                {
                    texture = Highlighted;
                }
                else
                {
                    texture = Active;
                }

                Draw(rect, texture);
            }
        }


        /// <summary>
        /// 绘制图片在Rect里，并调整size
        /// </summary>
        public void Draw(Rect rect, float drawSize)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Texture texture;
                if (!GUI.enabled)
                {
                    texture = Inactive;
                }
                else if (rect.Contains(Event.current.mousePosition))
                {
                    texture = Highlighted;
                }
                else
                {
                    texture = Active;
                }

                rect = rect.AlignCenter(drawSize, drawSize);
                Draw(rect, texture);
            }
        }

        /// <summary>
        /// 绘制图片在Rect里
        /// </summary>
        public void Draw(Rect rect, Texture texture)
        {
            if (Event.current.type == EventType.Repaint)
            {
                rect.x = (int)rect.x;
                rect.y = (int)rect.y;
                rect.width = (int)rect.width;
                rect.height = (int)rect.height;
                GUI.DrawTexture(rect, texture);
            }
        }
    }
}

