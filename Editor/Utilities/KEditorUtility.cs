//****************** 代码文件申明 ***********************
//* 文件：KEditorUtility
//* 作者：wheat
//* 创建时间：2024/06/07 09:13:48 星期五
//* 描述：拓展一些EditorUtility的功能
//*******************************************************

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
    }
}

