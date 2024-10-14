//****************** 代码文件申明 ***********************
//* 文件：KFramePathSearch
//* 作者：wheat
//* 创建时间：2024/06/03 09:57:20 星期一
//* 描述：用来查询其他的全局配置
//*******************************************************

using UnityEngine;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Reflection;
using KFrame.Attributes;

#if UNITY_EDITOR
using UnityEditor;

#endif

namespace KFrame.Utilities
{
    /// <summary>
    /// 用来查询其他的全局配置
    /// </summary>
    [CreateAssetMenu(fileName = "KFramePathSearch", menuName = "ScriptableObject/Editor/KFramePathSearch")]
    public class KFramePathSearch : ConfigBase
    {

#if UNITY_EDITOR

        #region 实例

        private static KFramePathSearch instance;
        public static KFramePathSearch Instance
        {
            get
            {
                if (instance == null)
                {
                    LoadInstanceIfAssetExists("Assets/KFrame/Assets/Editor/");
                }

                return instance;
            }
        }

        #endregion

        #region 编辑器初始化
        
        /// <summary>
        /// 编辑器初始化GlobalConfig
        /// </summary>
        public void InitGlobalConfig()
        {
            //白名单
            string[] whiteListAssembly = new[] { "Assembly-CSharp" };
            string[] whiteListNameSpace = new[] { "GameBuild","KFrame" };
            
            //获取所有程序集
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            Type baseType = typeof(GlobalConfigBase<>);
            //遍历程序集
            foreach (Assembly assembly in assemblies)
            {
                //如果不符合白名单那就跳过
                if (!assembly.FullName.ContainsAny(whiteListAssembly)) continue;
                
                //遍历程序集下的每一个类型
                Type[] types = assembly.GetTypes();
                foreach (Type type in types)
                {
                    //如果类型不在白名单内就跳过
                    if(!type.FullName.ContainsAny(whiteListNameSpace)) continue;
                    
                    //判断类型
                    if (type.BaseType is { IsGenericType: true } && type.BaseType.GetGenericTypeDefinition() == baseType
                                                                 && !type.IsAbstract)
                    {
                        var attributes = type.GetCustomAttributes<KGlobalConfigPathAttribute>();
                        foreach (var attribute in attributes)
                        {
                            //如果找不到会自动创建的话
                            if (attribute.CreateIfNotFound)
                            {
                                string[] array = AssetDatabase.FindAssets("t:" + type.Name);
                                //如果搜索不到，那就创建
                                if (array.Length == 0)
                                {
                                    var asset = ScriptableObject.CreateInstance(type);
                                    asset.name = attribute.FileName;
                                    AssetDatabase.CreateAsset(asset, attribute.AssetPath + attribute.FileName + ".asset");
                                }
                                
                            }
                        }
                        
                    }
                }
            }

        }

        #endregion

        #region 数据存储

        /// <summary>
        /// 用来保存记录路径
        /// </summary>
        [System.Serializable]
        public class SearchPathStack
        {
            public string NiceName;
            public string Path;
            public string GUID;
            public SearchPathStack()
            {

            }
            public SearchPathStack(string niceName, string path, string guid)
            {
                this.NiceName = niceName;
                this.Path = path;
                this.GUID = guid;
            }
        }

        /// <summary>
        /// 路径字典
        /// </summary>
        [NonSerialized]
        private Dictionary<string, SearchPathStack> pathDic;
        /// <summary>
        /// 用来存储现存的路径在列表的下标
        /// </summary>
        [NonSerialized]
        private Dictionary<string, int> pathIndexDic;

        [Header("请不要删除这个文件，尽量也不要移动这个文件！"), Header("这个文件是用来搜索、存储各种配置文件的路径的。")]
        /// <summary>
        /// 目前所有路径
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public List<SearchPathStack> Pathes;

        #endregion

        #region 数据获取与设置

        /// <summary>
        /// 获取搜索Asset
        /// </summary>
        internal static void LoadInstanceIfAssetExists(string assetPath, string defaultFileNameWithoutExtension = null)
        {
            //默认取NiceName
            string text = defaultFileNameWithoutExtension ?? typeof(KFramePathSearch).GetNiceName();
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
                instance = Resources.Load<KFramePathSearch>(text2 + text3);
            }
            //其他文件夹
            else
            {
                //直接从Asset里面加载
                string text4 = text;
                instance = AssetDatabase.LoadAssetAtPath<KFramePathSearch>(assetPath + text4 + ".asset");
                if (instance == null)
                {
                    //以防开头没写Assets/
                    instance = AssetDatabase.LoadAssetAtPath<KFramePathSearch>("Assets/" + assetPath + text4 + ".asset");
                }
            }

            //如果还是找不到那就尝试在EditorBuildSettings里面搜索
            string buildSettingsPath = nameof(KFrame.Utilities) + "." + text;
            if (instance == null)
            {
                EditorBuildSettings.TryGetConfigObject<KFramePathSearch>(buildSettingsPath, out instance);
                if(instance != null)
                {
                    Debug.LogWarning($"检测到{typeof(KFramePathSearch).Name}被移动了，但搜索到了，请尽量不要乱移动该文件。");
                }
            }

            //如果不存在的话那就尝试搜索
            if (instance == null)
            {
                string[] array = AssetDatabase.FindAssets("t:" + typeof(KFramePathSearch).Name);
                //如果搜索到了
                if (array.Length != 0)
                {
                    string guid = array[0];
                    string path = AssetDatabase.GUIDToAssetPath(guid);

                    //加载
                    instance = AssetDatabase.LoadAssetAtPath<KFramePathSearch>(path);

                    //保存到EditorBuildSettings里面
                    if(instance != null)
                    {
                        EditorBuildSettings.AddConfigObject(buildSettingsPath, instance, true);
                        Debug.LogWarning($"检测到{typeof(KFramePathSearch).Name}被移动了，但搜索到了，请尽量不要乱移动该文件。");
                    }
                    else
                    {
                        Debug.LogWarning($"检测到{typeof(KFramePathSearch).Name}被删除或移动了，并且无法搜索到，请不要删除该文件！");
                    }

                }
                else
                {
                    Debug.LogWarning($"检测到{typeof(KFramePathSearch).Name}被删除或移动了，并且无法搜索到，请不要删除该文件！");
                }
            }

        }
        /// <summary>
        /// 初始化子弟那
        /// </summary>
        private void InitDic()
        {
            //更新字典
            pathDic = new Dictionary<string, SearchPathStack>();
            pathIndexDic = new Dictionary<string, int>();
            for (int i = 0; i < Pathes.Count; i++)
            {
                pathDic.Add(Pathes[i].NiceName, Pathes[i]);
                pathIndexDic.Add(Pathes[i].NiceName, i);
            }
        }
        /// <summary>
        /// 获取路径
        /// </summary>
        public string GetPath(string niceName)
        {
            //如果字典为空那就初始化字典
            if (pathDic == null)
            {
                InitDic();
            }

            if (pathDic.ContainsKey(niceName))
            {
                return pathDic[niceName].Path;
            }
            else
            {
                return "";
            }
        }
        /// <summary>
        /// 获取GUID
        /// </summary>
        public string GetGUID(string niceName)
        {
            //如果字典为空那就初始化字典
            if (pathDic == null)
            {
                InitDic();
            }

            if (pathDic.ContainsKey(niceName))
            {
                return pathDic[niceName].GUID;
            }
            else
            {
                return "";
            }
        }
        /// <summary>
        /// 更新路径
        /// </summary>
        public void SetPath(string niceName, string path, string guid)
        {
            //如果字典为空那就初始化字典
            if (pathDic == null)
            {
                InitDic();
            }

            //如果已经有记录路径了
            if (pathDic.ContainsKey(niceName))
            {
                //那就更新路径
                pathDic[niceName].Path = path;
                Pathes[pathIndexDic[niceName]].Path = path;
                Pathes[pathIndexDic[niceName]].GUID = guid;
            }
            //没有的话那就添加
            else
            {
                SearchPathStack pathStack = new SearchPathStack(niceName, path, guid);
                pathDic[niceName] = pathStack;
                pathIndexDic[niceName] = Pathes.Count;
                Pathes.Add(pathStack);
            }
        }

        #endregion

#endif

    }
}

