//****************** 代码文件申明 ***********************
//* 文件：TagDataBase
//* 作者：wheat
//* 创建时间：2024/09/26 14:05:35 星期四
//* 描述：
//*******************************************************

namespace KFrame.Editor.Tools
{
    [System.Serializable]
    internal class TagDataBase
    {

        /// <summary>
        /// Tag的名称
        /// </summary>
        public string tagName;
        /// <summary>
        /// 描述(用户自己添加不会更新)
        /// </summary>
        public string description;

        public TagDataBase(string tagName)
        {
            this.tagName = tagName;
        }
        /// <summary>
        /// 从另一个data那更新数据
        /// </summary>
        /// <param name="other">复制数据的data</param>
        public void UpdateData(TagDataBase other)
        {
            description = other.description;
        }
    }
}

