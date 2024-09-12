//****************** 代码文件申明 ************************
//* 文件：ShowValueAttribute                                       
//* 作者：Koo
//* 创建时间：2024/03/01 15:22:22 星期五
//* 功能：直接显示数值的特性
//*****************************************************

using System;
using UnityEngine;

namespace KFrame.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ShowValueAttribute : PropertyAttribute { }
}