//****************** 代码文件申明 ************************
//* 文件：LocalizationConfig                      
//* 作者：wheat
//* 创建时间：2024/09/19 星期四 16:08:22
//* 功能：本地化配置的一些数据
//*****************************************************


using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.IO;
using KFrame.Attributes;
using KFrame.Utility;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace KFrame.UI
{
    [CreateAssetMenu(menuName = "KFrame/LocalizationConfig", fileName = "LocalizationConfig")]
    public class LocalizationConfig : GlobalConfigBase<LocalizationConfig>
    {
        #region 储存数据

        /// <summary>
        /// 本地化的文本数据
        /// </summary>
        public List<LocalizationStringData> TextDatas;
        /// <summary>
        /// 本地化的图片数据
        /// </summary>
        public List<LocalizationImageData> ImageDatas;
        

        #endregion

        #region 本地化搜索字典

        private Dictionary<string, Dictionary<LanguageType, string>> textDic;

        public Dictionary<string, Dictionary<LanguageType, string>> TextDic
        {
            get
            {
                if (textDic == null)
                {
                    Init();
                }

                return textDic;
            }
        }
        private Dictionary<string, Dictionary<LanguageType, Sprite>> imgDic;

        private Dictionary<string, Dictionary<LanguageType, Sprite>> ImgDic
        {
            get
            {
                if (imgDic == null)
                {
                    Init();
                }

                return imgDic;
            }
        }
        public Dictionary<LanguageType, int> LanguageFontSize
            = new Dictionary<LanguageType, int>();

        #endregion

        #region 初始化
        
        /// <summary>
        /// 初始化字典
        /// </summary>
        public void Init()
        {
            //注册语言类型的字典
            textDic = new Dictionary<string, Dictionary<LanguageType, string>>();
            imgDic = new Dictionary<string, Dictionary<LanguageType, Sprite>>();

            //然后遍历每个数据然后添加入字典中
            foreach (LocalizationStringData stringData in TextDatas)
            {
                RegisterTextData(stringData);
            }
            foreach (LocalizationImageData imageData in ImageDatas)
            {
                RegisterImageData(imageData);
            }
        }
        /// <summary>
        /// 注册文本数据
        /// </summary>
        /// <param name="stringData">文本数据</param>
        private void RegisterTextData(LocalizationStringData stringData)
        {
            textDic[stringData.Key] = new Dictionary<LanguageType, string>();
            //遍历所有数据，然后注册进入字典
            foreach (LocalizationStringDataBase data in stringData.Datas)
            {
                textDic[stringData.Key][data.Language] = data.Text;
            }
        }
        /// <summary>
        /// 注册图片数据
        /// </summary>
        /// <param name="imageData">图片数据</param>
        private void RegisterImageData(LocalizationImageData imageData)
        {
            imgDic[imageData.Key] = new Dictionary<LanguageType, Sprite>();
            //遍历所有数据，然后注册进入字典
            foreach (LocalizationImageDataBase data in imageData.Datas)
            {
                imgDic[imageData.Key][data.Language] = data.Sprite;
            }
        }
        #endregion

        #region 获取本地化信息

        /// <summary>
        /// 获取本地化文本
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="language">语言类型</param>
        /// <returns>本地化后的文本，如果没有就返回""</returns>
        public string GetLocalizedText(string key, LanguageType language)
        {
            if (TextDic.TryGetValue(key, out var dic) && dic.TryGetValue(language, out string text))
            {
                return text;
            }
            
            return "";
        }
        /// <summary>
        /// 尝试获取本地化文本
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="language">语言类型</param>
        /// <returns>本如果没有就返回false</returns>
        public bool TryGetLocalizedText(string key, LanguageType language, out string text)
        {
            text = GetLocalizedText(key, language);

            return text != string.Empty;
        }
        /// <summary>
        /// 获取本地化文本
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="language">语言类型</param>
        /// <returns>本地化后的文本，如果没有就返回null</returns>
        public Sprite GetLocalizedImage(string key, LanguageType language)
        {
            if (ImgDic.TryGetValue(key, out var dic) && dic.TryGetValue(language, out Sprite sprite))
            {
                return sprite;
            }
            
            return null;
        }
        /// <summary>
        /// 尝试获取本地化文本
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="language">语言类型</param>
        /// <returns>本如果没有就返回false</returns>
        public bool TryGetLocalizedImage(string key, LanguageType language, out Sprite sprite)
        {
            sprite = GetLocalizedImage(key, language);

            return sprite != null;
        }

        #endregion

        #region 编辑器相关

        #if UNITY_EDITOR

        private Dictionary<string, LocalizationStringData> textDataDic;

        public Dictionary<string, LocalizationStringData> TextDataDic
        {
            get
            {
                if (textDataDic == null)
                {
                    InitInEditor();
                }

                return textDataDic;
            }
        }
        private Dictionary<string, LocalizationImageData> imgDataDic;

        private Dictionary<string, LocalizationImageData> ImgDataDic
        {
            get
            {
                if (imgDataDic == null)
                {
                    InitInEditor();
                }

                return imgDataDic;
            }
        }
        /// <summary>
        /// 保存
        /// </summary>
        private void SaveAsset()
        {
            //保存
            EditorUtility.SetDirty(this);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }
        /// <summary>
        /// 编辑器相关的初始化
        /// </summary>
        private void InitInEditor()
        {
            //新建字典
            textDataDic = new Dictionary<string, LocalizationStringData>();
            imgDataDic = new Dictionary<string, LocalizationImageData>();
            
            //然后遍历每个数据然后添加入字典中
            foreach (LocalizationStringData stringData in TextDatas)
            {
                textDataDic[stringData.Key] = stringData;
            }
            foreach (LocalizationImageData imageData in ImageDatas)
            {
                imgDataDic[imageData.Key] = imageData;
            }
        }
        /// <summary>
        /// 获取StringData
        /// </summary>
        public LocalizationStringData GetStringData(string key)
        {
            if (textDataDic.TryGetValue(key, out var data))
            {
                return data;
            }

            return null;
        }
        /// <summary>
        /// 获取ImageData
        /// </summary>
        public LocalizationImageData GetImageData(string key)
        {
            if (imgDataDic.TryGetValue(key, out var data))
            {
                return data;
            }

            return null;
        }
        /// <summary>
        /// 保存stringData
        /// </summary>
        public void SaveStringData(LocalizationStringData stringData)
        {
            //如果数据不合规，那就返回
            if(stringData == null || string.IsNullOrEmpty(stringData.Key)) return;

            if (TextDataDic.TryGetValue(stringData.Key, out LocalizationStringData data))
            {
                data.CopyData(stringData);
            }
            else
            {
                data = new LocalizationStringData();
                data.CopyData(stringData);
                data.Key = stringData.Key;
                TextDataDic[data.Key] = data;
                TextDatas.Add(data);
            }
            
            //保存
            SaveAsset();
        }
        /// <summary>
        /// 保存imageData
        /// </summary>
        public void SaveImageData(LocalizationImageData imageData)
        {
            //如果数据不合规，那就返回
            if(imageData == null || string.IsNullOrEmpty(imageData.Key)) return;

            if (ImgDataDic.TryGetValue(imageData.Key, out LocalizationImageData data))
            {
                data.CopyData(imageData);
            }
            else
            {
                data = new LocalizationImageData();
                data.CopyData(imageData);
                data.Key = imageData.Key;
                ImgDataDic[data.Key] = data;
                ImageDatas.Add(data);
            }
            
            //保存
            SaveAsset();
        }
        /// <summary>
        /// 更新文本数据的key
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="key">key</param>
        public void UpdateStringDataKey(LocalizationStringData data, string key)
        {
            //如果key为空或者和原来一样，或者数据为空那就不做更改
            if(string.IsNullOrEmpty(key) || data == null || data.Key == key) return;
            
            //防止key重复
            if (TextDataDic.ContainsKey(key))
            {
                EditorUtility.DisplayDialog("错误", $"已经存在key为{key}的数据了！", "确认");
                return;
            }
            
            //先记录之前的key然后更新key
            string prevKey = data.Key;
            data.Key = key;
            //更新textDic
            if (textDic != null)
            {
                textDic.Remove(prevKey);
                RegisterTextData(data);
            }
            //更新textDataDic
            if (textDataDic != null)
            {
                textDataDic.Remove(prevKey);
                textDataDic[data.Key] = data;
            }
            
            //保存
            SaveAsset();
        }
        /// <summary>
        /// 更新图片数据的key
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="key">key</param>
        public void UpdateImageDataKey(LocalizationImageData data, string key)
        {
            //如果key为空或者和原来一样，或者数据为空那就不做更改
            if(string.IsNullOrEmpty(key) || data == null || data.Key == key) return;
            
            //防止key重复
            if (ImgDataDic.ContainsKey(key))
            {
                EditorUtility.DisplayDialog("错误", $"已经存在key为{key}的数据了！", "确认");
                return;
            }
            
            //先记录之前的key然后更新key
            string prevKey = data.Key;
            data.Key = key;
            //更新imgDic
            if (imgDic != null)
            {
                imgDic.Remove(prevKey);
                RegisterImageData(data);
            }
            //更新imgDataDic
            if (imgDataDic != null)
            {
                imgDataDic.Remove(prevKey);
                imgDataDic[data.Key] = data;
            }
            
            //保存
            SaveAsset();
        }
        
        #endif

        #endregion
       

    }
}