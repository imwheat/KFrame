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
            audioModule = FrameRoot.RootTransform.GetComponentInChildren<AudioModule>();
            audioModule.Init();
            InitAudioes();
        }

        /// <summary>
        /// 初始化音效配置
        /// </summary>
        private static void InitAudioes()
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
                    group.Children.Add(AudioDic.GetAudioGroup(index));
                }
            }
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

        public static bool IsMute
        {
            get => audioModule.IsMute;
            set { audioModule.IsMute = value; }
        }

        public static bool IsLoop
        {
            get => audioModule.IsLoop;
            set { audioModule.IsLoop = value; }
        }

        public static bool IsPause
        {
            get => audioModule.IsPause;
            set { audioModule.IsPause = value; }
        }

        #region 旧的

        /*
        /// <summary>
        /// 播放背景音乐
        /// </summary>
        /// <param name="clip">音乐片段</param>
        /// <param name="loop">是否循环</param>
        /// <param name="volume">音量，-1代表不设置，采用当前音量</param>
        /// <param name="fadeOutTime">渐出音量花费的时间</param>
        /// <param name="fadeInTime">渐入音量花费的时间</param>
        public static void PlayBGMAudio(AudioClip clip, bool loop = true, float volume = -1, float fadeOutTime = 0, float fadeInTime = 0)
            => audioModule.PlayBGMAudio(clip, loop, volume, fadeOutTime, fadeInTime);

        /// <summary>
        /// 使用音效数组播放背景音乐，自动循环
        /// </summary>
        /// <param name="fadeOutTime">渐出音量花费的时间</param>
        /// <param name="fadeInTime">渐入音量花费的时间</param>
        public static void PlayBGMAudioWithClips(AudioClip[] clips, float volume = -1, float fadeOutTime = 0, float fadeInTime = 0)
            => audioModule.PlayBGMAudioWithClips(clips, volume, fadeOutTime, fadeInTime);

        /// <summary>
        /// 停止背景音乐
        /// </summary>
        public static void StopBGMAudio() => audioModule.StopBGMAudio();

        /// <summary>
        /// 暂停背景音乐
        /// </summary>
        public static void PauseBGMAudio() => audioModule.PauseBGMAudio();

        /// <summary>
        /// 继续播放背景音乐
        /// </summary>
        public static void UnPauseBGMAudio() => audioModule.UnPauseBGMAudio();

        /// <summary>
        /// 播放一次特效音乐,并且绑定在某个游戏物体身上
        /// 但是不用担心，游戏物体销毁时，会瞬间解除绑定，回收音效播放器
        /// </summary>
        /// <param name="clip">音效片段</param>
        /// <param name="autoReleaseClip">播放完毕时候自动回收audioClip</param>
        /// <param name="component">挂载组件</param>
        /// <param name="volumeScale">音量 0-1</param>
        /// <param name="is3d">是否3D</param>
        /// <param name="callBack">回调函数-在音乐播放完成后执行</param>
        public static void PlayOnShot(AudioClip clip, Component component = null, bool autoReleaseClip = false, float volumeScale = 1, bool is3d = true, Action callBack = null)
            => audioModule.PlayAudio(clip, component, autoReleaseClip, volumeScale, is3d, callBack);
        /// <summary>
        /// 播放一次特效音乐
        /// </summary>
        /// <param name="clip">音效片段</param>
        /// <param name="position">播放的位置</param>
        /// <param name="autoReleaseClip">播放完毕时候自动回收audioClip</param>
        /// <param name="volumeScale">音量 0-1</param>
        /// <param name="is3d">是否3D</param>
        /// <param name="callBack">回调函数-在音乐播放完成后执行</param>
        public static void PlayOnShot(AudioClip clip, Vector3 position, bool autoReleaseClip = false, float volumeScale = 1, bool is3d = true, Action callBack = null)
            => audioModule.PlayOnShot(clip, position, autoReleaseClip, volumeScale, is3d, callBack);
        */

        #endregion

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
        public static void PlayBGMAudio(BGMStack bgmStack)
            => audioModule.PlayBGMAudio(bgmStack);

        /// <summary>
        /// 停止播放BGM
        /// </summary>
        /// <param name="immediate">立即停止</param>
        public static void StopBGM(bool immediate)
            => audioModule.StopBGM(immediate);

        /// <summary>
        /// 切换播放的音轨
        /// </summary>
        /// <param name="trackId">音轨id</param>
        public static void ChangeSoundTrack(int trackId)
            => audioModule.ChangeSoundTrack(trackId);

        /// <summary>
        /// 播放一次特效音乐
        /// </summary>
        /// <param name="audioStack">音效的Stack</param>
        /// <param name="pos">位置</param>
        /// <param name="parent">父级</param>
        /// <param name="callBack">回调函数（播放完后触发）</param>
        public static AudioPlay PlayAudio(AudioStack audioStack, Vector2 pos, Transform parent = null,
            Action callBack = null)
            => audioModule.PlayAudio(audioStack, pos, parent, callBack);
    }
}