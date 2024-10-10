//****************** 代码文件申明 ***********************
//* 文件：LanguageKeyCreateWindow
//* 作者：wheat
//* 创建时间：2024/10/10 09:07:09 星期四
//* 描述：语言包key创建窗口
//*******************************************************

using System.Collections.Generic;
using KFrame.Editor;
using UnityEditor;
using UnityEngine;

namespace KFrame.UI.Editor
{
    public class LanguageKeyCreateWindow : KEditorWindow
    {
        #region 参数引用
        
        /// <summary>
        /// 编辑器配置
        /// </summary>
        private static LocalizationEditorConfig EditorConfig => LocalizationEditorConfig.Instance;

        #endregion

        #region 编辑数据
        
        /// <summary>
        /// 待添加的列表
        /// </summary>
        private List<LanguagePackageKeyData> addList = new();

        #endregion

        #region GUI绘制相关
        
        /// <summary>
        /// GUI空格
        /// </summary>
        private static float Spacing => KGUIStyle.spacing;
        /// <summary>
        /// 每列的标题
        /// </summary>
        private static GUIStyle rowTitle;
        /// <summary>
        /// 每列的标题
        /// </summary>
        private static GUIStyle RowTitle
        {
            get
            {
                if (rowTitle == null)
                {
                    rowTitle = new GUIStyle(EditorStyles.boldLabel)
                    {
                        alignment = TextAnchor.MiddleCenter,
                    };
                }

                return rowTitle;
            }
        }
        /// <summary>
        /// label高度
        /// </summary>
        private static float labelHeight = 20f;
        /// <summary>
        /// 分割线宽度
        /// </summary>
        private static float lineWidth = 2f;
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

        #endregion

        #region 初始化相关

        private void Init()
        {
        }
        /// <summary>
        /// 打开语言包key编辑器创建面板
        /// </summary>
        /// <returns>该面板</returns>
        public static LanguageKeyCreateWindow ShowWindow()
        {
            LanguageKeyCreateWindow window = GetWindow<LanguageKeyCreateWindow>();
            window.titleContent = new GUIContent("语言包key创建器");
            window.Init();

            return window;
        }

        #endregion

        #region GUI绘制
        
        /// <summary>
        /// 绘制KeyData的GUI
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="index">列表下标</param>
        private void DrawKeyDataGUI(LanguagePackageKeyData data, int index)
        { 
            EditorGUILayout.BeginHorizontal();
            
            //获取列间隔
            float rowInterval = RowInterval;
            
            Rect dataRect = EditorGUILayout.GetControlRect(GUILayout.Height(labelHeight),
                GUILayout.Width(rowWidth));
            //key
            data.key = EditorGUI.TextField(dataRect, data.key);
            dataRect.x += rowInterval;
            
            //备注
            data.notes = EditorGUI.TextField(dataRect, data.notes);
            dataRect.x += rowInterval;
            
            //操作选项
            
            //删除数据
            if (GUI.Button(dataRect, "删除"))
            {
                addList.RemoveAt(index);
                Repaint();
            }
            
            EditorGUILayout.EndHorizontal();
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
        /// 绘制顶部GUI
        /// </summary>
        private void DrawTopGUI()
        {
            EditorGUILayout.BeginVertical();

            GUILayout.Space(5f);
            
            EditorGUILayout.BeginHorizontal();
            
            //Key
            Rect titleRect = EditorGUILayout.GetControlRect(GUILayout.Height(labelHeight),
                GUILayout.Width(rowWidth));
            EditorGUI.LabelField(titleRect,"Key", RowTitle);
            titleRect.x += RowInterval;
            
            //备注
            EditorGUI.LabelField(titleRect,"备注", RowTitle);
            titleRect.x += RowInterval;
            
            //编辑操作
            EditorGUI.LabelField(titleRect, "编辑操作", RowTitle);
            
            EditorGUILayout.EndHorizontal();
            
            GUILayout.Space(10f);
            
            EditorGUILayout.EndVertical();
        }
        /// <summary>
        /// 绘制分割线
        /// </summary>
        private void DrawLines()
        {
            EditorGUILayout.BeginVertical();

            //绘制横线
            Rect lineRect =
                EditorGUILayout.GetControlRect(GUILayout.ExpandWidth(true), GUILayout.Height(lineWidth));
            EditorGUI.DrawRect(lineRect, Color.black);
            
            //绘制竖线
            Rect rowLine = new Rect(rowWidth + rowSpacing / 2f, lineRect.y + lineWidth / 2f,
                lineWidth, position.height - lineRect.y - labelHeight - Spacing);
            for (int i = 0; i < rowCount - 1; i++)
            {
                EditorGUI.DrawRect(rowLine, Color.black);
                rowLine.x += RowInterval;
            }
            
            EditorGUILayout.EndVertical();
        }
        /// <summary>
        /// 绘制主要部分GUI
        /// </summary>
        private void DrawMainGUI()
        {
            //绘制分割线
            DrawLines();
            
            EditorGUILayout.BeginVertical();

            //遍历绘制GUI
            for (int i = 0; i < addList.Count; i++)
            {
                DrawKeyDataGUI(addList[i], i);
                GUILayout.Space(Spacing);
            }
            
            //新建按钮
            Rect addButtonRect = EditorGUILayout.GetControlRect(GUILayout.Height(labelHeight),
                GUILayout.Width(rowWidth));
            addButtonRect.x += RowInterval * 2f;
            if (GUI.Button(addButtonRect,"新建添加"))
            {
                addList.Add(new LanguagePackageKeyData(""));
            }
            
            EditorGUILayout.EndVertical();
        }
        /// <summary>
        /// 绘制底部的GUI
        /// </summary>
        private void DrawBottomGUI()
        {
            EditorGUILayout.BeginVertical();
            
            GUILayout.FlexibleSpace();
            
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("创建添加"))
            {
                AddKeys();
            }
            
            EditorGUILayout.EndHorizontal();
            
            GUILayout.Space(Spacing);

            EditorGUILayout.EndVertical();
        }
        protected override void OnGUI()
        {
            //检测GUI更新
            CheckGUILayoutUpdate();
            //绘制顶部GUI
            DrawTopGUI();
            //绘制主要部分GUI
            DrawMainGUI();
            //绘制底部GUI
            DrawBottomGUI();
            
            base.OnGUI();
        }

        #endregion

        #region 编辑创建
    
        /// <summary>
        /// 检测要添加的key是否合理
        /// </summary>
        /// <param name="data">待添加数据</param>
        /// <returns>不合理返回false</returns>
        private bool CheckAddKeyData(LanguagePackageKeyData data)
        {
            if(data == null || string.IsNullOrEmpty(data.key))
            {
                EditorUtility.DisplayDialog("错误", "添加的key不能为空！", "确定");
                return false;
            }

            if (EditorConfig.KeyDic.ContainsKey(data.key))
            {
                EditorUtility.DisplayDialog("错误", $"已经存在key为{data.key}的数据了，不能重复添加！", "确定");
                return false;
            }
            
            return true;
        }
        /// <summary>
        /// 创建添加key
        /// </summary>
        private void AddKeys()
        {
            //遍历待添加的数据
            foreach (var data in addList)
            {
                //只要有一个不合理那就返回
                if (!CheckAddKeyData(data))
                {
                    return;
                }
            }
            //再进行互相检测
            HashSet<string> addKey = new HashSet<string>();
            foreach (var data in addList)
            {
                if (!addKey.Add(data.key))
                {
                    EditorUtility.DisplayDialog("错误", $"新增列表中存在多个key为{data.key}的数据！\nkey不能重复！", "确定");
                    return;
                }
            }
            
            //检查没有问题了，那就逐个添加进数据库
            foreach (var data in addList)
            {
                EditorConfig.AddKey(data);
            }
            
            //添加完成，清空数据
            addList.Clear();
            
            //保存
            EditorConfig.SaveAsset();
        }

        #endregion
    }
}

