using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace KFrame.Utilities
{
    /// <summary>
    /// <para>文件工具类</para>
    /// <para>1.包括了对编辑器中的文件进行操作</para>
    /// <para>2.包括了一些常用的单位 如字节</para>
    /// <para>3.包括了对UnityPackage的简化操作</para>
    /// <para>4.IO 操作</para>
    /// </summary>
    public static class FileExtensions
    {
        #region 计算机单位

        /// <summary>
        /// 字节单位（1字节 = 8位）
        /// </summary>
        public const int Byte = 8;

        /// <summary>
        /// 千字节单位（1 KB = 1024字节）
        /// </summary>
        public const int Kilobyte = 1024;

        /// <summary>
        /// 兆字节单位（1 MB = 1024 KB）
        /// </summary>
        public const int Megabyte = 1024 * Kilobyte;

        #endregion

        #region IO操作

        private static BinaryFormatter binaryFormatter = new BinaryFormatter();


        /// <summary>
        /// 保存Json
        /// </summary>
        /// <param name="jsonData">Json数据</param>
        /// <param name="path">路径</param>
        /// <param name="suffix">文件后缀</param>
        public static void SaveJson(string jsonData, string path, string suffix = ".json")
        {
            //如果路径结尾没有后缀那就加上
            if (!path.EndsWith(suffix))
            {
                path += suffix;
            }
            File.WriteAllText(path, jsonData);
        }
        /// <summary>
        /// 保存Json
        /// </summary>
        /// <param name="jsonString">Json的字符串</param>
        /// <param name="path">路径</param>
        public static void SaveJsonWithUnescape(string jsonString, string path)
        {
            // 使用 Regex.Unescape 方法还原Unicode转义字符
            string unescapedJson = Regex.Unescape(jsonString);
            SaveJson(unescapedJson, path);
        }
        /// <summary>
        /// 格式化Json字符串
        /// </summary>
        /// <param name="json">json数据</param>
        /// <returns>格式化后的字符串</returns>
        public static string FormatJson(string json)
        {
            int level = 0;
            var result = new StringBuilder();
            var inQuotes = false;
            var lastChar = '\0';

            foreach (var character in json)
            {
                switch (character)
                {
                    case '{':
                    case '[':
                        result.Append(character);
                        if (!inQuotes)
                        {
                            result.AppendLine();
                            result.Append(new string(' ', ++level * 4));
                        }

                        break;
                    case '}':
                    case ']':
                        if (!inQuotes)
                        {
                            result.AppendLine();
                            result.Append(new string(' ', --level * 4));
                        }

                        result.Append(character);
                        break;
                    case '"':
                        if (lastChar != '\\')
                        {
                            inQuotes = !inQuotes;
                        }

                        result.Append(character);
                        break;
                    case ',':
                        result.Append(character);
                        if (!inQuotes)
                        {
                            result.AppendLine();
                            result.Append(new string(' ', level * 4));
                        }

                        break;
                    case ':':
                        result.Append(" : ");
                        break;
                    default:
                        result.Append(character);
                        break;
                }

                lastChar = character;
            }

            return result.ToString();
        }

        /// <summary>
        /// 读取Json为指定的类型对象
        /// </summary>
        /// <returns>从目标位置加载json数据，如果不存在那就返回null</returns>
        public static T LoadJson<T>(string path) where T : class
        {
            //防止路径忘记加后缀
            if (!path.EndsWith(".json"))
            {
                path += ".json";
            }
            
            //如果不存在那就返回null
            if (!File.Exists(path))
            {
                return null;
            }

            return JsonUtility.FromJson<T>(File.ReadAllText(path));
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="saveObject">保存的对象</param>
        /// <param name="path">保存的路径</param>
        public static void SaveFile(object saveObject, string path)
        {
            FileStream f = new FileStream(path, FileMode.OpenOrCreate);
            // 二进制的方式把对象写进文件
            binaryFormatter.Serialize(f, saveObject);
            f.Dispose();
        }

        /// <summary>
        /// 加载文件
        /// </summary>
        /// <typeparam name="T">加载后要转为的类型</typeparam>
        /// <param name="path">加载路径</param>
        public static T LoadFile<T>(string path) where T : class
        {
            if (!File.Exists(path))
            {
                return null;
            }

            FileStream file = new FileStream(path, FileMode.Open);
            // 将内容解码成对象
            T obj = (T)binaryFormatter.Deserialize(file);
            file.Dispose();
            return obj;
        }

        #endregion

        /// <summary>
        /// 在资源管理器中打开本地数据路径。
        /// </summary>
        public static void OpenLocalPath()
        {
            UnityEngine.Application.OpenURL("file:////" + GetLocalPathTo());
        }

        public static void OpenPathFolder(string folderPath)
        {
            if (System.IO.Directory.Exists(folderPath))
            {
                //EditorUtility.RevealInFinder(folderPath); // 打开文件夹
                Application.OpenURL("file:////" + GetLocalPathTo() + folderPath);
            }
        }

        /// <summary>
        /// 创建文件夹如果不存在的话
        /// </summary>
        /// <param name="path">文件夹路径</param>
        public static void CreateDirectoryIfNotExist(string path)
        {
#if UNITY_EDITOR

            //如果是编辑器的文件夹
            if(path.StartsWith("Assets") && !AssetDatabase.IsValidFolder(path))
            {
                UnityEngine.Windows.Directory.CreateDirectory(path);
                return;
            }

#endif
            
            //如果存在就不用创建
            if (Directory.Exists(path)) return;

            //创建
            Directory.CreateDirectory(path);
        }

        /// <summary>
        /// (用处不大的方法)获取本地数据路径的父目录，并在路径末尾添加斜杠。
        /// </summary>
        /// <returns>应用程序数据路径的父目录。</returns>
        public static string GetLocalPathTo()
        {
            // 获取应用程序数据路径
            string dataPath = UnityEngine.Application.dataPath;

            // 获取应用程序数据路径的父目录，并在末尾添加斜杠
            string parentPath = Path.GetDirectoryName(dataPath);
            // 使用 Path.DirectorySeparatorChar 来获取适用于当前操作系统的目录分隔符以替代固定的斜杠。
            string localPathToParent = parentPath + Path.DirectorySeparatorChar;

            return localPathToParent;
        }


        /// <summary>
        /// (用处不大的方法)获取应用程序数据路径，并在路径末尾添加斜杠。
        /// </summary>
        /// <returns>应用程序数据路径。</returns>
        public static string GetAssetsPathTo()
        {
            return UnityEngine.Application.dataPath + "/";
        }

        /// <summary>
        /// 把Unity相对路径转换成系统绝对路径
        /// </summary>
        /// <param name="assetsPath"></param>
        /// <returns></returns>
        public static string ConvertAssetPathToSystemPath(this string assetsPath)
        {
            return Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length) + assetsPath;
        }

        /// <summary>
        /// 把系统绝对路径转换成Unity相对路径
        /// </summary>
        /// <returns></returns>
        public static string ConvertSystemPathToAssetPath(this string systemPath)
        {
            //把前缀给换掉
            return systemPath.Replace(Application.dataPath, "Assets");
        }

        /// <summary>
        /// 将输入的文本保存到指定的文件路径，并输出日志信息。
        /// </summary>
        /// <param name="path">要保存到的文件路径。</param>
        /// <param name="text">要保存的文本内容。</param>
        public static void SaveTextToFile(string path, string text)
        {
            try
            {
                // 将文本保存到指定的文件路径
                File.WriteAllText(path, text);
                UnityEngine.Debug.Log("文本保存至: " + path);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("文本保存到文件时出错: " + e.Message);
            }
        }


        /// <summary>
        /// 通过递归获取向上几级的路径
        /// </summary>
        /// <param name="path">当前路径</param>
        /// <param name="levels">向上几级</param>
        /// <returns></returns>
        public static string GetParentDirectory(string path, int levels)
        {
            if (levels <= 0 || string.IsNullOrEmpty(path))
            {
                return path;
            }

            // 获取上一级目录的路径
            string parentDirectory = Path.GetDirectoryName(path);

            // 递归调用以获取更上一级的目录
            return GetParentDirectory(parentDirectory, levels - 1);
        }
        
        /// <summary>
        /// 整理返回一个合理的文件夹路径
        /// </summary>
        /// <param name="path">原路径</param>
        /// <returns>一个合理的路径</returns>
        public static string GetNiceDirectoryPath(this string path)
        {
            return path.Trim().TrimEnd('/', '\\').TrimStart('/', '\\')
                .Replace('\\', '/') + "/";
        }

        /// <summary>
        /// 检测Unity的资源是否存在 资源路径以相对的Assets开头
        /// </summary>
        public static bool CheckUnityFileExists(string path)
        {
            string projectFolderPath = Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length);
            return File.Exists(projectFolderPath + path);
        }

#if UNITY_EDITOR

        /// <summary>
        /// 将指定的资源路径导出为资源包文件，并进行递归导出。
        /// </summary>
        /// <param name="assetPathName">要导出的资源路径。</param>
        /// <param name="fileName">导出的文件名。</param>
        public static void ExportPackage(string assetPathName, string fileName)
        {
            try
            {
                // 使用 AssetDatabase.ExportPackage 方法进行资源包导出
                UnityEditor.AssetDatabase.ExportPackage(assetPathName, fileName,
                    UnityEditor.ExportPackageOptions.Recurse);
                UnityEngine.Debug.Log("UnityPackage包导出至: " + fileName);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("包导出失败: " + e.Message);
            }
        }
#endif


    }
}