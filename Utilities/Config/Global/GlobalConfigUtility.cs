//****************** 代码文件申明 ***********************
//* 文件：GlobalConfigUtility
//* 作者：wheat
//* 创建时间：2024/05/30 09:06:22 星期四
//* 描述：用于管理、搜索一些全局配置
//*******************************************************

using System;
using KFrame.Systems;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace KFrame.Utilities
{
    /// <summary>
    /// 搜索一些全局配置
    /// </summary>
    public static class GlobalConfigUtility<T> where T : ScriptableObject
    {
        private static T instance;

        /// <summary>
        /// 获取搜索Asset
        /// </summary>
        public static T GetInstance(string defaultAssetFolderPath, string defaultFileNameWithoutExtension = null, bool createIfCantFind = false)
        {
            if(instance == null)
            {
                LoadInstanceIfAssetExists(defaultAssetFolderPath, defaultFileNameWithoutExtension, createIfCantFind);
            }

            return instance;
        }
        /// <summary>
        /// 加载实例如果存在的话
        /// </summary>
        /// <param name="assetPath">Asset路径</param>
        /// <param name="defaultFileNameWithoutExtension">默认名称</param>
        /// <param name="createIfCantFind">如果找不到的话就自动创建</param>
        internal static void LoadInstanceIfAssetExists(string assetPath, string defaultFileNameWithoutExtension = null, bool createIfCantFind = false)
        {
            //默认取NiceName
            string text = defaultFileNameWithoutExtension ?? typeof(T).GetNiceName();
            
            
            //先尝试从资源管理器里面加载
            instance = ResSystem.LoadAsset<T>(assetPath + text + ".asset");
            if (instance == null)
            {
                instance = ResSystem.LoadAsset<T>(text);
            }
            if (instance != null)
            {
                return;
            }
                        
            //如果在Resource文件夹里
            if (StringExtensions.Contains(assetPath, "/resources/", StringComparison.OrdinalIgnoreCase))
            {
                //更新Resource文件夹路径
                string text2 = assetPath;
                int num = text2.LastIndexOf("/resources/", StringComparison.OrdinalIgnoreCase);
                if (num >= 0)
                {
                    text2 = text2.Substring(num + "/resources/".Length);
                }

                //从Resource里面加载
                string text3 = text;
                instance = Resources.Load<T>(text2 + text3);
            }
#if UNITY_EDITOR

            //其他文件夹
            else
            {
                //直接从Asset里面加载
                string text4 = text;
                instance = AssetDatabase.LoadAssetAtPath<T>(assetPath + text4 + ".asset");
                if (instance == null)
                {
                    //以防开头没写Assets/
                    instance = AssetDatabase.LoadAssetAtPath<T>("Assets/" + assetPath + text4 + ".asset");
                }
            }

            //如果不存在的话那就在路径搜索SO里面查找记录
            if (instance == null)
            {
                //先看看有没有之前的搜索记录
                string niceName = typeof(T).GetNiceName();
                string searchPath = KFramePathSearch.Instance.GetPath(niceName);

                //如果不为空的话，那就尝试加载
                if (string.IsNullOrEmpty(searchPath) == false)
                {
                    instance = AssetDatabase.LoadAssetAtPath<T>(searchPath);
                }

                //如果还是找不到，那就尝试用GUID搜索
                if(instance == null)
                {
                    //GUID不为空
                    string guid = KFramePathSearch.Instance.GetGUID(niceName);
                    if(string.IsNullOrEmpty(guid) == false)
                    {
                        string path = AssetDatabase.GUIDToAssetPath(guid);
                        //加载
                        instance = AssetDatabase.LoadAssetAtPath<T>(path);

                        //如果不为空那就更新路径
                        if (instance != null)
                        {
                            //更新记录到路径搜索SO
                            KFramePathSearch.Instance.SetPath(niceName, path, guid);
                        }
                    }
                }
            }


            //如果不存在的话那就尝试搜索
            if (instance == null)
            {
                string[] array = AssetDatabase.FindAssets("t:" + typeof(T).Name);
                //如果搜索到了
                if (array.Length != 0)
                {
                    string guid = array[0];
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    
                    //更新记录到路径搜索SO
                    KFramePathSearch.Instance.SetPath(typeof(T).GetNiceName(), path, guid);

                    //加载
                    instance = AssetDatabase.LoadAssetAtPath<T>(path);
                }
            }
            
            //怎么都找不到的话，如果允许创建的话，那就创建
            if (instance == null && createIfCantFind)
            {
                instance = ScriptableObject.CreateInstance<T>();
                instance.name = text;
                AssetDatabase.CreateAsset(instance, assetPath + text + ".asset");
                AssetDatabase.Refresh();
                AssetDatabase.SaveAssets();
            }
#endif

        }
    }
}

