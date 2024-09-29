//****************** 代码文件声明 ***********************
//* 文件：KGlobalConfigAttribute
//* 作者：wheat
//* 创建时间：2024/06/03 10:55:37 星期一
//* 描述：框架全局配置的特性，用于定位什么的
//*******************************************************

using System;
using System.ComponentModel;
using KFrame.Utilities;
using UnityEngine;

namespace KFrame.Attributes
{
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
        public string AssetPath => assetPath.GetNiceDirectoryPath();
        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName;
        /// <summary>
        /// 自动创建如果找不到的话
        /// </summary>
        public bool CreateIfNotFound;
        /// <summary>
        /// 全局配置的Attribute
        /// </summary>
        /// <param name="path">Asset路径</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="createIfNotFound">如果找不到的话就自动创建</param>
        public KGlobalConfigPathAttribute(string path, string fileName = "", bool createIfNotFound = false)
        {
            assetPath = path;
            FileName = fileName;
            CreateIfNotFound = createIfNotFound;
        }
        /// <summary>
        /// 全局配置的Attribute
        /// </summary>
        /// <param name="path">Asset路径</param>
        /// <param name="type">文件类型，用于作为文件名称</param>
        /// <param name="createIfNotFound">如果找不到的话就自动创建</param>
        public KGlobalConfigPathAttribute(string path, Type type, bool createIfNotFound = false) : this(path, type.GetNiceName(), createIfNotFound)
        {
        }
        
        /// <summary>
        /// 全局配置的Attribute
        /// </summary>
        /// <param name="pathType">Asset路径类型</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="createIfNotFound">如果找不到的话就自动创建</param>
        public KGlobalConfigPathAttribute(GlobalPathType pathType, string fileName, bool createIfNotFound = false) : this(KFrameAssetsPath.GetPath(pathType), fileName, createIfNotFound)
        {
        }
        /// <summary>
        /// 全局配置的Attribute
        /// </summary>
        /// <param name="pathType">Asset路径类型</param>
        /// <param name="type">文件类型，用于作为文件名称</param>
        /// <param name="createIfNotFound">如果找不到的话就自动创建</param>
        public KGlobalConfigPathAttribute(GlobalPathType pathType, Type type, bool createIfNotFound = false) : this(KFrameAssetsPath.GetPath(pathType), type, createIfNotFound)
        {
        }
        
        
    }
}

