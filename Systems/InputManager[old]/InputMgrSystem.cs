using UnityEngine;

namespace KFrame.Systems
{
    /// <summary>
    /// 基于旧输入系统InputManager的输入管理，目前仅仅用来方便进行功能测试
    /// </summary>
    public static class InputMgrSystem
    {
        private static bool isStart = false;

        /// <summary>
        /// 是否开启输入检测
        /// </summary>
        /// <param name="isOpen"></param>
        public static void StartOrEndCheck(bool isOpen)
        {
            isStart = isOpen;
        }

        public static void Init()
        {
            //构造函数中添加Update监听
            MonoSystem.AddUpdateListener(InputUpdate);
        }

        /// <summary>
        /// 目前主要是只监听一些测试用按键
        /// </summary>
        private static void InputUpdate()
        {
            //没有开启输入检测 就停止检测 直接return
            if (!isStart)
            {
                return;
            }

            //Debug.Log("Checking");
            CheckKeyCode(KeyCode.P);
        }

        /// <summary>
        /// 检测按键抬起按下 分发的事件
        /// </summary>
        /// <param name="key"></param>
        private static void CheckKeyCode(KeyCode key)
        {
            if (Input.GetKeyDown(key))
            {
                EventBroadCastSystem.EventTrigger<object>("某键按下", key);
            }

            if (Input.GetKeyUp(key))
            {
                EventBroadCastSystem.EventTrigger<object>("某键抬起", key);
            }
        }
    }
}