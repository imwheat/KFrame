//****************** 代码文件申明 ***********************
//* 文件：TestEditorWindow
//* 作者：wheat
//* 创建时间：2024/04/27 18:46:15 星期六
//* 描述：
//*******************************************************

using UnityEngine;
using UnityEditor;
using KFrame;
using KFrame.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using Object = UnityEngine.Object;
using KFrame.Attributes;
namespace KFrame.Editor
{
    [System.Serializable]
    public class TestClassA
    {
        [KFoldoutGroup("测试FoldoutGroup1"),KLabelText("测试Label1")]
        public int asd;
        [KTabGroup("测试TabGroup1")]
        public string asd2;
        [KTabGroup("测试TabGroup2")]
        public bool asd3;
        [KTabGroup("测试TabGroup2"), KLabelText("测试LabelList")]
        public List<int> asd4 = new List<int>() { 1,3,5,6};
        [KTabGroup("测试TabGroup1"), KLabelText("测试类B")]
        public TestClassB B;
        [KButton("asdsad", 1)]
        public void TestDebug()
        {

        }
    }
    [System.Serializable]
    public class TestClassB
    {
        [KTabGroup("测试TabGroup1"), KLabelText("测试LabelA")]
        public int aaa;
        [KTabGroup("测试TabGroup1"), KLabelText("测试LabelB")]
        public int aaa2;
        [KTabGroup("测试TabGroup1"), KLabelText("测试LabelC")]
        public int aaa3;
        public string bbb;
        public string ccc;
    }
    public class TestEditorWindow : EditorWindow
    {
        [SerializeField]
        private TestClassA tA;
        public int a;
        private SerializedObject so;
        private List<SerializedProperty> tAProperties;
        private SerializedPropertyPack spp;
        private Vector2 scroll;
        private List<string> testStrings;

        [MenuItem("测试/测试窗口")]
        public static void ShowWindow()
        {
            TestEditorWindow a = CreateWindow<TestEditorWindow>();
        }
        private void OnEnable()
        {
            tA = new TestClassA();
            tA.asd = 6;
            tA.asd3 = true;
            tA.asd2 = "ssssssss";
            so = new SerializedObject(this);
            tAProperties = KEditorGUI.GetSerializedPropertyIncludeChildren(so, "tA");
            spp = KEditorGUI.GetSPPackIncludeChildren(so, "tA");
            testStrings = new List<string>() { "苹果", "香蕉", "啊", "土豆", "汉堡包", "玉米", "啦啦啦" ,"草","米","三角","钝角"};
            //Sirenix.OdinInspector.Editor.GeneralDrawerConfig

        }
        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            scroll = EditorGUILayout.BeginScrollView(scroll);

            foreach (var property in tAProperties)
            {
                EditorGUITool.DrawSerializedProperty(so, property);
            }

            KEditor.DrawProperties(so, spp);

            KEditor.DrawProperties(so, tAProperties);

            Rect rect = EditorGUILayout.GetControlRect(GUILayout.ExpandWidth(true));
            if(GUI.Button(rect,"显示PopupWindow"))
            {
                PopupWindow.Show(rect, new KDropDownPopupWindow(testStrings, (x) => { Debug.Log(x); }));
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndVertical();
        }

    }
}

