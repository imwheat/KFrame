//****************** 代码文件申明 ************************
//* 文件：InputModule                      
//* 作者：wheat
//* 创建时间：2024/09/29 12:23:06 星期日
//* 功能：输入模块的基类
//*****************************************************

using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KFrame.Systems
{
    public class InputModule : MonoBehaviour
    {
        #region 输入配置

        /// <summary>
        /// 输入配置
        /// </summary>
        public GameInputAction InputConfig;
        /// <summary>
        /// 所分配到的输入设备序号
        /// </summary>
        public int InputIndex;

        #endregion
        
        #region 设备参数

        /// <summary>
        /// 当前输入设备信息
        /// </summary>
        [SerializeField] public InputDeviceData currentDeviceData;
        /// <summary>
        /// 当前的输入设备
        /// </summary>
        private InputDevice currentDevice;
        /// <summary>
        /// 当前的输入设备
        /// </summary>
        public InputDevice InputDevice
        {
            get => currentDevice;
            set
            {
                currentDevice = value;
                UpdateInputDevice();
            }
        }
        /// <summary>
        /// 鼠标设备
        /// </summary>
        private Mouse mouse;
        /// <summary>
        /// 键盘设备
        /// </summary>
        private Keyboard keyboard;
        /// <summary>
        /// 手柄
        /// </summary>
        private Gamepad gamepad;

        #endregion

        #region 事件

        /// <summary>
        /// 切换到鼠标的事件
        /// </summary>
        public Action OnSwitchMouse;
        /// <summary>
        /// 切换到键盘的事件
        /// </summary>
        public Action OnSwitchKeyboard;
        /// <summary>
        /// 切换到手柄的事件
        /// </summary>
        public Action OnSwitchGamepad;

        #endregion

        #region 生命周期

        protected virtual void Awake()
        {
            // 初始化当前的输入设备
            mouse = Mouse.current;
            keyboard = Keyboard.current;
            gamepad = Gamepad.current;
            
            //注册Module
            this.RegisterInputModule();
        }
        protected void OnEnable()
        {
            InputSystem.onActionChange += DetectCurrentInputDevice;
        }

        protected void OnDisable()
        {
            InputSystem.onActionChange -= DetectCurrentInputDevice;
        }

        protected void OnDestroy()
        {
            //注销Module
            this.UnRegisterInputModule();
        }


        #endregion

        #region 设备更新

        /// <summary>
        /// 检测当前的输入设备
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="change"></param>
        private void DetectCurrentInputDevice(object obj, InputActionChange change)
        {
            if (change == InputActionChange.ActionPerformed)
            {
                InputDevice = ((InputAction)obj).activeControl.device;
            }
        }
        /// <summary>
        /// 更新输入设备
        /// </summary>
        private void UpdateInputDevice()
        {
            switch (currentDevice)
            {
                case UnityEngine.InputSystem.Mouse:
                    OnSwitchMouse?.Invoke();
                    currentDeviceData.curInputScheme = InputDeviceScheme.KeyboardAndMouse;
                    break;
                case UnityEngine.InputSystem.Keyboard:
                    OnSwitchKeyboard?.Invoke();
                    currentDeviceData.curInputScheme = InputDeviceScheme.KeyboardAndMouse;
                    break;
                case UnityEngine.InputSystem.Gamepad:
                    OnSwitchGamepad?.Invoke();
                    currentDeviceData.curInputScheme = InputDeviceScheme.Gamepad;
                    break;
            }
        }

        #endregion
        
    }
}