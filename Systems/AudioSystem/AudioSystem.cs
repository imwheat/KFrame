using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace KFrame.Systems
{
    public static class AudioSystem
    {
        private static AudioModule audioModule;
        private static List<AudioMixerGroup> m_AudioMixerGroupList;
        private static List<AudioGroup> m_AudioGroupList;

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            InitAudio();
            audioModule = FrameRoot.RootTransform.GetComponentInChildren<AudioModule>();
            audioModule.Init();
        }

        /// <summary>
        /// 初始化音效配置
        /// </summary>
        private static void InitAudio()
        {
            //初始化Audio字典
            AudioDic.Init();

            //然后初始化Group
            m_AudioGroupList = AudioDic.GetAllAudioGroup();
            foreach (var group in m_AudioGroupList)
            {
                //更新获取Children
                group.Children = new List<AudioGroup>();
                //遍历获取
                foreach (var index in group.ChildrenIndexes)
                {
                    group.AddChild(index);
                }
                
                //设置默认音量
                group.InitVolume(1f);
            }
            
            //初始化MixerGroup
            m_AudioMixerGroupList = new List<AudioMixerGroup>(AudioLibrary.Instance.AudioMixerGroups);
        }

        public static float MasterVolume
        {
            get => audioModule.MasterlVolume;
            set { audioModule.MasterlVolume = value; }
        }

        public static float BGMVolume
        {
            get => audioModule.BGMVolume;
            set { audioModule.BGMVolume = value; }
        }

        public static float SFXVolume
        {
            get => audioModule.SFXVolume;
            set { audioModule.SFXVolume = value; }
        }
        public static float UIVolume
        {
            get => audioModule.UIVolume;
            set { audioModule.UIVolume = value; }
        }
        public static AudioGroup MasterGroup => audioModule.MasterGroup;
        public static AudioGroup BGMGroup => audioModule.BGMGroup;

        #region 静态音轨字段

        /// <summary>
        /// 平静状态音轨
        /// </summary>
        public static readonly int PeaceTrack = 2;

        /// <summary>
        /// 危险地区音轨
        /// </summary>
        public static readonly int DangerTrack = 3;

        /// <summary>
        /// 战斗状态音轨
        /// </summary>
        public static readonly int TTKTrack = 4;

        /// <summary>
        /// 战斗残血状态音轨
        /// </summary>
        public static readonly int TTK2Track = 5;

        /// <summary>
        /// 狂怒状态音轨
        /// </summary>
        public static readonly int FuryTrack = 6;

        #endregion

        /// <summary>
        /// 获取Audio分组
        /// </summary>
        /// <param name="i">下标</param>
        /// <returns>如果越界了就返回null</returns>
        public static AudioMixerGroup GetAudioGroup(int i)
        {
            //如果越界了就返回null
            if (m_AudioMixerGroupList == null) return null;
            if(i<0 || i >=  m_AudioMixerGroupList.Count) return null;

            return m_AudioMixerGroupList[i];
        }

        /// <summary>
        /// 播放BGM
        /// </summary>
        /// <param name="bgmStack">要播放的BGM的包</param>
        public static void PlayBGM(BGMStack bgmStack)
            => audioModule.PlayBGM(bgmStack);

        /// <summary>
        /// 播放BGM
        /// </summary>
        /// <param name="bgmIndex">要播放的BGM的id</param>
        public static void PlayBGM(int bgmIndex)
            => audioModule.PlayBGM(bgmIndex);

        /// <summary>
        /// 停止播放BGM
        /// </summary>
        /// <param name="immediate">立即停止</param>
        public static void StopBGM(bool immediate)
            => audioModule.StopBGM(immediate);

        /// <summary>
        /// 播放一次特效音乐
        /// </summary>
        /// <param name="audioStack">音效的Stack</param>
        /// <param name="pos">位置</param>
        /// <param name="parent">父级</param>
        /// <param name="callBack">回调函数（播放完后触发）</param>
        public static AudioPlay PlayAudio(AudioStack audioStack, Vector3 pos, Transform parent = null,
            Action callBack = null)
            => audioModule.PlayAudio(audioStack, pos, parent, callBack);
    }
}