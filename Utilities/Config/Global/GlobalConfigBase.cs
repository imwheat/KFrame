//****************** 代码文件申明 ***********************
//* 文件：GlobalConfigBase
//* 作者：wheat
//* 创建时间：2024/05/30 09:04:15 星期四
//* 描述：编辑器使用的一些全局配置
//*******************************************************

using KFrame.Attributes;
using System.Reflection;

namespace KFrame.Utilities
{
    /// <summary>
    /// 编辑器使用的一些全局配置
    /// </summary>
    public abstract class GlobalConfigBase<T> : ConfigBase where T : GlobalConfigBase<T>, new()
    {
        private static KGlobalConfigPathAttribute configAttribute;
        public static KGlobalConfigPathAttribute ConfigAttribute
        {
            get
            {
                if (configAttribute == null)
                {
                    configAttribute = typeof(T).GetCustomAttribute<KGlobalConfigPathAttribute>();
                    if (configAttribute == null)
                    {
                        configAttribute = new KGlobalConfigPathAttribute(typeof(T).GetNiceName(), "", false);
                    }
                }

                return configAttribute;
            }
        }
        /// <summary>
        /// 实例
        /// </summary>
        public static T Instance => GlobalConfigUtility<T>.GetInstance(ConfigAttribute.AssetPath, ConfigAttribute.FileName, ConfigAttribute.CreateIfNotFound);

    }
}

