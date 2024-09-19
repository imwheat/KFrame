//****************** 代码文件申明 ************************
//* 文件：UIBase                                       
//* 作者：wheat
//* 创建时间：2024/09/14 17:56:35 星期六
//* 描述：UI的基类
//*****************************************************
using System;
using System.Collections.Generic;
using KFrame.Systems;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace KFrame.UI
{
    /// <summary>
    /// 窗口基类
    /// </summary>
    public class UIBase : MonoBehaviour
    {
        #region 参数

        /// <summary>
        /// 层级
        /// </summary>
        protected int currentLayer;
        /// <summary>
        /// 层级
        /// </summary>
        public int CurrentLayer
        {
            get => currentLayer;
        }
        /// <summary>
        /// Key
        /// </summary>
        protected string uiKey;
        /// <summary>
        /// Key
        /// </summary>
        public string UIKey
        {
            get => uiKey;
        }
        /// <summary>
        /// 窗口类型
        /// </summary>
        public Type Type
        {
            get { return this.GetType(); }
        }
        /// <summary>
        /// 当前的语言类型
        /// </summary>
        protected LanguageType curLanguage;

        #endregion

        #region 生命周期

        protected virtual void OnEnable()
        {
            OnShow();
        }

        protected virtual void OnDisable()
        {
            OnClose();
        }

        protected virtual void OnDestroy()
        {
            UnRegisterEventListener();
            LocalizationSystem.UnregisterLanguageEvent(UpdateLanguageGeneralLogic);
        }

        #endregion

        #region 通用基础虚方法

        /// <summary>
        /// 初始化在创建额时候执行一次
        /// </summary>
        public virtual void Init(UIData data)
        {
            currentLayer = data.LayerNum;
            uiKey = data.UIKey;
            
            RegisterEventListener();
            // 绑定本地化事件
            LocalizationSystem.RegisterLanguageEvent(UpdateLanguageGeneralLogic);
            OnUpdateLanguage(LocalizationSystem.LanguageType);

        }
        /// <summary>
        /// 显示时执行额外内容
        /// </summary>
        protected virtual void OnShow() { }
        /// <summary>
        /// 关闭时额外执行的内容
        /// </summary>
        protected virtual void OnClose() { }

        #endregion

        #region UI基础调用

        /// <summary>
        /// 显示UI界面
        /// </summary>
        public void Show()
        {
            //如果已经打开了那就返回
            if(gameObject.activeSelf) return;

            UISystem.Show(this);
        }
        /// <summary>
        /// 关闭窗口
        /// </summary>
        public void Close()
        {
            //如果已经关掉了那就不用再调用
            if(!gameObject.activeSelf) return;
            
            UISystem.Close(this);
        }

        #endregion

        /// <summary>
        /// 注册事件
        /// </summary>
        protected virtual void RegisterEventListener() { }

        /// <summary>
        /// 取消事件
        /// </summary>
        protected virtual void UnRegisterEventListener() { }
        

        #region 本地化

        protected void UpdateLanguageGeneralLogic(LanguageType languageType)
        {
            OnUpdateLanguage(languageType);
        }

        /// <summary>
        /// 当语言更新时
        /// </summary>
        protected virtual void OnUpdateLanguage(LanguageType languageType)
        {
            //如果当前语言类型和要修改的一样那就返回
            if(curLanguage == languageType) return;
            curLanguage = languageType;
        }

        #endregion
    }
}