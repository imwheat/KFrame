namespace KFrame.Extensions
{
    public static class ArrayExtensions
    {
        /// <summary>
        /// 比较两个对象数组是否相等
        /// </summary>
        /// <param name="objs">要比较的对象数组</param>
        /// <param name="other">用于比较的另一个对象数组</param>
        /// <returns>如果两个数组相等，则返回 true；否则返回 false</returns>
        public static bool ArraryEquals(this object[] objs, object[] other)
        {
            // 如果 'other' 为 null 或者 'objs' 的类型与 'other' 的类型不相同
            if (other == null || objs.GetType() != other.GetType())
            {
                return false;
            }

            // 如果数组的长度相等
            if (objs.Length == other.Length)
            {
                for (int i = 0; i < objs.Length; i++)
                {
                    // 如果两个数组在相同索引的元素不相等 (Equals)比较对象的相等性
                    if (!objs[i].Equals(other[i]))
                    {
                        return false;
                    }
                }
            }
            else
            {
                // 数组长度不相等，直接返回 false
                return false;
            }

            // 所有的比较都通过，返回 true，表示数组相等
            return true;
        }
        
        
        
    }
}