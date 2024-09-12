using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace KFrame.Systems
{
    /// <summary>
    /// 窗口基类
    /// </summary>
    public class UI_WindowBase : MonoBehaviour
    {
        #region 自动获取Control

        private Dictionary<string, List<UIBehaviour>> controlDic = new Dictionary<string, List<UIBehaviour>>();

        protected virtual void Awake()
        {
            FindChildrenControl<Button>();
            FindChildrenControl<Image>();
            FindChildrenControl<Text>();
            FindChildrenControl<Toggle>();
            FindChildrenControl<Slider>();
        }

        /// <summary>
        /// 找到子对象对应控件
        /// </summary>
        /// <typeparam name="T">控件泛型种类</typeparam>
        private void FindChildrenControl<T>() where T : UIBehaviour
        {
            T[] controls = this.GetComponentsInChildren<T>(); //会找到该游戏物体下所有的T类型的组件

            for (int i = 0; i < controls.Length; i++)
            {
                string objName = controls[i].gameObject.name;
                objName = controls[i].gameObject.name;
                if (controlDic.ContainsKey(objName))
                {
                    controlDic[objName].Add(controls[i]);
                }
                else
                {
                    controlDic.Add(objName, new List<UIBehaviour>() { controls[i] });
                }

                if (controls[i] is Button) //当控件是Button
                {
                    (controls[i] as Button).onClick.AddListener(() => { RegisterButtonOnClick(objName); });
                }

                if (controls[i] is Toggle) //当控件是Toggle
                {
                    (controls[i] as Toggle).onValueChanged.AddListener((value) =>
                    {
                        RegisterToggleOnValueChange(objName, value);
                    });
                }
            }
        }

        /// <summary>
        /// 根据名称得到对应的控件脚本
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controlName">控件的GO名称</param>
        /// <returns></returns>
        protected T GetControl<T>(string controlName) where T : UIBehaviour
        {
            if (controlDic.ContainsKey(controlName))
            {
                for (int i = 0; i < controlDic[controlName].Count; i++)
                {
                    if (controlDic[controlName][i] is T)
                    {
                        return controlDic[controlName][i] as T;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 根据类型获取组件
        /// 同GetComponentsInChildren 只是封装了一层来统一API
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T[] GetControls<T>() where T : UIBehaviour
        {
            return GetComponentsInChildren<T>();
        }


        /// <summary>
        /// 注册按钮的点击监听
        /// </summary>
        /// <param name="btnName"></param>
        protected virtual void RegisterButtonOnClick(string btnName) { }

        /// <summary>
        /// Toggle的值改变时的监听
        /// </summary>
        /// <param name="toggleName"></param>
        /// <param name="value"></param>
        protected virtual void RegisterToggleOnValueChange(string toggleName, bool value) { }

        #endregion


        protected bool uiEnable;

        public bool UIEnable
        {
            get => uiEnable;
        }

        protected int currentLayer;

        public int CurrentLayer
        {
            get => currentLayer;
        }

        // 窗口类型
        public Type Type
        {
            get { return this.GetType(); }
        }

        public bool EnableLocalization => localizationOdinConfig == null;

        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void Init() { }


        public void ShowGeneralLogic(int layerNum)
        {
            this.currentLayer = layerNum;
            if (!uiEnable)
            {
                RegisterEventListener();
                // 绑定本地化事件
                LocalizationSystem.RegisterLanguageEvent(UpdateLanguageGeneralLogic);
            }

            OnShow();
            OnUpdateLanguage(LocalizationSystem.LanguageType);
            uiEnable = true;
        }

        /// <summary>
        /// 显示时执行额外内容
        /// </summary>
        protected virtual void OnShow() { }

        /// <summary>
        /// 关闭的基本逻辑
        /// </summary>
        public void CloseGeneralLogic()
        {
            uiEnable = false;
            UnRegisterEventListener();
            LocalizationSystem.UnregisterLanguageEvent(UpdateLanguageGeneralLogic);
            OnClose();
        }

        /// <summary>
        /// 关闭时额外执行的内容
        /// </summary>
        protected virtual void OnClose() { }

        /// <summary>
        /// 注册事件
        /// </summary>
        protected virtual void RegisterEventListener() { }

        /// <summary>
        /// 取消事件
        /// </summary>
        protected virtual void UnRegisterEventListener() { }

        #region 本地化

        /// <summary>
        /// 当本地化配置中不包含指定key时，会自动在全局配置中尝试
        /// </summary>
        [FormerlySerializedAs("localizationConfig")] [SerializeField, LabelText("局部本地化配置"), InfoBox("如果局部配置中没有key 或者没有局部设置 会去GameRoot的全局设置中找Key")]
        public LocalizationOdinConfig localizationOdinConfig;

        protected void UpdateLanguageGeneralLogic(LanguageType languageType)
        {
            OnUpdateLanguage(languageType);
        }

        /// <summary>
        /// 当语言更新时
        /// </summary>
        protected virtual void OnUpdateLanguage(LanguageType languageType) { }

        #endregion
    }
}