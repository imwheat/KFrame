//****************** 代码文件申明 ************************
//* 文件：UILayerBase                      
//* 作者：wheat
//* 创建时间：2023年09月06日 星期三 16:05
//* 描述：对UI的层级底层
//*****************************************************

using System;
using UnityEngine;
using UnityEngine.UI;

namespace KFrame.Systems
{
    [Serializable]
    public class UILayerBase
    {
        public Transform Root;
        public bool EnableMask = true;
        public Image MaskImage;
        private int count = 0;
        public void OnWindowShow()
        {
            count += 1;
            Update();
        }
        public void OnWindowClose() 
        {
            count -= 1;
            Update();
        }
        private void Update()
        {
            if (EnableMask == false) return;
            MaskImage.raycastTarget = count != 0;
            int posIndex = Root.childCount - 2;
            MaskImage.transform.SetSiblingIndex(posIndex < 0 ? 0 : posIndex);
        }
        public void Reset()
        {
            count = 0;
            Update();
        }
    }
}