//****************** 代码文件申明 ************************
//* 文件：KInputSystem                                       
//* 作者：wheat
//* 创建时间：2024/09/29 12:59:05 星期日
//* 功能：管理输入
//*****************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KFrame.Systems
{
    public static class KInputSystem
    {
        /// <summary>
        /// 输入模块
        /// </summary>
        public static readonly Dictionary<int, InputModule> InputModuleDic = new();
        /// <summary>
        /// 主输入设备
        /// 通常为玩家1
        /// </summary>
        private static InputModule mainInputModule;

        /// <summary>
        /// 当前的输入方案
        /// </summary>
        private static InputDeviceScheme inputScheme;

        /// <summary>
        /// 当前的输入方案
        /// </summary>
        public static InputDeviceScheme InputScheme => inputScheme;

        
        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            
        }
        /// <summary>
        /// 注册输入模块
        /// </summary>
        /// <param name="module">输入模块</param>
        public static void RegisterInputModule(this InputModule module)
        {
            //如果为空那就返回
            if(module == null) return;

            //获取id，然后更新
            int id = InputModuleDic.Count;
            module.InputIndex = id;
            InputModuleDic[module.InputIndex] = module;
            //如果是0那就设为主设备
            if (id == 0)
            {
                BindMainInputDevice(module);
            }
        }
        /// <summary>
        /// 注销输入模块
        /// </summary>
        /// <param name="module">输入模块</param>
        public static void UnRegisterInputModule(this InputModule module)
        {
            //如果为空那就返回
            if(module == null) return;

            InputModuleDic.Remove(module.InputIndex);
        }

        #region 设备切换相关
        
        /// <summary>
        /// 切换到鼠标设备
        /// </summary>
        private static void OnSwitchMouse()
        {
            inputScheme = InputDeviceScheme.KeyboardAndMouse;
        }
        /// <summary>
        /// 切换到鼠标设备
        /// </summary>
        private static void OnSwitchKeyboard()
        {
            inputScheme = InputDeviceScheme.KeyboardAndMouse;
        }
        /// <summary>
        /// 切换到鼠标设备
        /// </summary>
        private static void OnSwitchGamepad()
        {
            inputScheme = InputDeviceScheme.Gamepad;
        }
        /// <summary>
        /// 绑定主输入设备
        /// </summary>
        private static void BindMainInputDevice(InputModule inputModule)
        {
            mainInputModule = inputModule;
            mainInputModule.OnSwitchMouse += OnSwitchMouse;
            mainInputModule.OnSwitchKeyboard += OnSwitchKeyboard;
            mainInputModule.OnSwitchGamepad += OnSwitchGamepad;
        }
        #endregion
        
    }
}