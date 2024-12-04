using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using KFrame.Utilities;
using KFrame.UI;
using UnityEngine.Serialization;

namespace KFrame.Systems
{
    public class AudioModule : MonoBehaviour
    {
        private static GameObjectPoolModule _poolModule;

        [SerializeField, LabelText("背景音乐播放器")] private BGMPlayer bgmPlayer;
        
        /// <summary>
        /// MasterMixerGroup
        /// </summary>
        [SerializeField, LabelText("MasterMixerGroupId")]
        private int masterMixerGroupIndex = 0;
        /// <summary>
        /// BGMMixerGroup
        /// </summary>
        [SerializeField, LabelText("BGMMixerGroupId")]
        private int bgmMixerGroupIndex = 1;
        /// <summary>
        /// SFXMixerGroup
        /// </summary>
        [SerializeField, LabelText("SFXMixerGroupId")]
        private int sfxMixerGroupIndex = 2;
        /// <summary>
        /// UIMixerGroup
        /// </summary>
        [SerializeField, LabelText("uiMixerGroupId")]
        private int uiMixerGroupIndex = 3;
        /// <summary>
        /// UIGroup
        /// </summary>
        [SerializeField, LabelText("UIGroup")]
        private AudioGroup uiGroup;
        /// <summary>
        /// UIGroup
        /// </summary>
        public AudioGroup UIGroup => uiGroup;
        /// <summary>
        /// MasterGroup
        /// </summary>
        [SerializeField, LabelText("MasterGroup")]
        private AudioGroup masterGroup;
        /// <summary>
        /// MasterGroup
        /// </summary>
        public AudioGroup MasterGroup => masterGroup;
        /// <summary>
        /// BGMGroup
        /// </summary>
        [SerializeField, LabelText("BGMGroup")]
        private AudioGroup bgmGroup;
        /// <summary>
        /// BGMGroup
        /// </summary>
        public AudioGroup BGMGroup => bgmGroup;
        /// <summary>
        /// SFXGroup
        /// </summary>
        [SerializeField, LabelText("SFXGroup")]
        private AudioGroup sfxGroup;
        
        [SerializeField, LabelText("效果播放器预制体2D")]
        private GameObject effectAudioPlayPrefab2D;
        [SerializeField, LabelText("效果播放器预制体3D")]
        private GameObject effectAudioPlayPrefab3D;

        [SerializeField, LabelText("对象池预设播放器数量")]
        private int effectAudioDefaultQuantity = 20;

        [SerializeField, LabelText("混音器")] private AudioMixer _AudioMixer;
        [SerializeField, LabelText("混音器")] public AudioMixer AudioMixer => _AudioMixer;

        /// <summary>
        /// 正在播放的音效的字典，用来处理高频播放的音效
        /// </summary>
        private Dictionary<int, List<AudioPlay>> m_AudioPlayingDic;

        #region 音量、播放控制

        /// <summary>
        /// 主音量
        /// </summary>
        public float MasterlVolume
        {
            get => masterGroup.Volume;
            set
            {
                if (UISetPropertyUtility.SetStruct(ref masterGroup.Volume, value))
                {
                    masterGroup.UpdateVolume(value);
                }
            }
        }
        /// <summary>
        /// BGM音量
        /// </summary>
        public float BGMVolume
        {
            get => bgmGroup.Volume;
            set
            {
                if (UISetPropertyUtility.SetStruct(ref bgmGroup.Volume, value))
                {
                    bgmGroup.UpdateVolume(value);
                }
            }
        }

        public float SFXVolume
        {
            get => sfxGroup.Volume;
            set
            {
                if (UISetPropertyUtility.SetStruct(ref sfxGroup.Volume, value))
                {
                    sfxGroup.UpdateVolume(value);
                }
            }
        }
        public float UIVolume
        {
            get => uiGroup.Volume;
            set
            {
                if (UISetPropertyUtility.SetStruct(ref uiGroup.Volume, value))
                {
                    uiGroup.UpdateVolume(value);
                }
            }
        }

        #endregion

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            Transform poolRoot = new GameObject("AudioPlayerPoolRoot").transform;
            poolRoot.SetParent(transform);
            _poolModule = new GameObjectPoolModule();
            _poolModule.Init(poolRoot);
            _poolModule.InitGameObjectPool(effectAudioPlayPrefab2D, -1, effectAudioDefaultQuantity);
            _poolModule.InitGameObjectPool(effectAudioPlayPrefab3D, -1, effectAudioDefaultQuantity);
            audioPlayRoot = new GameObject("audioPlayRoot").transform;
            audioPlayRoot.SetParent(transform);
            m_AudioPlayingDic = new Dictionary<int, List<AudioPlay>>();
            
            //初始化Group
            masterGroup = AudioDic.GetAudioGroup(masterMixerGroupIndex);
            bgmGroup = AudioDic.GetAudioGroup(bgmMixerGroupIndex);
            sfxGroup = AudioDic.GetAudioGroup(sfxMixerGroupIndex);
            uiGroup = AudioDic.GetAudioGroup(uiMixerGroupIndex);
            
            //防空
            if(bgmPlayer==null)
            {
                bgmPlayer = GetComponentInChildren<BGMPlayer>();
                if (bgmPlayer == null)
                {
                    GameObject obj = ResSystem.InstantiateGameObject(transform, nameof(bgmPlayer));
                    bgmPlayer = obj.GetComponent<BGMPlayer>();
                }
            }
        }

        #region 背景音乐

        /// <summary>
        /// 播放BGM
        /// </summary>
        /// <param name="bgmStack">要播放的BGM的包</param>
        public void PlayBGM(BGMStack bgmStack)
        {
            bgmPlayer.PlayBGM(bgmStack);
        }
        /// <summary>
        /// 播放BGM
        /// </summary>
        /// <param name="bgmIndex">要播放的BGM的id</param>
        public void PlayBGM(int bgmIndex)
        {
            bgmPlayer.PlayBGM(bgmIndex);
        }
        /// <summary>
        /// 停止播放BGM
        /// </summary>
        /// <param name="immediate">立即停止</param>
        [Button("停止播放BGM", 30), PropertySpace(5f, 5f), FoldoutGroup("测试按钮")]
        public void StopBGM(bool immediate)
        {
            bgmPlayer.StopBGMPlay(immediate);
        }

        #endregion

        #region 音效

        private Transform audioPlayRoot;

        /// <summary>
        /// 在指定位置播放音效
        /// </summary>
        /// <param name="audioStack">音效配置</param>
        /// <param name="pos">位置</param>
        /// <param name="parent">父级</param>
        /// <param name="callBack">回调</param>
        public AudioPlay PlayAudio(AudioStack audioStack, Vector3 pos, Transform parent = null, Action callBack = null)
        {
            //先生成音效播放器
            GameObject audioPlayer = PoolSystem.GetOrNewGameObject(audioStack.Is3D?effectAudioPlayPrefab3D:effectAudioPlayPrefab2D, parent);

            if (pos != null)
            {
                audioPlayer.transform.position = pos;
            }

            AudioPlay audioPlay = audioPlayer.GetComponent<AudioPlay>();
            audioPlay.PlayingAudioIndex = audioStack.AudioIndex;
            AudioModifyData data = null;
            Action<AudioPlay> action = null;

            //如果会限制播放数量
            if (audioStack.LimitPlay)
            {
                //注册
                data = RegisterPlayAudioDic(audioPlay, audioStack.LimitPlayCount);

                //注销事件
                action = audioPlay => UnRegisterPlayAudioDic(audioPlay);
            }

            //开始播放音效
            audioPlay.PlaySound(audioStack, callBack, data, action);

            //返回AudioPlay
            return audioPlay;
        }
        /// <summary>
        /// 注册音效播放字典
        /// </summary>
        /// <param name="audioPlay">要播放的音效</param>
        /// <param name="limitCount">限制播放数量</param>
        /// <returns>返回一些修改参数，没有就为null</returns>
        private AudioModifyData RegisterPlayAudioDic(AudioPlay audioPlay, int limitCount)
        {
            //防空
            if (audioPlay == null) return null;

            int audioId = audioPlay.PlayingAudioIndex;

            //防止无用信息
            if (audioId <= 0) return null;

            AudioModifyData modifyData = null;

            //如果字典中不存在那就先创建一个list
            if (m_AudioPlayingDic.ContainsKey(audioId) == false)
            {
                m_AudioPlayingDic.Add(audioId, new List<AudioPlay>());
            }

            //获取播放列表
            List<AudioPlay> playList = m_AudioPlayingDic[audioId];

            //如果当前音效播放的数量大于最大数量，那就暂停前面播放的音效的
            if (playList.Count >= limitCount)
            {

                //升高音调
                modifyData = new AudioModifyData
                {
                    Pitch = playList[0].AudioSource.pitch + 0.05f,
                };

                //把之前播放的音效给关掉
                playList[0].Vanish();
            }

            //然后把新的播放的音效加入列表
            playList.Add(audioPlay);

            //返回data
            return modifyData;
        }
        /// <summary>
        /// 注销音效播放字典
        /// </summary>
        private void UnRegisterPlayAudioDic(AudioPlay audioPlay)
        {
            //防空
            if (audioPlay == null) return;

            int audioId = audioPlay.PlayingAudioIndex;

            //如果字典中不存在那就返回
            if (m_AudioPlayingDic.ContainsKey(audioId) == false)
            {
                return;
            }

            //注销
            List<AudioPlay> playList = m_AudioPlayingDic[audioId];
            playList.Remove(audioPlay);
        }

        #endregion
    }

    public class AudioModifyData
    {
        public float Volume = float.MinValue;
        public float Pitch = float.MinValue;
    }

}
