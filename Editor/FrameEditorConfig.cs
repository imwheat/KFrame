//****************** 代码文件申明 ***********************
//* 文件：FrameEditorConfig
//* 作者：wheat
//* 创建时间：2024/10/24 21:34:04 星期四
//* 描述：框架编辑器配置
//*******************************************************

using KFrame.Attributes;
using KFrame.Utilities;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.Serialization;

namespace KFrame.Editor
{
    [KGlobalConfigPath(GlobalPathType.Editor, "FrameEditorConfig", true)]
    public class FrameEditorConfig : GlobalConfigBase<FrameEditorConfig>
    {
        /// <summary>
        /// AB包配置
        /// </summary>
        [SerializeField]
        private AddressableAssetSettings addressableAssetSettings;

        /// <summary>
        /// AB包配置
        /// </summary>
        public AddressableAssetSettings AddressableAssetSettings
        {
            get
            {
                if (addressableAssetSettings == null)
                {
                    var find = AssetDatabase.FindAssets("t:AddressableAssetSettings");
                    if (find != null && find.Length > 0)
                    {
                        addressableAssetSettings =
                            AssetDatabase.LoadAssetAtPath<AddressableAssetSettings>(
                                AssetDatabase.GUIDToAssetPath(find[0]));
                    }
                }

                return addressableAssetSettings;
            }
        }
    }
}

