#if UNITY_EDITOR

using UnityEngine;
using UnityEngine.UI;

namespace KFrame.UIEditor
{
    /// <summary>
    /// 用来检测UI的组件那些开启了射线检测
    /// 开启了射线检测的组件会使用黄色边框勾出
    /// </summary>
    public class UIEditorHelp : MonoBehaviour
    {
        private static Vector3[] fourCorners = new Vector3[4];

        private void OnDrawGizmos()
        {
            foreach (MaskableGraphic graphic in GameObject.FindObjectsOfType<MaskableGraphic>())
            {
                if (graphic.raycastTarget)
                {
                    RectTransform rectTransform = graphic.transform as RectTransform;
                    rectTransform.GetWorldCorners(fourCorners);
                    Gizmos.color = Color.yellow;
                    for (int i = 0; i < 4; i++)
                    {
                        Gizmos.DrawLine(fourCorners[i], fourCorners[(i + 1) % 4]);
                    }
                }
            }
        }
    }
}

#endif