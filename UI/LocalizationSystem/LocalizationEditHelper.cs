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


        #region 参数

        /// <summary>
        /// 当前的语言类型
        /// </summary>
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
        /// 本地化的key
        /// </summary>
        private string key;

        /// <summary>
        /// 本地化的Key
        /// </summary>
        public string Key
        {
            get => key;
            set
            {
                if (linkedData != null && Parent != null)
                {
                    linkedData.Key = value;
                    EditorUtility.SetDirty(Parent);
                }
                key = value;
            }
        }
        /// <summary>
        /// 管理这个UI本地化的父级
        /// </summary>
        public UIBase Parent;
        /// <summary>
        /// 绑定的本地化数据
        /// </summary>
        private UILocalizationData linkedData;
        /// <summary>
        /// 本地化对象
        /// </summary>
        [SerializeField]
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

        #endregion


        #region 初始化

        private void Awake()
        {
            if (Target == null)
            {
                Target = GetComponent<Graphic>();
            }
            
            if (Parent == null)
            {
                Parent = GetComponentInParent<UIBase>();
            }
            RefreshLinkedData();
            
            InitData();
        }
        /// <summary>
        /// 刷新一下绑定数据
        /// </summary>
        public void RefreshLinkedData()
        {
            if (Parent != null)
            {
                linkedData = Parent.GetUILocalizationData(target);
                key = linkedData.Key;
            }
            else
            {
                linkedData = null;
            }
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

        #endregion


        #region 更新数据

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

        #endregion

        #region 保存和读取

        /// <summary>
        /// 保存数据
        /// </summary>
        public void SaveData()
        {
            //保存数据的时候key不能为空
            if (string.IsNullOrEmpty(Key))
            {
                EditorUtility.DisplayDialog("错误", "Key为空，不能保存数据", "确认");
                return;
            }

            if (DrawImgData)
            {
                ImageData.Key = Key;
                LocalizationConfig.Instance .SaveImageData(ImageData);
            }
            else
            {
                StringData.Key = Key;
                LocalizationConfig.Instance .SaveStringData(StringData);
            }
            
        }
        /// <summary>
        /// 从库里加载数据
        /// </summary>
        public void LoadData()
        {
            //加载数据的时候key不能为空
            if (string.IsNullOrEmpty(Key))
            {
                EditorUtility.DisplayDialog("错误", "Key为空，不能加载数据", "确认");
                return;
            }

            if (DrawImgData)
            {
                //尝试获取数据
                var imageData = LocalizationConfig.Instance.GetImageData(Key);
                if (imageData == null)
                {
                    EditorUtility.DisplayDialog("提示", $"不存在Key为{Key}的数据", "确认");
                }
                else
                {
                    //复制数据、更新UI
                    ImageData.CopyData(imageData);
                    UpdateLanguage(curLanguage);
                }
            }
            else
            {
                //尝试获取数据
                var stringData = LocalizationConfig.Instance.GetStringData(Key);
                if (stringData == null)
                {
                    EditorUtility.DisplayDialog("提示", $"不存在Key为{Key}的数据", "确认");
                }
                else
                {
                    //复制数据、更新UI
                    StringData.CopyData(stringData);
                    UpdateLanguage(curLanguage);
                }
            }
        }
        

        #endregion
        
        
        #endif
        
    }
}

