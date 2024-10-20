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
        private int curLanguage;
        /// <summary>
        /// 当前的语言类型
        /// </summary>
        public int CurLanguage
        {
            get => curLanguage;
            set
            {
                if (UISetPropertyUtility.SetStruct(ref curLanguage, value))
                {
                    UpdateLanguage(value);
                }
            }
        }
        /// <summary>
        /// 本地化的key
        /// </summary>
        [SerializeField]
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
        /// 本地化图片数据
        /// </summary>
        public LocalizationImageData ImageData;
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
                linkedData = Parent.GetUILocalizationData(target, key);
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
        private void InitData()
        {
            //防空初始化
            if (StringData == null)
            {
                StringData = new LocalizationStringData("");
            }
            if (ImageData == null)
            {
                ImageData = new LocalizationImageData("");
            }

            //获取语言的所有类型
            int[] languages = LocalizationDic.GetLanguageIdArray();

            //记录添加数据中没有的语言类型
            HashSet<int> stringLanguage = new HashSet<int>();
            HashSet<int> imageLanguage = new HashSet<int>();
            for (int i = StringData.Datas.Count - 1; i >= 0; i--)
            {
                if (!stringLanguage.Add(StringData.Datas[i].LanguageId))
                {
                    StringData.Datas.RemoveAt(i);
                }
            }
            for (int i = ImageData.Datas.Count - 1; i >= 0; i--)
            {
                if (!stringLanguage.Add(ImageData.Datas[i].LanguageId))
                {
                    ImageData.Datas.RemoveAt(i);
                }
            }
            
            //遍历目前已经有的语言类型
            foreach (int id in languages)
            {
                //如果数据里面还没有的话那就添加
                if (!stringLanguage.Contains(id))
                {
                    StringData.Datas.Add(new LocalizationStringDataBase(id, ""));
                }
                if (!imageLanguage.Contains(id))
                {
                    ImageData.Datas.Add(new LocalizationImageDataBase(id, null));
                }
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
        /// <summary>
        /// 更新语言
        /// </summary>
        public void UpdateLanguage(int languageType)
        {
            curLanguage = languageType;
            //更新UI
            UpdateUI();
            
            EditorUtility.SetDirty(target);
            EditorUtility.SetDirty(this);
        }
        /// <summary>
        /// 更新UI
        /// </summary>
        public void UpdateUI()
        {
            switch (target)
            {
                case Image img:
                    img.sprite = ImageData.Datas.Find((x) =>x.LanguageId == curLanguage).Sprite;
                    break;
                case Text text:
                    text.text = StringData.Datas.Find((x => x.LanguageId == curLanguage)).Text;
                    break;
                case TMP_Text tmpText:
                    tmpText.text = StringData.Datas.Find((x => x.LanguageId == curLanguage)).Text;
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
                LocalizationDic.SaveImageData(ImageData);
            }
            else
            {
                StringData.Key = Key;
                LocalizationDic.SaveUITextData(StringData);
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
                var imageData = LocalizationDic.GetImageData(Key);
                if (imageData == null)
                {
                    EditorUtility.DisplayDialog("提示", $"不存在Key为{Key}的数据", "确认");
                }
                else
                {
                    //复制数据、更新UI
                    ImageData.CopyData(imageData);
                    UpdateUI();
                }
            }
            else
            {
                //尝试获取数据
                var stringData = LocalizationDic.GetStringData(Key);
                if (stringData == null)
                {
                    EditorUtility.DisplayDialog("提示", $"不存在Key为{Key}的数据", "确认");
                }
                else
                {
                    //复制数据、更新UI
                    StringData.CopyData(stringData);
                    UpdateUI();
                }
            }
        }

        #endregion
        
        
        #endif
        
    }
}

