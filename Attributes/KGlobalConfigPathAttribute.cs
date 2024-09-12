//****************** 代码文件声明 ***********************
//* 文件：KGlobalConfigAttribute
//* 作者：wheat
//* 创建时间：2024/06/03 10:55:37 星期一
//* 描述：框架全局配置的特性，用于定位什么的
//*******************************************************

using System;
using System.ComponentModel;
using UnityEngine;

namespace KFrame.Attributes
{
    //只有在Unity编辑器里编译
    /// <summary>
    /// 框架全局配置的特性，用于定位什么的
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false , Inherited = true)]
    public class KGlobalConfigPathAttribute : Attribute
    {
        /// <summary>
        /// Asset路径
        /// </summary>
        private string assetPath;

        /// <summary>
        /// 绝对路径
        /// </summary>
        [Obsolete("一般不推荐使用绝对路径，请尽可能使用AssetPath")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string FullPath => Application.dataPath + "/" + AssetPath;

        /// <summary>
        /// Asset路径
        /// </summary>
        public string AssetPath => assetPath.Trim().TrimEnd('/', '\\').TrimStart('/', '\\')
            .Replace('\\', '/') + "/";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">Asset路径</param>
        public KGlobalConfigPathAttribute(string path)
        {
            assetPath = path;
        }
    }
}

