//****************** 代码文件声明 ***********************
//* 文件：KButtonAttribute
//* 作者：wheat
//* 创建时间：2024/05/20 11:25:34 星期一
//* 描述：可以显示方法按钮
//*******************************************************

using System;
using System.Diagnostics;

namespace KFrame.Attributes
{
    //只有在Unity编辑器里编译
    [Conditional("UNITY_EDITOR"), AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class KButtonAttribute : KGUIAttribute
    {
        public int Order;
        public string Label;
        public KButtonAttribute()
        {
            Order = 0;
        }
        public KButtonAttribute(string label, int order)
        {
            Order = order;
            Label = label;
        }
    }
}

