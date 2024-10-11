//****************** 代码文件申明 ***********************
//* 文件：ConfirmPanel
//* 作者：wheat
//* 创建时间：2024/10/05 18:23:18 星期六
//* 描述：UI设置确认面板
//*******************************************************

using System;
using KFrame.Utilities;
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
        /// <summary>
        /// 确认按钮文本
        /// </summary>
        [SerializeField]
        private TMP_Text confirmText;
        /// <summary>
        /// 取消按钮文本
        /// </summary>
        [SerializeField]
        private TMP_Text cancelText;
        
        
        #endregion

        #region UI逻辑

        /// <summary>
        /// 面板初始化数据
        /// </summary>
        private static ConfirmPanelData panelInitData;
        /// <summary>
        /// 取消选项事件
        /// </summary>
        private Action onCancel;
        /// <summary>
        /// 确认选项事件
        /// </summary>
        private Action onConfirm;

        #endregion

        #region 生命周期

        protected override void Awake()
        {
            base.Awake();
            
            //按键事件注册
            cancelBtn.OnClick.AddListener(OnClickCancel);
            confirmBtn.OnClick.AddListener(OnClickConfirm);
        }

        protected override void OnShow()
        {
            base.OnShow();
            
            LoadData();
        }
        /// <summary>
        /// 加载配置数据
        /// </summary>
        private void LoadData()
        {
            //如果为空那就返回
            if(panelInitData == null) return;
            
            //更新UI
            contentText.text = LocalizationSystem.GetUITextInCurLanguage(panelInitData.ContentKey);
            confirmText.text = LocalizationSystem.GetUITextInCurLanguage(panelInitData.ConfirmKey);
            cancelText.text = LocalizationSystem.GetUITextInCurLanguage(panelInitData.CancelKey);
            
            //注册事件
            onConfirm = panelInitData.ConfirmAction;
            onCancel = panelInitData.CancelAction;
            
            //设置切换面板
            switchPanelKey = panelInitData.SwitchPanelKey;
            
            //读取完后释放资源
            panelInitData.Dispose();
            panelInitData = null;
        }

        #endregion

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
        /// <param name="panel">原来的面板</param>
        /// <param name="switchPanelKey">关闭后切换的面板</param>
        /// <param name="contentKey">显示内容的key</param>
        /// <param name="confirmKey">确认的key</param>
        /// <param name="cancelKey">取消的key</param>
        /// <param name="cancelAction">取消事件</param>
        /// <param name="confirmAction">确认事件</param>
        public static void SwitchToConfirmPanel(UIPanelBase panel, string switchPanelKey, string contentKey, string confirmKey, string cancelKey,
            Action cancelAction, Action confirmAction)
        {
            //设置初始化数据
            panelInitData = new ConfirmPanelData(switchPanelKey, contentKey, confirmKey, cancelKey, cancelAction,
                confirmAction);
            
            //切换面板
            panel.SwitchToThisPanel<ConfirmPanel>(false);
        }
        /// <summary>
        /// 显示确认面板
        /// </summary>
        /// <param name="panel">要切换的面板</param>
        /// <param name="cancelAction">取消事件</param>
        /// <param name="confirmAction">确认事件</param>
        /// <typeparam name="T">切换的面板的类型</typeparam>
        public static void SwitchToConfirmPanel<T>(T panel, Action cancelAction, Action confirmAction) where T : UIPanelBase
        {
            string prefix = $"ui_{typeof(T).GetNiceName()}_";
            SwitchToConfirmPanel(panel, panel.Parent == null ? "" : panel.Parent.UIKey, prefix + "Content",
                prefix + "Confirm", prefix + "Cancel", cancelAction, confirmAction);
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

