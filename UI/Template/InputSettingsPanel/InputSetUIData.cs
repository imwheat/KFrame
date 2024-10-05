//****************** 代码文件申明 ***********************
//* 文件：InputSetUIData
//* 作者：wheat
//* 创建时间：2024/10/05 09:28:53 星期六
//* 描述：控制设置的UI数据
//*******************************************************


using UnityEngine.InputSystem;

namespace KFrame.UI
{
    [System.Serializable]
    public class InputSetUIData
    {
        /// <summary>
        /// 本地化配置key
        /// </summary>
        public string localizationKey;
        /// <summary>
        /// 绑定的按钮
        /// </summary>
        public InputSetButton bindButton;
        /// <summary>
        /// 绑定的id
        /// </summary>
        public int rebindId;
        /// <summary>
        /// 绑定的输入键位
        /// </summary>
        public InputActionReference bindInput;
    }
}

