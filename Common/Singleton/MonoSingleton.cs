using UnityEngine;


namespace KFrame
{
    /// <summary>
    /// Mono单例 可以被销毁 不会放置到DontDestroy
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance => instance;


        private static T instance;


        protected virtual void Awake()
        {
            instance = this as T;
        }
    }
}