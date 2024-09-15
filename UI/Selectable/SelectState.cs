//****************** 代码文件声明 ***********************
//* 文件：SelectState
//* 作者：wheat
//* 创建时间：2024/09/15 12:48:19 星期日
//* 描述：UI的选择状态
//*******************************************************


namespace KFrame.UI
{

    /// <summary>
    /// UI的选择状态
    /// </summary>
    public enum SelectState
    {
        /// <summary>
        /// UI可以被选择
        /// </summary>
        Normal = 0,
        /// <summary>
        /// UI被按下了
        /// </summary>
        Pressed = 1,
        /// <summary>
        /// UI被选择了
        /// </summary>
        Selected = 2,
        /// <summary>
        /// UI被禁用了
        /// </summary>
        Disabled = 3,
    }
}
