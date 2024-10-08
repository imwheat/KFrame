//****************** 代码文件申明 ***********************
//* 文件：TagAndLayerDatas
//* 作者：wheat
//* 创建时间：2024/09/26 09:28:20 星期四
//* 描述：存储Layer、Tag、SortingLayer的一些数据
//*******************************************************

using System.Collections.Generic;
using KFrame.Attributes;
using KFrame.Utilities;
using UnityEngine;

namespace KFrame.Editor.Tools
{
    [KGlobalConfigPath(GlobalPathType.Editor, typeof(TagAndLayerDatas), true)]
    internal class TagAndLayerDatas : GlobalConfigBase<TagAndLayerDatas>
    {
        #region 数据

        /// <summary>
        /// Layer数据库
        /// </summary>
        public List<LayerDataBase> layerDatas = new ();
        /// <summary>
        /// Tag数据库
        /// </summary>
        public List<TagDataBase> tagDatas = new ();
        /// <summary>
        /// SortingLayer数据库
        /// </summary>
        public List<SortingLayerDataBase> sortingLayerDatas = new ();
        /// <summary>
        /// Layer数据字典
        /// </summary>
        private Dictionary<int, LayerDataBase> layerDataDic;
        /// <summary>
        /// Tag数据字典
        /// </summary>
        private Dictionary<string, TagDataBase> tagDataDic;
        /// <summary>
        /// SortingLayer数据字典
        /// </summary>
        private Dictionary<int, SortingLayerDataBase> sortingLayerDataDic;
        /// <summary>
        /// Layer数据字典
        /// </summary>
        private Dictionary<int, LayerDataBase> LayerDataDic
        {
            get
            {
                //如果字典为空那就注册字典
                if (layerDataDic == null)
                {
                    InitLayerDic();
                }

                return layerDataDic;
            }
        }
        /// <summary>
        /// Layer数据字典
        /// </summary>
        private Dictionary<string, TagDataBase> TagDataDic
        {
            get
            {
                //如果字典为空那就注册字典
                if (tagDataDic == null)
                {
                    InitTagDic();
                }

                return tagDataDic;
            }
        }
        /// <summary>
        /// Layer数据字典
        /// </summary>
        private Dictionary<int, SortingLayerDataBase> SortingLayerDataDic
        {
            get
            {
                //如果字典为空那就注册字典
                if (sortingLayerDataDic == null)
                {
                    InitSortingLayerDic();
                }

                return sortingLayerDataDic;
            }
        }

        /// <summary>
        /// 上次layer更新的代码
        /// </summary>
        [SerializeField]
        internal string prevLayerUpdateCode;
        /// <summary>
        /// 上次Tag更新的代码
        /// </summary>
        [SerializeField]
        internal string prevTagUpdateCode;
        /// <summary>
        /// 上次SortingLayer更新的代码
        /// </summary>
        [SerializeField]
        internal string prevSortingLayerUpdateCode;
        
        #endregion

        #region 数据操作方法

                
        /// <summary>
        /// 通过Layer下标获取数据
        /// </summary>
        /// <param name="index">Layer下标</param>
        /// <returns>如果有数据就返回数据，没有的话返回null</returns>
        internal LayerDataBase GetLayerData(int index)
        {
            return LayerDataDic.GetValueOrDefault(index);
        }
        /// <summary>
        /// 通过Tag名称获取数据
        /// </summary>
        /// <param name="tagName">Tag名称</param>
        /// <returns>如果有数据就返回数据，没有的话返回null</returns>
        internal TagDataBase GetTagData(string tagName)
        {
            return TagDataDic.GetValueOrDefault(tagName);
        }
        /// <summary>
        /// 通过SortingLayer下标获取数据
        /// </summary>
        /// <param name="index">SortingLayer下标</param>
        /// <returns>如果有数据就返回数据，没有的话返回null</returns>
        internal SortingLayerDataBase GetSortingLayerData(int index)
        {
            return SortingLayerDataDic.GetValueOrDefault(index);
        }
        /// <summary>
        /// 初始化layer字典
        /// </summary>
        internal void InitLayerDic()
        {
            layerDataDic = new Dictionary<int, LayerDataBase>();

            foreach (LayerDataBase data in layerDatas)
            {
                layerDataDic[data.layerIndex] = data;
            }
        }
        /// <summary>
        /// 初始化layer字典
        /// </summary>
        internal void InitTagDic()
        {
            tagDataDic = new Dictionary<string, TagDataBase>();
    
            foreach (var data in tagDatas)
            {
                tagDataDic[data.tagName] = data;
            }
        }
        /// <summary>
        /// 初始化layer字典
        /// </summary>
        internal void InitSortingLayerDic()
        {
            sortingLayerDataDic = new Dictionary<int, SortingLayerDataBase>();

            foreach (var data in sortingLayerDatas)
            {
                sortingLayerDataDic[data.layerIndex] = data;
            }
        }

        #endregion

    }
}

