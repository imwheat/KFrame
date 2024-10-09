//****************** 代码文件申明 ***********************
//* 文件：ISystem
//* 作者：wheat
//* 创建时间：2024/10/08 18:07:07 星期二
//* 描述：处理逻辑的地方
//*******************************************************

using System;

namespace KFrame
{
    public interface ISystem : IContainer
    {
        #region 事件

        /// <summary>
        /// 添加注册Update事件
        /// </summary>
        /// <param name="updateEvent">Update事件</param>
        public void AddUpdateListener(Action updateEvent);

        /// <summary>
        /// 注销Update事件
        /// </summary>
        /// <param name="updateEvent">Update事件</param>
        public void RemoveUpdateListener(Action updateEvent);

        #endregion
    }
}

