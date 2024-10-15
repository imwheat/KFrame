//****************** 代码文件申明 ************************
//* 文件：ScriptsTemplateEditorWindow                      
//* 作者：wheat
//* 创建时间：2023年04月26日 星期五 09:47
//* 描述：用于自己创建自定义的脚本模板
//*****************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using KFrame.Editor.Tools;
using KFrame.Utilities;
using UnityEditor;
using UnityEngine;

namespace KFrame.Editor.ScriptTemplates
{
    public class ScriptsTemplateEditorWindow : KEditorWindow
    {
        #region 字段

        /// <summary>
        /// 编辑模式
        /// 0.添加模版
        /// 1.编辑模式
        /// 2.创建模版
        /// </summary>
        private int editorMode;
        private Vector2 scrollPosition;
        private Vector2 scrollPosition2;
        private string searchFilter;
        private TextAsset addTextAsset;
        private string templateName = "";
        private string template = "";
        private string groupName = "";
        private string groupName2 = "";

        private const string ScriptTop = "//****************** 代码文件声明 ***********************\n" +
                                         "//* 文件：#SCRIPTNAME#\n" +
                                         "//* 作者：#AUTHORNAME#\n" +
                                         "//* 创建时间：#CREATETIME#\n" +
                                         "//* 描述：\n" +
                                         "//*******************************************************\n";
        private string scriptsTop;
        /// <summary>
        /// 上次刷新的时候模版数量
        /// </summary>
        private int prevRefreshTemplateCount;
        /// <summary>
        /// 将CS文件转为模版的时候是否忽略方法参数
        /// </summary>
        private bool ignoreCsElement;
        /// <summary>
        /// 添加的脚本名称
        /// </summary>
        private string addCsTemplateName;
        /// <summary>
        /// 模版的GUI选项
        /// </summary>
        private List<GUIContent> templateGUI;
        /// <summary>
        /// 模版文件
        /// </summary>
        private List<TextAsset> templates;


        #endregion

        #region 脚本模版创建GUI相关
            
        /// <summary>
        /// 模版文件类型
        /// </summary>
        private enum TemplateAssetType
        {
            /// <summary>
            /// 空
            /// </summary>
            Null = 0,
            /// <summary>
            /// 文本文件
            /// </summary>
            Text = 1,
            /// <summary>
            /// 脚本文件
            /// </summary>
            ScriptFile = 2,
        }
        /// <summary>
        /// 模版文件类型
        /// </summary>
        private TemplateAssetType templateAssetType;
        /// <summary>
        /// 预览脚本的ScrollPosition
        /// </summary>
        private Vector2 previewCodeScrollPosition;
        /// <summary>
        /// 预览脚本的Style
        /// </summary>
        private GUIStyle previewCodeStyle;
        /// <summary>
        /// 预览脚本的Style
        /// </summary>
        private GUIStyle PreviewCodeStyle
        {
            get
            {
                if (previewCodeStyle == null)
                {
                    previewCodeStyle = new GUIStyle("ControlLabel")
                    {
                        fontSize = 12,
                        alignment = TextAnchor.UpperLeft,
                    };
                }

                return previewCodeStyle;
            }
        }
        /// <summary>
        /// 预览脚本的高度
        /// </summary>
        private float previewCodeHeight;
        /// <summary>
        /// 预览脚本
        /// </summary>
        private string previewCode;

        #endregion

        #region 生命周期
        public static void ShowWindow()
        {
            ScriptsTemplateEditorWindow window = GetWindow<ScriptsTemplateEditorWindow>();
            window.titleContent = new GUIContent("脚本模版编辑器");

        }
        private void OnEnable()
        {
            template = "";
            templateName = "";
            searchFilter = string.Empty;
            scriptsTop = ScriptTop;
            ignoreCsElement = true;
        }

        protected override void OnGUI()
        {
            //绘制顶部GUI
            DrawTopGUI();
            //根据模式绘制不同GUI
            switch (editorMode)
            {
                //添加已有模版
                case 0:
                    DrawAddExistTemplateGUI();
                    break;
                //编辑模式
                case 1:
                    DrawEditTemplateGUI();
                    break;
                //创建模版
                case 2:
                    DrawCreateTemplateGUI();
                    break;
            }

            base.OnGUI();
        }

        #endregion

        #region GUI绘制

        /// <summary>
        /// 绘制顶部GUI
        /// </summary>
        private void DrawTopGUI()
        {
            EditorGUILayout.BeginVertical();

            GUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("添加模版"))
            {
                editorMode = 0;
            }
            if (GUILayout.Button("编辑模版"))
            {
                editorMode = 1;
            }
            if (GUILayout.Button("创建模版"))
            {
                editorMode = 2;
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5);

            EditorGUILayout.EndVertical();
        }
        /// <summary>
        /// 绘制创建脚本的GUI
        /// </summary>
        private void DrawCreateTemplateGUI()
        {
            EditorGUILayout.BeginVertical();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            //模板名称
            GUILayout.Label("脚本模板名称:");
            templateName = EditorGUILayout.TextArea(templateName, GUILayout.Height(30));
            GUILayout.Space(5);

            //分类名称
            GUILayout.Label("脚本分类名称:(可选)");
            groupName = EditorGUILayout.TextArea(groupName, GUILayout.Height(30));
            GUILayout.Space(5);

            //脚本备注
            GUILayout.Label("脚本声明：");
            if (GUILayout.Button("复制声明"))
            {
                GUIUtility.systemCopyBuffer = scriptsTop;
            }
            scriptsTop = GUILayout.TextArea(scriptsTop, GUILayout.Height(100));


            GUILayout.Space(5);

            //模板内容
            GUILayout.Label("脚本模板内容\n(建议在Text内写好直接复制进来):");
            if (GUILayout.Button("粘贴剪切板内容"))
            {
                template = GUIUtility.systemCopyBuffer;
                //点击粘贴的时候要结束编辑TextField
                EditorGUIUtility.editingTextField = false;
            }
            else
            {
                template = EditorGUILayout.TextArea(template, GUILayout.Height(200));
            }

            EditorGUILayout.EndScrollView();

            GUILayout.FlexibleSpace();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("创建模版"))
            {
                //如果没有顶部声明的话，自动加一下
                if (string.IsNullOrEmpty(template) == false)
                {
                    if (template[0] != '/')
                    {
                        template = scriptsTop + '\n' + template;
                    }
                }

                UpdateScriptTemplate(templateName, template, groupName);
            }
            if (GUILayout.Button("打开脚本模板所在文件夹"))
            {
                FileExtensions.OpenPathFolder(ScriptTemplateConfig.FrameScriptTemplatesPath);
            }


            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }
        /// <summary>
        /// 更新预览脚本代码
        /// </summary>
        private void UpdatePreviewCode()
        {
            previewCodeScrollPosition = Vector2.zero;
            
            if (addTextAsset == null)
            {
                previewCode = "";
                templateAssetType = TemplateAssetType.Null;
            }
            else if (addTextAsset.GetAssetExtension() == ".cs")
            {
                previewCode = ConvertCsToTemplate(addTextAsset.text, ignoreCsElement);
                templateAssetType = TemplateAssetType.ScriptFile;
            }
            else
            {
                previewCode = addTextAsset.text;
                templateAssetType = TemplateAssetType.Text;
            }

            previewCodeHeight = previewCode.Count(c => c == '\n') * 20f;
        }
        /// <summary>
        /// 添加已有模版的GUI
        /// </summary>
        private void DrawAddExistTemplateGUI()
        {
            EditorGUILayout.BeginVertical();
            
            EditorGUI.BeginChangeCheck();
            
            //要添加的模版
            addTextAsset = EditorGUILayout.ObjectField("要添加的模版:",addTextAsset, typeof(TextAsset), false) as TextAsset;

            //是cs文件的话，那就显示是否启用忽略CS参数和方法选项
            if (templateAssetType == TemplateAssetType.ScriptFile)
            {
                GUILayout.Space(5);

                ignoreCsElement = EditorGUILayout.Toggle("忽略所有参数和方法:", ignoreCsElement);

                GUILayout.Space(5);

            }
            
            //检测刷新预览代码
            if (EditorGUI.EndChangeCheck())
            {
                UpdatePreviewCode();
            }            
            
            //模版名称
            GUILayout.Label("脚本模版名称:");
            addCsTemplateName = EditorGUILayout.TextField(addCsTemplateName, GUILayout.Height(30f));
            //分类名称
            GUILayout.Label("脚本分类名称:(可选)");
            groupName2 = EditorGUILayout.TextArea(groupName2, GUILayout.Height(30));

            //脚本预览
            if (GUILayout.Button("刷新预览脚本"))
            {
                UpdatePreviewCode();
            }

            if (previewCodeHeight > 0f)
            {
                EditorGUILayout.LabelField("预览代码", EditorStyles.boldLabel);
                
                previewCodeScrollPosition = EditorGUILayout.BeginScrollView(previewCodeScrollPosition);

                EditorGUILayout.LabelField(previewCode, PreviewCodeStyle,GUILayout.Height(previewCodeHeight));
            
                EditorGUILayout.EndScrollView();
            }
            
            //创建添加
            GUILayout.Space(5);
            if (GUILayout.Button("创建添加"))
            {
                //结束文本编辑
                EditorGUITool.EndEditTextField();
                if (string.IsNullOrEmpty(addCsTemplateName))
                {
                    EditorUtility.DisplayDialog("错误", "脚本模版的名称不能为空", "确认");
                }
                else
                {
                    UpdateScriptTemplate(addTextAsset, addCsTemplateName, groupName2, ignoreCsElement);
                }
            }
            
            EditorGUILayout.EndVertical();
        }
        /// <summary>
        /// 绘制移除脚本的GUI
        /// </summary>
        private void DrawEditTemplateGUI()
        {
            EditorGUILayout.BeginVertical();


            //如果GUI为空那就创建
            if(templateGUI == null || (templateGUI.Count != templates.Count && templates.Count != prevRefreshTemplateCount))
            {
                InitTemplateGUI();
            }

            if(templateGUI.Count == 0)
            {
                GUILayout.Label("目前还没有模版");
            }
            else
            {
                //搜索栏
                EditorGUILayout.LabelField("搜索栏:");
                searchFilter = EditorGUILayout.TextField(searchFilter);

                scrollPosition2 = EditorGUILayout.BeginScrollView(scrollPosition2);

                float btnSize = Mathf.Min(position.width / 4f, 40f);
                //绘制模版选项
                for (int i = 0; i < templateGUI.Count; i++)
                {
                    //如果不符合搜索结果就跳过
                    if (string.IsNullOrEmpty(searchFilter) == false && templateGUI[i].text.Contains(searchFilter) == false)
                    {
                        continue;
                    }

                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField(templateGUI[i]);

                    if(GUILayout.Button("打开", GUILayout.Width(btnSize)))
                    {
                        OpenTemplate(templates[i]);
                    }
                    if (GUILayout.Button("删除", GUILayout.Width(btnSize)))
                    {
                        DeleteTemplate(i);
                    }

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndScrollView();

            }

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndVertical();
        }
        /// <summary>
        /// 初始化GUI
        /// </summary>
        private void InitTemplateGUI()
        {
            templateGUI = new List<GUIContent>();
            templates = ScriptTemplateConfig.Templates;

            //移除空的
            for (int i = templates.Count - 1; i >= 0; i--)
            {
                if (templates[i] == null)
                {
                    templates.RemoveAt(i);
                }
            }

            //创建GUI
            foreach (var t in templates)
            {
                templateGUI.Add(new GUIContent(t.name));
            }

            prevRefreshTemplateCount = templates.Count;
        }
        /// <summary>
        /// 打开指定的template
        /// </summary>
        /// <param name="templateAsset">要打开的模版</param>
        private void OpenTemplate(TextAsset templateAsset)
        {
            AssetDatabase.OpenAsset(templateAsset);
        }
        #endregion

        #region 模版的创建



        /// <summary>
        /// 更新脚本模版
        /// </summary>
        /// <param name="templateName">模版名称</param>
        /// <param name="codeText">模版内容</param>
        /// <param name="groupName">分类</param>
        private static void UpdateScriptTemplate(string templateName, string codeText, string groupName = "")
        {
            //末尾空白给去掉
            templateName = templateName.TrimEnd();
            groupName = groupName.TrimEnd();
            //检测一下要生成的模版是否合理，如果不合理的话就终止生成模版
            if (CheckTemplateIsValid(templateName, codeText, groupName) == false)
            {
                return;
            }
            //模版文件名称
            string assetName = templateName + groupName;
            
            //尝试从字典里面获取现存的模版，如果没有那就新建
            if (!ScriptTemplateConfig.TemplateDic.TryGetValue(assetName +".cs", out var templateAsset))
            {
                //获取模版路径
                string templatePath = ScriptTemplateConfig.FrameScriptTemplatesPath;
                //如果有分组
                if (string.IsNullOrEmpty(groupName) == false)
                {
                    templatePath += groupName + "/";
                }
                
                //创建文件夹如果不存在
                FileExtensions.CreateDirectoryIfNotExist(templatePath.ConvertAssetPathToSystemPath());
                
                //创建MenuItem
                CreateScriptTemplateMenuItem(templateName, groupName);
                
                //获取Asset路径
                string assetPath = templatePath + assetName + ".cs.txt";
                //创建Asset
                File.WriteAllText(assetPath.ConvertAssetPathToSystemPath(), codeText);
                AssetDatabase.Refresh();
                templateAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);

                //添加进模版库
                ScriptTemplateConfig.AddTemplate(templateAsset);
                
                //编辑器窗口重绘
                ScriptsTemplateEditorWindow[] windows = Resources.FindObjectsOfTypeAll<ScriptsTemplateEditorWindow>();
                foreach (var window in windows)
                {
                    window.InitTemplateGUI();
                }
                
                Debug.Log($"路径：{templateAsset.GetAssetPath()}\n脚本创建成功。");
            }
            else
            {
                //如果已经有了那就更新代码
                File.WriteAllText(templateAsset.GetFullPath(), codeText);
                Debug.Log($"路径：{templateAsset.GetAssetPath()}\n脚本更新成功。");
            }
            
        }
        /// <summary>
        /// 通过文本文件更新模版
        /// </summary>
        /// <param name="groupName">分组名称</param>
        /// <param name="ignoreElements">将CS文件转为模版的时候是否忽略方法参数</param>
        /// <param name="textAsset">模版文件</param>
        /// <param name="assetName">保存模版名称</param>
        private static void UpdateScriptTemplate(TextAsset textAsset, string assetName,string groupName, bool ignoreElements)
        {
            //不能为空
            if(textAsset == null)
            {
                EditorUtility.DisplayDialog("错误", "不能添加空模版", "确认");
                return;
            }

            //更新生成脚本模版
            UpdateScriptTemplate(assetName, textAsset.CheckAssetExtension(".cs")
                ? ConvertCsToTemplate(textAsset.text, ignoreElements) : textAsset.text, groupName);
        }
        /// <summary>
        /// 检查模版是否合理
        /// </summary>
        /// <param name="templateName">模版名称</param>
        /// <param name="text">模版内容</param>
        /// <param name="group">分组名称</param>
        /// <returns>合理的话返回true</returns>
        private static bool CheckTemplateIsValid(string templateName, string text, string group = "")
        {
            //防止名称为空
            if (string.IsNullOrEmpty(templateName))
            {
                EditorUtility.DisplayDialog("错误", "模版名称不能为空！", "确认");
                return false;
            }
            //防止内容为空
            if (string.IsNullOrEmpty(text))
            {
                EditorUtility.DisplayDialog("错误", "模版内容不为空！", "确认");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 自动生成脚本模版的MenuItem
        /// </summary>
        /// <param name="templateName">模版名称</param>
        /// <param name="groupName">分组名称</param>
        private static bool CreateScriptTemplateMenuItem(string templateName, string groupName)
        {
            //生成新的MenuItem的代码
            return ScriptTool.AddCode<ScriptsTemplatesMenuItem>(GenerateScriptTemplateMenuItemCode(templateName, groupName), templateName + groupName);
        }

        /// <summary>
        /// 生成脚本模版的MenuItem的代码
        /// </summary>
        /// <param name="assetName">模版Asset文件名称</param>
        /// <param name="groupName">分组名称</param>
        private static string GenerateScriptTemplateMenuItemCode(string assetName, string groupName) 
        {
            StringBuilder sb = new StringBuilder();
            string space = "        ";
            string keyName = assetName + groupName;
            
            //先加上Attribute
            sb.Append(space).Append($"[MenuItem(\"Assets/Frame C#/");
            if(string.IsNullOrEmpty(groupName) == false)
            {
                sb.Append($"{groupName}/");
            }
            sb.Append($"{assetName}\", false, 0)]\n");
            //创建方法
            sb.Append(space).Append($"public static void Creat{keyName}()\n");
            sb.Append(space).Append('{').Append('\n');
            sb.Append(space).Append($"    ScriptsTemplatesHelper.CreateMyScript(\"{keyName}.cs\", \"{keyName}.cs\");\n");
            sb.Append(space).Append('}').Append('\n');

            //输出代码
            return sb.ToString();
        }

        #endregion

        #region CS脚本转模版

        /// <summary>
        /// 把cs文件的text转为模版text
        /// </summary>
        /// <param name="csText">代码文本</param>
        /// <param name="ignoreElements">忽略所有参数</param>
        private static string ConvertCsToTemplate(string csText, bool ignoreElements)
        {
            //新建StringBuilder
            StringBuilder sb = new StringBuilder();
            //添加顶部申明
            sb.Append(ScriptTop);
            
            //把原文本切分为一行一行
            string[] lines = csText.Split('\n');

            //跳过一开始的注释，找到类或接口的声明的首行
            int startIndex = 0;
            int scriptTopIndex = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith('/') || lines[i].EndsWith('/'))
                {
                    scriptTopIndex = i;
                }
                else if (lines[i].Contains("class") || lines[i].Contains("interface"))
                {
                    startIndex = i;
                    break;
                }
            }

            //越界没有找到目标
            if (startIndex >= lines.Length) return "";

            //获取type名称
            string typeName = "";
            int typeStartIndex = lines[startIndex].IndexOf("class", StringComparison.Ordinal);
            if (typeStartIndex == -1)
            {
                typeStartIndex = lines[startIndex].IndexOf("interface", StringComparison.Ordinal);

                //发生错误
                if (typeStartIndex == -1)
                {
                    Debug.LogError("cs脚本生成模版失败，开头type名称无法获取，可能是哪里有问题，请使用规范脚本代码。\n"+$"当前行数:{startIndex}，当前行文本:"+ lines[startIndex]);
                    return "";
                }

                typeStartIndex += "interface".Length;
            }
            else
            {
                typeStartIndex += "class".Length;
            }

            //获取type名称
            for (int i = typeStartIndex; i < lines[startIndex].Length; i++)
            {
                //如果是空格就跳过
                if (lines[startIndex][i] == ' ') continue;
                //到':'了就结束
                if (lines[startIndex][i] == ':') break;

                typeName += lines[startIndex][i];
            }

            //然后开始转为模版

            //替换一开始声明的typeName
            for (int i = scriptTopIndex + 1; i <= startIndex; i++)
            {
                //转换类型名称
                lines[i] = lines[i].Replace(typeName, ScriptTemplateConfig.ScriptName);
                //更新这一行的内容
                sb.Append(lines[i]).Append('\n');
            }

            //计算一开始的深度，并填写开头代码
            int depth = 1;
            for (int i = scriptTopIndex + 1; i < startIndex; i++)
            {
                //记录深度
                if (lines[i].Contains('{'))
                {
                    depth++;
                }
            }

            //忽略参数和方法
            if (ignoreElements)
            {
                //如果类声明那一行没有{那就加上一个
                if (!lines[startIndex].Contains('{'))
                {
                    //补上一个{
                    for (int i = 1; i < depth; i++)
                    {
                        sb.Append("    ");
                    }
                    sb.Append("{\n");
                }

                //空一行
                sb.Append('\n');

                //最后补上'}"
                while (depth>0)
                {
                    //补上空格
                    for (int i = 1; i < depth; i++)
                    {
                        sb.Append("    ");
                    }
                    sb.Append('}').Append('\n');
                    depth--;
                }
            }
            //包含参数和方法
            else
            {

                for (int i = startIndex + 1; i < lines.Length; i++)
                {
                    //转换类型名称
                    lines[i] = lines[i].Replace(typeName, ScriptTemplateConfig.ScriptName);
                    //添加这一行的文本
                    sb.Append(lines[i]).Append('\n');
                    //查看这一行是不是有方法(只支持常规写法)
                    if (lines[i].Contains('(') && (lines[i].Contains("private") || lines[i].Contains("public")
                        || lines[i].Contains("interval") || lines[i].Contains("protected")) && !lines[i].Contains('=') && !lines[i].Contains(';'))
                    {
                        //查找这一行的结尾
                        int k = FindEndIndexofMethod(lines, i);
                        
                        //如果找到了在同一行，那就移除掉{}
                        if (k == i)
                        {
                            int start = lines[i].IndexOf('{');
                            int end = lines[i].IndexOf('}');
                            if (start != -1 && end != -1)
                            {
                                lines[i] = lines[i].Substring(0, lines[i].Length - end + start);
                            }
                        }
                        
                        //如果找到了
                        if (k != -1)
                        {

                            //同步空格数量
                            string space = "";
                            for (int j = 0; j < lines[i].Length; j++)
                            {
                                //如果不是空格了那就结束
                                if (lines[i][j] != ' ') break;
                                space += ' ';
                            }

                            //更新i
                            i = k;

                            sb.Append(space).Append("{").Append('\n');
                            sb.Append(space).Append("    throw new System.NotImplementedException();").Append('\n');
                            sb.Append(space).Append("}");
                        }

                    }

                }

            }

            
            //输出结果
            return sb.ToString();
        }
        /// <summary>
        /// 找到一个方法的结束行
        /// 对不标准乱写的脚本不起效
        /// </summary>
        /// <returns>返回-1如果找不到的话</returns>
        private static int FindEndIndexofMethod(string[] lines, int startIndex)
        {
            //记录在哪一层
            int depth = 0;
            for (int i = startIndex; i < lines.Length; i++)
            {
                //每有一个'{'深度+1，有一个'}'深度减一
                if (lines[i].Contains('{'))
                {
                    depth++;
                }
                if (lines[i].Contains('}'))
                {
                    depth--;
                    if(depth == 0)
                    {
                        return i;
                    }
                }
            }

            //找不到返回-1
            return -1;
        }

        #endregion

        #region 模版删除

        /// <summary>
        /// 删除指定的template
        /// </summary>
        /// <param name="i">要移除的下标</param>
        private void DeleteTemplate(int i)
        {
            //获取模版名称
            string tName = templateGUI[i].text.Replace(".cs", string.Empty);

            //先删除文件
            string assetPath = templates[i].GetAssetPath();
            AssetDatabase.DeleteAsset(assetPath);
            //如果删除文件后文件夹空了，那就删除文件夹
            string directoryPath = Path.GetDirectoryName(assetPath.ConvertAssetPathToSystemPath());
            if (directoryPath != null && Directory.GetFiles(directoryPath).Length == 0)
            {
                KEditorUtility.DeleteFolder(Path.GetDirectoryName(assetPath));
            }
            
            //再删除MenuItem
            ScriptTool.RemoveCode<ScriptsTemplatesMenuItem>(tName);

            //再从GUI里面移除
            templateGUI.RemoveAt(i);
            //再从ScriptTemplateConfig里面移除
            ScriptTemplateConfig.Templates.RemoveAt(i);
            ScriptTemplateConfig.TemplateDic.Remove(tName);

            //保存
            EditorUtility.SetDirty(ScriptTemplateConfig.Instance);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        #endregion
        
    }
}