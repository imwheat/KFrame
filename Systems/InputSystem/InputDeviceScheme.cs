//****************** 代码文件申明 ***********************
//* 文件：InputDeviceScheme
//* 作者：wheat
//* 创建时间：2024/09/29 12:30:16 星期日
//* 描述：设备输入的方案
//*******************************************************

namespace KFrame.Systems
{
    /// <summary>
    /// 设备输入的方案
    /// 一般在UI方法用到
    /// </summary>
    public enum InputDeviceScheme
    {
        /// <summary>
        /// 键鼠
        /// </summary>
        KeyboardAndMouse = 0,
        /// <summary>
        /// 手柄
        /// </summary>
        Gamepad = 1,
    }
}

