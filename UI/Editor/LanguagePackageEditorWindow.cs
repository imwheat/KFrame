//****************** 代码文件申明 ***********************
//* 文件：LanguagePackageEditorWindow
//* 作者：wheat
//* 创建时间：2024/10/10 09:09:22 星期四
//* 描述：语言包编辑器
//*******************************************************

using System;
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
        private string newLanguageKey = "";
        /// <summary>
        /// 编辑语言名称
        /// </summary>
        private bool editLanguageName = false;
        /// <summary>
        /// key搜索
        /// </summary>
        private string keySearchText;
        /// <summary>
        /// 仅显示空选项
        /// </summary>
        private bool onlyShowEmpty;
        /// <summary>
        /// 当前正在编辑的选项
        /// </summary>
        private int curEditIndex = -1;

        
        #endregion

        #region GUI绘制

        /// <summary>
        /// GUI空格
        /// </summary>
        private static float Spacing => KGUIStyle.spacing;
        /// <summary>
        /// 线条宽度
        /// </summary>
        private static readonly float LineWidth = 2f;
        /// <summary>
        /// 每列的标题
        /// </summary>
        private static GUIStyle titleStyle;
        /// <summary>
        /// 每列的标题
        /// </summary>
        private static GUIStyle TitleStyle
        {
            get
            {
                if (titleStyle == null)
                {
                    titleStyle = new GUIStyle(EditorStyles.boldLabel)
                    {
                        alignment = TextAnchor.MiddleCenter,
                    };
                }

                return titleStyle;
            }
        }
        /// <summary>
        /// 列的数量
        /// </summary>
        private static int rowCount = 3;
        /// <summary>
        /// 列空格
        /// </summary>
        private static float rowSpacing = 15f;
        /// <summary>
        /// 列宽
        /// </summary>
        private static float rowWidth = -1f;
        /// <summary>
        /// 列间隔
        /// </summary>
        private static float RowInterval => rowSpacing + rowWidth;     
        /// <summary>
        /// 上次更新的GUI宽度
        /// </summary>
        private static float prevUpdateGUIWidth = -1f;
        /// <summary>
        /// label高度
        /// </summary>
        private static float labelHeight = 20f;
        /// <summary>
        /// 数据列表ScrollPosition
        /// </summary>
        private Vector2 dataListScrollPos;

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

        private void OnDestroy()
        {
            if (editPackage != null)
            {
                EditorUtility.SetDirty(editPackage);
                EditorUtility.SetDirty(LocalizationEditorConfig.Instance);
                LocalizationConfig.Instance.SaveAsset();
            }
        }

        #endregion

        #region GUI绘制
        
        /// <summary>
        /// 切换正在编辑的数据
        /// </summary>
        /// <param name="i">新的id</param>
        private void SwitchEditData(int i)
        {
            //如果是同一个那就返回
            if(curEditIndex == i) return;
            
            //更新状态
            curEditIndex = i;
        }
        /// <summary>
        /// 更新GUI的排版
        /// </summary>
        private void UpdateGUILayout()
        {
            //记录GUI宽度
            prevUpdateGUIWidth = position.width;
            
            //计算更新每列的宽度
            rowWidth = (position.width- rowSpacing * (rowCount -1)) / rowCount;
            
            //重绘
            Repaint();
        }
        /// <summary>
        /// 检测是否要进行GUI排版更新
        /// </summary>
        private void CheckGUILayoutUpdate()
        {
            //如果还没进行过更新，或者面板UI进行过较大程度的调整那就更新分布
            if (prevUpdateGUIWidth < 0f || Mathf.Abs(position.width - prevUpdateGUIWidth) > 1f)
            {
                UpdateGUILayout();
            }
        }
        /// <summary>
        /// 绘制文本数据GUI
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="index">下标</param>
        private void DrawLanguagePackageTextGUI(LanguagePackageTextData data, int index)
        {
            EditorGUILayout.BeginHorizontal();
            
            EditorGUI.BeginChangeCheck();
            
            //获取列间隔
            float rowInterval = RowInterval;
            
            Rect dataRect = EditorGUILayout.GetControlRect(GUILayout.Height(labelHeight),
                GUILayout.Width(rowWidth));
            //key
            EditorGUI.LabelField(dataRect, data.key, TitleStyle);
            dataRect.x += rowInterval;
            
            //备注
            string notes = "";
            if (EditorConfig.KeyDic.TryGetValue(data.key, out var keyData))
            {
                notes = keyData.notes;
            }
            EditorGUI.LabelField(dataRect, notes, TitleStyle);
            dataRect.x += rowInterval;
            
            //文本
            dataRect.xMax -= rowSpacing/2f + Spacing;
            data.text = EditorGUI.TextField(dataRect, data.text);

            if (EditorGUI.EndChangeCheck())
            {
                SwitchEditData(index);
            }
            
            EditorGUILayout.EndHorizontal();
        }
        /// <summary>
        /// 绘制顶部GUI
        /// </summary>
        private void DrawTopGUI()
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();
            //id
            EditorGUILayout.LabelField($"语言id {editPackage.language.languageId}");
            
            //语言名称
            editPackage.LanguageName = EditorGUILayout.TextField("语言名称",editPackage.LanguageName);
            
            EditorGUILayout.EndHorizontal();
            
            //语言key
            EditorGUILayout.BeginHorizontal();
            
            EditorGUILayout.LabelField("语言key");

            if (editLanguageName)
            {
                newLanguageKey = EditorGUILayout.TextField(newLanguageKey);
                if (GUILayout.Button("保存",GUILayout.Width(40f)))
                {
                    EditorConfig.UpdateLanguageKey(editPackage.language.languageKey, newLanguageKey);
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
                EditorGUILayout.LabelField(editPackage.language.languageKey);
                if (GUILayout.Button("编辑", GUILayout.Width(40f)))
                {
                    editLanguageName = true;
                    newLanguageKey = editPackage.language.languageKey;
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
            
            //搜索栏
            Rect searchRect = EditorGUILayout.GetControlRect(GUILayout.Height(labelHeight),
                GUILayout.Width(rowWidth));
            searchRect.x += RowInterval * 2f;
            searchRect.xMax -= rowSpacing / 2f + Spacing / 2f;
            keySearchText = KEditorGUI.SearchTextField(searchRect, keySearchText, false);
            
            //显示标题
            EditorGUILayout.BeginHorizontal();
            
            //Key
            Rect titleRect = EditorGUILayout.GetControlRect(GUILayout.Height(labelHeight),
                GUILayout.Width(rowWidth));
            EditorGUI.LabelField(titleRect,"Key", TitleStyle);
            titleRect.x += RowInterval;
            
            //备注
            EditorGUI.LabelField(titleRect,"备注", TitleStyle);
            titleRect.x += RowInterval;
            
            //文本
            EditorGUI.LabelField(titleRect, "文本", TitleStyle);
            
            EditorGUILayout.EndHorizontal();
            
            
            //绘制分割线
            Rect lineRect = EditorGUILayout.GetControlRect(GUILayout.Width(position.width), GUILayout.Height(LineWidth));
            EditorGUI.DrawRect(lineRect, Color.black);
            //绘制竖线
            Rect rowLine = new Rect(rowWidth + rowSpacing / 2f, lineRect.y + LineWidth / 2f,
                LineWidth, position.height - lineRect.y - labelHeight - Spacing);
            for (int i = 0; i < rowCount - 1; i++)
            {
                EditorGUI.DrawRect(rowLine, Color.black);
                rowLine.x += RowInterval;
            }
            GUILayout.Space(Spacing);
            
            dataListScrollPos = EditorGUILayout.BeginScrollView(dataListScrollPos);
            
            //遍历绘制GUI
            for (int i = 0; i < editPackage.datas.Count; i++)
            {
                var data = editPackage.datas[i];
                //如果不是当前正在编辑的，那就要进行筛选
                if (i != curEditIndex)
                {
                    //如果不符合搜索那就跳过
                    if(!data.key.Contains(keySearchText)) continue;
                    //如果不是空的，并且只显示空选项，那就跳过
                    if(!string.IsNullOrEmpty(data.text) && onlyShowEmpty) continue;
                }
                
                DrawLanguagePackageTextGUI(data, i);
                GUILayout.Space(Spacing);
            }
            
            EditorGUILayout.EndScrollView();
            
            EditorGUILayout.EndVertical();
        }
        /// <summary>
        /// 绘制底部GUI
        /// </summary>
        private void DrawBottomGUI()
        {
            EditorGUILayout.BeginVertical();
            
            GUILayout.FlexibleSpace();
            //绘制分割线
            Rect lineRect = EditorGUILayout.GetControlRect(GUILayout.Width(position.width), GUILayout.Height(LineWidth));
            EditorGUI.DrawRect(lineRect, Color.black);
            GUILayout.Space(Spacing);

            EditorGUILayout.BeginHorizontal();
            
            EditorGUITool.ShowAClearFoldOut(ref onlyShowEmpty, "仅显示空选项");
            
            EditorGUILayout.EndHorizontal();
            
            
            EditorGUILayout.EndVertical();
        }
        
        protected override void OnGUI()
        {
            //检测GUI排版更新
            CheckGUILayoutUpdate();
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

