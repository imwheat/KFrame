//****************** 代码文件申明 ************************
//* 文件：UIData                                       
//* 作者：wheat
//* 创建时间：2024/09/14 18:35:35 星期六
//* 描述：存储UI的数据
//*****************************************************
using System;
using KFrame.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;
namespace KFrame.UI
{
    /// <summary>
    /// UI元素数据
    /// </summary>
    [System.Serializable]
    public class UIData
    {
        /// <summary>
        /// 是否在初始的时候缓存加载
        /// </summary>
        [LabelText("是否在初始的时候缓存加载")] 
        public bool IsCache;
        /// <summary>
        /// 预制体Path或AssetKey
        /// </summary>
        [LabelText("预制体Path或AssetKey")] 
        public string AssetPath;
        /// <summary>
        /// UI层级
        /// </summary>
        [LabelText("UI层级")] 
        public int LayerNum;
        /// <summary>
        /// UIKey
        /// </summary>
        [LabelText("UIKey")] 
        public string UIKey;
        /// <summary>
        /// 这个窗口对象的预制体
        /// </summary>
        [LabelText("窗口预制体"), NonSerialized]
        public GameObject Prefab;
        
        /// <summary>
        /// UI元素数据
        /// </summary>
        /// <param name="uiKey">UIKey</param>
        /// <param name="assetPath">预制体Path或AssetKey</param>
        /// <param name="isCache">是否在初始的时候缓存加载</param>
        /// <param name="layerNum">UI层级</param>
        public UIData(string uiKey,string assetPath, bool isCache, int layerNum)
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
        public UIData(Type type,string assetPath,bool isCache, int layerNum) : this(type.GetNiceName(), assetPath, isCache, layerNum)
        {
        }
    }

}