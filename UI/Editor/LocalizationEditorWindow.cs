//****************** 代码文件申明 ***********************
//* 文件：LocalizationEditorWindow
//* 作者：wheat
//* 创建时间：2024/09/21 16:04:39 星期六
//* 描述：浏览、编辑本地化配置的编辑器
//*******************************************************

using UnityEngine;
using KFrame;
using KFrame.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using KFrame.Editor;
using UnityEditor;

namespace KFrame.UI.Editor
{
    public class LocalizationEditorWindow : KEditorWindow
    {

        #region 数据引用
        
        /// <summary>
        /// 本地化配置的数据
        /// </summary>
        private static LocalizationConfig Config => LocalizationConfig.Instance;
        /// <summary>
        /// 本地化编辑器的配置数据
        /// </summary>
        private static LocalizationEditorConfig EditorConfig => LocalizationEditorConfig.Instance;

        #endregion
        
        #region GUI绘制相关
        
        /// <summary>
        /// 选择绘制类型
        /// </summary>
        private enum SelectType
        {
            /// <summary>
            /// 文本数据
            /// </summary>
            StringData = 0,
            /// <summary>
            /// 图片数据
            /// </summary>
            ImageData = 1,
            /// <summary>
            /// 语言包
            /// </summary>
            LanguagePackage = 2,
            /// <summary>
            /// 语言包的Key
            /// </summary>
            LanguagePackageKey = 3,
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
            /// 语言包key的列数
            /// </summary>
            internal static int packageKeyRowCount = 3;
            /// <summary>
            /// 语言包key的列宽
            /// </summary>
            internal static float packageKeyRowWidth = -1f;
            /// <summary>
            /// 语言包key的列间隔
            /// </summary>
            internal static float packageKeyRowInterval => rowSpacing + packageKeyRowWidth;
            
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
        /// 选择的回调
        /// </summary>
        private Action<string> selectTextCallback; 
        /// <summary>
        /// 当前语言种类
        /// </summary>
        private int[] languageTypes;
        /// <summary>
        /// 语言筛选
        /// </summary>
        private Dictionary<int, bool> languageFilter;
        /// <summary>
        /// 记录对应语言的GUI位置坐标
        /// </summary>
        private Dictionary<int, float> languageGUIPos;
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
        /// 语言包key搜索
        /// </summary>
        private string packageKeySearchText;
        /// <summary>
        /// 文本数据的列表滚轮位置
        /// </summary>
        private Vector2 stringDataListScrollPos;
        /// <summary>
        /// 图片数据的列表滚轮位置
        /// </summary>
        private Vector2 imageDataListScrollPos;
        /// <summary>
        /// 语言包的列表滚轮位置
        /// </summary>
        private Vector2 packageListScrollPos;
        /// <summary>
        /// 语言包key的列表滚轮位置
        /// </summary>
        private Vector2 packageKeyListScrollPos;
        /// <summary>
        /// 正在修改编辑的GUI的key的下标
        /// </summary>
        private int editKeyIndex = -1;
        /// <summary>
        /// 创建新的语言包
        /// </summary>
        private bool createNewPackage = false;
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
        public static void ShowWindowAsLocalizedTextSelector(Action<string> selectCallback)
        {
            LocalizationEditorWindow window = EditorWindow.GetWindow<LocalizationEditorWindow>();
            window.titleContent = new GUIContent("点击选择一个Data");
            window.editorMode = EditorMode.Select;
            window.curSelectType = SelectType.LanguagePackageKey;
            window.selectTextCallback = selectCallback;
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
            languageTypes = LocalizationDic.GetLanguageIdArray();
            MStyle.visibleLanguageCount = languageTypes.Length;
            languageFilter = new Dictionary<int, bool>();
            languageGUIPos = new Dictionary<int, float>();
            foreach (var languageType in languageTypes)
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
        private void DrawTypeFilter(int id)
        {
            float toggleWidth = Mathf.Min(MStyle.filterTypeWidth / 3f, 20f);
            float labelWidth = MStyle.filterTypeWidth - toggleWidth - 5f;
            
            EditorGUILayout.LabelField(LocalizationDic.GetLanguageName(id),
                GUILayout.Width(labelWidth), GUILayout.Height(MStyle.labelHeight));
            
            GUILayout.Space(5f);
            
            EditorGUI.BeginChangeCheck();
            
            languageFilter[id] = EditorGUILayout.Toggle(languageFilter[id],
                GUILayout.Width(toggleWidth), GUILayout.Height(MStyle.labelHeight));

            //如果筛选条件变了，列的排布需要更新
            if (EditorGUI.EndChangeCheck())
            {
                //更新可见语言数量
                if (languageFilter[id])
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
                if(!languageFilter[stringData.LanguageId]) continue;
                languageRect.x = languageGUIPos[stringData.LanguageId];
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
                EditorUtility.SetDirty(Config);
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
                                Config.TextDatas.RemoveAt(index);
                                Config.SaveAsset();
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
                if(!languageFilter[stringData.LanguageId]) continue;
                languageRect.x = languageGUIPos[stringData.LanguageId];
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
                EditorUtility.SetDirty(Config);
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
                                Config.TextDatas.RemoveAt(index);
                                Config.SaveAsset();
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
        /// 绘制一行语言包key数据GUI
        /// </summary>
        /// <param name="data">要绘制的数据</param>
        /// <param name="index">在列表里面的下标</param>
        private void DrawPackageGUI(LanguagePackageReference data, int index)
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(data.LanguageKey, GUILayout.Height(MStyle.labelHeight)))
            {
                LanguagePackageEditorWindow.ShowWindow(EditorConfig.packages[index]);
            }
            
            EditorGUILayout.EndHorizontal();
        }
        /// <summary>
        /// 绘制一行语言包key数据GUI
        /// </summary>
        /// <param name="data">要绘制的数据</param>
        /// <param name="index">在列表里面的下标</param>
        private void DrawPackageKeyGUI(LanguagePackageKeyData data, int index)
        {
            EditorGUILayout.BeginHorizontal();
            
            //获取列间隔
            float rowInterval = MStyle.packageKeyRowInterval;
            
            Rect dataRect = EditorGUILayout.GetControlRect(GUILayout.Height(MStyle.labelHeight),
                GUILayout.Width(MStyle.packageKeyRowWidth));
            //如果选择编辑key，那就显示为输入栏
            if (editKeyIndex == index)
            {
                editKeyText = EditorGUI.TextField(dataRect, editKeyText);
            }
            //正常就显示key
            else
            {
                EditorGUI.LabelField(dataRect,data.key);
            }
            dataRect.x += rowInterval;
            
            EditorGUI.BeginChangeCheck();
            //备注
            data.notes = EditorGUI.TextField(dataRect, data.notes);

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(EditorConfig);
            }
            dataRect.x += rowInterval;
            
            //操作选项
            switch (editorMode)
            {
                case EditorMode.Edit:
                    dataRect.size = new Vector2((dataRect.size.x - MStyle.rowSpacing) / 2f, dataRect.size.y);
                    dataRect.x -= MStyle.spacing;
                    if (editKeyIndex == index)
                    {
                        if (GUI.Button(dataRect, "保存"))
                        {
                            EditorConfig.UpdateKey(data, editKeyText);
                    
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
                            editKeyText = data.key;
                            Repaint();
                        }

                        dataRect.x += dataRect.size.x + MStyle.rowSpacing - MStyle.spacing;
                
                        //删除数据
                        if (GUI.Button(dataRect, "删除"))
                        {
                            if (EditorUtility.DisplayDialog("警告", "这是一项危险操作，你确定要删除吗？", "确定", "取消"))
                            {
                                //移除数据
                                var keyData = EditorConfig.localizationKeys[index];
                                EditorConfig.RemoveKey(keyData);
                                EditorConfig.SaveAsset();
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
                        selectTextCallback?.Invoke(data.key);
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
            MStyle.packageKeyRowWidth = (position.width- MStyle.rowSpacing * (MStyle.packageKeyRowCount -1)) / MStyle.packageKeyRowCount;
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

            if (curSelectType != SelectType.LanguagePackage && curSelectType != SelectType.LanguagePackageKey)
            {
                EditorGUITool.ShowAClearFoldOut(ref filterFoldout, "语言筛选");
            }
                
            //绘制搜索栏
            if (curSelectType != SelectType.LanguagePackage)
            {
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
                    case SelectType.LanguagePackageKey:
                        packageKeySearchText = KEditorGUI.SearchTextField(searchRect, packageKeySearchText);
                        break;
                }
            }

            
            EditorGUILayout.EndHorizontal();
            
            //语言筛选
            if (filterFoldout && curSelectType != SelectType.LanguagePackage && curSelectType != SelectType.LanguagePackageKey)
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
                    EditorGUI.LabelField(titleRect, LocalizationDic.GetLanguageName(languageTypes[i]), MStyle.RowTitle);
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
            //绘制每列标题
            DrawRowTitle();
            //绘制分割线
            DrawLines();
            
            EditorGUILayout.BeginVertical();

            stringDataListScrollPos = EditorGUILayout.BeginScrollView(stringDataListScrollPos);

            for (int i = 0; i < Config.TextDatas.Count; i++)
            {
                //如果搜索栏不为空，并且该项不符合搜索结果就跳过
                if (!string.IsNullOrEmpty(stringDataSearchText) &&
                    !Config.TextDatas[i].Key.Contains(stringDataSearchText)) continue;
                
                DrawStringDataGUI(Config.TextDatas[i], i);  
            }
            
            EditorGUILayout.EndScrollView();
            
            EditorGUILayout.EndVertical();
        }
        /// <summary>
        /// 绘制图片数据GUI
        /// </summary>
        private void DrawImageDataGUI()
        {
            //绘制每列标题
            DrawRowTitle();
            //绘制分割线
            DrawLines();
            
            EditorGUILayout.BeginVertical();

            imageDataListScrollPos = EditorGUILayout.BeginScrollView(imageDataListScrollPos);

            for (int i = 0; i < Config.ImageDatas.Count; i++)
            {
                //如果搜索栏不为空，并且该项不符合搜索结果就跳过
                if (!string.IsNullOrEmpty(imageDataSearchText) &&
                    !Config.ImageDatas[i].Key.Contains(imageDataSearchText)) continue;
                
                DrawImageDataGUI(Config.ImageDatas[i], i);         
            }
            
            EditorGUILayout.EndScrollView();
            
            EditorGUILayout.EndVertical();
        }
        /// <summary>
        /// 绘制语言包编辑GUI
        /// </summary>
        private void DrawPackageGUI()
        {
            EditorGUILayout.BeginVertical();

            if (createNewPackage)
            {
                EditorGUILayout.BeginHorizontal();
                
                //新建语言名称
                editKeyText = EditorGUILayout.TextField("新建语言名称：", editKeyText);
                
                //创建按钮
                if (GUILayout.Button("创建", GUILayout.Width(40f)))
                {
                    if (string.IsNullOrEmpty(editKeyText))
                    {
                        EditorUtility.DisplayDialog("错误", "语言名称不能为空", "确定");
                    }
                    else
                    {
                        LocalizationEditorConfig.Instance.AddLanguagePackage(editKeyText, editKeyText);
                        editKeyText = "";
                        createNewPackage = false;
                    }
                }
                
                //取消按钮
                if (GUILayout.Button("取消", GUILayout.Width(40f)))
                {
                    editKeyText = "";
                    createNewPackage = false;
                }
                
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                if (GUILayout.Button("新建", GUILayout.Width(40f)))
                {
                    editKeyText = "";
                    createNewPackage = true;
                }
            }
            if (Config.packageReferences.Count == 0)
            {
                EditorGUILayout.LabelField("目前还没有创建语言包。", EditorStyles.boldLabel);
            }
            else
            {
                
                packageListScrollPos = EditorGUILayout.BeginScrollView(packageListScrollPos);
            
                //遍历绘制
                for (int i = 0; i < Config.packageReferences.Count; i++)
                {
                    DrawPackageGUI(Config.packageReferences[i], i);
                }
                
                EditorGUILayout.EndScrollView();

            }
            
            EditorGUILayout.EndVertical();
        }
        /// <summary>
        /// 绘制语言包key编辑GUI
        /// </summary>
        private void DrawPackageKeyGUI()
        {
            //绘制标题
            EditorGUILayout.BeginHorizontal();
            
            //获取列间隔
            float rowInterval = MStyle.packageKeyRowInterval;
            
            //Key
            Rect titleRect = EditorGUILayout.GetControlRect(GUILayout.Height(MStyle.labelHeight),
                GUILayout.Width(MStyle.packageKeyRowWidth));
            EditorGUI.LabelField(titleRect,"Key", MStyle.RowTitle);
            titleRect.x += rowInterval;
            
            //备注
            EditorGUI.LabelField(titleRect, "备注", MStyle.RowTitle);
            titleRect.x += rowInterval;

            //编辑操作
            EditorGUI.LabelField(titleRect, "编辑操作", MStyle.RowTitle);
            
            EditorGUILayout.EndHorizontal();
            
            //绘制分割线
            EditorGUILayout.BeginVertical();

            //绘制横线
            Rect lineRect =
                EditorGUILayout.GetControlRect(GUILayout.ExpandWidth(true), GUILayout.Height(MStyle.lineWidth));
            EditorGUI.DrawRect(lineRect, Color.black);
            
            //绘制竖线
            Rect rowLine = new Rect(MStyle.packageKeyRowWidth + MStyle.rowSpacing / 2f, lineRect.y + MStyle.lineWidth / 2f,
                MStyle.lineWidth, position.height - lineRect.y - MStyle.labelHeight - MStyle.spacing);
            for (int i = 0; i < MStyle.packageKeyRowCount - 1; i++)
            {
                EditorGUI.DrawRect(rowLine, Color.black);
                rowLine.x += rowInterval;
            }
            
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();

            packageKeyListScrollPos = EditorGUILayout.BeginScrollView(packageKeyListScrollPos);
            
            //遍历绘制
            for (int i = 0; i < EditorConfig.localizationKeys.Count; i++)
            {
                DrawPackageKeyGUI(EditorConfig.localizationKeys[i], i);
            }
            
            EditorGUILayout.EndScrollView();
            
            EditorGUILayout.EndVertical();
        }
        /// <summary>
        /// 绘制主区域GUI
        /// </summary>
        private void DrawMainGUI()
        {
            //根据当前所选类型进行绘制
            switch (curSelectType)
            {
                case SelectType.StringData:
                    DrawStringDataGUI();
                    break;
                case SelectType.ImageData:
                    DrawImageDataGUI();
                    break;
                case SelectType.LanguagePackage:
                    DrawPackageGUI();
                    break;
                case SelectType.LanguagePackageKey:
                    DrawPackageKeyGUI();
                    break;
            }
        }
        /// <summary>
        /// 切换当前绘制的类型
        /// </summary>
        private void SwitchDrawList(SelectType type)
        {
            curSelectType = type;
            createNewPackage = false;
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
                    if (GUILayout.Button("语言包", GUILayout.Width(MStyle.filterTypeWidth)))
                    {
                        SwitchDrawList(SelectType.LanguagePackage);
                    }
                    if (GUILayout.Button("语言包Key", GUILayout.Width(MStyle.filterTypeWidth)))
                    {
                        SwitchDrawList(SelectType.LanguagePackageKey);
                    }
            
                    GUILayout.FlexibleSpace();

                    if (curSelectType != SelectType.LanguagePackage)
                    {
                        if (GUILayout.Button("新建", GUILayout.Width(MStyle.filterTypeWidth)))
                        {
                            switch (curSelectType)
                            {
                                case SelectType.LanguagePackageKey:
                                    LanguageKeyCreateWindow.ShowWindow();
                                    break;
                                default:
                                    LocalizationCreateEditorWindow.ShowWindow();
                                    break;
                            }
                        }
                    }
            
                    EditorGUILayout.EndHorizontal();
            
                    EditorGUILayout.EndVertical();
                    break;
                case EditorMode.Select:
                    EditorGUILayout.BeginVertical();
            
                    GUILayout.FlexibleSpace();

                    EditorGUILayout.BeginHorizontal();

                    GUILayout.FlexibleSpace();

                    if (curSelectType != SelectType.LanguagePackage)
                    {
                        if (GUILayout.Button("新建", GUILayout.Width(MStyle.filterTypeWidth)))
                        {
                            switch (curSelectType)
                            {
                                case SelectType.LanguagePackageKey:
                                    LanguageKeyCreateWindow.ShowWindow();
                                    break;
                                default:
                                    LocalizationCreateEditorWindow.ShowWindow();
                                    break;
                            }
                        }
                    }
            
                    EditorGUILayout.EndHorizontal();
            
                    EditorGUILayout.EndVertical();
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
            LocalizationDic.UpdateUITextDataKey(data, key);
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
            LocalizationDic.UpdateUIImageDataKey(data, key);
        }
        #endregion
        
    }
}

