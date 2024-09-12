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
        [LabelText("BGMId")] public int BGMIndex;
        [LabelText("音轨id")] public int SoundTrackIndex;
        [LabelText("循环播放")] public bool Loop;
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
        public void PlayBGM(BGMClipStack bgmClip, AudioMixerGroup group)
        {
            //防空
            if (bgmClip==null||bgmClip.Clips.Count == 0) return;

            //设置BGMPlay参数
            BGMIndex = bgmClip.BGMIndex;
            SoundTrackIndex = bgmClip.SoundTrackIndex;
            Loop = bgmClip.Loop;


            //获取要播放的BGM的Clip
            AudioClip clip;

            //如果只有一个
            if (bgmClip.Clips.Count == 1)
            {
                //那就直接获取
                clip = bgmClip.Clips[0];
            }
            //有多个就随机获取
            else
            {
                //防空
                if (bgmClip.playIndexs == null) 
                    bgmClip.playIndexs = new List<int>();

                //如果随机播放里边里面的id没了，那就重新生成
                if(bgmClip.playIndexs.Count==0)
                {
                    //添加打乱顺序的id
                    for (int i = 0; i < bgmClip.Clips.Count; i++)
                    {
                        bgmClip.playIndexs.Insert(UnityEngine.Random.Range(0, bgmClip.playIndexs.Count), i);
                    }
                }

                //随机抽取一个id
                int k = UnityEngine.Random.Range(0, bgmClip.playIndexs.Count);
                clip = bgmClip.Clips[bgmClip.playIndexs[k]];
                //然后把抽到的去掉
                bgmClip.playIndexs.RemoveAt(k);
            }

            //播放
            MyAudioSource.clip = clip;
            //设置AudioSource参数
            MyAudioSource.outputAudioMixerGroup = group;
            MyAudioSource.volume = bgmClip.Volume;
            MyAudioSource.loop = Loop;
            MyAudioSource.Play();

        }
        /// <summary>
        /// 结束播放然后回到对象池
        /// </summary>
        public void EndPlay()
        {
            MyAudioSource.Stop();
            MyAudioSource.clip = null;
            PoolSystem.PushGameObject(gameObject);
        }
    }
}

