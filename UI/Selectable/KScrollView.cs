//****************** 代码文件声明 ***********************
//* 文件：KScrollView
//* 作者：wheat
//* 创建时间：2024/10/05 08:39:29 星期六
//* 描述：可以移动的窗口
//*******************************************************


using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KFrame.UI
{
    [AddComponentMenu("KUI/ScrollView", 55)]
    public class KScrollView : MonoBehaviour
    {
        #region UI配置

        /// <summary>
        /// 垂直方向的滚动条
        /// </summary>
        [SerializeField]
        private KScrollbar verticalBar;
        /// <summary>
        /// 内容物的父级
        /// </summary>
        private RectTransform contentRect;
        /// <summary>
        /// 最小值的时候的位置
        /// </summary>
        [SerializeField]
        private Vector2 minPos;
        /// <summary>
        /// 最大值的时候的位置
        /// </summary>
        [SerializeField]
        private Vector2 maxPos;
        /// <summary>
        /// 内容选项
        /// </summary>
        [SerializeField]
        private List<KSelectable> selections;

        #endregion

        #region UI逻辑
        /// <summary>
        /// 选项的下标字典
        /// </summary>
        private Dictionary<KSelectable, int> selectionIndexDic;

        #endregion

        #region UI操作

        private void Awake()
        {
            //初始化配置
            selectionIndexDic = new Dictionary<KSelectable, int>();
            for (var i = 0; i < selections.Count; i++)
            {
                selectionIndexDic[selections[i]] = i;
                selections[i].OnSelectEvent.AddListener(OnSelectUI);
            }
            verticalBar.onValueChanged.AddListener(OnScrollValueChanged);
        }

        private void OnDestroy()
        {
            selectionIndexDic.Clear();
            foreach (var selection in selections)
            {
                selection.OnSelectEvent.RemoveListener(OnSelectUI);
            }
        }

        #endregion

        #region UI事件
        
        /// <summary>
        /// 当滚轮值发生了变化
        /// </summary>
        /// <param name="v">新的值</param>
        private void OnScrollValueChanged(float v)
        {
            contentRect.anchoredPosition = Vector2.Lerp(minPos, maxPos, v);
        }
        /// <summary>
        /// 当内容物UI被选择了
        /// </summary>
        /// <param name="ui">被选择的UI</param>
        private void OnSelectUI(KSelectable ui)
        {
            if (selectionIndexDic.TryGetValue(ui, out var index))
            {
                verticalBar.Value = (float)index / (selections.Count - 1);
            }
        }

        #endregion
    }
}
