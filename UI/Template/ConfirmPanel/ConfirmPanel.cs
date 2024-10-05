//****************** 代码文件申明 ***********************
//* 文件：ConfirmPanel
//* 作者：wheat
//* 创建时间：2024/10/05 18:23:18 星期六
//* 描述：UI设置确认面板
//*******************************************************

using System;
using TMPro;
using UnityEngine;

namespace KFrame.UI
{
    [UIData(typeof(ConfirmPanel), "ConfirmPanel", true, 3)]
    public class ConfirmPanel : UIPanelBase
    {
        #region UI配置


        /// <summary>
        /// 取消按钮
        /// </summary>
        [SerializeField] 
        private KButton cancelBtn;
        /// <summary>
        /// 确认按钮
        /// </summary>
        [SerializeField] 
        private KButton confirmBtn;
        /// <summary>
        /// 显示内容文本
        /// </summary>
        [SerializeField]
        private TMP_Text contentText;
        
        
        #endregion

        #region UI逻辑

        /// <summary>
        /// 取消选项事件
        /// </summary>
        private Action onCancel;
        /// <summary>
        /// 确认选项事件
        /// </summary>
        private Action onConfirm;

        #endregion

        protected override void Awake()
        {
            base.Awake();
            
            //按键事件注册
            cancelBtn.OnClick.AddListener(OnClickCancel);
            confirmBtn.OnClick.AddListener(OnClickConfirm);
        }

        #region UI操作
        
        public override void OnPressESC()
        {
            base.OnPressESC();
            
            onCancel?.Invoke();
            ClearActions();
        }
        /// <summary>
        /// 显示确认面板
        /// </summary>
        /// <param name="switchPanelKey">关闭后切换的面板</param>
        /// <param name="contentKey">显示内容的key</param>
        /// <param name="confirmKey">确认的key</param>
        /// <param name="cancelKey">取消的key</param>
        /// <param name="cancelAction">取消事件</param>
        /// <param name="confirmAction">确认事件</param>
        public static void ShowPanel(string switchPanelKey, string contentKey, string confirmKey, string cancelKey,
            Action cancelAction, Action confirmAction)
        {
            
        }

        #endregion

        #region UI事件
        
        /// <summary>
        /// 点击了取消
        /// </summary>
        private void OnClickCancel()
        {
            OnPressESC();
        }
        /// <summary>
        /// 点击了确认
        /// </summary>
        private void OnClickConfirm()
        {
            onConfirm?.Invoke();
            ClearActions();
            OnPressESC();
        }
        /// <summary>
        /// 清空事件
        /// </summary>
        private void ClearActions()
        {
            onCancel = null;
            onConfirm = null;
        }

        #endregion

    }
}

