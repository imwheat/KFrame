using UnityEngine;

namespace KFrame.Utilities
{
    /// <summary>
    /// Koo向量工具
    /// </summary>
    public static class VectorExtensions
    {
        #region Vector类型的数值比较

        /// <summary>
        /// 比较两个 Vector2 是否满足小于关系 二维向量a中的每一个分量是否都是小于b的，并根据参数选择使用 "与" 或 "或" 操作符进行比较。 
        /// </summary>
        /// <param name="a">第一个 Vector2。</param>
        /// <param name="b">第二个 Vector2。</param>
        /// <param name="useAndOperate">是否使用 "与" 操作符，如果为 false，则使用 "或" 操作符。</param>
        /// <returns>如果满足小于关系，则返回 true；否则返回 false。</returns>
        public static bool LessThan(this Vector2 a, Vector2 b, bool useAndOperate = true)
        {
            if (useAndOperate)
                return (a.x < b.x) && (a.y < b.y);
            else
                return (a.x < b.x) || (a.y < b.y);
        }

        /// <summary>
        /// 比较两个 Vector2 是否满足大于关系 二维向量a中的每一个分量是否都是大于b的，并根据参数选择使用 "与" 或 "或" 操作符进行比较。
        /// </summary>
        /// <param name="a">第一个 Vector2。</param>
        /// <param name="b">第二个 Vector2。</param>
        /// <param name="useAndOperate">是否使用 "与" 操作符，如果为 false，则使用 "或" 操作符。</param>
        /// <returns>如果满足大于关系，则返回 true；否则返回 false。</returns>
        public static bool GreaterThan(this Vector2 a, Vector2 b, bool useAndOperate = true)
        {
            if (useAndOperate)
                return a.x > b.x && a.y > b.y;
            else
                return a.x > b.x || a.y > b.y;
        }

        /// <summary>
        /// 比较两个 Vector3 是否满足小于关系 三维向量a中的每一个分量是否都是小于b的，并根据参数选择使用 "与" 或 "或" 操作符进行比较。
        /// </summary>
        /// <param name="a">第一个 Vector3。</param>
        /// <param name="b">第二个 Vector3。</param>
        /// <param name="useAndOperate">是否使用 "与" 操作符，如果为 false，则使用 "或" 操作符。</param>
        /// <returns>如果满足小于关系，则返回 true；否则返回 false。</returns>
        public static bool LessThan(this Vector3 a, Vector3 b, bool useAndOperate = true)
        {
            if (useAndOperate)
                return (a.x < b.x) && (a.y < b.y) && (a.z < b.z);
            else
                return (a.x < b.x) || (a.y < b.y) || (a.z < b.z);
        }

        /// <summary>
        /// 比较两个 Vector3 是否满足大于关系 三维向量a中的每一个分量是否都是大于b的，并根据参数选择使用 "与" 或 "或" 操作符进行比较。
        /// </summary>
        /// <param name="a">第一个 Vector3。</param>
        /// <param name="b">第二个 Vector3。</param>
        /// <param name="useAndOperate">是否使用 "与" 操作符，如果为 false，则使用 "或" 操作符。</param>
        /// <returns>如果满足大于关系，则返回 true；否则返回 false。</returns>
        public static bool GreaterThan(this Vector3 a, Vector3 b, bool useAndOperate = true)
        {
            if (useAndOperate)
                return a.x > b.x && a.y > b.y && a.z > b.z;
            else
                return a.x > b.x || a.y > b.y || a.z > b.z;
        }

        /// <summary>
        /// 比较两个 Vector4 是否满足小于关系 四维向量a中的每一个分量是否都是小于b的，并根据参数选择使用 "与" 或 "或" 操作符进行比较。
        /// </summary>
        /// <param name="a">第一个 Vector4。</param>
        /// <param name="b">第二个 Vector4。</param>
        /// <param name="useAndOperate">是否使用 "与" 操作符，如果为 false，则使用 "或" 操作符。</param>
        /// <returns>如果满足小于关系，则返回 true；否则返回 false。</returns>
        public static bool LessThan(this Vector4 a, Vector4 b, bool useAndOperate = true)
        {
            if (useAndOperate)
                return a.x < b.x && a.y < b.y && a.z < b.z && a.w < b.w;
            else
                return a.x < b.x || a.y < b.y || a.z < b.z || a.w < b.w;
        }

        /// <summary>
        /// 比较两个 Vector4 是否满足大于关系 四维向量a中的每一个分量是否都是大于b的，并根据参数选择使用 "与" 或 "或" 操作符进行比较。
        /// </summary>
        /// <param name="a">第一个 Vector4。</param>
        /// <param name="b">第二个 Vector4。</param>
        /// <param name="useAndOperate">是否使用 "与" 操作符，如果为 false，则使用 "或" 操作符。</param>
        /// <returns>如果满足大于关系，则返回 true；否则返回 false。</returns>
        public static bool GreaterThan(this Vector4 a, Vector4 b, bool useAndOperate = true)
        {
            if (useAndOperate)
                return a.x > b.x && a.y > b.y && a.z > b.z && a.w > b.w;
            else
                return a.x > b.x || a.y > b.y || a.z > b.z || a.w > b.w;
        }

        #endregion

        #region Vector的操作

        /// <summary>
        /// 获取向量的绝对值
        /// </summary>
        /// <param name="vector2">传入的向量</param>
        /// <returns>传出的绝对值的向量</returns>
        public static Vector2 GetAbs(this Vector2 vector2)
        {
            return new Vector2(Mathf.Abs(vector2.x), Mathf.Abs(vector2.y));
        }

        /// <summary>
        /// 获取向量的绝对值
        /// </summary>
        /// <param name="vector3">传入的向量</param>
        /// <returns>传出的绝对值的向量</returns>
        public static Vector3 GetAbs(this Vector3 vector3)
        {
            return new Vector3(Mathf.Abs(vector3.x), Mathf.Abs(vector3.y), Mathf.Abs(vector3.z));
        }

        /// <summary>
        /// 转换为Vector3 Z轴默认化0 
        /// </summary>
        /// <param name="vector2"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Vector3 ToVector3(this Vector2 vector2, float z = 0)
        {
            return new Vector3(vector2.x, vector2.y, z);
        }
        
        

        #endregion
    }
}