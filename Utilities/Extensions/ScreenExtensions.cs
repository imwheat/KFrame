using UnityEngine;

namespace KFrame.Utilities
{
    /// <summary>
    /// 设备屏幕操作的拓展
    /// </summary>
    public static class ScreenExtensions
    {
        /// <summary>
        /// 检查当前屏幕是否为横屏模式。
        /// </summary>
        /// <returns>如果为横屏模式，则返回 true；否则返回 false。</returns>
        public static bool IsLandScape() { return Screen.width > Screen.height; }

        /// <summary>
        /// 获取当前屏幕的宽高比。
        /// </summary>
        /// <returns>屏幕的宽高比。</returns>
        public static float GetAspectRatio()
        {
            return IsLandScape() ? (float)Screen.width / Screen.height : (float)Screen.height / Screen.width;
        }

        /// <summary>
        /// 获取屏幕中心的坐标
        /// </summary>
        /// <returns></returns>
        public static Vector3 GetScreenWorldPositionVector3()
        {
            return new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        }

        public static Vector2 GetScreenWorldPositionVector2()
        {
            return new Vector2(Screen.width / 2f, Screen.height / 2f);
        }

        /// <summary>
        /// 检查当前屏幕的宽高比是否在给定的范围内。
        /// </summary>
        /// <param name="aspectRatio">要比较的目标宽高比。</param>
        /// <returns>如果在范围内，则返回 true；否则返回 false。</returns>
        public static bool IsAspectRange(float aspectRatio)
        {
            var aspect = GetAspectRatio();
            return aspect > (aspectRatio - 0.05) && aspect < (aspectRatio + 0.05);
        }
    }
}