//****************** 代码文件声明 ***********************
//* 文件：KFoldoutGroupAttribute
//* 作者：wheat
//* 创建时间：2024/04/30 20:04:48 星期二
//* 描述：折叠页的特性
//*******************************************************

using System;
using System.Diagnostics;

namespace KFrame.Attributes
{
    //只有在Unity编辑器里编译
    [Conditional("UNITY_EDITOR")]
    public class KFoldoutGroupAttribute : KGroupAttribute
    {
        public KFoldoutGroupAttribute(string groupName, int order = 0)
        {
            GroupName = groupName;
            Order = order;
        }
    }
}

