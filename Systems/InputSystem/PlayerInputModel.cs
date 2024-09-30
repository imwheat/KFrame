//****************** 代码文件申明 ***********************
//* 文件：PlayerInputModel
//* 作者：wheat
//* 创建时间：2024/09/30 08:21:37 星期一
//* 描述：存储玩家输入的Model
//*******************************************************

using UnityEngine;

namespace KFrame.Systems
{
    public class PlayerInputModel
    {
        #region 输入参数

        /// <summary>
        /// 移动输入
        /// </summary>
        public Vector2 Move;
        /// <summary>
        /// 鼠标滚轮输入
        /// </summary>
        public Vector2 Scroll;
        /// <summary>
        /// 攻击输入
        /// </summary>
        public bool IsAttack;
        /// <summary>
        /// 交互输入
        /// </summary>
        public bool IsInteract;
        /// <summary>
        /// 切换上一个物品
        /// </summary>
        public bool SwitchLastItem;
        /// <summary>
        /// 切换下一个物品
        /// </summary>
        public bool SwitchNextItem;

        #endregion
        
        /// <summary>
        /// 重置玩家输入
        /// </summary>
        public void ResetPlayerInput()
        {
            Move = Vector2.zero;
            Scroll = Vector2.zero;
            IsAttack = false;
            IsInteract = false;
            SwitchLastItem = false;
            SwitchNextItem = false;
            Scroll = Vector2.zero;
        }
    }
}

