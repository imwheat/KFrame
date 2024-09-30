//****************** 代码文件申明 ***********************
//* 文件：PlayerInputModule
//* 作者：wheat
//* 创建时间：2024/09/29 14:18:03 星期日
//* 描述：玩家的输入Module
//*******************************************************

using UnityEngine;
using UnityEngine.InputSystem;

namespace KFrame.Systems
{
    public class PlayerInputModule : InputModule
    {
        /// <summary>
        /// 玩家输入数据Model
        /// </summary>
        public PlayerInputModel InputModel;
        
        protected override void Init()
        {
            base.Init();

            InputModel = new PlayerInputModel();
        }

        #region 按键输入注册

        /// <summary>
        /// 注册按键输入
        /// </summary>
        protected override void RegisterInput()
        {
            base.RegisterInput();
            
            RegisterInputEvent(InputAction.Player.Move, OnMove, true, true, true);
            RegisterInputEvent(InputAction.Player.Scroll, OnScroll, true, true, true);
            RegisterInputEvent(InputAction.Player.Attack, OnAttack, true, true, true);
            RegisterInputEvent(InputAction.Player.Interact, OnInteract, true, true, true);
            RegisterInputEvent(InputAction.Player.SwitchLastItem, OnSwitchLastItem, true, true, true);
            RegisterInputEvent(InputAction.Player.SwitchNextItem, OnSwitchNextItem, true, true, true);
            RegisterInputEvent(InputAction.Player.Esc, OnPressESC, true, true, true);
        }
        /// <summary>
        /// 注销按键输入
        /// </summary>
        protected override void UnRegisterInput()
        {
            base.UnRegisterInput();
            
            //注销事件
            UnRegisterInputEvent(InputAction.Player.Move, OnMove);
            UnRegisterInputEvent(InputAction.Player.Scroll, OnScroll);
            UnRegisterInputEvent(InputAction.Player.Attack, OnAttack);
            UnRegisterInputEvent(InputAction.Player.Interact, OnInteract);
            UnRegisterInputEvent(InputAction.Player.SwitchLastItem, OnSwitchLastItem);
            UnRegisterInputEvent(InputAction.Player.SwitchNextItem, OnSwitchNextItem);
            UnRegisterInputEvent(InputAction.Player.Esc, OnPressESC);
            
            //重置玩家输入
            InputModel.ResetPlayerInput();
        }

        #endregion

        #region 按键事件处理

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="context">输入</param>
        private void OnMove(InputAction.CallbackContext context)
        {
            InputModel.Move = context.ReadValue<Vector2>();
        }
        /// <summary>
        /// 滚轮
        /// </summary>
        /// <param name="context">输入</param>
        private void OnScroll(InputAction.CallbackContext context)
        {
            InputModel.Scroll = context.ReadValue<Vector2>();
        }
        /// <summary>
        /// 攻击
        /// </summary>
        /// <param name="context">输入</param>
        private void OnAttack(InputAction.CallbackContext context)
        {
            InputModel.IsAttack = context.ReadValueAsButton();
        }
        /// <summary>
        /// 交互
        /// </summary>
        /// <param name="context">输入</param>
        private void OnInteract(InputAction.CallbackContext context)
        {
            InputModel.IsInteract = context.ReadValueAsButton();
        }
        /// <summary>
        /// 切换上一个物品
        /// </summary>
        /// <param name="context">输入</param>
        private void OnSwitchLastItem(InputAction.CallbackContext context)
        {
            InputModel.SwitchLastItem = context.ReadValueAsButton();
        }
        /// <summary>
        /// 切换下一个物品
        /// </summary>
        /// <param name="context">输入</param>
        private void OnSwitchNextItem(InputAction.CallbackContext context)
        {
            InputModel.SwitchNextItem = context.ReadValueAsButton();
        }
        /// <summary>
        /// 按下ESC
        /// </summary>
        /// <param name="context">输入</param>
        private void OnPressESC(InputAction.CallbackContext context)
        {
            bool v = context.ReadValueAsButton();
        }
        
        #endregion
        
    }
}

