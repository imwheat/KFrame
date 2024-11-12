//****************** 代码文件申明 ***********************
//* 文件：LocalizedTextKeyDrawer
//* 作者：wheat
//* 创建时间：2024/10/19 10:43:28 星期六
//* 描述：本地化文本key的Attribute的Drawer
//*******************************************************

using KFrame.Editor;
using UnityEditor;
using UnityEngine;

namespace KFrame.UI.Editor
{
    [CustomPropertyDrawer(typeof(LocalizedTextKeyAttribute))]
    public class LocalizedTextKeyDrawer : PropertyDrawer
    {
        #region 编辑参数

        /// <summary>
        /// 当前所选的语言包
        /// </summary>
        private static LanguagePackage package;
        /// <summary>
        /// 原文GUIStyle
        /// </summary>
        private static GUIStyle originalTextStyle;
        /// <summary>
        /// 原文GUIStyle
        /// </summary>
        private static GUIStyle OriginalTextStyle
        {
            get
            {
                if (originalTextStyle == null)
                {
                    originalTextStyle = new GUIStyle(EditorStyles.label)
                    {
                        alignment = TextAnchor.UpperLeft,
                    };
                }

                return originalTextStyle;
            }
        }
        /// <summary>
        /// 语言切换选项
        /// </summary>
        private static string[] languageSelections;
        /// <summary>
        /// Label的高度
        /// </summary>
        private static readonly float LabelHeight = 20f;
        /// <summary>
        /// 滚动视窗高度
        /// </summary>
        private static readonly float ScrollHeight = 100f;
        /// <summary>
        /// ScrollPos1
        /// </summary>
        private Vector2 _scrollPos1;
        /// <summary>
        /// ScrollPos2
        /// </summary>
        private Vector2 _scrollPos2;
        /// <summary>
        /// 语言原文
        /// </summary>
        private string originalText;
        /// <summary>
        /// 当前语言的id
        /// </summary>
        private int languageId;
        /// <summary>
        /// 当前所选语言
        /// </summary>
        private int languageSelectId;
        /// <summary>
        /// 正在编辑的数据
        /// </summary>
        private LanguagePackageTextData editData;
        /// <summary>
        /// 正在编辑的key
        /// </summary>
        private string editKey;
        
        #endregion


        #region 编辑操作

        /// <summary>
        /// 切换当前选择的语言
        /// </summary>
        /// <param name="id"></param>
        /// <param name="key"></param>
        private void UpdateEditData(int id, string key)
        {
            //获取设置语言包引用
            var packageRef = LocalizationConfig.Instance.packageReferences[id];
            //设置参数
            editKey = key;
            languageSelectId = id;
            languageId = packageRef.LanguageId;
            //加载资源
            package = AssetDatabase.LoadAssetAtPath<LanguagePackage>(packageRef.packagePath);
            //更新编辑资源
            editData = package.datas.Find(x => x.key == key);
            _scrollPos1 = Vector2.zero;
            _scrollPos2 = Vector2.zero;
            
            //如果当前正在编辑的就是原文，那就不显示翻译原文
            if (languageId == LocalizationConfig.EditorDefaultLanguageId)
            {
                originalText = "";
            }
            //如果选择了其他语言，会在旁边显示原文
            else
            {
                //找到原文的语言包，获取原文
                var originalPackage = AssetDatabase.LoadAssetAtPath<LanguagePackage>(LocalizationDic.GetPackageReference(LocalizationConfig.EditorDefaultLanguageId).packagePath);
                originalText = originalPackage.datas.Find(x => x.key == key).text;
            }
        }
        /// <summary>
        /// 更新字典数据
        /// </summary>
        private void UpdateDicData(string text)
        {
            //如果不相等就不要更新
            if(languageId != LocalizationDic.LanguageId) return;
            
            //更新字典
            LocalizationDic.UpdateLocalizedTextDic(editKey, text);
        }
        #endregion


        #region GUI绘制
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = base.GetPropertyHeight(property, label) * 2f;

            //如果有编辑数据的话，那就增加视窗的高度
            if (editData != null)
            {
                height += LabelHeight + KGUIStyle.spacing * 2f;
                height += ScrollHeight;
            }
            
            return height + 10f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //初始化语言选项
            if (languageSelections == null)
            {
                var packages = LocalizationConfig.Instance.packageReferences;
                languageSelections = new string[packages.Count];
                for (int i = 0; i < packages.Count; i++)
                {
                    languageSelections[i] = packages[i].LanguageName;
                }
                
            }
            
            //检测更新编辑数据
            if (editKey != property.stringValue)
            {
                UpdateEditData(languageId, property.stringValue);
            }
            
            EditorGUI.BeginProperty(position, label, property);
            
            if (property.propertyType == SerializedPropertyType.String)
            {
                //显示key
                Rect labelRect = new Rect(position.position, new Vector2(position.size.x / 4 *3f, LabelHeight));
                EditorGUI.LabelField(labelRect,label.text + ": " + property.stringValue);
                
                //选择key按钮
                Rect btnRect = labelRect;
                btnRect.x += labelRect.width;
                btnRect.width = position.width / 4f;
                if (GUI.Button(btnRect, "选择"))
                {
                    LocalizationEditorWindow.ShowWindowAsLocalizedTextSelector((key) =>
                    {
                        property.stringValue = key;
                        property.serializedObject.ApplyModifiedProperties();
                    });
                }
                
                //切换语言
                Rect popupRect = labelRect;
                popupRect.y += LabelHeight;
                popupRect.width = position.width;
                int id = EditorGUI.Popup(popupRect, "当前语言",languageSelectId, languageSelections);
                if (id != languageSelectId)
                {
                    UpdateEditData(id, property.stringValue);
                }
                
                //编辑文本
                if (editData != null)
                {
                    //计算HeaderLabel的Rect
                    Rect labelHeaderRect = popupRect;
                    labelHeaderRect.y += KGUIStyle.spacing + LabelHeight;
                    
                    //绘制ScrollView
                    Rect scrollViewRect = labelHeaderRect;
                    scrollViewRect.y += LabelHeight + KGUIStyle.spacing;
                    scrollViewRect.height = ScrollHeight;
                    
                    //如果正在编辑原文
                    if(string.IsNullOrEmpty(originalText))
                    {
                        //显示Header
                        GUI.Label(labelHeaderRect, "原文", EditorStyles.boldLabel);
                        
                        //使用TextArea的默认样式
                        //计算文本所需的高度
                        Rect textRect = scrollViewRect;
                        textRect.height = Mathf.Max(GUI.skin.textArea.CalcHeight(new GUIContent(editData.text), textRect.width),ScrollHeight);
                        Rect contentRect = textRect;
                        contentRect.width += 20f;
                    
                        //绘制ScrollView
                        _scrollPos1 = GUI.BeginScrollView(scrollViewRect, _scrollPos1, contentRect);
                    
                        //记录Undo
                        Undo.RecordObject(package, "LanguagePackage (Edit Text)");
                        
                        EditorGUI.BeginChangeCheck();
                        
                        editData.text = EditorGUI.TextArea(textRect, editData.text);
                        
                        //保存
                        if (EditorGUI.EndChangeCheck())
                        {
                            UpdateDicData(editData.text);
                            EditorUtility.SetDirty(package);
                        }
                    
                        GUI.EndScrollView();
                    }
                    //翻译其他语言
                    else
                    {
                        //显示Header
                        labelHeaderRect.width /= 2f;
                        GUI.Label(labelHeaderRect, "原文", EditorStyles.boldLabel);
                        labelHeaderRect.x += labelHeaderRect.width;
                        GUI.Label(labelHeaderRect, "译文", EditorStyles.boldLabel);
                        
                        //分割成2个ScrollView，一个显示原文，一个显示当前编辑的文本
                        scrollViewRect.width /= 2f;
                        Rect scrollViewRect2 = scrollViewRect;
                        scrollViewRect2.x += scrollViewRect.width;
                        
                        //使用TextArea的默认样式
                        //计算文本所需的高度
                        Rect textRect = scrollViewRect;
                        textRect.height = Mathf.Max(GUI.skin.textArea.CalcHeight(new GUIContent(originalText), textRect.width), ScrollHeight);
                        Rect textRect2 = textRect;
                        textRect2.x += textRect.width;
                        textRect2.height = Mathf.Max(GUI.skin.textArea.CalcHeight(new GUIContent(editData.text), textRect2.width), ScrollHeight);
                        Rect contentRect = textRect;
                        Rect contentRect2 = textRect2;
                        contentRect.width += 20f;
                        contentRect2.width += 20f;
                    
                        //绘制ScrollView
                        //显示原文
                        _scrollPos1 = GUI.BeginScrollView(scrollViewRect, _scrollPos1, contentRect);

                        EditorGUI.LabelField(textRect, originalText, OriginalTextStyle);
                    
                        GUI.EndScrollView();
                        
                        //显示编辑文本
                        _scrollPos2 = GUI.BeginScrollView(scrollViewRect2, _scrollPos2, contentRect2);
                        
                        //记录Undo
                        Undo.RecordObject(package, "LanguagePackage (Edit Text)");
                        
                        EditorGUI.BeginChangeCheck();
                        
                        editData.text = EditorGUI.TextArea(textRect2, editData.text);
                        
                        //保存
                        if (EditorGUI.EndChangeCheck())
                        {
                            UpdateDicData(editData.text);
                            EditorUtility.SetDirty(package);
                        }
                    
                        GUI.EndScrollView();
                    }
                    

                }
            }
            else
            {
                EditorGUI.LabelField(position,"错误改Attribute需要加载string上。");
            }
            
            EditorGUI.EndProperty();
        }

        #endregion

    }
}

