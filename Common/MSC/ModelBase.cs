//****************** 代码文件申明 ***********************
//* 文件：ModelBase
//* 作者：wheat
//* 创建时间：2024/10/09 08:48:46 星期三
//* 描述：一个简单实现了IModel的抽象类
//*******************************************************


namespace KFrame
{
    public abstract class ModelBase : IModel
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
        /// 释放资源
        /// </summary>
        public virtual void Dispose()
        {
            
        }

        #endregion
    }
}

