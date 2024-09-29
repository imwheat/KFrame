//****************** 代码文件申明 ************************
//* 文件：InputModule                      
//* 作者：wheat
//* 创建时间：2024/09/29 12:23:06 星期日
//* 功能：输入模块的基类
//*****************************************************

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace KFrame.Systems
{
    public class InputModule : MonoBehaviour
    {
        public GameInputAction CurInput;

        public PlayerInput Input { get; private set; }

        public int PlayerIndex { get; set; }

        /// <summary>
        /// 用来区别玩家的颜色
        /// </summary>
        public Color playerColor; 

        public InputModule(PlayerInput playerInput)
        {
            PlayerIndex = playerInput.playerIndex;
            Input = playerInput;
        }


        /// <summary>
        /// 当前的输入设备
        /// </summary>
        private InputDevice currentDevice;

        /// <summary>
        /// 当前的输入设备
        /// </summary>
        public InputDevice InputDevice
        {
            get
            {
                return currentDevice;
            }
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


        /// <summary>
        /// 玩家所分配到的输入设备序号
        /// </summary>
        [field: SerializeField] public int InputIndex;
        /// <summary>
        /// 当前输入设备信息
        /// </summary>
        [SerializeField] public InputDeviceData currentDeviceData;


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


        private void Awake()
        {
            // 初始化当前的输入设备
            mouse = Mouse.current;
            keyboard = Keyboard.current;
            gamepad = Gamepad.current;
        }
        void OnEnable()
        {
            InputSystem.onActionChange += DetectCurrentInputDevice;
        }

        void OnDisable()
        {
            InputSystem.onActionChange -= DetectCurrentInputDevice;
        }

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
    }
}