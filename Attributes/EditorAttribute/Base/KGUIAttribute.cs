//****************** 代码文件申明 ***********************
//* 文件：KGUIAttribute
//* 作者：wheat
//* 创建时间：2024/04/30 19:06:33 星期二
//* 描述：所有GUI重绘特性的基类
//*******************************************************

using System;
using System.Diagnostics;

namespace KFrame.Attributes
{
    //只有在Unity编辑器里编译
    [Conditional("UNITY_EDITOR")]
    public abstract class KGUIAttribute : Attribute
    {
        
    }
}

