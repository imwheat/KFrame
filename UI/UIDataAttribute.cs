//****************** 代码文件申明 ************************
//* 文件：UIDataAttribute                                       
//* 作者：wheat
//* 创建时间：2024/09/14 18:35:35 星期六
//* 描述：给框架使用的创建UI的Attribute
//*****************************************************
using System;
using KFrame.Utilities;

namespace KFrame.UI
{
    /// <summary>
    /// UI窗口特性设置
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class UIDataAttribute : Attribute
    {
        /// <summary>
        /// 是否在初始的时候缓存加载
        /// </summary>
        public bool IsCache;
        /// <summary>
        /// 预制体Path或AssetKey
        /// </summary>
        public string AssetPath;
        /// <summary>
        /// UI层级
        /// </summary>
        public int LayerNum;
        /// <summary>
        /// UIKey
        /// </summary>
        public string UIKey;
        
        /// <summary>
        /// UI元素数据
        /// </summary>
        /// <param name="uiKey">UIKey</param>
        /// <param name="assetPath">预制体Path或AssetKey</param>
        /// <param name="isCache">是否在初始的时候缓存加载</param>
        /// <param name="layerNum">UI层级</param>
        public UIDataAttribute(string uiKey,string assetPath, bool isCache, int layerNum)
        {
            this.IsCache = isCache;
            this.AssetPath = assetPath;
            this.LayerNum = layerNum;
            this.UIKey = uiKey;
        }

        /// <summary>
        /// UI元素数据
        /// </summary>
        /// <param name="type">UI的type直接作为UI的Key</param>
        /// <param name="assetPath">预制体Path或AssetKey</param>
        /// <param name="isCache">是否在初始的时候缓存加载</param>
        /// <param name="layerNum">UI层级</param>
        public UIDataAttribute(Type type,string assetPath,bool isCache, int layerNum) : this(type.GetNiceName(), assetPath, isCache, layerNum)
        {
        }
    }
}