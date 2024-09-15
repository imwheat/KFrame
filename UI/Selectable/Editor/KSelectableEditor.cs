//****************** 代码文件声明 ***********************
//* 文件：KSelectableEditor
//* 作者：wheat
//* 创建时间：2024/09/15 13:01:48 星期日
//* 描述：
//*******************************************************
using System;
using System.Collections.Generic;
using KFrame;
using KFrame.Editor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEngine.UI;

namespace KFrame.UI
{
    [CustomEditor(typeof(KSelectable), true)]
    public class KSelectableEditor : UnityEditor.Editor
    {
        /// <summary>
        /// 自动绘制排除的属性
        /// </summary>
        private string[] propertyToExclude = new[]
        {
            "m_Script","m_Navigation", "m_Transition", "m_Colors", "m_SpriteState", "m_AnimationTriggers", "m_TargetGraphic",
            "m_TargetGraphics", "m_Interactable"
        };
        
        /// <summary>
        /// 导航FoldOut
        /// </summary>
        private bool navigationFoldOut;
        protected SerializedProperty navigation;
        private SerializedProperty selectOnUp;
        private SerializedProperty selectOnDown;
        private SerializedProperty selectOnLeft;
        private SerializedProperty selectOnRight;
        
        protected SerializedProperty transition;
        
        protected SerializedProperty colors;
        protected SerializedProperty normalColor;
        protected SerializedProperty pressColor;
        protected SerializedProperty selectedlColor;
        protected SerializedProperty disableColor;
        protected SerializedProperty colorMultiplier;
        protected SerializedProperty fadeDuration;
        
        protected SerializedProperty spriteState;
        protected SerializedProperty pressSprite;
        protected SerializedProperty selectedlSprite;
        protected SerializedProperty disableSprite;
        
        protected SerializedProperty animationTriggers;
        protected SerializedProperty normalTrigger;
        protected SerializedProperty pressTrigger;
        protected SerializedProperty selectedlTrigger;
        protected SerializedProperty disableTrigger;
        
        protected SerializedProperty targetGraphic;
        protected SerializedProperty targetGraphics;
        protected SerializedProperty interactable;

        private void OnEnable()
        {
            //导航
            navigation = serializedObject.FindProperty("m_Navigation");
            if (navigation.FindPropertyRelative("m_Mode").intValue != 4)
            {
                navigation.FindPropertyRelative("m_Mode").intValue = 4;
                serializedObject.ApplyModifiedProperties();
            }
            selectOnUp = navigation.FindPropertyRelative("m_SelectOnUp");
            selectOnDown = navigation.FindPropertyRelative("m_SelectOnDown");
            selectOnLeft = navigation.FindPropertyRelative("m_SelectOnLeft");
            selectOnRight = navigation.FindPropertyRelative("m_SelectOnRight");
            
            //切换类型
            transition = serializedObject.FindProperty("m_Transition");
            
            //颜色切换
            colors = serializedObject.FindProperty("m_Colors");
            normalColor = colors.FindPropertyRelative("m_NormalColor");
            pressColor = colors.FindPropertyRelative("m_PressedColor");
            selectedlColor = colors.FindPropertyRelative("m_SelectedColor");
            disableColor = colors.FindPropertyRelative("m_DisabledColor");
            colorMultiplier = colors.FindPropertyRelative("m_ColorMultiplier");
            fadeDuration = colors.FindPropertyRelative("m_FadeDuration");
            
            //图片切换
            spriteState = serializedObject.FindProperty("m_SpriteState");
            pressSprite = spriteState.FindPropertyRelative("m_PressedSprite");
            selectedlSprite = spriteState.FindPropertyRelative("m_SelectedSprite");
            disableSprite = spriteState.FindPropertyRelative("m_DisabledSprite");
            
            //动画切换
            animationTriggers = serializedObject.FindProperty("m_AnimationTriggers");
            normalTrigger = animationTriggers.FindPropertyRelative("m_NormalTrigger");
            pressTrigger = animationTriggers.FindPropertyRelative("m_PressedTrigger");
            selectedlTrigger = animationTriggers.FindPropertyRelative("m_SelectedTrigger");
            disableTrigger = animationTriggers.FindPropertyRelative("m_DisabledTrigger");
            
            //目标图片
            targetGraphic = serializedObject.FindProperty("m_TargetGraphic");
            targetGraphics = serializedObject.FindProperty("m_TargetGraphics");
            
            //能否交互
            interactable = serializedObject.FindProperty("m_Interactable");
            
        }

        public override void OnInspectorGUI()
        {
            
            serializedObject.Update();
            
            EditorGUILayout.BeginVertical();

            KEditorGUI.PropertyField(interactable, "能否交互");
            
            GUILayout.Space(10f);
            
            EditorGUITool.ShowAClearFoldOut(ref navigationFoldOut, "导航");
            if (navigationFoldOut)
            {
                KEditorGUI.BeginVerticleWithSpace(20f);
                
                KEditorGUI.PropertyField(selectOnUp, "向上");
                KEditorGUI.PropertyField(selectOnDown, "向下");
                KEditorGUI.PropertyField(selectOnLeft, "向左");
                KEditorGUI.PropertyField(selectOnRight, "向右");
                
                KEditorGUI.EndVerticleWithSpace(0f);
            }
            
            GUILayout.Space(10f);

            KEditorGUI.PropertyField(transition, "切换类型");

            KEditorGUI.BeginVerticleWithSpace(20f);
            
            switch (transition.intValue)
            {
                //颜色切换
                case 1:
                    KEditorGUI.PropertyField(targetGraphic, "目标图像");
                    KEditorGUI.PropertyField(targetGraphics, "目标图像列表");
                    KEditorGUI.PropertyField(normalColor,"普通状态");
                    KEditorGUI.PropertyField(pressColor,"按下状态");
                    KEditorGUI.PropertyField(selectedlColor,"选择状态");
                    KEditorGUI.PropertyField(disableColor,"禁用状态");
                    KEditorGUI.PropertyField(colorMultiplier,"颜色倍率");
                    KEditorGUI.PropertyField(fadeDuration,"渐变时间");
                    break;
                //图片切换
                case 2:
                    KEditorGUI.PropertyField(targetGraphic, "目标图像");
                    KEditorGUI.PropertyField(targetGraphics, "目标图像列表");
                    KEditorGUI.PropertyField(pressSprite,"按下状态");
                    KEditorGUI.PropertyField(selectedlSprite,"选择状态");
                    KEditorGUI.PropertyField(disableSprite,"禁用状态");
                    break;
                //动画切换
                case 3:
                    KEditorGUI.PropertyField(normalTrigger,"普通状态");
                    KEditorGUI.PropertyField(pressTrigger,"按下状态");
                    KEditorGUI.PropertyField(selectedlTrigger,"选择状态");
                    KEditorGUI.PropertyField(disableTrigger,"禁用状态");
                    break;
            }
            
            KEditorGUI.EndVerticleWithSpace(0f);
            
            GUILayout.Space(10f);
            
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            //绘制剩余的属性
            DrawPropertiesExcluding(serializedObject, propertyToExclude);
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}
