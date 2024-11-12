//****************** 代码文件申明 ***********************
//* 文件：LanguagePackage
//* 作者：wheat
//* 创建时间：2024/10/09 13:17:33 星期三
//* 描述：存储了一种游戏语言的包
//*******************************************************

#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections.Generic;
using UnityEngine;

namespace KFrame.UI
{
    [CreateAssetMenu(menuName = "ScriptableObject/LanguagePackage", fileName = "LanguagePackage")]
    public class LanguagePackage : ConfigBase
    {
        /// <summary>
        /// 语言类型
        /// </summary>
        public LanguageClass language;
        /// <summary>
        /// 数据
        /// </summary>
        public List<LanguagePackageTextData> datas = new();

        #region 编辑器使用
        
        /// <summary>
        /// 语言id
        /// </summary>
        public int LanguageId
        {
            get => language.languageId;
            set => language.languageId = value;
        }

        /// <summary>
        /// 语言key
        /// </summary>
        public string LanguageKey
        {
            get => language.languageKey;
            set => language.languageKey = value;
        }

        /// <summary>
        /// 语言名称
        /// </summary>
        public string LanguageName
        {
            get => language.languageName;
            set => language.languageName = value;
        }

        #if UNITY_EDITOR
        
        /// <summary>
        /// key字典
        /// </summary>
        private Dictionary<string, LanguagePackageTextData> keyDic;

        /// <summary>
        /// key字典
        /// </summary>
        public Dictionary<string, LanguagePackageTextData> KeyDic
        {
            get
            {
                if (keyDic == null)
                {
                    keyDic = new Dictionary<string, LanguagePackageTextData>();
                    foreach (var data in datas)
                    {
                        keyDic[data.key] = data;
                    }
                }

                return keyDic;
            }
        }
        
        /// <summary>
        /// 添加新的数据
        /// </summary>
        /// <param name="key">新的数据的key</param>
        public void AddKey(string key)
        {
            var data = new LanguagePackageTextData(key);
            datas.Add(data);
            KeyDic[key] = data;
            
            EditorUtility.SetDirty(this);
        }
        /// <summary>
        /// 删除指定key的数据
        /// </summary>
        /// <param name="key">要删除的key</param>
        public void RemoveKey(string key)
        {
            if (!KeyDic.Remove(key, out var data)) return;

            datas.Remove(data);
            
            EditorUtility.SetDirty(this);
        }
        /// <summary>
        /// 更新key
        /// </summary>
        /// <param name="prevKey">之前的key</param>
        /// <param name="newKey">新的key</param>
        public void UpdateKey(string prevKey, string newKey)
        {
            //如果有这个数据，那就更新数据
            if (KeyDic.TryGetValue(prevKey, out var data))
            {
                data.key = newKey;
                KeyDic.Remove(prevKey);
                KeyDic[newKey] = data;
                
                EditorUtility.SetDirty(this);
            }
            //如果没有那就添加
            else
            {
                AddKey(newKey);
            }
        }
        #endif

        #endregion
    }
}

