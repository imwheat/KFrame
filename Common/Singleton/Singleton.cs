namespace KFrame
{
    /// <summary>
    /// 提供的单例类
    /// </summary>
    /// <typeparam name="T">单例的类型</typeparam>
    public abstract class Singleton<T> where T : Singleton<T>, new()
    {
        private static T instance; //单例实例

        private static readonly object objlock = new object(); //线程锁

        public static T Instance
        {
            get
            {
                if (instance != null) return instance;
                lock (objlock)
                {
                    //这里再进行一次判空 是为了防止多线程中两个线程同时构建了这个实例 这样就不会导致单例模式被破坏

                    if (instance == null)
                    {
                        instance = new T();
                    }
                    //等同于
                    //instance ??= new T(); 
                }

                return instance;
            }
        }

        /// <summary>
        /// 没有人会去new单例 所以不需要构造函数
        /// 让构造函数私有化
        /// </summary>
        // private KooSingleton()
        // {
        // }
    }
}