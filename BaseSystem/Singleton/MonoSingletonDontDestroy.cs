using System;
using UnityEngine;

namespace KFrame
{
    /// <summary>
    /// Mono单例
    /// 运行游戏自动放置于DontDestroy
    /// </summary>
    /// <typeparam name="T">单例类型</typeparam>
    public class MonoSingletonDontDestroy<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;

        public static T Instance => instance;

        /// <summary>
        /// 线程锁
        /// </summary>
        private static readonly object objlock = new object();

        protected virtual void Awake()
        {
            lock (objlock)
            {
                if (instance == null)
                {
                    instance = this as T;

                    if (this.transform == transform.root)
                        //如果是游戏根物体 就不销毁
                        DontDestroyOnLoad(this);
                    else
                        //不销毁游戏根物体
                        DontDestroyOnLoad(transform.root);
                }

                if (instance != null && instance != this) // 防止Editor下的Instance已经存在，并且是自身
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}