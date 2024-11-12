//****************** 代码文件申明 ************************
//* 文件：UIBase                                       
//* 作者：wheat
//* 创建时间：2024/09/14 17:56:35 星期六
//* 描述：UI的基类
//*****************************************************
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR

using UnityEditor;

#endif

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
        protected int curLanguage = -1;
        /// <summary>
        /// 本地化配置列表
        /// </summary>
        [SerializeField]
        protected List<UILocalizationData> localiztionSets;

        #endregion

        #region 生命周期

        protected virtual void Awake()
        {
            //注册事件
            RegisterEventListener();
            //更新语言
            OnUpdateLanguage(LocalizationSystem.UILanguageType);
        }

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
        }

        #endregion

        #region 通用基础虚方法

        /// <summary>
        /// 初始化在显示的时候执行一次
        /// </summary>
        public virtual void Init(UIData data)
        {
            currentLayer = data.LayerNum;
            uiKey = data.UIKey;
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
            if (!gameObject.activeSelf) return;
            
            UISystem.Close(this);
        }

        #endregion

        /// <summary>
        /// 注册事件
        /// </summary>
        protected virtual void RegisterEventListener()
        {
            LocalizationSystem.RegisterUILanguageEvent(OnUpdateLanguage);
        }

        /// <summary>
        /// 取消事件
        /// </summary>
        protected virtual void UnRegisterEventListener() 
        { 
            LocalizationSystem.UnregisterUILanguageEvent(OnUpdateLanguage);
        }
        

        #region 本地化

        /// <summary>
        /// 当语言更新时
        /// </summary>
        protected virtual void OnUpdateLanguage(int languageType)
        {
            //如果当前语言类型和要修改的一样那就返回
            if(curLanguage == languageType) return;
            curLanguage = languageType;
            UpdateChildLanguage();
        }
        /// <summary>
        /// 更新子集的语言
        /// </summary>
        protected void UpdateChildLanguage()
        {
            //遍历更新每个UI
            foreach (UILocalizationData uiLocalizationData in localiztionSets)
            {
                uiLocalizationData.UpdateUILanguage(curLanguage);                
            }
        }
        #if UNITY_EDITOR
        /// <summary>
        /// 编辑器使用，临时的
        /// 用于搜索Graphic绑定的数据
        /// </summary>
        protected Dictionary<Graphic, UILocalizationData> uiDataDic;

        protected Dictionary<Graphic, UILocalizationData> UIDataDic
        {
            get
            {
                if (uiDataDic == null)
                {
                    InitUIDataDic();
                }

                return uiDataDic;
            }
        }
        /// <summary>
        /// 初始化字典
        /// </summary>
        private void InitUIDataDic()
        {
            uiDataDic = new();
            for (int i = localiztionSets.Count -1; i >= 0; i--)
            {
                if (localiztionSets[i] == null || localiztionSets[i].Component == null)
                {
                    localiztionSets.RemoveAt(i);
                }
                else
                {
                    uiDataDic[localiztionSets[i].Component] = localiztionSets[i];
                }
            }
        }
        /// <summary>
        /// 编辑器使用
        /// 搜寻这个UI下可以本地化配置的项
        /// </summary>
        public void CollectLocalizationUI()
        {
             //从子集获取可以本地化的UI
            Graphic[] uis = GetComponentsInChildren<Graphic>();

            //遍历每个UI
            foreach (var ui in uis)
            {
                //如果已经访问过了，那就跳过
                if (UIDataDic.ContainsKey(ui)) continue;
                var data = new UILocalizationData("", ui);
                UIDataDic.Add(ui, data);

                //添加到本地化列表
                localiztionSets.Add(data);

            }
            
            //保存
            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty(this.gameObject);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// 获取component对应的数据
        /// </summary>
        /// <returns>找不到就新建</returns>
        public UILocalizationData GetUILocalizationData(Graphic component, string key = "")
        {
            if (component == null) return null;

            //如果已经有了，那就返回
            if (UIDataDic.TryGetValue(component, out UILocalizationData data))
            {
                return data;
            }
            //没有的话那就新建然后添加返回
            else
            {
                data = new UILocalizationData(key, component);
                UIDataDic[component] = data;
                localiztionSets.Add(data);
                
                //保存
                EditorUtility.SetDirty(this);
                return data;
            }
        }
        
        #endif
        
        #endregion
    }
}