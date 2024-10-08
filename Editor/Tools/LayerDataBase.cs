//****************** 代码文件申明 ***********************
//* 文件：LayerDataBase
//* 作者：wheat
//* 创建时间：2024/09/26 09:33:20 星期四
//* 描述：单个Layer的存储数据
//*******************************************************

namespace KFrame.Editor.Tools
{
    /// <summary>
    /// 单个Layer的存储数据
    /// </summary>
    [System.Serializable]
    internal class LayerDataBase
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
        /// 会发生碰撞的层级
        /// </summary>
        public string collisionLayer;
        /// <summary>
        /// 描述(用户自己添加不会更新)
        /// </summary>
        public string description;

        public LayerDataBase()
        {
            
        }

        public LayerDataBase(int layerIndex, string layerName)
        {
            this.layerIndex = layerIndex;
            this.layerName = layerName;
        }
        /// <summary>
        /// 从另一个data那更新数据
        /// </summary>
        /// <param name="other">复制数据的data</param>
        public void UpdateData(LayerDataBase other)
        {
            description = other.description;
        }
    }
}

