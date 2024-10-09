//****************** 代码文件申明 ***********************
//* 文件：SystemBase
//* 作者：wheat
//* 创建时间：2024/10/09 08:53:11 星期三
//* 描述：一个简单实现了ISystem的抽象类
//*******************************************************

using UnityEngine;
using KFrame;
using KFrame.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;

namespace KFrame
{
    public abstract class SystemBase : ISystem
    {
        #region 属性
        
        /// <summary>
        /// Model所属对象
        /// </summary>
        public IController Owner { get; set; }

        #endregion

        #region 接口方法

        /// <summary>
        /// 获取Model
        /// </summary>
        /// <typeparam name="T">Model的类型</typeparam>
        /// <returns>返回Model</returns>
        public T GetModel<T>() where T : IModel, new()
        {
            return Owner.GetModel<T>();
        }
        /// <summary>
        /// 获取System
        /// </summary>
        /// <typeparam name="T">System的类型</typeparam>
        /// <returns>返回System</returns>
        public T GetSystem<T>() where T : ISystem, new()
        {
            return Owner.GetSystem<T>();
        }
        /// <summary>
        /// 添加注册Update事件
        /// </summary>
        /// <param name="updateEvent">Update事件</param>
        public void AddUpdateListener(Action updateEvent)
        {
            Owner.AddUpdateListener(updateEvent);
        }
        /// <summary>
        /// 注销Update事件
        /// </summary>
        /// <param name="updateEvent">Update事件</param>
        public void RemoveUpdateListener(Action updateEvent)
        {
            Owner.RemoveUpdateListener(updateEvent);
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public virtual void Dispose()
        {
            
        }

        #endregion
    }
}

