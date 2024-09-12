//****************** 代码文件声明 ***********************
//* 文件：KGroupAttribute
//* 作者：wheat
//* 创建时间：2024/04/30 20:01:52 星期二
//* 描述：分组特性的基类
//*******************************************************

using System;
using System.Diagnostics;

namespace KFrame.Attributes
{
    //只有在Unity编辑器里编译
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public abstract class KGroupAttribute : KLayoutAttribute
    {
        /// <summary>
        /// 分组名称
        /// </summary>
        public string GroupName;
    }
}

