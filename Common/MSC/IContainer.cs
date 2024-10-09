//****************** 代码文件申明 ***********************
//* 文件：IContainer
//* 作者：wheat
//* 创建时间：2024/10/08 18:16:56 星期二
//* 描述：存储、获取被Controller管理的容器
//*******************************************************

using System;

namespace KFrame
{
    public interface IContainer : IDisposable
    {
        public IController Owner { get; set; }

        /// <summary>
        /// 获取Model
        /// </summary>
        /// <typeparam name="T">Model的类型</typeparam>
        /// <returns>返回Model</returns>
        public T GetModel<T>() where T : IModel, new();

        /// <summary>
        /// 获取System
        /// </summary>
        /// <typeparam name="T">System的类型</typeparam>
        /// <returns>返回System</returns>
        public T GetSystem<T>() where T : ISystem, new();
    }
}

