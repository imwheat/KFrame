//****************** 代码文件申明 ************************
//* 文件：BGMPlay                                       
//* 作者：wheat
//* 创建时间：2024/01/21 14:30:38 星期日
//* 描述：播放BGM用的
//*****************************************************
using UnityEngine;
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using KFrame;
using UnityEngine.Audio;

namespace KFrame.Systems
{
    public class BGMPlay : MonoBehaviour
    {
        [LabelText("播放器")] public AudioSource MyAudioSource;
        [LabelText("当前在播放的BGMStack")] public BGMStack CurStack;
        [LabelText("循环播放")] public bool Loop;
        /// <summary>
        /// 调节音量
        /// 在淡入淡出的时候会用到
        /// </summary>
        [LabelText("调节音量")] public float ModifyVolume;
        /// <summary>
        /// 音量
        /// </summary>
        [LabelText("调节音量")] public float Volume;
        private void Awake()
        {
            //防空
            if (MyAudioSource == null)
            {
                MyAudioSource = GetComponent<AudioSource>();

                if(MyAudioSource == null)
                {
                    MyAudioSource = gameObject.AddComponent<AudioSource>();
                    MyAudioSource.loop = false;
                    MyAudioSource.playOnAwake = false;
                }
            }
        }
        /// <summary>
        /// 播放BGM
        /// </summary>
        public void PlayBGM(BGMStack bgmClip, AudioMixerGroup group)
        {
            //防空
            if (bgmClip==null) return;

            //设置BGMPlay参数
            CurStack = bgmClip;
            Loop = bgmClip.Loop;
            ModifyVolume = 1f;
            Volume = bgmClip.Volume * AudioSystem.BGMGroup.CurVolume;
            AudioSystem.BGMGroup.UpdateVolumeAction += UpdateVolume;

            //播放
            MyAudioSource.clip = bgmClip.GetClip();
            //设置AudioSource参数
            MyAudioSource.outputAudioMixerGroup = group;
            MyAudioSource.volume = Volume;
            MyAudioSource.loop = Loop;
            MyAudioSource.Play();

        }
        /// <summary>
        /// 更新音量
        /// </summary>
        public void UpdateVolume(float v)
        {
            Volume = v;
            MyAudioSource.volume = Volume * ModifyVolume;
        }
        /// <summary>
        /// 更新调节音量
        /// </summary>
        public void UpdateModifyVolume(float v)
        {
            ModifyVolume = v;
            MyAudioSource.volume = Volume * ModifyVolume;
        }
        /// <summary>
        /// 结束播放然后回到对象池
        /// </summary>
        public void EndPlay()
        {
            MyAudioSource.Stop();
            MyAudioSource.clip = null;
            Volume = 0f;
            AudioSystem.BGMGroup.UpdateVolumeAction -= UpdateVolume;

            PoolSystem.PushGameObject(gameObject);
        }
    }
}

