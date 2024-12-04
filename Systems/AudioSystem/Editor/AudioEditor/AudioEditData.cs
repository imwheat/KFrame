//****************** 代码文件申明 ************************
//* 文件：AudioEditData                                       
//* 作者：wheat
//* 创建时间：2024/09/02 10:09:06 星期一
//* 描述：音效文件的编辑数据
//*****************************************************
using UnityEngine;
using KFrame.Systems;
using System;
using System.Collections.Generic;
using KFrame;
using Sirenix.OdinInspector;
using UnityEngine.Audio;
using UnityEngine.Serialization;

#if UNITY_EDITOR

namespace KFrame.Systems
{
    [System.Serializable]
    public class AudioEditData
    {
        /// <summary>
        /// 创建一个默认参数的EditData
        /// </summary>
        /// <returns></returns>
        public static AudioEditData Default()
        {
            return new AudioEditData()
            {
                ClipIndexes = new List<int>(),
                Clips = new List<AudioClip>(),
                AudioGroupIndex = 0,
                Volume = 1f,
                Pitch = 1f,
                RandomMinPitch = 1f,
                RandomMaxPitch = 1f,
                MaxPitch = 1f,
                LimitContinuousPlay = true,
                LimitPlayCount = 2,
                Loop = false,
                Is3D = true,
                AudioType = AudioesType.None,
            };
        }

        #region 参数
        
        /// <summary>
        /// 音效id
        /// </summary>
        [SerializeField, LabelText("音效id")] 
        public int AudioIndex;
        /// <summary>
        /// 名称
        /// </summary>
        [field: SerializeField, LabelText("名称")] 
        public string AudioName;
        /// <summary>
        /// Clip下标
        /// </summary>
        [SerializeField, LabelText("Clip下标")]
        public List<int> ClipIndexes;
        /// <summary>
        /// Clip
        /// </summary>
        [NonSerialized, LabelText("Clip")]
        public List<AudioClip> Clips;
        /// <summary>
        /// 分组
        /// </summary>
        [SerializeField, LabelText("分组")]
        public AudioMixerGroup AudioMixerGroup;
        /// <summary>
        /// 分组id
        /// </summary>
        [field: SerializeField, LabelText("分组id")]
        public int AudioGroupIndex;
        /// <summary>
        /// 音量
        /// </summary>
        [field: SerializeField, LabelText("音量"), Range(0, 1f)] 
        public float Volume;
        /// <summary>
        /// 音高
        /// </summary>
        [field: SerializeField, LabelText("音高"), Range(-3, 3f)]
        public float Pitch;
        /// <summary>
        /// 随机最低音高
        /// </summary>
        [field: SerializeField, LabelText("随机最低音高"), Range(-3, 3f)]
        public float RandomMinPitch;
        /// <summary>
        /// 随机最高音高
        /// </summary>
        [field: SerializeField, LabelText("随机最高音高"), Range(-3, 3f)] 
        public float RandomMaxPitch;
        /// <summary>
        /// 最高音高
        /// </summary>
        [field: SerializeField, LabelText("最高音高"), Range(-3, 3f)] 
        public float MaxPitch;
        /// <summary>
        /// 限制连续播放
        /// </summary>
        [field: SerializeField, LabelText("限制连续播放")] 
        public bool LimitContinuousPlay;
        /// <summary>
        /// 限制连续播放数量
        /// </summary>
        [field: SerializeField, LabelText("限制连续播放数量")]
        public int LimitPlayCount;
        /// <summary>
        /// 循环播放
        /// </summary>
        [field: SerializeField, LabelText("循环播放")] 
        public bool Loop;
        /// <summary>
        /// 3D音效
        /// </summary>
        [field: SerializeField, LabelText("3D音效")]
        public bool Is3D;
        /// <summary>
        /// 音效类型
        /// </summary>
        [field: SerializeField, LabelText("音效类型")] 
        public AudioesType AudioType;

        #endregion

        private AudioEditData()
        {
            
        }
        /// <summary>
        /// 复制一个新的数据
        /// </summary>
        /// <param name="copyData">要复制的数据</param>
        public AudioEditData(AudioEditData copyData)
        {
            ClipIndexes = new List<int>();
            Clips = new List<AudioClip>();
            CopyData(copyData);
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="audioStack">复制的AudioStack</param>
        public AudioEditData(AudioStack audioStack)
        {
            AudioName = audioStack.AudioName;
            ClipIndexes = new List<int>(audioStack.Clips);
            Clips = new List<AudioClip>();
            AudioDic.FindAudioClipsByIndexes(ClipIndexes, Clips);
            AudioGroupIndex = audioStack.AudioGroupIndex;
            Volume = audioStack.Volume;
            Pitch = audioStack.Pitch;
            RandomMinPitch = audioStack.RandomMinPitch;
            RandomMaxPitch = audioStack.RandomMaxPitch;
            MaxPitch = audioStack.MaxPitch;
            LimitContinuousPlay = audioStack.LimitPlay;
            LimitPlayCount = audioStack.LimitPlayCount;
            Loop = audioStack.Loop;
            Is3D = audioStack.Is3D;
            AudioType = audioStack.AudioesType;
        }
        /// <summary>
        /// 加载模版
        /// </summary>
        /// <param name="template">复制的模版数据</param>
        public void LoadTemplate(AudioEditData template)
        {
            AudioGroupIndex = template.AudioGroupIndex;
            AudioMixerGroup = AudioDic.GetAudioMixerGroup(AudioGroupIndex);
            Volume = template.Volume;
            Pitch = template.Pitch;
            RandomMinPitch = template.RandomMinPitch;
            RandomMaxPitch = template.RandomMaxPitch;
            MaxPitch = template.MaxPitch;
            LimitContinuousPlay = template.LimitContinuousPlay;
            LimitPlayCount = template.LimitPlayCount;
            Loop = template.Loop;
            Is3D = template.Is3D;
            AudioType = template.AudioType;
        }
        /// <summary>
        /// 复制数据
        /// </summary>
        /// <param name="copyData">复制的数据</param>
        public void CopyData(AudioEditData copyData)
        {
            AudioIndex = copyData.AudioIndex;
            AudioName = copyData.AudioName;
            ClipIndexes = new List<int>(copyData.ClipIndexes);
            Clips = new List<AudioClip>(copyData.Clips);
            CopyParams(copyData);
        }
        /// <summary>
        /// 复制参数
        /// </summary>
        /// <param name="copyData">复制的数据</param>
        public void CopyParams(AudioEditData copyData)
        {
            AudioGroupIndex = copyData.AudioGroupIndex;
            AudioMixerGroup = copyData.AudioMixerGroup;
            Volume = copyData.Volume;
            Pitch = copyData.Pitch;
            RandomMinPitch = copyData.RandomMinPitch;
            RandomMaxPitch = copyData.RandomMaxPitch;
            MaxPitch = copyData.MaxPitch;
            LimitContinuousPlay = copyData.LimitContinuousPlay;
            LimitPlayCount = copyData.LimitPlayCount;
            Loop = copyData.Loop;
            Is3D = copyData.Is3D;
            AudioType = copyData.AudioType;
        }
        /// <summary>
        /// 把数据粘贴给AudioStack
        /// </summary>
        /// <param name="audioStack">要拷贝数据的Stack</param>
        public void PasteData(AudioStack audioStack)
        {
            //如果为空就返回
            if(audioStack == null) return;

            audioStack.AudioName = AudioName;
            audioStack.AudioesType = AudioType;
            audioStack.AudioGroupIndex = AudioGroupIndex;
            audioStack.Clips = new List<int>(ClipIndexes);
            audioStack.Volume = Volume;
            audioStack.Pitch = Pitch;
            audioStack.MaxPitch = MaxPitch;
            audioStack.RandomMaxPitch = RandomMaxPitch;
            audioStack.RandomMinPitch = RandomMinPitch;
            audioStack.LimitPlay = LimitContinuousPlay;
            audioStack.LimitPlayCount = LimitPlayCount;
            audioStack.Loop = Loop;
            audioStack.Is3D = Is3D;

        }
    }
}

#endif
