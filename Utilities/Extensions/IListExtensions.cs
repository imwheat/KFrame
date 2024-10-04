
using System.Collections;
using System.Linq;

namespace KFrame.Utilities
{
    public static class IListExtensions
    {
        /// <summary>
        /// 比较两个对象数组是否相等
        /// </summary>
        /// <param name="objs">要比较的对象数组</param>
        /// <param name="other">用于比较的另一个对象数组</param>
        /// <returns>如果两个数组相等，则返回 true；否则返回 false</returns>
        public static bool ValueEquals(this IList objs, IList other)
        {
            //如果 'other'、'objs' 为 null 或者 'objs' 的类型与 'other' 的类型不相同 或者长度不同
            if (other == null || objs == null || objs.GetType() != other.GetType() || objs.Count != other.Count)
            {
                return false;
            }
            
            //所有的比较都通过，返回 true，表示数组相等
            return !objs.Cast<object>().Where((t, i) => !t.Equals(other[i])).Any();
        }

        /// <summary>
        /// 查找元素在数组中的下标，如果有多个相同的返回第一个
        /// </summary>
        /// <param name="objs">数组</param>
        /// <param name="element">要寻找的元素</param>
        /// <param name="notFound">如果找不到就返回这个</param>
        /// <returns>元素在数组中的下标没有就返回notFound</returns>
        public static int FindElementIndex(this IList objs, object element, int notFound = -1)
        {
            //如果 'objs'、'element' 为 null 或者 'objs' 的类型与 'element' 的类型不相同
            if (objs == null || element == null || objs.Count == 0 || objs[0].GetType() != element.GetType())
            {
                return notFound;
            }
            //遍历每一个下标返回第一个匹配的
            for (var i = 0; i < objs.Count; i++)
            {
                if (objs[i].Equals(element)) return i;
            }

            return notFound;
        }
        
    }
}