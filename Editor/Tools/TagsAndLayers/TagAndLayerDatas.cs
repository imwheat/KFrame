//****************** 代码文件申明 ***********************
//* 文件：TagAndLayerDatas
//* 作者：wheat
//* 创建时间：2024/09/26 09:28:20 星期四
//* 描述：存储Layer的一些数据
//*******************************************************

using KFrame.Utilities;
using System.Collections.Generic;
using KFrame.Attributes;

namespace KFrame.Editor
{
    [KGlobalConfigPath(GlobalPathType.Editor, typeof(TagAndLayerDatas), true)]
    internal class TagAndLayerDatas : GlobalConfigBase<TagAndLayerDatas>
    {
        #region 数据

        /// <summary>
        /// 数据库
        /// </summary>
        public List<LayerDataBase> datas = new ();
        /// <summary>
        /// 层级数据字典
        /// </summary>
        private Dictionary<int, LayerDataBase> dataDic;
        /// <summary>
        /// 层级数据字典
        /// </summary>
        private Dictionary<int, LayerDataBase> DataDic
        {
            get
            {
                //如果字典为空那就注册字典
                if (dataDic == null)
                {
                    dataDic = new Dictionary<int, LayerDataBase>();

                    foreach (LayerDataBase data in datas)
                    {
                        dataDic[data.layerIndex] = data;
                    }
                }

                return dataDic;
            }
        }

        #endregion

        #region 数据操作方法

                
        /// <summary>
        /// 通过层级下标获取数据
        /// </summary>
        /// <param name="index">层级下标</param>
        /// <returns>如果有数据就返回数据，没有的话返回null</returns>
        internal LayerDataBase GetData(int index)
        {
            return DataDic.GetValueOrDefault(index);
        }
        /// <summary>
        /// 新增数据
        /// </summary>
        /// <param name="data">数据</param>
        internal void UpdateData(LayerDataBase data)
        {
            //先尝试获取一下之前的数据
            var prevData = GetData(data.layerIndex);
            //如果没有数据那就新建加入数据库和字典
            if (prevData == null)
            {
                DataDic[data.layerIndex] = data;
                datas.Add(data);
            }
            //如果已经有了那就更新一下
            else
            {
                prevData.UpdateData(data);
            }
            
        }

        #endregion

    }
}

