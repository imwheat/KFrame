//****************** 代码文件申明 ***********************
//* 文件：LocalizationDic
//* 作者：wheat
//* 创建时间：2024/10/20 08:48:03 星期日
//* 描述：本地化的字典
//*******************************************************

#if UNITY_EDITOR

using UnityEditor;

#endif

using System;
using System.Collections.Generic;
using KFrame.Systems;
using UnityEngine;

namespace KFrame.UI
{
    public static class LocalizationDic
    {
        #region 参数引用
        
        /// <summary>
        /// 本地化配置
        /// </summary>
        public static LocalizationConfig Config => LocalizationConfig.Instance;

        #endregion

        #region 属性参数

        /// <summary>
        /// 初始化标记
        /// </summary>
        private static bool initted = false;
        /// <summary>
        /// 语言id
        /// </summary>
        private static int languageId = -1;

        /// <summary>
        /// 语言id
        /// </summary>
        public static int LanguageId => languageId;

        #endregion
        
        #region 字典

        /// <summary>
        /// 语言包字典
        /// </summary>
        private static Dictionary<int, LanguagePackageReference> packageDic;
        /// <summary>
        /// 本地化文本字典
        /// </summary>
        private static Dictionary<string, string> textDictionary;
        /// <summary>
        /// UI本地化文本字典
        /// </summary>
        private static Dictionary<string, Dictionary<int, string>> uiTextDic;
        /// <summary>
        /// UI本地化图片字典
        /// </summary>
        private static Dictionary<string, Dictionary<int, Sprite>> uiImageDic;
        /// <summary>
        /// 字体大小字典
        /// </summary>
        public static Dictionary<int, int> LanguageFontSize
            = new Dictionary<int, int>();

        #endregion
        
        #region 初始化

        static LocalizationDic()
        {
            Init();
        }
        
        /// <summary>
        /// 初始化字典
        /// </summary>
        public static void Init()
        {
            //如果已经初始化过了，那就返回
            if(initted) return;
            
            //标记初始化完成
            initted = true;
            
            //注册语言类型的字典
            packageDic = new Dictionary<int, LanguagePackageReference>();
            textDictionary = new Dictionary<string, string>();
            uiTextDic = new Dictionary<string, Dictionary<int, string>>();
            uiImageDic = new Dictionary<string, Dictionary<int, Sprite>>();

            //然后遍历每个数据然后添加入字典中
            foreach (var packageReference in Config.packageReferences)
            {
                packageDic[packageReference.LanguageId] = packageReference;
            }
            foreach (LocalizationStringData stringData in Config.TextDatas)
            {
                RegisterUITextData(stringData);
            }
            foreach (LocalizationImageData imageData in Config.ImageDatas)
            {
                RegisterUIImageData(imageData);
            }
            
#if UNITY_EDITOR
            //编辑器的初始化
            InitInEditor();

            //如果游戏还没运行，默认加载编辑器使用的语言包
            if (Application.isPlaying == false)
            {
                LoadLanguagePackage(LocalizationConfig.EditorDefaultLanguageId);
            }
#endif

        }
        /// <summary>
        /// 加载语言包
        /// </summary>
        /// <param name="id">语言的id</param>
        public static void LoadLanguagePackage(int id)
        {
            //乳沟相等就不需要更新
            if(languageId == id) return;
            
            //如果不存在该语言类型就报错然后返回
            if (!TryGetPackageReference(id, out LanguagePackageReference packageReference))
            {
                Debug.LogError($"错误：不存在id为{id}的语言");
                return;
            }

            //从资源库里面读取加载语言包
            LanguagePackage package = ResSystem.LoadAsset<LanguagePackage>(packageReference.packagePath);
            if (package == null)
            {
                throw new Exception($"错误：路径{packageReference.packagePath} 的语言包不存在！");
            }
            
            //更新语言id
            languageId = id;
            
            //遍历每个数据，然后更新文本
            foreach (var data in package.datas)
            {
                textDictionary[data.key] = data.text;
            }
        }
        /// <summary>
        /// 注册UI文本数据
        /// </summary>
        /// <param name="stringData">文本数据</param>
        private static void RegisterUITextData(LocalizationStringData stringData)
        {
            uiTextDic[stringData.Key] = new Dictionary<int, string>();
            //遍历所有数据，然后注册进入字典
            foreach (LocalizationStringDataBase data in stringData.Datas)
            {
                uiTextDic[stringData.Key][data.LanguageId] = data.Text;
            }
        }
        /// <summary>
        /// 注册UI图片数据
        /// </summary>
        /// <param name="imageData">图片数据</param>
        private static void RegisterUIImageData(LocalizationImageData imageData)
        {
            uiImageDic[imageData.Key] = new Dictionary<int, Sprite>();
            //遍历所有数据，然后注册进入字典
            foreach (LocalizationImageDataBase data in imageData.Datas)
            {
                uiImageDic[imageData.Key][data.LanguageId] = data.Sprite;
            }
        }
        #endregion
        
        #region 获取本地化信息

        /// <summary>
        /// 获取语言包
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>语言包信息</returns>
        public static LanguagePackageReference GetPackageReference(int id)
        {
            return packageDic.GetValueOrDefault(id, null);
        }
        /// <summary>
        /// 尝试获取语言包
        /// </summary>
        /// <param name="id">语言包id</param>
        /// <param name="packageReference">获取得到语言包</param>
        /// <returns>如果得到了返回true</returns>
        public static bool TryGetPackageReference(int id, out LanguagePackageReference packageReference)
        {
            return packageDic.TryGetValue(id, out packageReference);
        }
        /// <summary>
        /// 获取本地化文本
        /// </summary>
        /// <param name="key">文本key</param>
        /// <returns>语言包里的文本</returns>
        public static string GetLocalizedText(string key)
        {
            return textDictionary.GetValueOrDefault(key, "");
        }

        /// <summary>
        /// 尝试获取本地化文本
        /// </summary>
        /// <param name="key">文本key</param>
        /// <param name="text">输出文本</param>
        /// <returns>如果没有那就输出false</returns>
        public static bool TryGetLocalizedText(string key, out string text)
        {
            return textDictionary.TryGetValue(key, out text);
        }
        /// <summary>
        /// 获取本地化UI文本
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="language">语言类型</param>
        /// <returns>本地化后的文本，如果没有就返回""</returns>
        public static string GetUIText(string key, int language)
        {
            if (uiTextDic.TryGetValue(key, out var dic) && dic.TryGetValue(language, out string text))
            {
                return text;
            }
            
            return "";
        }

        /// <summary>
        /// 尝试获取本地化UI文本
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="language">语言类型</param>
        /// <param name="text">输出文本</param>
        /// <returns>本如果没有就返回false</returns>
        public static bool TryGetUIText(string key, int language, out string text)
        {
            text = GetUIText(key, language);

            return text != string.Empty;
        }
        /// <summary>
        /// 获取本地化UI图片
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="language">语言类型</param>
        /// <returns>本地化后的图片，如果没有就返回null</returns>
        public static Sprite GetUIImage(string key, int language)
        {
            if (uiImageDic.TryGetValue(key, out var dic) && dic.TryGetValue(language, out Sprite sprite))
            {
                return sprite;
            }
            
            return null;
        }

        /// <summary>
        /// 尝试获取本地化UI图片
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="language">语言类型</param>
        /// <param name="sprite">输出图片</param>
        /// <returns>本如果没有就返回false</returns>
        public static bool TryGetUIImage(string key, int language, out Sprite sprite)
        {
            sprite = GetUIImage(key, language);

            return sprite != null;
        }

        #endregion
        
        #region 编辑器相关

        #if UNITY_EDITOR

        /// <summary>
        /// UI文本数据字典
        /// </summary>
        private static Dictionary<string, LocalizationStringData> uiTextDataDic;
        /// <summary>
        /// UI图片数据字典
        /// </summary>
        private static Dictionary<string, LocalizationImageData> uiImageDataDic;
        /// <summary>
        /// 语言包的名称字典
        /// </summary>
        private static Dictionary<string, LanguagePackageReference> packageNameDic;

        /// <summary>
        /// 保存
        /// </summary>
        private static void SaveConfig()
        {
            //保存
            EditorUtility.SetDirty(Config);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }
        
        /// <summary>
        /// 编辑器相关的初始化
        /// </summary>
        private static void InitInEditor()
        {
            //新建字典
            uiTextDataDic = new Dictionary<string, LocalizationStringData>();
            uiImageDataDic = new Dictionary<string, LocalizationImageData>();
            
            //然后遍历每个数据然后添加入字典中
            foreach (LocalizationStringData stringData in Config.TextDatas)
            {
                uiTextDataDic[stringData.Key] = stringData;
            }
            foreach (LocalizationImageData imageData in Config.ImageDatas)
            {
                uiImageDataDic[imageData.Key] = imageData;
            }
        }
        /// <summary>
        /// 检测UI文本的key是否已经存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool CheckUITextKey(string key)
        {
            return uiTextDic.ContainsKey(key);
        }
        /// <summary>
        /// 检测UI图片的key是否已经存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool CheckUIImageKey(string key)
        {
            return uiImageDic.ContainsKey(key);
        }
        /// <summary>
        /// 获取StringData
        /// </summary>
        public static LocalizationStringData GetStringData(string key)
        {
            return uiTextDataDic.GetValueOrDefault(key);
        }
        /// <summary>
        /// 获取ImageData
        /// </summary>
        public static LocalizationImageData GetImageData(string key)
        {
            return uiImageDataDic.GetValueOrDefault(key);
        }
        /// <summary>
        /// 保存stringData
        /// </summary>
        public static void SaveUITextData(LocalizationStringData stringData)
        {
            //如果数据不合规，那就返回
            if(stringData == null || string.IsNullOrEmpty(stringData.Key)) return;

            if (uiTextDataDic.TryGetValue(stringData.Key, out LocalizationStringData data))
            {
                data.CopyData(stringData);
            }
            else
            {
                data = new LocalizationStringData(stringData.Key);
                data.CopyData(stringData);
                uiTextDataDic[data.Key] = data;
                Config.TextDatas.Add(data);
            }
            
            //保存
            SaveConfig();
        }
        /// <summary>
        /// 保存imageData
        /// </summary>
        public static void SaveImageData(LocalizationImageData imageData)
        {
            //如果数据不合规，那就返回
            if(imageData == null || string.IsNullOrEmpty(imageData.Key)) return;

            if (uiImageDataDic.TryGetValue(imageData.Key, out LocalizationImageData data))
            {
                data.CopyData(imageData);
            }
            else
            {
                data = new LocalizationImageData(imageData.Key);
                data.CopyData(imageData);
                uiImageDataDic[data.Key] = data;
                Config.ImageDatas.Add(data);
            }
            
            //保存
            SaveConfig();
        }
        /// <summary>
        /// 更新文本数据的key
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="key">key</param>
        public static void UpdateUITextDataKey(LocalizationStringData data, string key)
        {
            //如果key为空或者和原来一样，或者数据为空那就不做更改
            if(string.IsNullOrEmpty(key) || data == null || data.Key == key) return;
            
            //防止key重复
            if (uiTextDataDic.ContainsKey(key))
            {
                EditorUtility.DisplayDialog("错误", $"已经存在key为{key}的数据了！", "确认");
                return;
            }
            
            //先记录之前的key然后更新key
            string prevKey = data.Key;
            data.Key = key;
            //更新UI文本字典
            uiTextDic.Remove(prevKey);
            RegisterUITextData(data);
            
            //更新UI文本数据字典
            uiTextDataDic.Remove(prevKey);
            uiTextDataDic[data.Key] = data;
            
            //保存
            SaveConfig();
        }
        /// <summary>
        /// 更新图片数据的key
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="key">key</param>
        public static void UpdateUIImageDataKey(LocalizationImageData data, string key)
        {
            //如果key为空或者和原来一样，或者数据为空那就不做更改
            if(string.IsNullOrEmpty(key) || data == null || data.Key == key) return;
            
            //防止key重复
            if (uiImageDataDic.ContainsKey(key))
            {
                EditorUtility.DisplayDialog("错误", $"已经存在key为{key}的数据了！", "确认");
                return;
            }
            
            //先记录之前的key然后更新key
            string prevKey = data.Key;
            data.Key = key;
            //更新UI图片字典
            uiImageDic.Remove(prevKey);
            RegisterUIImageData(data);
            //更新UI图片数据字典
            uiImageDataDic.Remove(prevKey);
            uiImageDataDic[data.Key] = data;
            
            //保存
            SaveConfig();
        }
        /// <summary>
        /// 获取语言的id数组
        /// </summary>
        /// <returns></returns>
        public static int[] GetLanguageIdArray()
        {
            int[] languages = new int[Config.packageReferences.Count];
            for (var index = 0; index < Config.packageReferences.Count; index++)
            {
                var languagePackage = Config.packageReferences[index];
                languages[index] = languagePackage.LanguageId;
            }

            return languages;
        }
        /// <summary>
        /// 通过语言key获取语言id
        /// </summary>
        /// <param name="key">语言的key</param>
        /// <returns>找不到就返回-1</returns>
        public static int GetLanguageId(string key)
        {
            if (packageNameDic.TryGetValue(key, out var data))
            {
                return data.LanguageId;
            }

            return -1;
        }
        /// <summary>
        /// 通过语言id获取语言名称
        /// </summary>
        /// <param name="id"></param>
        /// <returns>能获取到就返回名称，没有就返回""</returns>
        public static string GetLanguageName(int id)
        {
            return packageDic.TryGetValue(id, out var data) ? data.LanguageName : "";
        }
        /// <summary>
        /// 编辑器使用的
        /// 更新本地化文本字典
        /// </summary>
        /// <param name="key"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static void UpdateLocalizedTextDic(string key, string text)
        {
            textDictionary[key] = text;
        }
        
        #endif

        #endregion
    }
}

