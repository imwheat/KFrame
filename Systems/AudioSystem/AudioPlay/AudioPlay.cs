//****************** 代码文件申明 ************************
//* 文件：AudioPlay                                       
//* 作者：wheat
//* 创建时间：2023/09/29 18:39:23 星期五
//* 描述：音效播放
//*****************************************************
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

namespace KFrame.Systems
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioPlay : MonoBehaviour
    {
        /// <summary>
        /// 正在播放的音效
        /// </summary>
        public int PlayingAudioIndex;
        /// <summary>
        /// AudioSource
        /// </summary>
        public AudioSource AudioSource;
        /// <summary>
        /// 绑定的音效分组
        /// </summary>
        public AudioGroup LinkedGroup;
        /// <summary>
        /// 消失的时候的回调
        /// </summary>
        private event Action onVanish;
        /// <summary>
        /// 注销事件
        /// </summary>
        private event Action<AudioPlay> onVanishUnRegisterAudioPlay;

        private void Awake()
        {
            InitAudio();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        private void InitAudio()
        {
            //获取AudioSource
            if (AudioSource == null)
            {
                AudioSource = GetComponent<AudioSource>();
            }

            //初始化属性
            AudioSource.playOnAwake = false;
            AudioSource.loop = false;
            AudioSource.clip = null;
        }
        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="audioStack">音效的Stack</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="modifyData">额外更改数据</param>
        public void PlaySound(AudioStack audioStack, Action callBack = null, AudioModifyData modifyData = null, Action<AudioPlay> unregister = null)
        {
            //如果什么都没有就return
            if (audioStack == null || audioStack.Clips == null || audioStack.Clips.Count == 0) return;

            float pitch = 1f;

            if (!Mathf.Approximately(audioStack.RandomMaxPitch, audioStack.RandomMinPitch))
            {
                pitch = UnityEngine.Random.Range(audioStack.RandomMinPitch, audioStack.RandomMaxPitch);
            }

            //获取音量
            float volume = audioStack.Volume;
            //如果有分组信息，那就再根据分组的音量调节音量
            LinkedGroup = AudioDic.GetAudioGroup(audioStack.AudioGroupIndex);
            if (LinkedGroup != null)
            {
                volume *= LinkedGroup.CurVolume;
                LinkedGroup.UpdateVolumeAction += UpdateVolume;
            }

            AudioClip clip = audioStack.GetRandomAudio();
            PlayingAudioIndex = audioStack.AudioIndex;
            
            //如果有额外更改就应用更改
            if (modifyData != null)
            {
                AudioModifyData data = modifyData;
                if (!Mathf.Approximately(data.Pitch, float.MinValue))
                {
                    pitch = data.Pitch;
                }
                if (!Mathf.Approximately(data.Volume, float.MinValue))
                {
                    volume = data.Volume;
                }
            }

            pitch = Mathf.Clamp(pitch, -3f, audioStack.MaxPitch);

            //播放音效
            PlaySound(clip, AudioSystem.GetAudioGroup(audioStack.AudioGroupIndex), volume, pitch, audioStack.Loop, callBack, unregister);
        }
        /// <summary>
        /// 播放声音
        /// (不推荐直接使用这个)
        /// </summary>
        /// <param name="audioClip">声音的Clip</param>
        /// <param name="group">输出的Group</param>
        /// <param name="pitch">音高</param>
        /// <param name="loop">循环播放</param>
        /// <param name="callBack">回调函数</param>
        public void PlaySound(AudioClip audioClip, AudioMixerGroup group, float volume = 1f, float pitch = 1f, bool loop = false,
            Action callBack = null, Action<AudioPlay> unregister = null)
        {
            //防空初始化
            if (AudioSource == null) InitAudio();
            
            //赋值
            onVanish += callBack;
            onVanishUnRegisterAudioPlay += unregister;
            AudioSource.outputAudioMixerGroup = group;
            AudioSource.clip = audioClip;
            AudioSource.pitch = pitch;
            AudioSource.volume = volume;
            AudioSource.loop = loop;

            //如果AudioSource被关了，或者gameobject被关了就不播放了
            if (AudioSource.enabled == false || gameObject.activeSelf == false)
            {
                Vanish();
            }
            //一切正常才播放
            else
            {
                AudioSource.Play();
            }
        }

        private void Update()
        {
            if (!AudioSource.isPlaying && AudioSource.clip != null)
            {
                //如果是循环播放的话那就重新开始播放
                if (AudioSource.loop)
                {
                    AudioSource.Play();
                }
                //不是就消失
                else
                {
                    Vanish();
                }
            }
        }
        /// <summary>
        /// 更新音量
        /// </summary>
        private void UpdateVolume(float volume)
        {
            AudioSource.volume = volume;
        }
        /// <summary>
        /// 卸载场景事件 
        /// </summary>
        private void OnUnloadSceneEvent()
        {
            //卸载场景的时候还在播放的音效要消失
            Vanish();
        }
        /// <summary>
        /// 消失
        /// </summary>
        public void Vanish()
        {
            //如果有事件的话调用
            onVanish?.Invoke();
            onVanishUnRegisterAudioPlay?.Invoke(this);
            PlayingAudioIndex = 0;
            //清空事件
            onVanish = null;
            onVanishUnRegisterAudioPlay = null;
            if(LinkedGroup != null)
            {
                LinkedGroup.UpdateVolumeAction -= UpdateVolume;
            }
            //清空clip
            AudioSource.clip = null;
            //恢复默认音高
            AudioSource.pitch = 1f;
            //推入对象池
            if (gameObject.activeSelf)
            {
                PoolSystem.PushGameObject(gameObject);
            }

        }

    }
}

