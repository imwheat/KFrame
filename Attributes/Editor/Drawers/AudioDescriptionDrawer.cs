//****************** 代码文件申明 ***********************
//* 文件：AudioDescriptionDrawer
//* 作者：wheat
//* 创建时间：2025/01/15 17:22:21 星期三
//* 描述：音效显示相关的Drawer
//*******************************************************

using System.Collections.Generic;
using KFrame.Editor;
using KFrame.Systems;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace KFrame.Attributes.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(AudioDescriptionAttribute))]
    public class AudioDescriptionDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return base.GetPropertyHeight(property, label) * 2.5f;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			if (property.propertyType == SerializedPropertyType.Integer)
			{
				EditorGUI.BeginChangeCheck();

				//获取intValue
				var newValue = EditorGUI.IntField(new Rect(position.x, position.y, position.width / 5f * 4f, position.height / 3f)
					, label, property.intValue);

				//根据int的值获取所需那音效信息
				if (property.intValue == 0)
				{
					EditorGUI.LabelField(
						new Rect(position.x, position.y + position.height / 2f, position.width, position.height / 2f),
						"所选音效", "无");
				}
				else if (property.intValue == -1)
				{
					EditorGUI.LabelField(
						new Rect(position.x, position.y + position.height / 2f, position.width, position.height / 2f),
						"所选音效", "默认(如果你看见说明还没装上去)");
				}
				else
				{
					AudioStack audioStack = GetAudioStack(property.intValue);
					if (audioStack != null)
					{
						EditorGUI.LabelField(
							new Rect(position.x, position.y + position.height / 2f, position.width / 5 * 4f, position.height / 2f),
							"所选音效", audioStack.AudioName);

						if (EditorGUITool.IsEditorPlayingAudio())
						{
							if (GUI.Button(new Rect(position.x + position.width / 5 * 4f, position.y + position.height / 2f, position.width / 5f, position.height / 2f), EditorIcons.Pause.Raw))
							{
								EditorGUITool.EditorStopPlayAudio();
							}
						}
						else
						{
							if (GUI.Button(new Rect(position.x + position.width / 5 * 4f, position.y + position.height / 2f, position.width / 5f, position.height / 2f), EditorIcons.Play.Raw))
							{
								EditorGUITool.EditorPlayAudio(audioStack.GetRandomAudio());
							}
						}

					}
					else
					{
						EditorGUI.LabelField(
							new Rect(position.x, position.y + position.height / 2f, position.width, position.height / 2f),
							"无法找到目标", "输入有误！");
					}
				}


				if (EditorGUI.EndChangeCheck())
				{
					property.intValue = newValue;
				}

				//按钮选择音效
				if (GUI.Button(new Rect(position.x + position.width / 5 * 4f, position.y, position.width / 5f, position.height / 2f), "选择"))
				{
					//显示菜单
					AudioEditor.ShowAsSelector((x) =>
					{
						property.intValue = x;
						property.serializedObject.ApplyModifiedProperties();
					});
				}
			}

			EditorGUI.EndProperty();
		}

		private AudioStack GetAudioStack(int index)
		{
			//先获取物品信息库
			AudioLibrary audioLibrary = AudioLibrary.Instance;

			if (audioLibrary != null)
			{
				List<AudioStack> audioes = audioLibrary.Audioes;

				if (audioes.Count == 0)
				{
					return null;
				}

				//再从音效信息库中获取音效文件
				AudioStack audioStack = audioes.Find(x => x.AudioIndex == index);

				//然后返回角色信息，没找到就返回null
				if (audioStack != null)
				{
					return audioStack;
				}
				else
				{
					return null;
				}
			}
			else
			{
				return null;
			}
		}
    }
}

