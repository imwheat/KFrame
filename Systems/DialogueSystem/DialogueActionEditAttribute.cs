//****************** 代码文件申明 ***********************
//* 文件：DialogueActionEditAttribute
//* 作者：wheat
//* 创建时间：2024/10/21 15:27:34 星期一
//* 描述：修改对话树事件绘制的Attribute
//*******************************************************

using System.Diagnostics;
using UnityEngine;

namespace KFrame.Systems.DialogueSystem
{
    [Conditional("UNITY_EDITOR")]
    public class DialogueActionEditAttribute : PropertyAttribute
    {
    }
}

