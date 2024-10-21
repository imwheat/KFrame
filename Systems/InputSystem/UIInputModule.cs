//****************** 代码文件申明 ***********************
//* 文件：UIInputModule
//* 作者：wheat
//* 创建时间：2024/09/30 08:37:43 星期一
//* 描述：
//*******************************************************

using System;
using KFrame.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KFrame.Systems
{
    public class UIInputModule: IDisposable
    {

        #region 输入配置

        /// <summary>
        /// 输入配置
        /// </summary>
        private readonly UIInputAction inputAction = new();

        #endregion
        
        #region 设备参数

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
        public Action OnSwitchMouse = null;
        /// <summary>
        /// 切换到键盘的事件
        /// </summary>
        public Action OnSwitchKeyboard = null;
        /// <summary>
        /// 切换到手柄的事件
        /// </summary>
        public Action OnSwitchGamepad = null;

        #endregion

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            //初始化当前的输入设备
            mouse = Mouse.current;
            keyboard = Keyboard.current;
            gamepad = Gamepad.current;
            
            //注册按键输入
            RegisterInput();
            inputAction.Enable();
        }

        #region 开关控制

        /// <summary>
        /// 启用输入控制
        /// </summary>
        public void EnableInput()
        {
            inputAction.Enable();
        }
        /// <summary>
        /// 禁用输入控制
        /// </summary>
        public void DisableInput()
        {
            inputAction.Disable();
        }

        #endregion
        
        #region 输入配置
      
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
        private void RegisterInput()
        {
            RegisterInputEvent(inputAction.UI.Navigate, OnNavigation, true, true,true);
            RegisterInputEvent(inputAction.UI.Submit, OnSubmit, false, true,true);
            RegisterInputEvent(inputAction.UI.Interact, OnSubmit, true, true,true);
            RegisterInputEvent(inputAction.UI.Esc, OnPressEsc, true);
        }


        /// <summary>
        /// 注销输入
        /// </summary>
        private void UnRegisterInput()
        {
            UnRegisterInputEvent(inputAction.UI.Navigate, OnNavigation);
            UnRegisterInputEvent(inputAction.UI.Submit, OnSubmit);
            UnRegisterInputEvent(inputAction.UI.Esc, OnPressEsc);
        }

        
        #endregion

        #region 按键输入事件
        
        /// <summary>
        /// 导航
        /// </summary>
        /// <param name="context">输入事件</param>
        private void OnNavigation(InputAction.CallbackContext context)
        {
            //如果当前选择UI为空或者不可交互，按下导航按键的时候会选择当前界面的默认UI
            if (UISelectSystem.CurSelectUI == null || !UISelectSystem.CurSelectUI.IsInteractable() || !UISelectSystem.CurSelectUI.IsActive())
            {
                UISelectSystem.SelectDefaultUI();
            }
        }
        /// <summary>
        /// 按下确认/交互键
        /// </summary>
        /// <param name="context">输入事件</param>
        private void OnSubmit(InputAction.CallbackContext context)
        {
            if (DialogueSystem.DialogueSystem.InDialogue && context.ReadValueAsButton())
            {
                DialogueSystem.DialogueSystem.DialoguePanel?.Next();
            }
        }
        /// <summary>
        /// Esc
        /// </summary>
        /// <param name="context">输入事件</param>
        private void OnPressEsc(InputAction.CallbackContext context)
        {
            if (context.ReadValueAsButton())
            {
                UISelectSystem.PressEsc();
            }
        }
        
        #endregion

        #region 设备更新

        /// <summary>
        /// 更新输入设备
        /// </summary>
        private void UpdateInputDevice()
        {
            switch (currentDevice)
            {
                case UnityEngine.InputSystem.Mouse:
                    OnSwitchMouse?.Invoke();
                    break;
                case UnityEngine.InputSystem.Keyboard:
                    OnSwitchKeyboard?.Invoke();
                    break;
                case UnityEngine.InputSystem.Gamepad:
                    OnSwitchGamepad?.Invoke();
                    break;
            }
        }

        #endregion
        
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            inputAction.Dispose();
        }
    }
}

