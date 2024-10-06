//****************** 代码文件申明 ***********************
//* 文件：KFrameAssetsPath
//* 作者：wheat
//* 创建时间：2024/06/03 10:19:51 星期一
//* 描述：用来查询、管理框架配置的一些路径
//*******************************************************

#if UNITY_EDITOR

using UnityEditor;

#endif 

using System;

namespace KFrame.Utilities
{
    /// <summary>
    /// 用来查询、管理框架配置的一些路径
    /// </summary>
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public static class KFrameAssetsPath
    {
        /// <summary>
        /// 框架路径
        /// </summary>
        private static readonly string DefaultFramePath = "Assets/KFrame/";
        /// <summary>
        /// 框架Assets路径
        /// </summary>
        private static string FrameAssetsPath => DefaultFramePath + "Assets/";
        /// <summary>
        /// 框架编辑器Assets路径
        /// </summary>
        private static string FrameEditorAssetsPath => FrameAssetsPath + "Editor/";
        static KFrameAssetsPath()
        {
            #if UNITY_EDITOR
            
            //如果找不到文件夹路径了
            if (!AssetDatabase.IsValidFolder(DefaultFramePath))
            {
                if (KFramePathSearch.Instance == null)
                {
                    throw new Exception("错误！文件搜索SO丢失了，无法进行路径搜索");
                }
                else
                {
                    //那就从文件路径SO开始搜寻
                    string searchSOPath = AssetDatabase.GetAssetPath(KFramePathSearch.Instance);
                    string prevPath = searchSOPath;
                    //找到最外层的KFrame文件夹
                    while (searchSOPath.Contains("KFrame"))
                    {
                        prevPath = searchSOPath;
                        searchSOPath = FileExtensions.GetParentDirectory(searchSOPath, 1);
                    }
                    //返回路径
                    DefaultFramePath = prevPath.GetNiceDirectoryPath();
                }
                
            }
            #endif
            
        }
        /// <summary>
        /// 根据类型获取全局路径
        /// </summary>
        /// <param name="pathType">路径类型</param>
        /// <returns>对应的路径</returns>
        public static string GetPath(GlobalPathType pathType)
        {
            switch (pathType)
            {
                case GlobalPathType.Frame:
                    return DefaultFramePath;
                case GlobalPathType.Assets:
                    return FrameAssetsPath;
                case GlobalPathType.Editor:
                    return FrameEditorAssetsPath;
            }

            return "";
        }
    }
}

