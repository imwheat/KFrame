//****************** 代码文件申明 ***********************
//* 文件：MSCMonoBehaviour
//* 作者：wheat
//* 创建时间：2024/10/08 18:09:08 星期二
//* 描述：基于MSC系统的MonoBehaviour
//*******************************************************

using UnityEngine;
using System;
using System.Collections.Generic;
using KFrame.Utilities;

namespace KFrame
{
    public abstract class MSCMonoBehaviour : MonoBehaviour, IController
    {
        #region 参数属性

        /// <summary>
        /// 存储Model的字典
        /// </summary>
        private Dictionary<string, IModel> _modelDic;
        /// <summary>
        /// 存储System的字典
        /// </summary>
        private Dictionary<string, ISystem> _systemDic;
        /// <summary>
        /// Update生命周期调用事件
        /// </summary>
        private Action _onControllerUpdate;

        #endregion

        #region 生命周期
        
        
        /// <summary>
        /// 初始化注册Model
        /// </summary>
        protected virtual void InitRegisterModels()
        {
            _modelDic = new Dictionary<string, IModel>();
        }

        /// <summary>
        /// 初始化注册System
        /// </summary>
        protected virtual void InitRegisterSystems()
        {
            _systemDic = new Dictionary<string, ISystem>();
        }
        /// <summary>
        /// 初始化MSC系统
        /// </summary>
        private void InitMSC()
        {
            InitRegisterModels();
            InitRegisterSystems();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public virtual void Dispose()
        {
            //遍历释放资源
            foreach (var model in _modelDic.Values)
            {
                model.Dispose();
            }
            foreach (var system in _systemDic.Values)
            {
                system.Dispose();
            }
            //清空事件
            _onControllerUpdate = null;
        }
        
        protected virtual void Awake()
        {
            //初始化MSC系统
            InitMSC();
        }

        protected virtual void Update()
        {
            _onControllerUpdate?.Invoke();
        }

        protected virtual void OnDestroy()
        {
            Dispose();
        }

        #endregion

        #region 创建与删除

        /// <summary>
        /// 注册Model
        /// </summary>
        /// <typeparam name="T">Model的类型</typeparam>
        public T RegisterModel<T>() where T : IModel, new()
        {
            var model = new T();
            model.Owner = this;
            _modelDic[typeof(T).GetNiceName()] = model;

            return model;
        }

        /// <summary>
        /// 注册System
        /// </summary>
        /// <typeparam name="T">System的类型</typeparam>
        public T RegisterSystem<T>() where T : ISystem, new()
        {
            //新建System
            var system = new T();
            //设置Owner，塞入字典
            system.Owner = this;
            _systemDic[typeof(T).GetNiceName()] = system;

            return system;
        }
        /// <summary>
        /// 注销Model
        /// </summary>
        /// <typeparam name="T">Model的类型</typeparam>
        public void UnRegisterModel<T>() where T : IModel, new()
        {
            //获取key然后获取Model，然后删除
            var keyName = typeof(T).GetNiceName();
            if (!_modelDic.Remove(keyName, out var model)) return;
            //释放资源
            model.Dispose();
        }
        /// <summary>
        /// 注销System
        /// </summary>
        /// <typeparam name="T">System的类型</typeparam>
        public void UnRegisterSystem<T>() where T : ISystem, new()
        {
            //获取key然后获取system，然后删除
            var keyName = typeof(T).GetNiceName();
            if (!_systemDic.Remove(keyName, out var system)) return;
            //释放资源
            system.Dispose();
        }
        #endregion

        #region 获取

        /// <summary>
        /// 获取Model
        /// </summary>
        /// <typeparam name="T">Model的类型</typeparam>
        /// <returns>返回Model</returns>
        public T GetModel<T>() where T : IModel, new()
        {
            //尝试从字典中获取，如果没有那就创建注册
            var keyName = typeof(T).GetNiceName();
            if (_modelDic.TryGetValue(keyName, out var model))
            {
                return (T)model;
            }
            else
            {
                return RegisterModel<T>();
            }
        }

        /// <summary>
        /// 获取System
        /// </summary>
        /// <typeparam name="T">System的类型</typeparam>
        /// <returns>返回System</returns>
        public T GetSystem<T>() where T : ISystem, new()
        {
            //尝试从字典中获取，如果没有那就创建注册
            var keyName = typeof(T).GetNiceName();
            if (_systemDic.TryGetValue(keyName, out var system))
            {
                return (T)system;
            }
            else
            {
                return RegisterSystem<T>();
            }
        }

        #endregion

        #region 事件
        
        /// <summary>
        /// 添加注册Update事件
        /// </summary>
        /// <param name="updateEvent">Update事件</param>
        public void AddUpdateListener(Action updateEvent)
        {
            _onControllerUpdate += updateEvent;
        }
        /// <summary>
        /// 注销Update事件
        /// </summary>
        /// <param name="updateEvent">Update事件</param>
        public void RemoveUpdateListener(Action updateEvent)
        {
            _onControllerUpdate -= updateEvent;
        }

        #endregion
    }
}
