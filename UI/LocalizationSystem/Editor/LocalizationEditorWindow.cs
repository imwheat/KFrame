//****************** 代码文件申明 ***********************
//* 文件：LocalizationEditorWindow
//* 作者：wheat
//* 创建时间：2024/09/21 16:04:39 星期六
//* 描述：浏览、编辑本地化配置的编辑器
//*******************************************************

using UnityEngine;
using KFrame;
using KFrame.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using KFrame.Editor;
using UnityEditor;

namespace KFrame.UI
{
    public class LocalizationEditorWindow : KEditorWindow
    {

        #region 数据引用
        
        /// <summary>
        /// 本地化配置的数据
        /// </summary>
        private LocalizationConfig config => LocalizationConfig.Instance;

        #endregion
        
        #region GUI绘制相关
        
        /// <summary>
        /// 选择绘制类型
        /// </summary>
        private enum SelectType
        {
            StringData = 0,
            ImageData = 1,
        }
        /// <summary>
        /// 编辑器模式
        /// </summary>
        private enum EditorMode
        {
            /// <summary>
            /// 编辑模式
            /// </summary>
            Edit = 0,
            /// <summary>
            /// 选择模式
            /// </summary>
            Select = 1,
        }
        private static class MStyle
        {
#if UNITY_2018_3_OR_NEWER
            internal static readonly float spacing = EditorGUIUtility.standardVerticalSpacing;
#else
            internal static readonly float spacing = 2.0f;
#endif
            /// <summary>
            /// 按钮宽度
            /// </summary>
            internal static readonly float optionBtnWidth = 40f;
            /// <summary>
            /// Label高度
            /// </summary>
            internal static readonly float labelHeight = 20f;
            /// <summary>
            /// 筛选类型的宽度
            /// </summary>
            internal static readonly float filterTypeWidth = 80f;
            /// <summary>
            /// 线条宽度
            /// </summary>
            internal static readonly float lineWidth = 2f;
            /// <summary>
            /// 列宽度
            /// </summary>
            internal static float rowWidth = -1f;
            /// <summary>
            /// 列数量
            /// </summary>
            internal static int rowCount = -1;
            /// <summary>
            /// 可见的语言数量
            /// </summary>
            internal static int visibleLanguageCount = -1;
            /// <summary>
            /// 上次更新列宽度GUI的宽度
            /// </summary>
            internal static float prevUpdateGUIWidth = -1f;
            /// <summary>
            /// 列空格
            /// </summary>
            internal static float rowSpacing = 15f;
            /// <summary>
            /// 列间隔
            /// </summary>
            internal static float rowInterval => rowSpacing + rowWidth;

            /// <summary>
            /// 每列的标题
            /// </summary>
            private static GUIStyle rowTitle;
            /// <summary>
            /// 每列的标题
            /// </summary>
            internal static GUIStyle RowTitle
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
        }
        /// <summary>
        /// 当前选择的绘制类型
        /// </summary>
        private SelectType curSelectType;
        /// <summary>
        /// 编辑器模式
        /// </summary>
        private EditorMode editorMode;
        /// <summary>
        /// 选择的回调
        /// </summary>
        private Action<LocalizationDataBase> selectCallback; 
        /// <summary>
        /// 当前语言种类
        /// </summary>
        private LanguageType[] languageTypes;
        /// <summary>
        /// 语言筛选
        /// </summary>
        private Dictionary<LanguageType, bool> languageFilter;
        /// <summary>
        /// 记录对应语言的GUI位置坐标
        /// </summary>
        private Dictionary<LanguageType, float> languageGUIPos;
        /// <summary>
        /// 筛选栏折叠
        /// </summary>
        private bool filterFoldout;
        /// <summary>
        /// 文本数据搜索栏
        /// </summary>
        private string stringDataSearchText;
        /// <summary>
        /// 图片数据搜索栏
        /// </summary>
        private string imageDataSearchText;
        /// <summary>
        /// 文本数据的列表滚轮位置
        /// </summary>
        private Vector2 stringDataListScrollPos;
        /// <summary>
        /// 图片数据的列表滚轮位置
        /// </summary>
        private Vector2 imageDataListScrollPos;
        /// <summary>
        /// 正在修改编辑的GUI的key的下标
        /// </summary>
        private int editKeyIndex = -1;
        /// <summary>
        /// 正在编辑的Key的文本
        /// </summary>
        private string editKeyText;
        
        #endregion

        #region 生命周期

        private void Awake()
        {
            Init();
        }

        private void OnEnable()
        {
            Init();
        }

        #endregion
        
        #region 初始化

        /// <summary>
        /// 打开窗口
        /// </summary>
        public static void ShowWindow()
        {
            LocalizationEditorWindow window = EditorWindow.GetWindow<LocalizationEditorWindow>();
            window.titleContent = new GUIContent("本地化编辑器");
            window.editorMode = EditorMode.Edit;
        }
        /// <summary>
        /// 打开窗口选择文本数据
        /// </summary>
        /// <param name="selectCallback">选择后的回调</param>
        public static void ShowWindowAsStringSelector(Action<LocalizationDataBase> selectCallback)
        {
            LocalizationEditorWindow window = EditorWindow.GetWindow<LocalizationEditorWindow>();
            window.titleContent = new GUIContent("点击选择一个Data");
            window.editorMode = EditorMode.Select;
            window.curSelectType = SelectType.StringData;
            window.selectCallback = selectCallback;
        }
        /// <summary>
        /// 打开窗口选择图片数据
        /// </summary>
        /// <param name="selectCallback">选择后的回调</param>
        public static void ShowWindowAsImageSelector(Action<LocalizationDataBase> selectCallback)
        {
            LocalizationEditorWindow window = EditorWindow.GetWindow<LocalizationEditorWindow>();
            window.titleContent = new GUIContent("点击选择一个Data");
            window.editorMode = EditorMode.Select;
            window.curSelectType = SelectType.ImageData;
            window.selectCallback = selectCallback;
        }
        private void Init()
        {
            languageTypes = EnumExtensions.GetValues<LanguageType>();
            MStyle.visibleLanguageCount = languageTypes.Length;
            languageFilter = new Dictionary<LanguageType, bool>();
            languageGUIPos = new Dictionary<LanguageType, float>();
            foreach (LanguageType languageType in languageTypes)
            {
                languageFilter[languageType] = true;
            }
            
            //更新GUI分布
            UpdateRowLayout();
        }
        
        #endregion

        #region 绘制单个GUI

        /// <summary>
        /// 绘制单个筛选类型选项
        /// </summary>
        private void DrawTypeFilter(LanguageType type)
        {
            float toggleWdith = Mathf.Min(MStyle.filterTypeWidth / 3f, 20f);
            float labelWidth = MStyle.filterTypeWidth - toggleWdith - 5f;
            
            EditorGUILayout.LabelField(EditorGUITool.TryGetEnumLabel(type),
                GUILayout.Width(labelWidth), GUILayout.Height(MStyle.labelHeight));
            
            GUILayout.Space(5f);
            
            EditorGUI.BeginChangeCheck();
            
            languageFilter[type] = EditorGUILayout.Toggle(languageFilter[type],
                GUILayout.Width(toggleWdith), GUILayout.Height(MStyle.labelHeight));

            //如果筛选条件变了，列的排布需要更新
            if (EditorGUI.EndChangeCheck())
            {
                //更新可见语言数量
                if (languageFilter[type])
                {
                    MStyle.visibleLanguageCount++;
                }
                else
                {
                    MStyle.visibleLanguageCount--;
                }
                
                UpdateRowLayout();
            }
            
        }
        /// <summary>
        /// 绘制一行文本数据GUI
        /// </summary>
        /// <param name="data">要绘制的数据</param>
        /// <param name="index">在列表里面的下标</param>
        private void DrawStringDataGUI(LocalizationStringData data, int index)
        {
            EditorGUILayout.BeginHorizontal();
            
            //获取列间隔
            float rowInterval = MStyle.rowInterval;
            
            Rect dataRect = EditorGUILayout.GetControlRect(GUILayout.Height(MStyle.labelHeight),
                GUILayout.Width(MStyle.rowWidth));
            //如果选择编辑key，那就显示为输入栏
            if (editKeyIndex == index)
            {
                editKeyText = EditorGUI.TextField(dataRect, editKeyText);
            }
            //正常就显示key
            else
            {
                EditorGUI.LabelField(dataRect,data.Key);
            }
            dataRect.x += rowInterval * (MStyle.visibleLanguageCount + 1);
            Rect languageRect = dataRect;
            
            EditorGUI.BeginChangeCheck();
            
            //绘制每个选项
            for (int i = 0; i < data.Datas.Count; i++)
            {
                LocalizationStringDataBase stringData = data.Datas[i];
                //如果不显示这个语言那就跳过
                if(!languageFilter[stringData.Language]) continue;
                languageRect.x = languageGUIPos[stringData.Language];
                switch (editorMode)
                {
                    case EditorMode.Edit:
                        stringData.Text = EditorGUI.TextField(languageRect, stringData.Text);
                        break;
                    case EditorMode.Select:
                        EditorGUI.LabelField(languageRect, stringData.Text);
                        break;
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(config);
            }

            switch (editorMode)
            {
                case EditorMode.Edit:
                    dataRect.size = new Vector2((dataRect.size.x - MStyle.rowSpacing) / 2f, dataRect.size.y);
                    dataRect.x -= MStyle.spacing;
                    if (editKeyIndex == index)
                    {
                        if (GUI.Button(dataRect, "保存"))
                        {
                            UpdateStringKey(data, editKeyText);
                    
                            editKeyIndex = -1;
                            Repaint();
                        }

                        dataRect.x += dataRect.size.x + MStyle.rowSpacing - MStyle.spacing;
            
                        if (GUI.Button(dataRect, "取消"))
                        {
                            editKeyIndex = -1;
                            Repaint();
                        }
                    }
                    else
                    {
                        //修改数据的key
                        if (GUI.Button(dataRect, "修改key"))
                        {
                            editKeyIndex = index;
                            editKeyText = data.Key;
                            Repaint();
                        }

                        dataRect.x += dataRect.size.x + MStyle.rowSpacing - MStyle.spacing;
                
                        //删除数据
                        if (GUI.Button(dataRect, "删除"))
                        {
                            if (EditorUtility.DisplayDialog("警告", "这是一项危险操作，你确定要删除吗？", "确定", "取消"))
                            {
                                //移除数据
                                config.TextDatas.RemoveAt(index);
                                config.SaveAsset();
                                //重绘GUI
                                Repaint();
                            }
                        }
                    }
                    break;
                case EditorMode.Select:
                    dataRect.width -= MStyle.rowSpacing / 2f;
                    if (GUI.Button(dataRect, "选择"))
                    {
                        selectCallback?.Invoke(data);
                        Close();
                    }
                    break;
            }
            
            EditorGUILayout.EndHorizontal();
        }
        /// <summary>
        /// 绘制一行图片数据GUI
        /// </summary>
        /// <param name="data">要绘制的数据</param>
        /// <param name="index">在列表里面的下标</param>
        private void DrawImageDataGUI(LocalizationImageData data, int index)
        {
            EditorGUILayout.BeginHorizontal();
            
            //获取列间隔
            float rowInterval = MStyle.rowInterval;
            
            Rect dataRect = EditorGUILayout.GetControlRect(GUILayout.Height(MStyle.labelHeight),
                GUILayout.Width(MStyle.rowWidth));
            //如果选择编辑key，那就显示为输入栏
            if (editKeyIndex == index)
            {
                editKeyText = EditorGUI.TextField(dataRect, editKeyText);
            }
            //正常就显示key
            else
            {
                EditorGUI.LabelField(dataRect,data.Key);
            }
            dataRect.x += rowInterval * (MStyle.visibleLanguageCount + 1);
            Rect languageRect = dataRect;
            
            EditorGUI.BeginChangeCheck();
            
            //绘制每个选项
            for (int i = 0; i < data.Datas.Count; i++)
            {
                LocalizationImageDataBase stringData = data.Datas[i];
                //如果不显示这个语言那就跳过
                if(!languageFilter[stringData.Language]) continue;
                languageRect.x = languageGUIPos[stringData.Language];
                switch (editorMode)
                {
                    case EditorMode.Edit:
                        stringData.Sprite = (Sprite)EditorGUI.ObjectField(languageRect, stringData.Sprite, typeof(Sprite), false);
                        break;
                    case EditorMode.Select:
                        EditorGUI.DrawPreviewTexture(languageRect, stringData.Sprite.texture);
                        break;
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(config);
            }

            switch (editorMode)
            {
                case EditorMode.Edit:
                    dataRect.size = new Vector2((dataRect.size.x - MStyle.rowSpacing) / 2f, dataRect.size.y);
                    dataRect.x -= MStyle.spacing;

                    if (editKeyIndex == index)
                    {
                        if (GUI.Button(dataRect, "保存"))
                        {
                            UpdateImageKey(data, editKeyText);
                    
                            editKeyIndex = -1;
                            Repaint();
                        }

                        dataRect.x += dataRect.size.x + MStyle.rowSpacing - MStyle.spacing;
            
                        if (GUI.Button(dataRect, "取消"))
                        {
                            editKeyIndex = -1;
                            Repaint();
                        }
                    }
                    else
                    {
                        //修改数据的key
                        if (GUI.Button(dataRect, "修改key"))
                        {
                            editKeyIndex = index;
                            editKeyText = data.Key;
                            Repaint();
                        }

                        dataRect.x += dataRect.size.x + MStyle.rowSpacing - MStyle.spacing;
                
                        //删除数据
                        if (GUI.Button(dataRect, "删除"))
                        {
                            if (EditorUtility.DisplayDialog("警告", "这是一项危险操作，你确定要删除吗？", "确定", "取消"))
                            {
                                //移除数据
                                config.TextDatas.RemoveAt(index);
                                config.SaveAsset();
                                //重绘GUI
                                Repaint();
                            }
                        }
                    }
                    break;
                case EditorMode.Select:
                    dataRect.width -= MStyle.rowSpacing / 2f;
                    if (GUI.Button(dataRect, "选择"))
                    {
                        selectCallback?.Invoke(data);
                        Close();
                    }
                    break;
            }
            
            
            EditorGUILayout.EndHorizontal();
        }

        #endregion

        #region GUI更新


        /// <summary>
        /// 更新语言的分布
        /// </summary>
        private void UpdateLanguageLayout()
        {
            float rowInterval = MStyle.rowInterval; //列间隔
            int k = 1; //记录当前可见的语言数量
            for (int i = 0; i < languageTypes.Length; i++)
            {
                //如果可见
                if (languageFilter[languageTypes[i]])
                {
                    //那就更新坐标
                    languageGUIPos[languageTypes[i]] = rowInterval * k;
                    k++;//计数+1
                }
            }
        }
        /// <summary>
        /// 更新列排版分布
        /// </summary>
        private void UpdateRowLayout()
        {
            //记录列宽度
            MStyle.prevUpdateGUIWidth = position.width;
            //目前列的数量是可见语言数量+2
            MStyle.rowCount = MStyle.visibleLanguageCount + 2;
            //计算更新每列的宽度
            MStyle.rowWidth = (position.width- MStyle.rowSpacing * (MStyle.rowCount -1)) / MStyle.rowCount;
            //更新每个语言的位置分布
            UpdateLanguageLayout();
            //重绘
            Repaint();
        }

        /// <summary>
        /// 检测是否需要进行列排版分布的更新
        /// </summary>
        private void CheckRowLayoutUpdate()
        {
            //如果还没进行过更新，或者面板UI进行过较大程度的调整那就更新分布
            if (MStyle.prevUpdateGUIWidth < 0f || Mathf.Abs(position.width - MStyle.prevUpdateGUIWidth) > 1f)
            {
                UpdateRowLayout();
            }
        }

        
        #endregion
        
        #region 绘制GUI

        protected override void OnGUI()
        {
            //绘制筛选选项
            DrawFilterOptions();
            //绘制主区域GUI
            DrawMainGUI();
            //绘制底部GUI
            DrawBottomGUI();
            //检测GUI更新
            CheckRowLayoutUpdate();
            
            base.OnGUI();
        }
        /// <summary>
        /// 绘制筛选选项
        /// </summary>
        private void DrawFilterOptions()
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();
            
            EditorGUITool.ShowAClearFoldOut(ref filterFoldout, "语言筛选");
                
            //绘制搜索栏
            Rect searchRect =
                EditorGUILayout.GetControlRect(GUILayout.Height(MStyle.labelHeight), GUILayout.ExpandWidth(true));
            switch (curSelectType)
            {
                case SelectType.StringData:
                    stringDataSearchText = KEditorGUI.SearchTextField(searchRect, stringDataSearchText);
                    break;
                case SelectType.ImageData:
                    imageDataSearchText = KEditorGUI.SearchTextField(searchRect, imageDataSearchText);
                    break;
            }
            
            EditorGUILayout.EndHorizontal();
            
            //语言筛选
            if (filterFoldout)
            {
                EditorGUILayout.BeginHorizontal();

                for (int i = 0; i < languageTypes.Length; i++)
                {
                    DrawTypeFilter(languageTypes[i]);
                }
            
                EditorGUILayout.EndHorizontal();
            }
            
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
                EditorGUILayout.GetControlRect(GUILayout.ExpandWidth(true), GUILayout.Height(MStyle.lineWidth));
            EditorGUI.DrawRect(lineRect, Color.black);
            
            //绘制竖线
            float rowInterval = MStyle.rowInterval;
            Rect rowline = new Rect(MStyle.rowWidth + MStyle.rowSpacing / 2f, lineRect.y + MStyle.lineWidth / 2f,
                MStyle.lineWidth, position.height - lineRect.y - MStyle.labelHeight - MStyle.spacing);
            for (int i = 0; i < MStyle.rowCount - 1; i++)
            {
                EditorGUI.DrawRect(rowline, Color.black);
                rowline.x += rowInterval;
            }
            
            EditorGUILayout.EndVertical();
        }
        /// <summary>
        /// 绘制每列的标题
        /// </summary>
        private void DrawRowTitle()
        {
            EditorGUILayout.BeginHorizontal();
            
            //获取列间隔
            float rowInterval = MStyle.rowInterval;
            
            Rect titleRect = EditorGUILayout.GetControlRect(GUILayout.Height(MStyle.labelHeight),
                GUILayout.Width(MStyle.rowWidth));
            EditorGUI.LabelField(titleRect,"Key", MStyle.RowTitle);
            titleRect.x += rowInterval;
            
            //绘制每个选项
            for (int i = 0; i < languageTypes.Length; i++)
            {
                if (languageFilter[languageTypes[i]])
                {
                    EditorGUI.LabelField(titleRect, EditorGUITool.TryGetEnumLabel(languageTypes[i]), MStyle.RowTitle);
                    titleRect.x += rowInterval;
                }
            }

            EditorGUI.LabelField(titleRect, "编辑操作", MStyle.RowTitle);
            
            EditorGUILayout.EndHorizontal();
        }
        /// <summary>
        /// 绘制文本数据GUI
        /// </summary>
        private void DrawStringDataGUI()
        {
            EditorGUILayout.BeginVertical();

            stringDataListScrollPos = EditorGUILayout.BeginScrollView(stringDataListScrollPos);

            for (int i = 0; i < config.TextDatas.Count; i++)
            {
                //如果搜索栏不为空，并且该项不符合搜索结果就跳过
                if (!string.IsNullOrEmpty(stringDataSearchText) &&
                    !config.TextDatas[i].Key.Contains(stringDataSearchText)) continue;
                
                DrawStringDataGUI(config.TextDatas[i], i);  
            }
            
            EditorGUILayout.EndScrollView();
            
            EditorGUILayout.EndVertical();
        }
        /// <summary>
        /// 绘制图片数据GUI
        /// </summary>
        private void DrawImageDataGUI()
        {
            EditorGUILayout.BeginVertical();

            imageDataListScrollPos = EditorGUILayout.BeginScrollView(imageDataListScrollPos);

            for (int i = 0; i < config.ImageDatas.Count; i++)
            {
                //如果搜索栏不为空，并且该项不符合搜索结果就跳过
                if (!string.IsNullOrEmpty(imageDataSearchText) &&
                    !config.ImageDatas[i].Key.Contains(imageDataSearchText)) continue;
                
                DrawImageDataGUI(config.ImageDatas[i], i);         
            }
            
            EditorGUILayout.EndScrollView();
            
            EditorGUILayout.EndVertical();
        }
        /// <summary>
        /// 绘制主区域GUI
        /// </summary>
        private void DrawMainGUI()
        {
            //绘制每列标题
            DrawRowTitle();
            //绘制分割线
            DrawLines();
            
            //根据当前所选类型进行绘制
            switch (curSelectType)
            {
                case SelectType.StringData:
                    DrawStringDataGUI();
                    break;
                case SelectType.ImageData:
                    DrawImageDataGUI();
                    break;
            }
        }
        /// <summary>
        /// 切换当前绘制的类型
        /// </summary>
        private void SwitchDrawList(SelectType type)
        {
            curSelectType = type;
            editKeyIndex = -1;
            editKeyText = "";
            Repaint();
        }
        /// <summary>
        /// 绘制底部GUI
        /// </summary>
        private void DrawBottomGUI()
        {
            switch (editorMode)
            {
                case EditorMode.Edit:
                    EditorGUILayout.BeginVertical();
            
                    GUILayout.FlexibleSpace();

                    EditorGUILayout.BeginHorizontal();

                    if (GUILayout.Button("文本数据", GUILayout.Width(MStyle.filterTypeWidth)))
                    {
                        SwitchDrawList(SelectType.StringData);
                    }
            
                    if (GUILayout.Button("图片数据", GUILayout.Width(MStyle.filterTypeWidth)))
                    {
                        SwitchDrawList(SelectType.ImageData);
                    }
            
                    GUILayout.FlexibleSpace();
            
                    if (GUILayout.Button("新建", GUILayout.Width(MStyle.filterTypeWidth)))
                    {
                        LocalizationCreateEditorWindow.ShowWindow();
                    }
            
                    EditorGUILayout.EndHorizontal();
            
                    EditorGUILayout.EndVertical();
                    break;
                case EditorMode.Select:
                    break;
            }
        }

        #endregion

        #region 编辑保存数据

        /// <summary>
        /// 更新key
        /// </summary>
        private void UpdateStringKey(LocalizationStringData data, string key)
        {
            //停止文本编辑
            EditorGUITool.EndEditTextField();
            
            if (string.IsNullOrEmpty(key))
            {
                EditorUtility.DisplayDialog("错误", "保存的key不能为空！", "确认");
                return;
            }
            if(data == null || key == data.Key) return;
            
            //更新key
            config.UpdateStringDataKey(data, key);
        }
        /// <summary>
        /// 更新key
        /// </summary>
        private void UpdateImageKey(LocalizationImageData data, string key)
        {
            //停止文本编辑
            EditorGUITool.EndEditTextField();
            
            if (string.IsNullOrEmpty(key))
            {
                EditorUtility.DisplayDialog("错误", "保存的key不能为空！", "确认");
                return;
            }
            if(data == null || key == data.Key) return;
            
            //更新key
            config.UpdateImageDataKey(data, key);
        }
        #endregion
        
    }
}

