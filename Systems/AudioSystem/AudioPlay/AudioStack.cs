//****************** 代码文件申明 ************************
//* 文件：AudioStack                                       
//* 作者：wheat
//* 创建时间：2023/09/29 18:54:33 星期五
//* 描述：用于存储音效信息
//*****************************************************
using KFrame.Attributes;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace KFrame.Systems
{
    [System.Serializable]
    public class AudioStack
    {
        #region 参数（不要在游戏运行的时候更改）
        
        [SerializeField, LabelText("id"), KLabelText("id")] public int AudioIndex;
        [SerializeField, LabelText("名称"), KLabelText("名称")] public string AudioName;
        [SerializeField, LabelText("Clip"), KLabelText("Clip")] public List<int> Clips;
        [SerializeField, LabelText("分组"), KLabelText("分组")] public int AudioGroupIndex;
        [SerializeField, LabelText("音量"), KLabelText("音量"), Range(0, 1f)] public float Volume;
        [SerializeField, LabelText("音高"), KLabelText("音高"), Range(-3, 3f)] public float Pitch;
        [SerializeField, LabelText("随机最低音高"), KLabelText("随机最低音高"), Range(-3, 3f)] public float RandomMinPitch;
        [SerializeField, LabelText("随机最高音高"), KLabelText("随机最高音高"), Range(-3, 3f)] public float RandomMaxPitch;
        [SerializeField, LabelText("最高音高"), KLabelText("最高音高"), Range(-3, 3f)] public float MaxPitch;
        [SerializeField, LabelText("限制播放"), KLabelText("限制播放")] public bool LimitPlay;
        [SerializeField, LabelText("限制播放数量"), KLabelText("限制播放数量")] public int LimitPlayCount;
        [SerializeField, LabelText("循环播放"), KLabelText("循环播放")] public bool Loop;
        [SerializeField, LabelText("3D音效"), KLabelText("3D音效")] public bool Is3D;
        [SerializeField, LabelText("音效类型"), KLabelText("音效类型")] public AudioesType AudioesType;

        #endregion

        #region 其他

        /// <summary>
        /// 随机播放的时候用到的，播放一个少一个
        /// </summary>
        [NonSerialized]
        public List<int> PlayIndexs;

        #endregion

        #region 构造函数
        public AudioStack()
        {
            Clips = null;
            AudioGroupIndex = 0;
            Volume = 1f;
            Pitch = 1f;
            LimitPlayCount = AudioConfigs.DefaultMaxSameAudioPlayCount;
            RandomMaxPitch = 1f;
            RandomMinPitch = 1f;
            MaxPitch = 1f;
            Loop = false;
            LimitPlay = false;
            Is3D = false;
            AudioesType = AudioesType.None;
        }
        public AudioStack(int audioIndex, string audioName, List<int> clips, int audioGroupIndex, float volume, float pitch,
            float randomMinPitch, float randomMaxPitch, float maxPitch, bool limitPlay, int limitCount,bool loop, bool is3D, AudioesType audioesType)
        {
            AudioIndex = audioIndex;
            AudioName = audioName;
            Clips = clips;
            AudioGroupIndex = audioGroupIndex;
            Volume = volume;
            Pitch = pitch;
            RandomMinPitch = randomMinPitch;
            RandomMaxPitch = randomMaxPitch;
            MaxPitch = maxPitch;
            LimitPlay = limitPlay;
            LimitPlayCount = limitCount;
            Loop = loop;
            Is3D = is3D;
            AudioesType = audioesType;
        }

        #endregion

        #region 音效播放相关方法

        /// <summary>
        /// 获取一个AudioClip
        /// </summary>
        /// <param name="i">Clips列表中的下标</param>
        /// <returns></returns>
        public AudioClip GetAudioClip(int i)
        {
            //如果越界直接返回null
            if (i < 0 || i >= Clips.Count) return null;
            
            return AudioDic.GetAudioClip(Clips[i]);
        }
        /// <summary>
        /// 随机获取一个Clips列表中的AudioClip
        /// </summary>
        /// <returns></returns>
        public AudioClip GetRandomAudio()
        {
            //如果音源就一个
            if (Clips.Count == 1)
            {
                //那么直接选取
                return GetAudioClip(0);
            }
            //如果音源有多个
            else
            {
                //那会随机播放，并且保证每个都会播放一次

                //如果播放id空了，那就生成一下
                if (PlayIndexs == null || PlayIndexs.Count == 0)
                {
                    if (PlayIndexs == null)
                        PlayIndexs = new List<int>();

                    for (int i = 0; i < Clips.Count; i++)
                    {
                        PlayIndexs.Add(i);
                    }
                }

                //随机选取一个播放
                int k = UnityEngine.Random.Range(0, PlayIndexs.Count);
                int id = PlayIndexs[k];
                //然后把它从未播放的列表中去除
                PlayIndexs.RemoveAt(k);

                //选取作为播放的音源
                return GetAudioClip(id);
            }
        }

        #endregion

    }
}

