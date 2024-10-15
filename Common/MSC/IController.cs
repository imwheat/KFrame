//****************** 代码文件申明 ***********************
//* 文件：IController
//* 作者：wheat
//* 创建时间：2024/10/08 18:04:25 星期二
//* 描述：统筹控制管理的中心，System和Model都被其管理
//*******************************************************

using System;
using UnityEngine;

namespace KFrame
{
    public interface IController : IDisposable
    {
        #region 属性
        
        /// <summary>
        /// 控制器的GameObject
        /// </summary>
        public GameObject GameObject { get; }

        #endregion
        
        #region 创建与删除

        /// <summary>
        /// 注册Model
        /// </summary>
        /// <typeparam name="T">Model的类型</typeparam>
        public T RegisterModel<T>() where T : IModel, new();
        /// <summary>
        /// 注册Model
        /// </summary>
        /// <typeparam name="T">Model的类型</typeparam>
        public T RegisterModel<T>(T model) where T : IModel;
        /// <summary>
        /// 注册System
        /// </summary>
        /// <typeparam name="T">System的类型</typeparam>
        public T RegisterSystem<T>() where T : ISystem, new();
        /// <summary>
        /// 注册System
        /// </summary>
        /// <typeparam name="T">System的类型</typeparam>
        public T RegisterSystem<T>(T system, string keyName) where T : ISystem;
        /// <summary>
        /// 注销Model
        /// </summary>
        /// <typeparam name="T">Model的类型</typeparam>
        public void UnRegisterModel<T>() where T : IModel, new();

        /// <summary>
        /// 注销System
        /// </summary>
        /// <typeparam name="T">System的类型</typeparam>
        public void UnRegisterSystem<T>() where T : ISystem, new();
        
        #endregion

        #region 获取

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

        #endregion

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

