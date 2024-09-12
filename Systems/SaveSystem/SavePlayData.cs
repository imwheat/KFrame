//************************* 代码文件申明 *********************
//* 文件：SavePlayData                                       
//* 作者：wheat
//* 创建时间：2024/02/23 18:55:06 星期五
//* 描述：存档游玩数据，包含玩家所在地区、玩家名等基础信息
//************************************************************

using UnityEngine;
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace KFrame.Systems
{
    [System.Serializable]
    public class SavePlayData
    {
        [LabelText("玩家名称"), TabGroup("基础")]
        public string PlayerName;

        [LabelText("游玩时长"), TabGroup("基础")]
        public float PlayTime;

    }
}