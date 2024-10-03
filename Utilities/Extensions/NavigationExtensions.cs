//****************** 代码文件申明 ***********************
//* 文件：NavigationExtensions
//* 作者：wheat
//* 创建时间：2024/10/03 09:59:21 星期四
//* 描述：拓展Navigation的一些方法
//*******************************************************

using UnityEngine.UI;

namespace KFrame.Utilities
{
    public static class NavigationExtensions
    {
        /// <summary>
        /// 复制一份Navigation只改变向上选择的情况
        /// </summary>
        /// <param name="navigation">原数据</param>
        /// <param name="selectable">向上选择的结果</param>
        /// <returns>复制一份Navigation只改变向上选择的情况</returns>
        public static Navigation ChangeSelectOnUp(this Navigation navigation,Selectable selectable)
        {
            return new Navigation()
            {
                mode = navigation.mode,
                wrapAround = navigation.wrapAround,
                selectOnDown = navigation.selectOnDown,
                selectOnLeft = navigation.selectOnLeft,
                selectOnRight = navigation.selectOnRight,
                selectOnUp = selectable,
            };
        }
        /// <summary>
        /// 复制一份Navigation只改变向上选择的情况
        /// </summary>
        /// <param name="navigation">原数据</param>
        /// <param name="selectable">向上选择的结果</param>
        /// <returns>复制一份Navigation只改变向上选择的情况</returns>
        public static Navigation ChangeSelectOnLeft(this Navigation navigation,Selectable selectable)
        {
            return new Navigation()
            {
                mode = navigation.mode,
                wrapAround = navigation.wrapAround,
                selectOnDown = navigation.selectOnDown,
                selectOnLeft = selectable,
                selectOnRight = navigation.selectOnRight,
                selectOnUp = navigation.selectOnUp,
            };
        }
        /// <summary>
        /// 复制一份Navigation只改变向上选择的情况
        /// </summary>
        /// <param name="navigation">原数据</param>
        /// <param name="selectable">向上选择的结果</param>
        /// <returns>复制一份Navigation只改变向上选择的情况</returns>
        public static Navigation ChangeSelectOnRight(this Navigation navigation,Selectable selectable)
        {
            return new Navigation()
            {
                mode = navigation.mode,
                wrapAround = navigation.wrapAround,
                selectOnDown = navigation.selectOnDown,
                selectOnLeft = navigation.selectOnLeft,
                selectOnRight = selectable,
                selectOnUp = navigation.selectOnUp,
            };
        }
        /// <summary>
        /// 复制一份Navigation只改变向上选择的情况
        /// </summary>
        /// <param name="navigation">原数据</param>
        /// <param name="selectable">向上选择的结果</param>
        /// <returns>复制一份Navigation只改变向上选择的情况</returns>
        public static Navigation ChangeSelectOnDown(this Navigation navigation,Selectable selectable)
        {
            return new Navigation()
            {
                mode = navigation.mode,
                wrapAround = navigation.wrapAround,
                selectOnDown = selectable,
                selectOnLeft = navigation.selectOnLeft,
                selectOnRight = navigation.selectOnRight,
                selectOnUp = navigation.selectOnUp,
            };
        }
    }
}

