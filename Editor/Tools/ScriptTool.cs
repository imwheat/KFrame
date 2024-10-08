//****************** 代码文件申明 ************************
//* 文件：ScriptTool                      
//* 作者：wheat
//* 创建时间：2023年04月26日 星期五 09:47
//* 描述：有一些管理、创建脚本的工具方法
//*****************************************************

using System;
using System.Collections.Generic;
using System.IO;
using KFrame.Utilities;
using UnityEditor;
using UnityEngine;
using StringExtensions = KFrame.Utilities.StringExtensions;

namespace KFrame.Editor.Tools
{
    public static class ScriptTool
    {
        private const string CodeGeneratorStartTextRegion = "#region 代码生成开始标识";
        private const string CodeGeneratorStartText = "//代码生成开始标识";


        private const string CodeGeneratorEndTextRegion = "#endregion 代码生成结束标识";
        private const string CodeGeneratorEndText = "//代码生成结束标识";

        /// <summary>
        /// 通过位置标记来更新代码(添加)
        /// 位置标记为:
        /// <example>
        /// "#region 代码生成开始标识" 
        /// "#endregion 代码生成结束标识"
        /// </example>
        /// </summary>
        /// <param name="addContent">要添加的代码</param>
        /// <typeparam name="T">类类型</typeparam>
        /// <param name="addContentTag">新增部分的代码的Tag</param>
        /// <param name="regionTag">限制region的Tag</param>
        /// <param name="space">空格</param>
        /// <param name="useRegion">代码生成标识是否使用region</param>
        public static bool AddCode<T>(string addContent, string addContentTag, string regionTag = "", string space = "\t\t",bool useRegion = false)
        {
            //先查找cs文件路径
            bool findPath = TryGetCsFilePath<T>(out string classFilePath);
            if (findPath == false)
            {
                Debug.LogWarning("终止生成,请在形参csPath填入绝对路径");
                return false;
            }

            //然后开始生成
            return AddScriptCode(addContent, addContentTag, classFilePath, regionTag, space, useRegion);
        }

        /// <summary>
        /// 通过位置标记来更新代码(添加)
        /// 位置标记为:
        /// <example>
        /// "#region 代码生成开始标识" 
        /// "#endregion 代码生成结束标识"
        /// </example>
        /// </summary>
        /// <param name="addContent">要添加的代码</param>
        /// <param name="addContentTag">新增部分的代码的Tag</param>
        /// <param name="regionTag">限制region的Tag</param>
        /// <param name="csPath">cs文件的路径</param>
        /// <param name="space">空格</param>
        /// <param name="useRegion">代码生成标识是否使用region</param>
        private static bool AddScriptCode(string addContent, string addContentTag, string csPath, string regionTag = "", string space = "\t\t",bool useRegion = false)
        {
            //检查路径
            if (File.Exists(csPath) == false)
            {
                Debug.LogError($"错误该路径文件不存在{csPath}");
                return false;
            }

            //读取cs文件
            string csText = File.ReadAllText(csPath);

            //检测一下有没有画好region，有的话获取region的下标，没有的话返回
            int startRegionIndex = csText.IndexOf(CodeGeneratorStartTextRegion + regionTag, StringComparison.Ordinal) + (CodeGeneratorStartTextRegion + regionTag).Length;
            int endRegionIndex = csText.IndexOf(CodeGeneratorEndTextRegion + regionTag, StringComparison.Ordinal);
            if (startRegionIndex == -1 || endRegionIndex == -1)
            {
                Debug.LogError("请先画好region的范围以后再生成");
                return false;
            }

            //开始和结束的Tag
            string codeGeneratorStartText = (useRegion ? CodeGeneratorStartTextRegion : CodeGeneratorStartText) + addContentTag;
            if (codeGeneratorStartText == null) throw new ArgumentNullException(nameof(codeGeneratorStartText));
            string codeGeneratorEndText = (useRegion ? CodeGeneratorEndTextRegion : CodeGeneratorEndText) + addContentTag;

            //检查一下是不是已经存在了
            int existIndex = StringExtensions.IndexOf(csText, codeGeneratorStartText, startRegionIndex, endRegionIndex);
            if (existIndex != -1)
            {
                Debug.LogError($"已经存在相同的tag的代码了，如果你要更新替换旧的请使用{nameof(UpdateCode)}");
                return false;
            }
            //在原代码中添加新的部分
            string newCsText = csText.Insert(endRegionIndex, $"\n{space}{codeGeneratorStartText}\n\n{addContent}\n{space}{codeGeneratorEndText}\n\n{space}");
            //写入结果
            File.WriteAllText(csPath, newCsText);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();

            return true;
        }
        /// <summary>
        /// 通过位置标记来更新代码(删除)
        /// 位置标记为:
        /// <example>
        /// "#region 代码生成开始标识" 
        /// "#endregion 代码生成结束标识"
        /// </example>
        /// </summary>
        /// <typeparam name="T">类类型</typeparam>
        /// <param name="removeContentTag">删除部分的代码的Tag</param>
        /// <param name="regionTag">限制region的Tag</param>
        /// <param name="useRegion">代码生成标识是否使用region</param>
        public static void RemoveCode<T>(string removeContentTag, string regionTag = "", bool useRegion = false)
        {
            //先查找cs文件路径
            bool findPath = TryGetCsFilePath<T>(out string classFilePath);
            if (findPath == false)
            {
                Debug.LogWarning("终止生成,请在形参csPath填入绝对路径");
                return;
            }

            //然后开始生成
            RemoveScriptCode(removeContentTag, classFilePath, regionTag, useRegion);
        }
        /// <summary>
        /// 通过位置标记来更新代码(删除)
        /// 位置标记为:
        /// <example>
        /// "#region 代码生成开始标识" 
        /// "#endregion 代码生成结束标识"
        /// </example>
        /// </summary>
        /// <param name="removeContentTag">删除部分的代码的Tag</param>
        /// <param name="regionTag">限制region的Tag</param>
        /// <param name="csPath">cs文件的路径</param>
        /// <param name="useRegion">代码生成标识是否使用region</param>
        private static void RemoveScriptCode(string removeContentTag, string csPath, string regionTag = "", bool useRegion = false)
        {
            //检查路径
            if (File.Exists(csPath) == false)
            {
                Debug.LogError($"错误该路径文件不存在{csPath}");
                return;
            }

            //读取cs文件
            string csText = File.ReadAllText(csPath);

            //检测一下有没有画好region，有的话获取region的下标，没有的话返回
            int startRegionIndex = csText.IndexOf(CodeGeneratorStartTextRegion + regionTag, StringComparison.Ordinal) + (CodeGeneratorStartTextRegion + regionTag).Length + 1;
            int endRegionIndex = csText.IndexOf(CodeGeneratorEndTextRegion + regionTag, StringComparison.Ordinal) - 1;
            if (startRegionIndex == -1 || endRegionIndex == -1)
            {
                Debug.LogError("请先画好region的范围以后再删除");
                return;
            }

            //开始和结束的Tag
            string codeGeneratorStartText = (useRegion ? CodeGeneratorStartTextRegion : CodeGeneratorStartText) + removeContentTag;
            string codeGeneratorEndText = (useRegion ? CodeGeneratorEndTextRegion : CodeGeneratorEndText) + removeContentTag;

            //检查一下是不是已经不存在了
            int startIndex = StringExtensions.IndexOf(csText, codeGeneratorStartText, startRegionIndex, endRegionIndex);
            int endIndex = StringExtensions.IndexOf(csText, codeGeneratorEndText, startRegionIndex, endRegionIndex);
            if (startIndex == -1 || endIndex == -1)
            {
                Debug.Log( $"标记开始文本{codeGeneratorStartText}，结束文本{codeGeneratorEndText},区域开始id:{startRegionIndex},区域结束id:{endRegionIndex}，搜寻到的开始id{startIndex}, 搜寻到的结束id:{endIndex}");
                Debug.Log($"一开始就不存在该部分代码");
                return;
            }
            endIndex += codeGeneratorEndText.Length + 1;
            
            //在原代码中删除要移除的内容
            string newCsText = csText.Remove(startIndex, endIndex - startIndex);
            //写入结果
            File.WriteAllText(csPath, newCsText);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// 通过位置标记来更新代码(删除旧的)
        /// 位置标记为:
        /// <example>
        /// "#region 代码生成开始标识" 
        /// "#endregion 代码生成结束标识"
        /// </example>
        /// </summary>
        /// <param name="fileName">文件名称</param>
        /// <param name="updateContent">代码更新内容</param>
        /// <param name="space">空格</param>
        /// <param name="customTag">自定义tag</param>
        /// <param name="useRegion">代码生成标识是否使用region</param>
        /// <param name="refresh">刷新unity</param>
        public static void UpdateCode(string fileName, string updateContent, string space = "", string customTag = "", bool useRegion = true, bool refresh = true)
        {
            bool findPath = TryGetCsFilePath(fileName, out var classFilePath);
            if (findPath == false)
            {
                Debug.LogWarning("终止生成,请在形参csPath填入绝对路径");
                return;
            }

            UpdateScriptCode(updateContent, classFilePath, space, customTag, useRegion, refresh);
        }

        /// <summary>
        /// 通过位置标记来更新代码(删除旧的)
        /// 位置标记为:
        /// <example>
        /// "#region 代码生成开始标识" 
        /// "#endregion 代码生成结束标识"
        /// </example>
        /// </summary>
        /// <param name="updateContent">代码更新内容</param>
        /// <param name="space">空格</param>
        /// <param name="customTag">自定义tag</param>
        /// <param name="useRegion">代码生成标识是否使用region</param>
        /// <param name="refresh">刷新unity</param>
        /// <typeparam name="T">类类型</typeparam>
        public static void UpdateCode<T>(string updateContent, string space = "", string customTag = "", bool useRegion = true, bool refresh = true)
        {
            bool findPath = TryGetCsFilePath<T>(out string classFilePath);
            if (findPath == false)
            {
                Debug.LogWarning("终止生成,请在形参csPath填入绝对路径");
                return;
            }

            UpdateScriptCode(updateContent, classFilePath, space, customTag, useRegion, refresh);
        }

        /// <summary>
        /// 通过位置标记来更新代码(删除旧的)
        /// 会在位置标记的两行中间更新代码内容
        /// 位置标记为:
        /// <example>
        /// "#region 代码生成开始标识" 
        /// "#endregion 代码生成结束标识"
        /// </example>
        /// </summary>
        /// <param name="updateContent">生成的更新内容</param>
        /// <param name="csPath">文件路径</param>
        /// <param name="space">空格</param>
        /// <param name="customTag">自定义tag标识</param>
        /// <param name="useRegion">代码生成标识是否使用region</param>
        /// <param name="refresh">刷新unity</param>
        private static void UpdateScriptCode(string updateContent, string csPath, string space = "",
            string customTag = "", bool useRegion = true, bool refresh = true)
        {
            //检查路径
            if (File.Exists(csPath) == false)
            {
                Debug.LogError($"错误该路径文件不存在{csPath}");
                return;
            }

            //读取cs文件
            string csText = File.ReadAllText(csPath);

            string codeGeneratorStartText = (useRegion ? CodeGeneratorStartTextRegion : CodeGeneratorStartText) + customTag;
            string codeGeneratorEndText = (useRegion ? CodeGeneratorEndTextRegion : CodeGeneratorEndText) + customTag;

            //找到新的标记位置
            int startIndex = csText.IndexOf(codeGeneratorStartText, StringComparison.Ordinal) +
                             codeGeneratorStartText.Length;

            int endIndex = csText.IndexOf(codeGeneratorEndText, StringComparison.Ordinal);

            //先将start和end之间的代码删除干净
            string newContents = csText.Remove(startIndex, endIndex - startIndex);
            //在start后插入defineStr
            newContents = newContents.Insert(startIndex, "\n\n" + updateContent + "\n"+space);

            //写入结果
            File.WriteAllText(csPath, newContents);
            if (refresh)
            {
                AssetDatabase.Refresh();
                AssetDatabase.SaveAssets();
            }
        }

        /// <summary>
        /// 查找某个指定的cs文件
        /// </summary>
        /// <param name="fileName">文件名称</param>
        /// <param name="csPath">输出结果</param>
        /// <param name="searchInFolders">限制的文件夹</param>
        /// <returns>如果找不到返回false</returns>
        private static bool TryGetCsFilePath(string fileName,out string csPath, string[] searchInFolders = null)
        {
            //如果没有限制的搜索文件夹，那就默认整个Assets
            searchInFolders ??= new string[] { "Assets" };

            csPath = "";
            // 获取程序集所在目录
            var findAssets = AssetDatabase.FindAssets(fileName, searchInFolders);

            //如果找到了多个文件
            if (findAssets.Length > 1)
            {
                //检测每个结果
                List<string> findResult = new List<string>();
                foreach (var result in findAssets)
                {
                    //如果文件后缀是cs那就加入结果
                    string path = AssetDatabase.GUIDToAssetPath(result);
                    if (path.EndsWith(".cs"))
                    {
                        findResult.Add(path);
                    }
                }
                //如果只找到一个，那就返回
                if (findResult.Count == 1)
                {
                    csPath = findResult[0];
                    return true;
                }
                //有多个相同的
                string logContent = "类名" + fileName + "找到了多个可能路径:" + "\n";
                foreach (var findAsset in findAssets)
                {
                    logContent += AssetDatabase.GUIDToAssetPath(findAsset) + "\n";
                }

                logContent += "将返回输入的形参csPath+'\n";

                Debug.LogWarning(logContent);

                return false;
            }
            else if (findAssets.Length == 1)
            {
                csPath = AssetDatabase.GUIDToAssetPath(findAssets[0]).ConvertAssetPathToSystemPath();
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 查找某个指定的cs文件
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="csPath">输出结果</param>
        /// <param name="searchInFolders">限制的文件夹</param>
        /// <returns>如果找不到返回false</returns>
        private static bool TryGetCsFilePath<T>(out string csPath, string[] searchInFolders = null)
        {
            //如果没有限制的搜索文件夹，那就默认整个Assets
            searchInFolders ??= new string[] { "Assets" };

            return TryGetCsFilePath(typeof(T).Name, out csPath, searchInFolders);
        }

        /// <summary>
        /// 根据类型找到类文件的路径
        /// </summary>
        /// <returns></returns>
        public static string FindClassFilePath(Type type)
        {
            if (type == null)
            {
                return "";
            }

            string csPath = "";

            // 获取程序集所在目录
            string[] findAssets = AssetDatabase.FindAssets(type.Name);


            if (findAssets.Length > 1)
            {
                string logContent = "类名" + type.Name + "找到了多个可能路径:" + "\n";
                foreach (var findAsset in findAssets)
                {
                    logContent += AssetDatabase.GUIDToAssetPath(findAsset) + "\n";
                }

                logContent += "将返回第一条路径，请确认无误";
                Debug.LogWarning(logContent);

                csPath = findAssets[0];
                return csPath;
            }
            else if (findAssets.Length == 1)
            {
                return AssetDatabase.GUIDToAssetPath(findAssets[0]);
            }
            else
            {
                return csPath;
            }
        }


        private static int FindMatchingClosingBrace(string text, int startIndex)
        {
            var count = 0;

            for (var i = startIndex; i < text.Length; i++)
            {
                switch (text[i])
                {
                    case '{':
                        count++;
                        break;
                    case '}':
                    {
                        count--;
                        if (count == 0)
                        {
                            return i;
                        }

                        break;
                    }
                }
            }

            return -1; // 未找到匹配的右大括号
        }

    }

}
