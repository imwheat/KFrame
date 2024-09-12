//****************** 代码文件申明 ************************
//* 文件：BGMClipStack                                       
//* 作者：wheat
//* 创建时间：2024/01/20 10:38:05 星期六
//* 描述：存储BGM的音源的信息
//*****************************************************
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine.Audio;

namespace KFrame.Systems
{
    [System.Serializable]
    public class BGMClipStack
    {
        [field: SerializeField, LabelText("id")] public int BGMIndex { get; private set; }
        [field: SerializeField, LabelText("名称")] public string AudioName { get; private set; }
        [field: SerializeField, LabelText("音轨id")] public int SoundTrackIndex { get; private set; }
        [field: SerializeField, LabelText("Clip")] public List<AudioClip> Clips { get; private set; }
        [field: SerializeField, LabelText("音量"), Range(0, 1f)] public float Volume { get; private set; }
        [field: SerializeField, LabelText("循环播放")] public bool Loop { get; private set; }

        #region 属性

        public List<int> playIndexs;

        #endregion

        public BGMClipStack(int audioIndex, string audioName, int audioTrackIndex, List<AudioClip> clips, 
            float volume, bool loop)
        {
            BGMIndex = audioIndex;
            AudioName = audioName;
            SoundTrackIndex = audioTrackIndex;
            Clips = clips;
            Volume = volume;
            Loop = loop;
        }
    }
}

