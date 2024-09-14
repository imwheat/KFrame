//****************** 代码文件申明 ***********************
//* 文件：BGMEditData
//* 作者：wheat
//* 创建时间：2024/09/13 13:03:56 星期五
//* 描述：BGM的编辑数据
//*******************************************************

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace KFrame.Systems
{
    public class BGMEditData
    {
        #region 参数
        /// <summary>
        /// BGMid
        /// </summary>
        public int BGMIndex;
        /// <summary>
        /// BGM名称
        /// </summary>
        public string BGMName;
        /// <summary>
        /// BGM音量
        /// </summary>
        public float Volume;
        /// <summary>
        /// 循环播放
        /// </summary>
        public bool Loop;
        /// <summary>
        /// Clip的id
        /// </summary>
        public int ClipIndex;
        /// <summary>
        /// Clip
        /// </summary>
        [NonSerialized]
        public AudioClip Clip;

        #endregion

        public BGMEditData()
        {
            Volume = 1f;
            Loop = false;
            ClipIndex = -1;
            Clip = null;
        }

        public BGMEditData(BGMStack stack)
        {
            BGMIndex = stack.BGMIndex;
            BGMName = stack.BGMName;
            Volume = stack.Volume;
            Loop = stack.Loop;
            ClipIndex = stack.ClipIndex;
            Clip = AudioDic.GetBGMClip(ClipIndex);
        }
        /// <summary>
        /// 把EditData里面的数据粘贴给Stack
        /// </summary>
        public void PasteData(BGMStack stack)
        {
            //防空
            if(stack == null) return;
            
            stack.BGMIndex = BGMIndex;
            stack.BGMName = BGMName;
            stack.Volume = Volume;
            stack.Loop = Loop;
            stack.ClipIndex = ClipIndex;
        }
    }
}

