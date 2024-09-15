using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using KFrame.UI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace KFrame.Systems
{
    /// <summary>
    /// 本地化收集者
    /// </summary>
    public class UILocalizationCollector : SerializedMonoBehaviour
    {
        [FormerlySerializedAs("localizationConfig")] [LabelText("局部配置信息"), Tooltip("如果局部配置信息为空,会自动读取全局本地化信息")]
        public LocalizationOdinConfig localizationOdinConfig;

        [LabelText("当前语言"),SerializeField]
        private LanguageType currentLanguage;

        [TableList]
        public List<UILocalizationData> localizationDataList = new List<UILocalizationData>();

        private Action<object, string> analyzer;

        private void Reset()
        {
            UIBase window = GetComponent<UIBase>();
            if (window != null && window.localizationOdinConfig != null)
            {
                localizationOdinConfig = window.localizationOdinConfig;
            }
        }

        private void OnEnable()
        {
            LocalizationSystem.RegisterLanguageEvent(OnUpdateLanguage);
            OnUpdateLanguage(LocalizationSystem.LanguageType);
        }

        private void OnDisable()
        {
            LocalizationSystem.UnregisterLanguageEvent(OnUpdateLanguage);
        }

        private void OnUpdateLanguage(LanguageType type)
        {
            //如果没有局部配置，那就取全局配置
            if(localizationOdinConfig ==null)
            {
                localizationOdinConfig = LocalizationSystem.GetGlobalConfig();
            }

            //如果当前语言已经是该语言了就返回
            if(currentLanguage== type) return;

            foreach (UILocalizationData item in localizationDataList)
            {
                Analysis(item.component, item.key, type);
            }

            currentLanguage = type;
        }

        /// <summary>
        /// 初始化分析器
        /// 分析器：根据组件的类型不同返回不同的数据
        /// </summary>
        /// <param name="analyzer"></param>
        public void InitAnalyzer(Action<object, string> analyzer)
        {
            this.analyzer = analyzer;
        }

        /// <summary>
        /// 优先采用外部传进来的解析器
        /// 如果没有则采用内部简单解析器，优先在本地配置中寻找，如果没有则在全局配置中寻找
        /// </summary>
        public void Analysis(MaskableGraphic component, string key, LanguageType languageType)
        {
            if (component == null) return;
            if (analyzer != null)
            {
                analyzer.Invoke(component, key);
                return;
            }

            // 内置简单解析
            if (component is Text)
            {
                LocalizationStringData data = null;
                if (localizationOdinConfig != null)
                    data = localizationOdinConfig.GetContent<LocalizationStringData>(key, languageType);
                if (data == null) data = LocalizationSystem.GetContent<LocalizationStringData>(key, languageType);
                if (data != null)
                {
                    Text _text = (Text)component;
                    _text.text = data.content;

                    if (localizationOdinConfig != null)
                    {
                        //如果有配置字体大小
                        if (localizationOdinConfig.LanguageFontSize.TryGetValue(currentLanguage, out var curSize) &&
                            localizationOdinConfig.LanguageFontSize.TryGetValue(languageType, out var fontSize))
                        {
                            //那就调整一下字体大小
                            _text.fontSize = Mathf.RoundToInt((float)_text.fontSize / curSize * fontSize);
                        }
                    }
                }
            }
            else if (component is Image)
            {
                LocalizationImageData data = null;
                if (localizationOdinConfig != null)
                    data = localizationOdinConfig.GetContent<LocalizationImageData>(key, languageType);
                if (data == null) data = LocalizationSystem.GetContent<LocalizationImageData>(key, languageType);
                if (data != null) ((Image)component).sprite = data.content;
            }
        }

#if UNITY_EDITOR

        #region Inspector编辑方法

        [Button("一键配置子集中的可本地化UI",30)]
        protected void FindLocalizationUI()
        {
            //新建一个list
            localizationDataList = new List<UILocalizationData>();
            //从子集获取可以本地化的UI
            LocalizationUITag[] uis = GetComponentsInChildren<LocalizationUITag>();

            //获取本地化设置
            LocalizationOdinConfig odinConfig;
            //如果没有局部设置那就取全局的
            if(localizationOdinConfig==null)
            {
                odinConfig = ResSystem.LoadAsset<LocalizationOdinConfig>("GlobalLocalizationConfig");
            }
            //有的话就取局部的
            else
            {
                odinConfig = localizationOdinConfig;
            }

            //防空
            if(odinConfig.config==null)
            {
                odinConfig.config = new Dictionary<string, Dictionary<LanguageType, LocalizationDataBase>>();
            }

            //遍历每一个UI
            foreach (var ui in uis)
            {
                //新建一个设置
                UILocalizationData data = new UILocalizationData();
                MaskableGraphic component = ui.GetComponent<MaskableGraphic>();
                data.component = component;
                data.key = ui.Key;

                // 将配置存放到SO文件中
                if (component is Text)
                {
                    if(ui.CNstring!=string.Empty)
                    {
                        Dictionary<LanguageType, LocalizationDataBase> dic;
                        if (odinConfig.config.ContainsKey(ui.Key))
                        {
                            dic = odinConfig.config[ui.Key];
                        }
                        else
                        {
                            dic = new Dictionary<LanguageType, LocalizationDataBase>();
                            odinConfig.config[ui.Key] = dic;
                        }

                        LocalizationStringData cn = new LocalizationStringData();
                        cn.content = ui.CNstring;
                        LocalizationStringData ct = new LocalizationStringData();
                        ct.content = ui.CTstring;
                        LocalizationStringData en = new LocalizationStringData();
                        en.content = ui.ENstring;
                        LocalizationStringData jp = new LocalizationStringData();
                        jp.content = ui.JPstring;

                        dic[LanguageType.SimplifiedChinese] = cn;
                        dic[LanguageType.TraditionalChinese] = ct;
                        dic[LanguageType.English] = en;
                        dic[LanguageType.Japanese] = jp;
                    }

                }
                else if (component is Image)
                {
                    if (ui.CNimg != null)
                    {
                        Dictionary<LanguageType, LocalizationDataBase> dic;
                        if (odinConfig.config.ContainsKey(ui.Key))
                        {
                            dic = odinConfig.config[ui.Key];
                        }
                        else
                        {
                            dic = new Dictionary<LanguageType, LocalizationDataBase>();
                            odinConfig.config[ui.Key] = dic;
                        }

                        LocalizationImageData cn = new LocalizationImageData();
                        cn.content = ui.CNimg;
                        LocalizationImageData ct = new LocalizationImageData();
                        ct.content = ui.CTimg;
                        LocalizationImageData en = new LocalizationImageData();
                        en.content = ui.ENimg;
                        LocalizationImageData jp = new LocalizationImageData();
                        jp.content = ui.JPimg;

                        dic[LanguageType.SimplifiedChinese] = cn;
                        dic[LanguageType.TraditionalChinese] = ct;
                        dic[LanguageType.English] = en;
                        dic[LanguageType.Japanese] = jp;
                    }
                }

                localizationDataList.Add(data);
            }

            //保存
            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty(odinConfig);
        }
        /// <summary>
        /// 只是同步一下配置中的数据
        /// </summary>
        [Button("一键更新子集中的本地化数据",30)]
        protected void FillLocalizationUI()
        {
            //从子集获取可以本地化的UI
            LocalizationUITag[] uis = GetComponentsInChildren<LocalizationUITag>();

            //获取本地化设置
            LocalizationOdinConfig odinConfig;
            //如果没有局部设置那就取全局的
            if (localizationOdinConfig == null)
            {
                odinConfig = ResSystem.LoadAsset<LocalizationOdinConfig>("GlobalLocalizationConfig");
            }
            //有的话就取局部的
            else
            {
                odinConfig = localizationOdinConfig;
            }

            //防空
            if (odinConfig.config == null)
            {
                odinConfig.config = new Dictionary<string, Dictionary<LanguageType, LocalizationDataBase>>();
            }

            //遍历每个UI
            foreach (var ui in uis)
            {

                if(odinConfig.config.TryGetValue(ui.Key,out Dictionary<LanguageType,LocalizationDataBase> dic))
                {
                    //简体中文
                    if (dic.TryGetValue(LanguageType.SimplifiedChinese,out LocalizationDataBase dataCN))
                    {
                        if(dataCN is LocalizationStringData)
                        {
                            ui.CNstring = ((LocalizationStringData)dataCN).content;
                        }
                        else if(dataCN is LocalizationImageData)
                        {
                            ui.CNimg = ((LocalizationImageData)dataCN).content;
                        }
                    }

                    //繁体中文
                    if (dic.TryGetValue(LanguageType.TraditionalChinese, out LocalizationDataBase dataCT))
                    {
                        if (dataCT is LocalizationStringData)
                        {
                            ui.CTstring = ((LocalizationStringData)dataCT).content;
                        }
                        else if (dataCT is LocalizationImageData)
                        {
                            ui.CTimg = ((LocalizationImageData)dataCT).content;
                        }
                    }
                    
                    //英文
                    if (dic.TryGetValue(LanguageType.English, out LocalizationDataBase dataEN))
                    {
                        if (dataEN is LocalizationStringData)
                        {
                            ui.ENstring = ((LocalizationStringData)dataEN).content;
                        }
                        else if (dataEN is LocalizationImageData)
                        {
                            ui.ENimg = ((LocalizationImageData)dataEN).content;
                        }
                    }

                    //日语
                    if (dic.TryGetValue(LanguageType.Japanese, out LocalizationDataBase dataJP))
                    {
                        if (dataJP is LocalizationStringData)
                        {
                            ui.JPstring = ((LocalizationStringData)dataJP).content;
                        }
                        else if (dataJP is LocalizationImageData)
                        {
                            ui.JPimg = ((LocalizationImageData)dataJP).content;
                        }
                    }
                }

                //保存数据
                EditorUtility.SetDirty(ui);

            }
        }

        #endregion

#endif

    }

    public class UILocalizationData
    {
        public MaskableGraphic component;
        public string key;
    }
}