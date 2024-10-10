//****************** 代码文件申明 ***********************
//* 文件：LanguagePackageEditorWindow
//* 作者：wheat
//* 创建时间：2024/10/10 09:09:22 星期四
//* 描述：语言包编辑器
//*******************************************************

using UnityEditor;
using KFrame.Editor;
using UnityEngine;

namespace KFrame.UI.Editor
{
    public class LanguagePackageEditorWindow : KEditorWindow
    {
        #region 数据引用
        
        /// <summary>
        /// 本地化编辑器配置数据
        /// </summary>
        private static LocalizationEditorConfig EditorConfig => LocalizationEditorConfig.Instance;

        #endregion
        
        #region 编辑参数

        /// <summary>
        /// 正在编辑的语言包
        /// </summary>
        private LanguagePackage editPackage;

        #endregion

        #region 初始化
        
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="package">要编辑的语言包</param>
        private void Init(LanguagePackage package)
        {
            editPackage = package;
        }
        /// <summary>
        /// 打开窗口，开始编辑
        /// </summary>
        /// <param name="package">要编辑的语言包</param>
        /// <returns>当前窗口</returns>
        public static LanguagePackageEditorWindow ShowWindow(LanguagePackage package)
        {
            if (package == null)
            {
                EditorUtility.DisplayDialog("错误", "要打开的语言包为空！", "确认");
                return null;
            }

            LanguagePackageEditorWindow window = GetWindow<LanguagePackageEditorWindow>();
            window.titleContent = new GUIContent("语言包编辑器");
            window.Init(package);

            return window;
        }

        #endregion

    }
}

