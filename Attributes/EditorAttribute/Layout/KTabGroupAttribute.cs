//****************** 代码文件声明 ***********************
//* 文件：KTabGroupAttribute
//* 作者：wheat
//* 创建时间：2024/04/30 19:18:43 星期二
//* 描述：分页特性，添加这个特性的字段、属性可以被放在同一分页
//*******************************************************

using System;
using System.Diagnostics;

namespace KFrame.Attributes
{
    //只有在Unity编辑器里编译
    [Conditional("UNITY_EDITOR")]
    public class KTabGroupAttribute : KGroupAttribute
    {
        public KTabGroupAttribute(string groupName, int order =0) 
        { 
            GroupName = groupName;
            Order = order;
        }
    }
}

