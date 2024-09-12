//****************** 代码文件申明 ***********************
//* 文件：UnityVersion
//* 作者：wheat
//* 创建时间：2024/05/28 19:00:40 星期二
//* 描述：判断Unity的版本用的
//*******************************************************

using UnityEngine;
using UnityEditor;

namespace KFrame.Editor
{
    /// <summary>
    /// 判断Unity的版本用的
    /// </summary>
    [InitializeOnLoad]
    public static class UnityVersion
    {
        /// <summary>
        /// 当前Unity的大版本
        /// </summary>
        public static readonly int Major;

        /// <summary>
        /// 当前Unity的小版本
        /// </summary>
        public static readonly int Minor;

        static UnityVersion()
        {
            string[] array = Application.unityVersion.Split(new char[1] { '.' });
            if (array.Length < 2)
            {
                Debug.LogError("无法获取当前的Unity版本： '" + Application.unityVersion);
                return;
            }

            if (!int.TryParse(array[0], out Major))
            {
                Debug.LogError("无法获取Unity的大版本：" + array[0] + "' 当前版本：'" + Application.unityVersion + "'.");
            }

            if (!int.TryParse(array[1], out Minor))
            {
                Debug.LogError("无法获取Unity的小版本：" + array[1] + "' 当前版本：'" + Application.unityVersion + "'.");
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void EnsureLoaded()
        {
        }

        /// <summary>
        /// 判断当前版本是不是高于某个版本
        /// </summary>
        /// <param name="major">大版本</param>
        /// <param name="minor">小版本</param>
        /// <returns>true，如果版本高于比较版本</returns>
        public static bool IsVersionOrGreater(int major, int minor)
        {
            if (Major <= major)
            {
                if (Major == major)
                {
                    return Minor >= minor;
                }

                return false;
            }

            return true;
        }
    }
}

