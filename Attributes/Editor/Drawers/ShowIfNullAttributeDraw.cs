// #if UNITY_EDITOR
// using Sirenix.OdinInspector.Editor;
// using UnityEngine;
//
// /// <summary>
// /// 自定义绘制器类，继承自Odin的OdinAttributeDrawer，用于处理ShowIfNullAttribute特性
// /// </summary>
// [DrawerPriority(DrawerPriorityLevel.WrapperPriority)] // 设置绘制器的优先级，这影响到它在Inspector中的呈现顺序
// public class ShowIfNullAttributeDrawer : OdinAttributeDrawer<ShowIfNullAttribute>
// {
//     /// <summary>
//     /// 重写基类方法，定义特性的绘制逻辑
//     /// </summary>
//     /// <param name="label"></param>
//     protected override void DrawPropertyLayout(GUIContent label)
//     {
//         if (this.Property.ValueEntry.WeakSmartValue == null) // 检查字段是否为空
//         {
//             this.CallNextDrawer(label); // 如果字段为空，调用下一个绘制器来显示字段
//         }
//     }
// }
// #endif