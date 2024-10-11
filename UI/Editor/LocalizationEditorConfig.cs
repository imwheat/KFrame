//****************** 代码文件申明 ***********************
//* 文件：LocalizationEditorConfig
//* 作者：wheat
//* 创建时间：2024/10/09 18:48:30 星期三
//* 描述：本地化编辑器配置
//*******************************************************

using System.Collections.Generic;
using System.IO;
using KFrame.Utilities;
using KFrame.Attributes;
using KFrame.Editor;
using UnityEditor;
using UnityEngine;

namespace KFrame.UI.Editor
{
    [KGlobalConfigPath(GlobalPathType.Editor, typeof(LocalizationEditorConfig), true)]
    public class LocalizationEditorConfig : GlobalConfigBase<LocalizationEditorConfig>
    {
        #region 数据引用
        
        /// <summary>
        /// 语言包创建文件夹
        /// </summary>
        private static string PackageCreateFolder =>
            KFrameAssetsPath.GetPath(GlobalPathType.Assets) + "LanguagePackages/";

        #endregion
        
        #region 数据存储

        /// <summary>
        /// 本地化配置的key
        /// </summary>
        public List<LanguagePackageKeyData> localizationKeys = new();
        /// <summary>
        /// 语言包
        /// </summary>
        public List<LanguagePackage> packages = new();

        #endregion

        #region 数据处理

        /// <summary>
        /// key字典
        /// </summary>
        private Dictionary<string, LanguagePackageKeyData> keyDic;
        /// <summary>
        /// key字典
        /// </summary>
        public Dictionary<string, LanguagePackageKeyData> KeyDic
        {
            get
            {
                if (keyDic == null)
                {
                    keyDic = new Dictionary<string, LanguagePackageKeyData>();
                    foreach (var data in localizationKeys)
                    {
                        keyDic[data.key] = data;
                    }
                }

                return keyDic;
            }
        }

        #endregion

        #region key编辑
        
        /// <summary>
        /// 添加新的key
        /// </summary>
        /// <param name="keyData"></param>
        public void AddKey(LanguagePackageKeyData keyData)
        {
            if (keyData == null)
            {
                EditorUtility.DisplayDialog("错误", "不能添加空的key", "确定");
                return;
            }
            if (KeyDic.ContainsKey(keyData.key))
            {
                EditorUtility.DisplayDialog("错误", "不能添加重复的key", "确定");
                return;
            }

            //添加key
            localizationKeys.Add(keyData);
            for (int i = 0; i < packages.Count; i++)
            {
                packages[i].AddKey(keyData.key);
            }
            //更新字典
            if (keyDic != null)
            {
                keyDic[keyData.key] = keyData;
            }
            
            //保存
            EditorUtility.SetDirty(this);
        }
        /// <summary>
        /// 删除key
        /// </summary>
        /// <param name="keyData"></param>
        public void RemoveKey(LanguagePackageKeyData keyData)
        {
            if (keyData == null || string.IsNullOrEmpty(keyData.key))
            {
                EditorUtility.DisplayDialog("错误", "不能删除空的key", "确定");
                return;
            }

            //删除key
            localizationKeys.Remove(keyData);
            for (int i = 0; i < packages.Count; i++)
            {
                packages[i].RemoveKey(keyData.key);
            }
            //更新字典
            if (keyDic != null)
            {
                keyDic.Remove(keyData.key);
            }
            
            //保存
            EditorUtility.SetDirty(this);
        }
        /// <summary>
        /// 尝试更新某个数据的key
        /// </summary>
        /// <param name="data">待更新数据</param>
        /// <param name="key">新的key</param>
        public void UpdateKey(LanguagePackageKeyData data, string key)
        {
            //不为空
            if(data == null) return;
            
            //key不能为空
            if (string.IsNullOrEmpty(key))
            {
                EditorUtility.DisplayDialog("错误", "Key不能为空", "确定");
                return;
            }
            
            //如果库里已经有这个key了，那就保存失败
            if (KeyDic.ContainsKey(key))
            {
                EditorUtility.DisplayDialog("错误", $"已经存在key为{key}的数据了。", "确定");
            }
            else
            {
                //先修改先前语言包的key
                for (int i = 0; i < packages.Count; i++)
                {
                    packages[i].UpdateKey(data.key, key);
                }
                
                //更新key保存
                data.key = key;
                
                //保存
                this.SaveAsset();
            }
        }

        #endregion

        #region 语言包编辑

        /// <summary>
        /// 创建添加新的语言包
        /// </summary>
        /// <param name="languageKey">语言key</param>
        /// <param name="languageName">语言名称</param>
        public void AddLanguagePackage(string languageKey, string languageName)
        {
            //防空
            if (string.IsNullOrEmpty(languageKey))
            {
                EditorUtility.DisplayDialog("错误", "语言的名称不能为空", "确定");
                return;
            }
            
            //获取本地化配置文件
            LocalizationConfig config = LocalizationConfig.Instance;
            
            //创建新的语言包
            LanguagePackage package = ScriptableObject.CreateInstance<LanguagePackage>();
            int id = config.packageReferences.Count;
            LanguageClass languageClass = new LanguageClass(id, languageKey, languageName);
            package.language = languageClass;
            //把目前已有的key添加入数据
            foreach (var data in localizationKeys)
            {
                package.datas.Add(new LanguagePackageTextData(data.key));
            }
            
            //获取路径
            string path = PackageCreateFolder + languageKey + ".asset";
            
            //在配置中添加引用
            config.packageReferences.Add(new LanguagePackageReference(languageClass, path));
            EditorUtility.SetDirty(config);
            
            //添加入语言包列表
            packages.Add(package);
            EditorUtility.SetDirty(this);
            
            //保存文件
            FileExtensions.CreateDirectoryIfNotExist(Path.GetDirectoryName(path));
            AssetDatabase.CreateAsset(package, path);
        }
        /// <summary>
        /// 删除语言包
        /// </summary>
        /// <param name="languageName">语言名称</param>
        public void RemoveLanguagePackage(string languageName)
        {
            //先寻找目标语言包，要是没有就返回
            LanguagePackage package = packages.Find(x => x.language.languageKey == languageName);
            if(package == null) return;
            
            //从列表中移除
            packages.Remove(package);
            //获取本地化配置文件
            LocalizationConfig config = LocalizationConfig.Instance;
            //找到指定目标然后删除
            for (int i = 0; i < config.packageReferences.Count; i++)
            {
                if (config.packageReferences[i].LanguageKey == languageName)
                {
                    config.packageReferences.RemoveAt(i);
                    break;
                }
            }
            
            //保存
            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty(config);
            
            //删除文件
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(package));
            
        }
        /// <summary>
        /// 更新语言名称
        /// </summary>
        /// <param name="prevName">之前的名称</param>
        /// <param name="newName">新的名称</param>
        public void UpdateLanguageKey(string prevName, string newName)
        {
            if (string.IsNullOrEmpty(newName))
            {
                EditorUtility.DisplayDialog("错误", "语言Key不能为空！", "确认");
                return;
            }
            //如果和之前的一样那就返回
            if(prevName == newName) return;
            //先寻找要改名字的语言包，如果没有就返回
            LanguagePackage target = null;
            foreach (var package in packages)
            {
                if (package.name == prevName)
                {
                    target = package;
                }
                else if (package.name == newName)
                {
                    EditorUtility.DisplayDialog("错误", "语言Key不能重复！", "确认");
                    return;
                }
            }
            if(target == null) return;
            
            //获取本地化配置文件
            LocalizationConfig config = LocalizationConfig.Instance;
            
            //获取数据然后修改名称
            var references = config.packageReferences.Find(x => x.LanguageKey == prevName);
            if (references != null)
            {
                references.LanguageKey = newName;
            }
            target.language.languageKey = newName;
            
            //保存
            EditorUtility.SetDirty(target);
            EditorUtility.SetDirty(config);
            this.SaveAsset();
        }

        #endregion
    }
}

