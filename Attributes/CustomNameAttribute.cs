using System;
using UnityEngine;

namespace KFrame.Attributes
{
    /// <summary>
    /// 可以自定义属性在Inspector中显示的名字
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
    public class DisplayNameAttribute : PropertyAttribute
    {
        public readonly string displayName;

        public DisplayNameAttribute(string displayName)
        {
            this.displayName = displayName;
        }
    }
}