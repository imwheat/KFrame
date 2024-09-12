//****************** 代码文件声明 ***********************
//* 文件：KLayoutAttribute
//* 作者：wheat
//* 创建时间：2024/04/30 19:16:03 星期二
//* 描述：一些调整Layout的Attribute的基类
//*******************************************************

using System;
using System.Diagnostics;

namespace KFrame.Attributes
{
    //只有在Unity编辑器里编译
    [Conditional("UNITY_EDITOR")]
    public abstract class KLayoutAttribute : KGUIAttribute
    {
        /// <summary>
        /// 优先度
        /// </summary>
        public int Order;
    }
}

