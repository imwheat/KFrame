//****************** 代码文件申明 ***********************
//* 文件：LocalizedTextKeyAttribute
//* 作者：wheat
//* 创建时间：2024/10/19 10:42:52 星期六
//* 描述：本地化文本key的Attribute
//*******************************************************

using System;
using UnityEngine;

namespace KFrame.UI
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class LocalizedTextKeyAttribute : PropertyAttribute
    {
        
    }
}

