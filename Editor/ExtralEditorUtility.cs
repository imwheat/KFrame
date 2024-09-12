using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace KFrame.Editor
{
    public static class ExtralEditorUtility
    {

        #region 获取对象的Hierarchy路径

        /// <summary>
        /// 获取对象的Hierarchy路径
        /// </summary>
        /// <param name="target">GameObject</param>
        public static void GetGameObjectHierarchyPath(GameObject target)
        {
            Transform targetTransform = target.transform;
            GetGameObjectHierarchyPath(targetTransform);
        }

        /// <summary>
        /// 获取对象的Hierarchy路径，针对Transform组件
        /// </summary>
        /// <param name="target">Transform</param>
        public static void GetGameObjectHierarchyPath(Transform target)
        {
            string resultPath = target.name;
            //一直遍历到没有父节点为止
            while (target.parent != null)
            {
                target = target.parent;
                resultPath = target.name + "/" + resultPath;
            }

            GUIUtility.systemCopyBuffer = resultPath;
        }

        #endregion


        #region 打开浏览器网页

        public static void OpenURLInWebBrowser(string url)
        {
            // 使用EditorUtility打开默认浏览器来访问链接
            EditorUtility.OpenWithDefaultApp(url);
        }

        #endregion


        public static List<string> FindFoldersWithEndingPath(string endingPath)
        {
            List<string> matchingFolders = new List<string>();
            string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();

            foreach (string assetPath in allAssetPaths)
            {
                if (assetPath.EndsWith(endingPath) && AssetDatabase.IsValidFolder(assetPath))
                {
                    matchingFolders.Add(assetPath);
                }
            }

            return matchingFolders;
        }
    }

}

