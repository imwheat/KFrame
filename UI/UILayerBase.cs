//****************** 代码文件申明 ************************
//* 文件：UILayerBase                      
//* 作者：wheat
//* 创建时间：2024/09/15 08:03:35 星期日
//* 描述：对UI的层级底层
//*****************************************************

using System;
using UnityEngine;
using UnityEngine.UI;

namespace KFrame.UI
{
    [Serializable]
    public class UILayerBase
    {
        public Transform Root;
    }
}