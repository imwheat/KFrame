using UnityEditor;
using UnityEngine;

public class ExMyLayout
{
    // [InitializeOnLoadMethod]  //添加此特性表示此方法会在每次编译完成后首先调用
    // static void InitializeOnLoadMethod()
    // {
    //     EditorApplication.hierarchyWindowItemOnGUI = delegate (int instanceID, Rect selectionRect)
    //     {
    //         //在Hierarchy视图中选择一个资源
    //         if (Selection.activeObject && instanceID == Selection.activeObject.GetInstanceID())
    //         {
    //             float width = 20f;
    //             float height = 13f;
    //             selectionRect.x += (selectionRect.width - width) - 15f;
    //             selectionRect.y += 1.5f;
    //             selectionRect.width = width;
    //             selectionRect.height = height;
    //             //点击事件
    //             if (/*GUI.Button(selectionRect,AssetDatabase.LoadAssetAtPath<Texture>("Assets/unity.png"))*/
    //             GUI.Button(selectionRect, "✘"))
    //             {
    //                 if (Selection.activeObject.hideFlags != HideFlags.NotEditable)
    //                 {
    //                     if (Selection.activeObject != null)
    //                     {
    //                         Selection.activeObject.hideFlags = HideFlags.NotEditable;
    //                         Debug.LogFormat("KooHelp:{0}", Selection.activeObject.name + "这个物体被锁定啦！");
    //                     }
    //                 }
    //                 else if (Selection.activeObject.hideFlags == HideFlags.NotEditable)
    //                 {
    //                     if (Selection.activeObject != null)
    //                     {
    //                         Selection.activeObject.hideFlags = HideFlags.None;
    //                         Debug.LogFormat("KooHelp:{0}", Selection.activeObject.name + "这个物体解锁啦！");
    //                     }
    //                 }
    //             }
    //         }
    //     };
    // }
}
