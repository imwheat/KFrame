//****************** 代码文件申明 ***********************
//* 文件：KEditorUtility
//* 作者：wheat
//* 创建时间：2024/06/07 09:13:48 星期五
//* 描述：拓展一些EditorUtility的功能
//*******************************************************

using System.IO;
using KFrame.Utilities;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

namespace KFrame.Editor
{
    /// <summary>
    /// 拓展一些<see cref="EditorUtility"/>的功能
    /// </summary>
    public static class KEditorUtility
    {
        /// <summary>
        /// 保存某项Asset
        /// </summary>
        public static void SaveAsset(this Object @object)
        {
            //空不能保存
            if (@object == null) return;

            //保存
            EditorUtility.SetDirty(@object);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }
        /// <summary>
        /// 增加编辑器宏
        /// </summary>
        public static void AddScriptCompilationSymbol(string name)
        {
            BuildTargetGroup buildTargetGroup = UnityEditor.EditorUserBuildSettings.selectedBuildTargetGroup;
            string group = UnityEditor.PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            if (!group.Contains(name))
            {
                UnityEditor.PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, group + ";" + name);
            }
        }
        /// <summary>
        /// 移除编辑器宏
        /// </summary>
        public static void RemoveScriptCompilationSymbol(string name)
        {
            BuildTargetGroup buildTargetGroup = UnityEditor.EditorUserBuildSettings.selectedBuildTargetGroup;
            string group = UnityEditor.PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            if (group.Contains(name))
            {
                UnityEditor.PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup,
                    group.Replace(";" + name, string.Empty));
            }
        }
        /// <summary>
        /// 获取Asset的文件后缀
        /// </summary>
        /// <param name="asset">查询的Asset</param>
        /// <returns>Asset的后缀</returns>
        public static string GetAssetExtension(this Object asset)
        {
            if (asset == null) return "";

            return Path.GetExtension(AssetDatabase.GetAssetPath(asset));
        }
        /// <summary>
        /// 检测Asset的文件后缀
        /// </summary>
        /// <param name="asset">查询的Asset</param>
        /// <param name="extension">后缀</param>
        /// <returns>如果是目标后缀的话返回true</returns>
        public static bool CheckAssetExtension(this Object asset, string extension)
        {
            return asset.GetAssetExtension() == extension;
        }
        /// <summary>
        /// 获取Asset的Asset路径
        /// </summary>
        /// <param name="asset">查询的Asset</param>
        /// <returns>Asset的路径</returns>
        public static string GetAssetPath(this Object asset)
        {
            if (asset == null) return "";
            
            return AssetDatabase.GetAssetPath(asset);
        }
        /// <summary>
        /// 获取Asset在电脑里的本地路径
        /// </summary>
        /// <param name="asset">查询的Asset</param>
        /// <returns>Asset在电脑里的本地路径</returns>
        public static string GetFullPath(this Object asset)
        {
            return asset.GetAssetPath().ConvertAssetPathToSystemPath();
        }
        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="assetPath">Asset的路径</param>
        public static void DeleteFolder(string assetPath)
        {
            //路径不能为空
            if(string.IsNullOrEmpty(assetPath)) return;
            //转为本地路径
            string fullPath = assetPath.ConvertAssetPathToSystemPath();
            string folderName = Path.GetFileName(fullPath);
            //删除文件夹以及它的meta文件
            Directory.Delete(fullPath);
            File.Delete(FileExtensions.GetParentDirectory(fullPath, 1) + "/" + folderName + ".meta");
        }
        /// <summary>
        /// 错误警告的Dialog窗口
        /// </summary>
        /// <param name="message">要显示的内容</param>
        /// <param name="title">标题</param>
        /// <param name="ok">确定文本内容</param>
        /// <returns></returns>
        public static bool ErrorDialog(string message, string title = "错误", string ok = "确定")
        {
            return EditorUtility.DisplayDialog(title, message, ok);
        }
        /// <summary>
        /// 获取剪切板的文本内容
        /// </summary>
        /// <returns>剪切板的文本内容</returns>
        public static string GetCopyText()
        {
            return GUIUtility.systemCopyBuffer;
        }
        /// <summary>
        /// 设置剪切板的文本内容
        /// </summary>
        public static void SetCopyText(string text)
        {
            GUIUtility.systemCopyBuffer = text;
        }
    }
}

