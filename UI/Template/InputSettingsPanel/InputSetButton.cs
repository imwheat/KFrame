//****************** 代码文件申明 ***********************
//* 文件：InputSetButton
//* 作者：wheat
//* 创建时间：2024/10/05 11:25:09 星期六
//* 描述：按键设置按钮
//*******************************************************

using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KFrame.UI
{
    public class InputSetButton: KButton
    {
        #region UI配置

        /// <summary>
        /// 配置数据
        /// </summary>
        public InputSetUIData Data;
        /// <summary>
        /// 按键名称文本
        /// </summary>
        public TMP_Text KeyNameText;
        /// <summary>
        /// 按键文本
        /// </summary>
        public TMP_Text KeyText;
        #endregion

        #region UI逻辑
        
        /// <summary>
        /// 要重新绑定的按键
        /// </summary>
        private InputAction actionToRebind => Data.bindInput.action;
        /// <summary>
        /// 绑定按键id
        /// </summary>
        private int id => Data.rebindId;
        private InputActionRebindingExtensions.RebindingOperation rebindingOperation;
        /// <summary>
        /// 修改前的数据
        /// </summary>
        private string prevData;

        #endregion

        #region 生命周期

        protected override void Awake()
        {
            base.Awake();
            
            OnClick.AddListener(RebindKey);
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            
            DisposeOperation();
        }

        #endregion


        #region UI操作

        /// <summary>
        /// 更新UI
        /// </summary>
        public void UpdateUI()
        {
            //防空
            if (actionToRebind.bindings.Count == 0) return;

            KeyText.text = actionToRebind.bindings[id].ToDisplayString();
        }
        /// <summary>
        /// 释放按键设置操作
        /// </summary>
        private void DisposeOperation()
        {
            rebindingOperation?.Dispose();
            rebindingOperation = null;
        }
        /// <summary>
        /// 重新绑定按键
        /// </summary>
        private void RebindKey()
        {
            //如果已经在设置了那就先取消
            rebindingOperation?.Cancel();

            //定义绑定按键方法
            rebindingOperation = actionToRebind.PerformInteractiveRebinding(id)
                //取消时调用
                .OnCancel(
                    operation =>
                    {
                        DisposeOperation();
                        UpdateUI();
                    })
                //完成时调用
                .OnComplete(
                    operation =>
                    {
                        //清除然后更新UI
                        DisposeOperation();
                        UpdateUI();
                    });

            //调用方法
            rebindingOperation.Start();
            KeyText.text = "...";

        }
        /// <summary>
        /// 重置按键
        /// </summary>
        public void ResetKey()
        {
            actionToRebind.RemoveBindingOverride(id);
            SaveRebind();
            UpdateUI();
        }
        /// <summary>
        /// 取消重新绑定
        /// 恢复本次修改前的按键
        /// </summary>
        public void CancelRebind()
        {
            actionToRebind.LoadBindingOverridesFromJson(prevData);
            SaveRebind();
        }
        /// <summary>
        /// 保存按键绑定数据
        /// </summary>
        public void SaveRebind()
        {
            //更新备份数据
            SaveBackup();
            //更新按键设置数据
            InputSetHelper.SetPlayerKeySet(InputSettingsPanel.PlayerIndex, actionToRebind.GetSaveKey(), prevData);
        }
        /// <summary>
        /// 保存备份还原数据
        /// </summary>
        public void SaveBackup()
        {
            prevData = actionToRebind.SaveBindingOverridesAsJson();
        }

        #endregion

        
    }
}

