//****************** 代码文件申明 ************************
//* 文件：EditorGUITool                                       
//* 作者：wheat
//* 创建时间：2024/03/04 13:43:33 星期一
//* 描述：方便绘制一些GUI
//*****************************************************

using UnityEngine;
using KFrame.Systems;
using System;
using UnityEditor;
using System.IO;
using System.Reflection;
using UnityEditor.AddressableAssets.Settings;
using Object = UnityEngine.Object;
using System.Collections.Generic;

namespace KFrame.Editor
{
    public static partial class EditorGUITool
    {
        #region 音效播放反射调用

        private static Type audioUtilType;

        private static MethodInfo stopAllPreviewclipsMethod;
        private static MethodInfo playPreviewclipMethod;
        private static MethodInfo isPreviewClipPlayingMethod;

        #endregion

        #region 窗口反射调用

        private static Type dockAreaType;
        private static Type viewType;
        private static Type containerWindowType;

        private static FieldInfo hostViewFieldInfo;
        private static PropertyInfo containerWindowPropertyInfo;
        private static PropertyInfo viewParentPropertyInfo;
        private static MethodInfo addTabMethod;
        private static MethodInfo removeTabMethod;
        private static MethodInfo addViewChildMethod;
        private static MethodInfo closeContainerWindowMethod;

        #endregion

        /// <summary>
        /// 存储本地化信息的路径
        /// </summary>
        private static string localizationPath = "Assets/8.Data/Library/EditorLocalizaitionConfig.asset";
        /// <summary>
        /// 本地化库
        /// </summary>
        private static LocalizationOdinConfig _localizationOdinConfig;
        /// <summary>
        /// AB包配置路径
        /// </summary>
        private static string ABSettingsPath = "Assets/6.ProjectPresets/AddressableAssetsData/AddressableAssetSettings.asset";
        /// <summary>
        /// AB包配置
        /// </summary>
        public static AddressableAssetSettings ABSettings
        {
            get
            {
                if (_addressableAssetSettings == null)
                {
                    _addressableAssetSettings = AssetDatabase.LoadAssetAtPath<AddressableAssetSettings>(ABSettingsPath);
                }

                return _addressableAssetSettings;
            }
        }
        /// <summary>
        /// AB包配置
        /// </summary>
        private static AddressableAssetSettings _addressableAssetSettings;

        #region GUI显示工具

        /// <summary>
        /// 显示一个Enum选项
        /// </summary>
        public static void ShowEnumSelectOption<TEnum>(string label, string btnLabel, Action<int> action,
            bool enableHor = true, params GUILayoutOption[] options) where TEnum : struct
        {
            //开始水平排版
            if (enableHor)
            {
                EditorGUILayout.BeginHorizontal();
            }

            //显示标签
            if (string.IsNullOrEmpty(label) == false)
            {
                if (options != null && options.Length != 0)
                {
                    EditorGUILayout.LabelField(label, EditorStyles.boldLabel, options);
                }
                else
                {
                    EditorGUILayout.LabelField(label, GUILayout.Width(100));
                }
            }


            //更新按钮的Label，从本地化文本里面查找一下
            string key = typeof(TEnum).Name + "_" + btnLabel;
            string value = GetEditorLocalizationText(key);
            //要是有的话就更新
            if (key != value)
            {
                btnLabel = value;
            }

            //显示按钮
            if (options != null && options.Length != 0)
            {
                if (GUILayout.Button(btnLabel, options))
                {
                    EnumEditorWindow.ShowEnumSelectOption<TEnum>(action);
                }
            }
            else
            {
                if (GUILayout.Button(btnLabel))
                {
                    EnumEditorWindow.ShowEnumSelectOption<TEnum>(action);
                }
            }

            //结束水平排版
            if (enableHor)
            {
                EditorGUILayout.EndHorizontal();
            }
        }
        /// <summary>
        /// 获取Editor本地化文本
        /// </summary>
        /// <param name="text">原文本</param>
        /// <returns></returns>
        public static string GetEditorLocalizationText(string text)
        {
            //防空
            if (_localizationOdinConfig == null)
            {
                _localizationOdinConfig = AssetDatabase.LoadAssetAtPath(localizationPath, typeof(LocalizationOdinConfig)) as LocalizationOdinConfig;
            }

            //如果还是为空那就直接返回原文
            if (_localizationOdinConfig == null)
            {
                return text;
            }

            //尝试从本地化配置中获取data
            LocalizationStringData data = _localizationOdinConfig.GetContent<LocalizationStringData>(text, LanguageType.SimplifiedChinese);

            //如果没有那就返回原文
            if (data == null)
            {
                return text;
            }
            //有的话那就返回data中的文本
            else
            {
                return data.content;
            }
        }
        /// <summary>
        /// 显示一个明显的foldout
        /// </summary>
        /// <param name="foldout">bool折叠传参</param>
        /// <param name="label">标签</param>
        public static void ShowAClearFoldOut(ref bool foldout, string label)
        {
            //初始化Style
            GUIStyle style = new GUIStyle();
            style.richText = true;
            label = $"<color=white><b>{label}</b></color>";

            EditorGUILayout.BeginHorizontal();

            // 使用自定义的Toggle
            foldout = EditorGUILayout.Toggle(foldout, GUILayout.Width(15));

            // 空开一点
            GUILayout.Space(2);

            // 使用GUILayout.Label显示富文本
            GUILayout.Label(label, style, GUILayout.ExpandWidth(true));


            EditorGUILayout.EndHorizontal();
        }
        /// <summary>
        /// 显示一个明显的输入栏
        /// </summary>
        public static void ShowAClearTextFiled(ref string text, string label)
        {
            //初始化Style
            GUIStyle style = new GUIStyle();
            style.richText = true;
            label = $"<color=white><b>{label}</b></color>";

            EditorGUILayout.BeginHorizontal();

            // 使用GUILayout.Label显示富文本
            GUILayout.Label(label, style, GUILayout.ExpandWidth(true));

            // 空开一点
            GUILayout.Space(2);

            // 使用自定义的Toggle
            text = EditorGUILayout.TextField(text);


            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 显示一个明显的输入栏
        /// </summary>
        public static void ShowAClearTextFiled(ref int text, string label)
        {
            //初始化Style
            GUIStyle style = new GUIStyle();
            style.richText = true;
            label = $"<color=white><b>{label}</b></color>";

            EditorGUILayout.BeginHorizontal();

            // 使用GUILayout.Label显示富文本
            GUILayout.Label(label, style, GUILayout.ExpandWidth(true));

            // 空开一点
            GUILayout.Space(2);

            // 使用自定义的Toggle
            text = EditorGUILayout.IntField(text);


            EditorGUILayout.EndHorizontal();
        }

        #endregion

        #region AB包工具

        /// <summary>
        /// 把指定文件添加进AB包中
        /// </summary>
        /// <param name="obj">要添加的东西</param>
        /// <param name="addressPath">保存路径</param>
        /// <param name="saveGroup">保存的分组</param>
        public static void AddToAddressable(Object obj, string addressPath, AddressableAssetGroup saveGroup)
        {
            AddToAddressable(GetPathHelper(obj), addressPath, saveGroup);
        }
        /// <summary>
        /// 把指定文件添加进AB包中
        /// </summary>
        /// <param name="obj">要添加的东西</param>
        /// <param name="addressPath">保存路径</param>
        /// <param name="saveGroupName">保存的分组名称</param>
        public static void AddToAddressable(Object obj, string addressPath, string saveGroupName)
        {
            AddToAddressable(GetPathHelper(obj), addressPath, ABSettings.FindGroup(saveGroupName));
        }
        /// <summary>
        /// 把指定路径的文件添加进AB包中
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="addressPath">保存路径</param>
        /// <param name="saveGroupName">保存的分组名称</param>
        public static void AddToAddressable(string path, string addressPath, string saveGroupName)
        {
            AddToAddressable(path, addressPath, ABSettings.FindGroup(saveGroupName));
        }
        /// <summary>
        /// 把指定路径的文件添加进AB包中
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="addressPath">保存路径</param>
        /// <param name="saveGroup">要保存的group</param>
        public static void AddToAddressable(string path, string addressPath, AddressableAssetGroup saveGroup)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(addressPath))
            {
                Debug.LogWarning("路径错误无法添加");
                return;
            }

            if (saveGroup == null)
            {
                Debug.LogWarning("所选分组为空，无法添加");
                return;
            }

            //添加到addressable里面
            AddressableAssetEntry entry = ABSettings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(path), saveGroup);
            entry.address = addressPath;

            //保存设置
            ABSettings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entry, true, true);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }
        /// <summary>
        /// 把指定文件从AB包中移除
        /// </summary>
        /// <param name="obj">要移除的文件</param>
        public static void RemoveFromAddressable(Object obj)
        {
            RemoveFromAddressable(GetPathHelper(obj));
        }
        /// <summary>
        /// 把指定路径的文件从AB包中移除
        /// </summary>
        /// <param name="path">文件路径</param>
        public static void RemoveFromAddressable(string path)
        {
            //移除
            ABSettings.RemoveAssetEntry(AssetDatabase.AssetPathToGUID(path));

            //保存设置
            EditorUtility.SetDirty(ABSettings);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }
        /// <summary>
        /// 获取Asset路径
        /// </summary>
        private static string GetPathHelper(Object obj)
        {
            return AssetDatabase.GetAssetPath(obj);
        }
        #endregion

        #region 转化工具

        /// <summary>
        /// 将sprite转化为tex输出
        /// </summary>
        /// <param name="sp">要获取的图片</param>
        /// <param name="red">标红图片</param>
        public static Texture2D ConvertSpriteToTex(Sprite sp, bool red = false)
        {
            //防空
            if (sp == null) return null;

            int width = (int)sp.rect.width;
            int height = (int)sp.rect.height;

            //转为Texture输出
            Texture2D res = new Texture2D(width, height, sp.texture.format, false);

            //复制一份Texture
            Graphics.CopyTexture(sp.texture, 0, 0, (int)sp.rect.x, (int)sp.rect.y, width, height, res, 0, 0, 0, 0);

            //如果限制红色通道
            if (red)
            {
                var colors = res.GetPixels();

                for (int i = 0; i < colors.Length; i++)
                {
                    colors[i] = new Color(colors[i].r, 0, 0, colors[i].a);
                }

                res.SetPixels(colors);
            }

            //设置基础属性然后应用
            res.alphaIsTransparency = true;
            res.filterMode = FilterMode.Point;
            res.Apply();


            return res;
        }
        /// <summary>
        /// 防止路径冲突
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="format">格式</param>
        /// <param name="id">id</param>
        public static string AvoidPathCollision(string path, string format, int id)
        {
            //先获取路径
            var res = path;
            //如果id不为0那就加入(id)
            if (id != 0)
            {
                res += "(" + id + ")";
            }

            //最后添加后缀
            res += format;

            //如果文件已存在那就继续更改
            if (File.Exists(res))
            {
                return AvoidPathCollision(path, format, id + 1);
            }
            else
            {
                //不存在就输出
                return res;
            }
        }

        #endregion

        #region 音效播放

        /// <summary>
        /// 播放音效
        /// </summary>
        public static void EditorPlayAudio(AudioClip clip)
        {
            //如果为空就返回
            if (clip == null) return;

            //防空
            if (audioUtilType == null)
            {
                audioUtilType = typeof(AudioImporter).Assembly.GetType("UnityEditor.AudioUtil");
            }
            //通过反射获取各个方法
            if (audioUtilType != null && playPreviewclipMethod == null)
            {
                playPreviewclipMethod = audioUtilType.GetMethod("PlayPreviewClip", BindingFlags.Public | BindingFlags.Static);
                isPreviewClipPlayingMethod = audioUtilType.GetMethod("IsPreviewClipPlaying", BindingFlags.Public | BindingFlags.Static);
                stopAllPreviewclipsMethod = audioUtilType.GetMethod("StopAllPreviewClips", BindingFlags.Public | BindingFlags.Static);
            }

            //暂停所有播放
            stopAllPreviewclipsMethod.Invoke(null, null);
            //播放新的音效
            playPreviewclipMethod.Invoke(null, new object[] { clip, 0, false });
        }
        /// <summary>
        /// 停止播放
        /// </summary>
        public static void EditorStopPlayAudio()
        {
            //防空
            if (stopAllPreviewclipsMethod == null) return;

            //暂停所有播放
            stopAllPreviewclipsMethod.Invoke(null, null);
        }
        /// <summary>
        /// 检测是否在播放音效
        /// </summary>
        /// <returns></returns>
        public static bool IsEditorPlayingAudio()
        {
            //如果为空那就说明不在播放
            if (isPreviewClipPlayingMethod == null) return false;

            //不为空那就调用方法
            return (bool)isPreviewClipPlayingMethod?.Invoke(null, null);
        }

        #endregion

        #region 窗口工具

        /// <summary>
        /// 初始化
        /// </summary>
        public static void InitEditorWindowReflection()
        {
            //如果不为空的话那就不需要反射了
            if (dockAreaType != null) return;

            //反射获取相关信息
            dockAreaType = typeof(EditorWindow).Assembly.GetType("UnityEditor.DockArea");
            viewType = typeof(EditorWindow).Assembly.GetType("UnityEditor.View");
            containerWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.ContainerWindow");
            addTabMethod = (dockAreaType).GetMethod("AddTab", new Type[] { typeof(EditorWindow), typeof(bool) });
            addViewChildMethod = (viewType).GetMethod("AddChild", new Type[] {viewType});
            removeTabMethod = (dockAreaType).GetMethod("RemoveTab", new Type[] { typeof(EditorWindow), typeof(bool), typeof(bool) });
            closeContainerWindowMethod = (containerWindowType).GetMethod("Close", BindingFlags.Instance | BindingFlags.Public);
            hostViewFieldInfo = typeof(EditorWindow).GetField("m_Parent", BindingFlags.Instance | BindingFlags.NonPublic);
            containerWindowPropertyInfo = dockAreaType.GetProperty("window", BindingFlags.Instance | BindingFlags.Public);
            viewParentPropertyInfo = dockAreaType.GetProperty("parent", BindingFlags.Instance | BindingFlags.Public);
        }
        /// <summary>
        /// 把一个窗口作为tab加在另一个窗口上面
        /// </summary>
        /// <param name="parent">作为parent的窗口</param>
        /// <param name="tab">被添加作为tab的窗口</param>
        public static void AddWindowTab(EditorWindow parent, EditorWindow tab)
        {
            //检测初始化
            InitEditorWindowReflection();

            //获取它们的HostView
            object parentHostView = hostViewFieldInfo.GetValue(parent);
            object tabHostView = hostViewFieldInfo.GetValue(tab);
            //先把要作为tab的从自己原先的身上移除
            removeTabMethod.Invoke(tabHostView, new object[] {tab, tabHostView != parentHostView, false });
            //然后加在parent上
            addTabMethod.Invoke(parentHostView, new object[] {tab, true});
            //清零
            GUIUtility.hotControl = 0;
        }
        /// <summary>
        /// 把一个窗口加在另一个窗口上作为splitview
        /// </summary>
        /// <param name="parent">作为parent的窗口</param>
        /// <param name="tab">被添加作为tab的窗口</param>
        public static void AddView(EditorWindow parent, EditorWindow tab)
        {
            //检测初始化
            InitEditorWindowReflection();

            //获取它们的HostView
            object parentHostView = hostViewFieldInfo.GetValue(parent);
            object viewParent = viewParentPropertyInfo.GetValue(parentHostView);
            object tabHostView = hostViewFieldInfo.GetValue(tab);
            object tabContainerWindow = containerWindowPropertyInfo.GetValue(tabHostView);

            //加上去
            addViewChildMethod.Invoke(viewParent, new object[] {tabHostView});
            //关掉tab的ContainerWindow
            closeContainerWindowMethod.Invoke(tabContainerWindow, null);
            //清零
            GUIUtility.hotControl = 0;
        }
        /// <summary>
        /// 检测俩个窗口的DockArea是否相等
        /// </summary>
        public static bool IsSameDockArea(EditorWindow window1, EditorWindow window2)
        {
            //检测初始化
            InitEditorWindowReflection();

            //获取它们的DockArea
            object parentDockArea = hostViewFieldInfo.GetValue(window1);
            object tabDockArea = hostViewFieldInfo.GetValue(window2);

            //判断是否相等
            return parentDockArea == tabDockArea;
        }
        /// <summary>
        /// 检测俩个窗口的ContainerWindow是否相等
        /// </summary>
        public static bool IsSameContainerWindow(EditorWindow window1, EditorWindow window2)
        {
            //检测初始化
            InitEditorWindowReflection();

            //获取它们的DockArea
            object parentContainerWindow = containerWindowPropertyInfo.GetValue(hostViewFieldInfo.GetValue(window1));
            object tabContainerWindow = containerWindowPropertyInfo.GetValue(hostViewFieldInfo.GetValue(window2));

            //判断是否相等
            return parentContainerWindow == tabContainerWindow;
        }
        /// <summary>
        /// 获取一个编辑器窗口的view的parent
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        public static object GetViewParent(EditorWindow window)
        {
            return viewParentPropertyInfo.GetValue(hostViewFieldInfo.GetValue(window));
        }
        /// <summary>
        /// 获取一个编辑器窗口的ContainerWindow
        /// </summary>
        public static object GetContainerWindow(EditorWindow window)
        {
            return containerWindowPropertyInfo.GetValue(hostViewFieldInfo.GetValue(window));
        }

        #endregion

        #region 其他通用操作
        
        /// <summary>
        /// 停止编辑文本框
        /// </summary>
        public static void EndEditTextField()
        {
            EditorGUIUtility.editingTextField = false;
            EditorGUIUtility.hotControl = 0;
        }
        /// <summary>
        ///   <para>显示选择窗口</para>
        /// </summary>
        /// <param name="title">窗口标题</param>
        /// <param name="message">显示信息</param>
        /// <param name="ok">确认的文本</param>
        /// <param name="cancel">取消的文本</param>
        /// <param name="callback">回调函数</param>
        /// <returns>
        ///   <para>点击确认就返回Confirm，点击取消返回Cancel，直接关掉返回None</para>
        /// </returns>
        public static void DisplayDialog(string title, string message, string ok, string cancel, Action<EditorSelectWindow.ConfirmType> callback)
        {
            EditorSelectWindow.DisplayDialog(title, message, ok, cancel, callback);
        }

        #endregion
        
    }

    //编辑器GUI绘制属性
    public static partial class EditorGUITool
    {
        /// <summary>
        /// 以默认的方式绘制的属性
        /// </summary>
        /// <param name="instance">被绘制的类的实例</param>
        public static void DrawDefalutProperties<T>(T instance)
        {
            //获取所有字段
            FieldInfo[] fieldInfos = typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            //获取所有属性
            PropertyInfo[] propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            //然后遍历绘制
            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                KEditorGUI.DrawField(instance, fieldInfo, KEditorGUI.DrawNativeFiled);
            }
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                KEditorGUI.DrawProperty(instance, propertyInfo, KEditorGUI.DrawNativeProperty);
            }
        }
        /// <summary>
        /// 以默认的方式绘制的属性
        /// </summary>
        public static void DrawSerializedProperty(SerializedObject serializedObject,SerializedProperty property)
        {
            //更新序列化object
            serializedObject.UpdateIfRequiredOrScript();

            KEditorGUI.DrawNativeProperty(property);
            //应用更改
            serializedObject.ApplyModifiedProperties();
        }

    }

}
