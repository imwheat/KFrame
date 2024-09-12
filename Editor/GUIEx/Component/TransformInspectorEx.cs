// using System.Reflection;
// using UnityEngine;
// using UnityEditor;
//
//
// /// <summary>
// /// Unity反射会出现一些烦人的红错 基于拓展Transform对开发并没有什么帮助 我放弃了对Transform的拓展 
// /// </summary>
// //CustomEditor特性表明这是一个自定义的Editor脚本
// [CustomEditor(typeof(Transform))]
// public class TransformInspectorPlus : Editor
// {
//     private Editor _editor;
//     private Transform _theTarget;
//
//
//     private static bool isTransformUse = true;
//
//
//     //是否折叠KooTransformHelp
//     private const string IsFoldoutStateKey = "IsFoldoutStateKey";
//     private static int isFoldoutState = -1;
//
//     internal static bool IsFoldoutState
//     {
//         get
//         {
//             if (isFoldoutState == -1)
//             {
//                 isFoldoutState = EditorPrefs.GetInt(IsFoldoutStateKey, 1);
//             }
//
//             return isFoldoutState == 1;
//         }
//         set
//         {
//             int newValue = value ? 1 : 0;
//             if (newValue != isFoldoutState)
//             {
//                 isFoldoutState = newValue;
//                 EditorPrefs.SetInt(IsFoldoutStateKey, isFoldoutState);
//             }
//         }
//     }
//
//     //是否启用KooTransformHelp
//     private const string IsUseTranformHelpKey = "IsUseTranformHelpKey";
//     private static int isUseTranformHelp = -1;
//
//     internal static bool IsUseTranformHelp
//     {
//         get
//         {
//             if (isUseTranformHelp == -1)
//             {
//                 isUseTranformHelp = EditorPrefs.GetInt(IsUseTranformHelpKey, 1);
//             }
//
//             return isUseTranformHelp == 1;
//         }
//         set
//         {
//             int newValue = value ? 1 : 0;
//             if (newValue != isUseTranformHelp)
//             {
//                 isUseTranformHelp = newValue;
//                 EditorPrefs.SetInt(IsUseTranformHelpKey, isUseTranformHelp);
//             }
//         }
//     }
//
//
//     /// <summary>
//     /// 是否使用统一的缩放比
//     /// </summary>
//     private const string IsUniformScaleKey = "IsUniformScaleKey";
//
//     private static int isUniformScale = -1;
//
//     internal static bool IsUniformScale
//     {
//         get
//         {
//             if (isUniformScale == -1)
//             {
//                 isUniformScale = EditorPrefs.GetInt(IsUniformScaleKey, 1);
//             }
//
//             return isUniformScale == 1;
//         }
//         set
//         {
//             int newValue = value ? 1 : 0;
//             if (newValue != isUniformScale)
//             {
//                 isUniformScale = newValue;
//                 EditorPrefs.SetInt(IsUniformScaleKey, isUniformScale);
//             }
//         }
//     }
//
//     private void Awake()
//     {
//         _theTarget = (Transform)this.target;
//
//
//         IsUniformScale = false;
//
//         _editor = Editor.CreateEditor(target,
//             Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.TransformInspector", false)); 
//     }
//
//     private void OnEnable() { }
//
//     private void OnDestroy()
//     {
//         _editor = null;
//     }
//
//     public override void OnInspectorGUI()
//     {
//         if (IsUseTranformHelp)
//         {
//             // 绘制折叠框
//             IsFoldoutState = EditorGUILayout.Foldout(IsFoldoutState, "KooTranformHelp", toggleOnLabelClick: true);
//             if (IsFoldoutState)
//             {
//                 // 设置按钮的对齐方式
//                 GUI.skin.button.alignment = TextAnchor.MiddleCenter;
//
//                 GUILayout.BeginVertical();
//                 GUILayout.BeginHorizontal();
//                 GUILayout.Space(10);
//
//                 #region 归零归一
//
//                 GUILayout.BeginVertical();
//
//                 if (GUILayout.Button("本地坐标归零"))
//                 {
//                     Undo.RecordObject(_theTarget, "Modify Position");
//                     _theTarget.localPosition = Vector3.zero;
//                 }
//
//                 GUILayout.Space(1);
//
//                 if (GUILayout.Button("本地角度归零"))
//                 {
//                     Undo.RecordObject(_theTarget, "Modify Rotation");
//                     _theTarget.localRotation = Quaternion.identity;
//                 }
//
//                 GUILayout.EndVertical();
//
//                 #endregion
//
//                 GUILayout.Space(20);
//
//                 #region 本地重置
//
//                 GUILayout.BeginVertical();
//
//                 if (GUILayout.Button("复制世界坐标"))
//                 {
//                     CopyToClipboard(_theTarget.transform.position, "坐标");
//                 }
//
//                 GUILayout.Space(1);
//
//                 if (GUILayout.Button("复制世界角度"))
//                 {
//                     CopyToClipboard(_theTarget.transform.eulerAngles, "角度");
//                 }
//
//                 GUILayout.EndVertical();
//
//                 #endregion
//
//                 GUILayout.Space(20);
//
//                 #region 世界重置
//
//                 GUILayout.BeginVertical();
//
//                 if (GUILayout.Button("复制局部坐标"))
//                 {
//                     CopyToClipboard(_theTarget.transform.localPosition, "局部坐标");
//                 }
//
//                 GUILayout.Space(1);
//
//                 if (GUILayout.Button("复制局部角度"))
//                 {
//                     CopyToClipboard(_theTarget.transform.localEulerAngles, "局部角度");
//                 }
//
//                 GUILayout.EndVertical();
//
//                 #endregion
//
//                 GUILayout.Space(10);
//                 GUILayout.EndHorizontal();
//
//                 #region 尺寸设置
//
//                 GUILayout.BeginHorizontal();
//                 GUILayout.Space(10);
//
//                 if (GUILayout.Button("坐标归整"))
//                 {
//                     RoundLocalPosition(_theTarget.transform);
//                 }
//
//                 GUILayout.Space(20);
//
//                 if (GUILayout.Button("尺寸归整"))
//                 {
//                     RoundLocalScale(_theTarget.transform);
//                 }
//
//                 GUILayout.Space(20);
//
//                 if (GUILayout.Button("尺寸正比"))
//                 {
//                     SetUniformScale(_theTarget.transform);
//                 }
//
//                 GUILayout.Space(10);
//                 GUILayout.EndHorizontal();
//
//                 #endregion
//
//                 GUILayout.EndVertical();
//             }
//
//             _editor.OnInspectorGUI();
//         }
//         else
//         {
//             GUI.enabled = isTransformUse;
//             _editor.OnInspectorGUI();
//         }
//     }
//
//     [MenuItem("CONTEXT/Transform/KooTransformHelp/是否禁用Transform编辑")]
//     public static void KooTransformForbid()
//     {
//         isTransformUse = !isTransformUse;
//     }
//
//     [MenuItem("CONTEXT/Transform/KooTransformHelp/开关KooTransformHelp")]
//     public static void KooTransformHelp()
//     {
//         IsUseTranformHelp = !IsUseTranformHelp;
//     }
//
//     // 复制向量到剪贴板，并打印日志
//     void CopyToClipboard(Vector3 vector, string label)
//     {
//         GUIUtility.systemCopyBuffer = vector.ToString();
//         Debug.Log($"KooHelp: 复制{label}: {vector}");
//     }
//
//     // 将本地坐标归整到整数值
//     void RoundLocalPosition(Transform obj)
//     {
//         Vector3 vector = obj.localPosition;
//         vector = new Vector3(Mathf.FloorToInt(vector.x), Mathf.FloorToInt(vector.y), Mathf.FloorToInt(vector.z));
//         obj.localPosition = vector;
//         Debug.Log("KooHelp: 此物体的本地坐标已经规整啦！");
//     }
//
//     // 将本地尺寸归整到整数值
//     void RoundLocalScale(Transform obj)
//     {
//         Vector3 vector = obj.localScale;
//         vector = new Vector3(Mathf.FloorToInt(vector.x), Mathf.FloorToInt(vector.y), Mathf.FloorToInt(vector.z));
//         obj.localScale = vector;
//         Debug.Log("KooHelp: 此物体本地尺寸已经规整啦！");
//     }
//
//     // 将本地尺寸按最小等比设置
//     void SetUniformScale(Transform obj)
//     {
//         Vector3 vector = obj.localScale;
//         float min = Mathf.Min(vector.x, Mathf.Min(vector.y, vector.z));
//         obj.localScale = new Vector3(min, min, min);
//         Debug.Log("KooHelp: 此物体本地尺寸按最小等比啦！");
//     }
//
//     #region 旧版的功能
//
//     //     GUILayout.EndHorizontal();
//     //     Vector3 vector;
//     //     if (useWorldSystem)
//     //     {
//     //         GUILayout.BeginHorizontal();
//     //         {
//     //             vector = EditorGUILayout.Vector3Field("Position:", theTarget.position);
//     //             if (vector != theTarget.position)
//     //             {
//     //                 Undo.RecordObject(theTarget, "Modify Position");
//     //                 theTarget.position = vector;
//     //             }
//     //
//     //             //GUI.color = Color.green;
//     //             if (GUILayout.Button("To 0", "toolbarbutton", GUILayout.MaxWidth(70)))
//     //             {
//     //                 Undo.RecordObject(theTarget, "Modify Position");
//     //                 theTarget.position = Vector3.zero;
//     //             }
//     //             //GUI.color = Color.white;
//     //         }
//     //         GUILayout.EndHorizontal();
//     //
//     //         GUILayout.BeginHorizontal();
//     //         {
//     //             vector = EditorGUILayout.Vector3Field("EulerAngles:", theTarget.eulerAngles);
//     //             if (vector != theTarget.eulerAngles)
//     //             {
//     //                 Undo.RecordObject(target, "Modify Rotation");
//     //                 theTarget.eulerAngles = vector;
//     //             }
//     //
//     //             //GUI.color = Color.gray;
//     //             if (GUILayout.Button("To 0", "toolbarbutton", GUILayout.MaxWidth(70)))
//     //             {
//     //                 Undo.RecordObject(theTarget, "Modify Rotation");
//     //                 theTarget.rotation = Quaternion.identity;
//     //             }
//     //             //GUI.color = Color.white;
//     //         }
//     //         GUILayout.EndHorizontal();
//     //     }
//     //     else
//     //     {
//     //         GUILayout.BeginHorizontal();
//     //         {
//     //             vector = EditorGUILayout.Vector3Field("Local Position:", theTarget.localPosition);
//     //             if (vector != theTarget.localPosition)
//     //             {
//     //                 Undo.RecordObject(target, "Modify Position");
//     //                 theTarget.localPosition = vector;
//     //             }
//     //
//     //             //GUI.color = Color.green;
//     //             if (GUILayout.Button("To 0", "toolbarbutton", GUILayout.MaxWidth(70)))
//     //             {
//     //                 Undo.RecordObject(theTarget, "Modify Position");
//     //                 theTarget.localPosition = Vector3.zero;
//     //             }
//     //             //GUI.color = Color.white;
//     //         }
//     //         GUILayout.EndHorizontal();
//     //
//     //         GUILayout.BeginHorizontal();
//     //         {
//     //             vector = EditorGUILayout.Vector3Field("Local EulerAngles:", theTarget.localEulerAngles);
//     //             if (vector != theTarget.localEulerAngles)
//     //             {
//     //                 Undo.RecordObject(theTarget, "Modify Rotation");
//     //                 theTarget.localEulerAngles = vector;
//     //             }
//     //
//     //             //GUI.color = Color.green;
//     //             if (GUILayout.Button("To 0", "toolbarbutton", GUILayout.MaxWidth(70)))
//     //             {
//     //                 Undo.RecordObject(theTarget, "Modify Rotation");
//     //                 theTarget.localRotation = Quaternion.identity;
//     //             }
//     //             //GUI.color = Color.white;
//     //         }
//     //         GUILayout.EndHorizontal();
//     //     }
//     //
//     //     GUILayout.BeginHorizontal();
//     //     {
//     //         IsUniformScale =
//     //             GUILayout.Toggle(IsUniformScale, "\u224c", "toolbarbutton", GUILayout.MaxWidth(20));
//     //         if (IsUniformScale)
//     //         {
//     //             float v = EditorGUILayout.FloatField("Scale:", theTarget.localScale.x);
//     //             if (theTarget.localScale.x != v)
//     //             {
//     //                 Undo.RecordObject(theTarget, "ScaleIsUniform");
//     //                 theTarget.localScale = new Vector3(v, v, v);
//     //             }
//     //         }
//     //         else
//     //         {
//     //             Vector3 tmporary = EditorGUILayout.Vector3Field("Scale:", theTarget.localScale);
//     //             if (tmporary != theTarget.localScale)
//     //             {
//     //                 Undo.RecordObject(theTarget, "ScaleIsUniform");
//     //                 theTarget.localScale = tmporary;
//     //             }
//     //         }
//     //
//     //         //GUI.color = Color.green;
//     //         if (GUILayout.Button("To 1", "toolbarbutton", GUILayout.MaxWidth(35)))
//     //         {
//     //             Undo.RecordObject(theTarget, "scaleIsUniform to one");
//     //             theTarget.localScale = Vector3.one;
//     //         }
//     //
//     //         if (GUILayout.Button("To 0", "toolbarbutton", GUILayout.MaxWidth(35)))
//     //         {
//     //             Undo.RecordObject(theTarget, "scaleIsUniform to zero");
//     //             theTarget.localScale = Vector3.zero;
//     //         }
//     //         //GUI.color = Color.white;
//     //     }
//     //     GUILayout.EndHorizontal();
//     //
//     //     GUILayout.Space(5);
//     // }
//     // GUILayout.EndVertical();
//
//     #endregion
// }