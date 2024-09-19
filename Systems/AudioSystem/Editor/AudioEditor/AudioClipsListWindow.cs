//****************** 代码文件申明 ************************
//* 文件：AudioClipsListWindow                                       
//* 作者：wheat
//* 创建时间：2024/09/04 08:24:51 星期三
//* 描述：编辑器选择音效Clips使用的窗口
//*****************************************************

#if UNITY_EDITOR

using UnityEngine;
using System.Collections.Generic;
using KFrame;
using KFrame.Systems;
using UnityEditor;

namespace KFrame.Systems
{
    public class AudioClipsListWindow : EditorWindow
    {
        #region 参数引用

        /// <summary>
        /// 音效库
        /// </summary>
        private static AudioLibrary library => AudioLibrary.Instance;
        /// <summary>
        /// 正在编辑的列表
        /// </summary>
        private List<AudioClip> editClips;

        #endregion
        
        #region GUI显示相关

        /// <summary>
        /// 滚轮位置
        /// </summary>
        private Vector2 listScrollPosition;

        /// <summary>
        /// 标准化统一当前编辑器的绘制Style
        /// </summary>
        private static class MStyle
        {
#if UNITY_2018_3_OR_NEWER
            internal static readonly float spacing = EditorGUIUtility.standardVerticalSpacing;
#else
            internal static readonly float spacing = 2.0f;
#endif
            internal static readonly float btnWidth = 40f;
            internal static readonly float labelHeight = 15f;
            internal static readonly float boxHeight = 50f;
            
        }

        #endregion

        #region 初始化生命周期相关

        /// <summary>
        /// 打开面板
        /// </summary>
        /// <param name="audioStack">要编辑的音效</param>
        /// <returns></returns>
        public static AudioClipsListWindow ShowWindow(List<AudioClip> clipList)
        {
	        AudioClipsListWindow window = EditorWindow.GetWindow<AudioClipsListWindow>();
            window.titleContent = new GUIContent("音效列表编辑器");
            window.Init(clipList);

            return window;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="audioStack"></param>
        private void Init(List<AudioClip> clipList)
        {
            editClips = clipList;
        }

        private void OnDisable()
        {
	        if (editClips != null)
	        {
		        //遍历移除为空的项
		        for (int i = editClips.Count - 1; i >= 0; i--)
		        {
			        if (editClips[i] == null)
			        {
				        editClips.RemoveAt(i);
			        }
		        }
	        }
        }

        #endregion
        
        #region GUI绘制

        private void OnGUI()
        {
	        //绘制拖拽放入的BOX
            DrawDragBox();
            
            //绘制列表
            DrawList();
        }
        /// <summary>
        /// 绘制拖拽放入的BOX
        /// </summary>
        private void DrawDragBox()
        {
	        EditorGUILayout.BeginVertical();
	        
            GUILayout.Label("音效Clip列表");
                        
	        GUILayout.Label("把Clip丢进这里批量添加:", EditorStyles.boldLabel);
                        
	        //获取当前事件
	        Event currentEvent = Event.current;
                        
	        //画出一个拖拽区域
	        Rect dropArea = GUILayoutUtility.GetRect(0f, 50f, GUILayout.ExpandWidth(true));
	        GUI.Box(dropArea, "放入Clip区域");
                        
	        switch (currentEvent.type)
	        {
		        case EventType.DragUpdated:
		        case EventType.DragPerform:
			        //如果鼠标不在拖拽区域就返回
			        if (!dropArea.Contains(currentEvent.mousePosition))
				        break;
                        
			        //把鼠标的显示改为Copy的样子
			        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        
			        //如果放下了鼠标
			        if (currentEvent.type == EventType.DragPerform)
			        {
				        //那就完成拖拽
				        DragAndDrop.AcceptDrag();
                        
				        //然后遍历选中的物品
				        foreach (UnityEngine.Object draggedObject in DragAndDrop.objectReferences)
				        {
					        //如果是AudioClip
					        if (draggedObject is AudioClip)
					        {
						        //那就获取添加到列表里面
						        AudioClip clip = (AudioClip)draggedObject;
								editClips.Add(clip);
								
					        }
				        }
			        }
			        Event.current.Use();
			        break;
	        }
	        
	        EditorGUILayout.EndVertical();
	        
        }
        /// <summary>
        /// 绘制列表
        /// </summary>
        private void DrawList()
        {
	        EditorGUILayout.BeginVertical();
	
	        listScrollPosition = EditorGUILayout.BeginScrollView(listScrollPosition);
			
	        //计算UI尺寸
	        float btnWidth = Mathf.Min(position.width / 3f, MStyle.btnWidth);
	        float labelWidth = position.width - btnWidth;
	        
	        for (int i = 0; i < editClips.Count; i++)
	        {
		        EditorGUILayout.BeginHorizontal();
		        
		        editClips[i] = (AudioClip)EditorGUILayout.ObjectField(editClips[i], typeof(AudioClip), false,
			        GUILayout.Height(MStyle.labelHeight), GUILayout.Width(labelWidth));
		        if (GUILayout.Button("删除", GUILayout.Width(btnWidth), GUILayout.Height(MStyle.labelHeight)))
		        {
			        editClips.RemoveAt(i);
			        i--;
			        break;
		        }
		        
		        EditorGUILayout.EndHorizontal();
		        
		        GUILayout.Space(MStyle.spacing);
	        }

	        GUILayout.Space(5f);
	        
	        if (GUILayout.Button("新增一个", GUILayout.Height(MStyle.labelHeight)))
	        {
		        editClips.Add(null);
	        }
	        
	        EditorGUILayout.EndScrollView();
	        
	        EditorGUILayout.EndVertical();
        }

        #endregion
        
    }
}

#endif