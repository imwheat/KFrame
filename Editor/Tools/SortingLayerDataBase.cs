//****************** 代码文件申明 ***********************
//* 文件：SortingLayerDataBase
//* 作者：wheat
//* 创建时间：2024/09/26 14:05:57 星期四
//* 描述：
//*******************************************************

namespace KFrame.Editor.Tools
{
    [System.Serializable]
    internal class SortingLayerDataBase
    {
        /// <summary>
        /// 层级id
        /// </summary>
        public int layerIndex;
        /// <summary>
        /// 层级名称
        /// </summary>
        public string layerName;
        /// <summary>
        /// 描述(用户自己添加不会更新)
        /// </summary>
        public string description;

        public SortingLayerDataBase()
        {
            
        }

        public SortingLayerDataBase(int layerIndex, string layerName)
        {
            this.layerIndex = layerIndex;
            this.layerName = layerName;
        }
        /// <summary>
        /// 从另一个data那更新数据
        /// </summary>
        /// <param name="other">复制数据的data</param>
        public void UpdateData(SortingLayerDataBase other)
        {
            description = other.description;
        }
    }
}

