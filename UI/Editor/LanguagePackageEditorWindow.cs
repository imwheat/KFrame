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
        /// <summary>
        /// key的列表ScrollPosition
        /// </summary>
        private Vector2 keyListScrollPos;
        /// <summary>
        /// 要更改的心得语言名称
        /// </summary>
        private string newLanguageName = "";
        /// <summary>
        /// 编辑语言名称
        /// </summary>
        private bool editLanguageName = false;

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

        #region GUI绘制

        /// <summary>
        /// 绘制文本数据GUI
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="index">下标</param>
        private void DrawLanguagePackageTextGUI(LanguagePackageTextData data, int index)
        {
            EditorGUILayout.BeginHorizontal();
            
            
            
            EditorGUILayout.EndHorizontal();
        }
        /// <summary>
        /// 绘制顶部GUI
        /// </summary>
        private void DrawTopGUI()
        {
            EditorGUILayout.BeginVertical();
            
            //id
            EditorGUILayout.LabelField($"语言id {editPackage.language.languageId}");
            
            //语言名称
            EditorGUILayout.BeginHorizontal();
            
            EditorGUILayout.LabelField("语言名称");

            if (editLanguageName)
            {
                newLanguageName = EditorGUILayout.TextField(newLanguageName);
                if (GUILayout.Button("保存",GUILayout.Width(40f)))
                {
                    EditorConfig.UpdateLanguageName(editPackage.language.languageName, newLanguageName);
                    editLanguageName = false;
                    Repaint();
                }
                if (GUILayout.Button("取消",GUILayout.Width(40f)))
                {
                    editLanguageName = false;
                    Repaint();
                }
            }
            else
            {
                EditorGUILayout.LabelField(editPackage.language.languageName);
                if (GUILayout.Button("编辑", GUILayout.Width(40f)))
                {
                    editLanguageName = true;
                    newLanguageName = editPackage.language.languageName;
                    Repaint();
                }
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.EndVertical();
        }
        /// <summary>
        /// 绘制主要GUI
        /// </summary>
        private void DrawMainGUI()
        {
            EditorGUILayout.BeginVertical();
            
            //遍历绘制GUI
            for (int i = 0; i < editPackage.datas.Count; i++)
            {
                DrawLanguagePackageTextGUI(editPackage.datas[i], i);
            }
            
            EditorGUILayout.EndVertical();
        }
        /// <summary>
        /// 绘制底部GUI
        /// </summary>
        private void DrawBottomGUI()
        {
            
        }
        
        protected override void OnGUI()
        {
            //绘制顶部GUI
            DrawTopGUI();
            //绘制主要GUI
            DrawMainGUI();
            //绘制底部GUI
            DrawBottomGUI();
            
            base.OnGUI();
        }

        #endregion

    }
}

