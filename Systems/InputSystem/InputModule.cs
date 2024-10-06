//****************** 代码文件申明 ************************
//* 文件：InputModule                      
//* 作者：wheat
//* 创建时间：2024/09/29 12:23:06 星期日
//* 功能：输入模块的基类
//*****************************************************

using System;
using KFrame.UI;
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
        public GameInputAction InputAction;
        /// <summary>
        /// 所分配到的输入设备序号
        /// </summary>
        public int InputIndex;
        /// <summary>
        /// 按键配置MapId
        /// </summary>
        [SerializeField]
        private int inputActionMapId;

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
            //初始化
            Init();

        }
        /// <summary>
        /// 初始化
        /// </summary>
        protected virtual void Init()
        {
            InputAction = new GameInputAction();
            
            //初始化当前的输入设备
            mouse = Mouse.current;
            keyboard = Keyboard.current;
            gamepad = Gamepad.current;
            
            //注册Module
            this.RegisterInputModule();
            
            //注册按键输入
            RegisterInput();
            InputAction.Enable();
        }
        protected void OnEnable()
        {
            InputSystem.onActionChange += DetectCurrentInputDevice;
            ReloadInputOverride();
        }

        protected void OnDisable()
        {
            InputSystem.onActionChange -= DetectCurrentInputDevice;
        }

        protected void OnDestroy()
        {
            //注销Module
            this.UnRegisterInputModule();
            //释放资源
            InputAction.Dispose();
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

        #region 输入配置
    
        /// <summary>
        /// 重新加载按键配置
        /// </summary>
        private void ReloadInputOverride()
        {
            //遍历每一个按键，然后读取数据
            foreach (var action in InputAction.asset.actionMaps[inputActionMapId])
            {
                action.LoadKeySet(InputIndex);
            }
        }
        /// <summary>
        /// 选择性的注册输入事件
        /// </summary>
        /// <param name="inputAction">操作事件</param>
        /// <param name="action">输入事件</param>
        /// <param name="started">started的时候调用</param>
        /// <param name="performed">performed的时候调用</param>
        /// <param name="canceled">canceled的时候调用</param>
        protected void RegisterInputEvent(InputAction inputAction, Action<InputAction.CallbackContext> action,
            bool started = false, bool performed = false, bool canceled = false)
        {
            if (started) inputAction.started += action;
            if (performed) inputAction.performed += action;
            if (canceled) inputAction.canceled += action;
        }

        /// <summary>
        /// 注销所有输入事件
        /// </summary>
        /// <param name="inputAction">操作事件</param>
        /// <param name="action">输入事件</param>
        protected void UnRegisterInputEvent(InputAction inputAction, Action<InputAction.CallbackContext> action)
        {
            inputAction.started -= action;
            inputAction.performed -= action;
            inputAction.canceled -= action;
        }
                
        /// <summary>
        /// 注册输入
        /// </summary>
        protected virtual void RegisterInput()
        {
            
        }
                
        /// <summary>
        /// 注销输入
        /// </summary>
        protected virtual void UnRegisterInput()
        {
            
        }

        
        #endregion
        
    }
}