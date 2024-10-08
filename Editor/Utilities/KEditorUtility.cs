//****************** 代码文件申明 ***********************
//* 文件：KEditorUtility
//* 作者：wheat
//* 创建时间：2024/06/07 09:13:48 星期五
//* 描述：拓展一些EditorUtility的功能
//*******************************************************

using System.IO;
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
    }
}

