//****************** 代码文件申明 ***********************
//* 文件：LocalizationEditHelper
//* 作者：wheat
//* 创建时间：2024/09/20 19:20:08 星期五
//* 描述：帮助进行UI本地化配置使用的组件
//*******************************************************

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;
using UnityEngine.UI;

#if UNITY_EDITOR
using TMPro;
using UnityEditor;

#endif

namespace KFrame.UI
{
    [ExecuteInEditMode]
    public class LocalizationEditHelper : MonoBehaviour
    {
        #if UNITY_EDITOR
        
        private LanguageType curLanguage;

        /// <summary>
        /// 当前的语言类型
        /// </summary>
        public LanguageType CurLanguage
        {
            get
            {
                return curLanguage;
            }
            set
            {
                if (UISetPropertyUtility.SetStruct<LanguageType>(ref curLanguage, value))
                {
                    UpdateLanguage(value);
                }
            }
        }
        /// <summary>
        /// 本地化的Key
        /// </summary>
        public string Key;
        /// <summary>
        /// 本地化对象
        /// </summary>
        private Graphic target;
        /// <summary>
        /// 本地化的对象
        /// </summary>
        public Graphic Target
        {
            get
            {
                return target;
            }
            set
            {
                if (UISetPropertyUtility.SetClass<Graphic>(ref target, value))
                {
                    //更新绘制类型
                    UpdateDrawType();
                }
            }
        }
        /// <summary>
        /// 本地化文本数据
        /// </summary>
        public LocalizationStringData StringData;
        /// <summary>
        /// 本地化文本字典
        /// </summary>
        private Dictionary<LanguageType, LocalizationStringDataBase> stringDic;
        /// <summary>
        /// 本地化图片数据
        /// </summary>
        public LocalizationImageData ImageData;
        /// <summary>
        /// 本地化图片字典
        /// </summary>
        private Dictionary<LanguageType, LocalizationImageDataBase> imageDic;
        /// <summary>
        /// 是否绘制image的数据
        /// </summary>
        public bool DrawImgData { get; private set; }

        private void Awake()
        {
            if (Target == null)
            {
                Target = GetComponent<Graphic>();
            }
            InitData();
        }

        /// <summary>
        /// 初始化Data
        /// </summary>
        public void InitData()
        {
            //防空初始化
            if (StringData == null)
            {
                StringData = new LocalizationStringData();
            }
            if (ImageData == null)
            {
                ImageData = new LocalizationImageData();
            }

            stringDic = new Dictionary<LanguageType, LocalizationStringDataBase>();
            imageDic = new Dictionary<LanguageType, LocalizationImageDataBase>();
            
            //获取语言的所有类型
            LanguageType[] languages = (LanguageType[])Enum.GetValues(typeof(LanguageType));
            //记录添加数据中没有的语言类型
            HashSet<LanguageType> stringLanguage = new HashSet<LanguageType>();
            HashSet<LanguageType> imageLanguage = new HashSet<LanguageType>();
            for (int i = StringData.Datas.Count - 1; i >= 0; i--)
            {
                if (stringLanguage.Contains(StringData.Datas[i].Language))
                {
                    StringData.Datas.RemoveAt(i);
                }
                else
                {
                    stringLanguage.Add(StringData.Datas[i].Language);
                }

            }
            for (int i = ImageData.Datas.Count - 1; i >= 0; i--)
            {
                if (stringLanguage.Contains(ImageData.Datas[i].Language))
                {
                    ImageData.Datas.RemoveAt(i);
                }
                else
                {
                    stringLanguage.Add(ImageData.Datas[i].Language);
                }
            }
            
            //遍历目前已经有的语言类型
            foreach (LanguageType language in languages)
            {
                //如果数据里面还没有的话那就添加
                if (!stringLanguage.Contains(language))
                {
                    StringData.Datas.Add(new LocalizationStringDataBase(language, ""));
                }
                if (!imageLanguage.Contains(language))
                {
                    ImageData.Datas.Add(new LocalizationImageDataBase(language, null));
                }
            }
            
            //注册字典
            foreach (LocalizationStringDataBase stringData in StringData.Datas)
            {
                stringDic[stringData.Language] = stringData;
            }
            foreach (LocalizationImageDataBase imageData in ImageData.Datas)
            {
                imageDic[imageData.Language] = imageData;
            }
        }
        /// <summary>
        /// 更新绘制类型
        /// </summary>
        private void UpdateDrawType()
        {
            //如果是图片类型的话，那就绘制图片数据
            switch (target)
            {
                case Image img:
                    DrawImgData = true;
                    break;
                default:
                    DrawImgData = false;
                    break;
            }
        }

        public void UpdateLanguage(LanguageType languageType)
        {
            curLanguage = languageType;
            
            switch (target)
            {
                case Image img:
                    img.sprite = imageDic[languageType].Sprite;
                    break;
                case Text text:
                    text.text = stringDic[languageType].Text;
                    break;
                case TMP_Text tmpText:
                    tmpText.text = stringDic[languageType].Text;
                    break;
            }
            
            EditorUtility.SetDirty(target);
            EditorUtility.SetDirty(this);
        }
        
        #endif
        
    }
}

