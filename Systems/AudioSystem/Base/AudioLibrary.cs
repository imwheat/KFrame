//****************** 代码文件申明 ************************
//* 文件：AudioLibrary                                       
//* 作者：wheat
//* 创建时间：2023/10/01 10:04:00 星期日
//* 描述：负责存储和管理AudioStack
//*****************************************************
using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine.Audio;
using KFrame.Utilities;

namespace KFrame.Systems
{
    [CreateAssetMenu(fileName = "AudioLibrary", menuName = "ScriptableObject/AudioLibrary")]
    public class AudioLibrary : GlobalConfigBase<AudioLibrary>
    {
        [field: SerializeField, LabelText("音效库"), FoldoutGroup("信息预览")] public List<AudioStack> Audioes { get; private set; }
        [field: SerializeField, LabelText("BGM库"), FoldoutGroup("信息预览")] public List<BGMStack> BGMs { get; private set; }
        [field: SerializeField, LabelText("分组列表"), FoldoutGroup("参数设置")] public List<AudioGroup> AudioGroups { get; private set; }

        [field: SerializeField, LabelText("音效音源库"), FoldoutGroup("参数设置"), ListDrawerSettings(ShowIndexLabels = true)]
        public List<AudioClip> AudioClips { get; private set; }
        [field: SerializeField, LabelText("BGM音源库"), FoldoutGroup("参数设置"), ListDrawerSettings(ShowIndexLabels = true)] 
        public List<AudioClip> BGMClips { get; private set; }
        [field: SerializeField, LabelText("混音器分组库"), FoldoutGroup("参数设置")] public List<AudioMixerGroup> AudioMixerGroups { get; private set; }
        
        #if UNITY_EDITOR
        
        /// <summary>
        /// 当前库中最大的音效id
        /// </summary>
        private static int maxAudioIndex = 0;
        /// <summary>
        /// 当前库中最大的音效id
        /// </summary>
        public static int MaxAudioIndex
        {
            get
            {
                //等于0说明还没初始化那就更新一下
                if (maxAudioIndex == 0)
                {
                    foreach (AudioStack audioStack in Instance.Audioes)
                    {
                        if (maxAudioIndex < audioStack.AudioIndex)
                        {
                            maxAudioIndex = audioStack.AudioIndex;
                        }
                    }
                }
                
                return maxAudioIndex;
            }
            set => maxAudioIndex = value;
        }
        /// <summary>
        /// 音效字典
        /// </summary>
        private Dictionary<int, AudioStack> _audioDic;
        /// <summary>
        /// 音效字典
        /// </summary>
        public Dictionary<int, AudioStack> AudioDic
        {
            get
            {
                if (_audioDic == null)
                {
                    InitAudioDic();
                }

                return _audioDic;
            }
        }
        
        /// <summary>
        /// 音效字典
        /// </summary>
        private Dictionary<string, AudioStack> _audioNameDic;
        /// <summary>
        /// 音效字典
        /// </summary>
        public Dictionary<string, AudioStack> AudioNameDic
        {
            get
            {
                if (_audioNameDic == null)
                {
                    InitAudioDic();
                }

                return _audioNameDic;
            }
        }
        /// <summary>
        /// 音效Clip字典
        /// </summary>
        private Dictionary<AudioClip, int> _clipDic;
        /// <summary>
        /// 音效Clip字典
        /// </summary>
        public Dictionary<AudioClip, int> ClipDic
        {
            get
            {
                if (_audioDic == null)
                {
                    InitAudioClipDic();
                }

                return _clipDic;
            }
        }
        
        #endif
        
        #if UNITY_EDITOR

        #region 编辑器相关
        
        private void UpdateAudioDic(AudioStack audioStack)
        {
            _audioDic[audioStack.AudioIndex] = audioStack;
        }
        private void UpdateAudioNameDic(AudioStack audioStack)
        {
            _audioNameDic[audioStack.AudioName] = audioStack;
        }
        private void UpdateAudioDics(AudioStack audioStack)
        {
            UpdateAudioDic(audioStack);
            UpdateAudioNameDic(audioStack);
        }


        /// <summary>
        /// 初始化字典
        /// </summary>
        private void InitAudioDic()
        {
            _audioDic = new Dictionary<int, AudioStack>();
            foreach (AudioStack audioStack in Audioes)
            {
                UpdateAudioDic(audioStack);
            }
        }
        /// <summary>
        /// 初始化字典
        /// </summary>
        private void InitAudioNameDic()
        {
            _audioNameDic = new Dictionary<string, AudioStack>();
            foreach (AudioStack audioStack in Audioes)
            {
                UpdateAudioNameDic(audioStack);
            }
        }
        /// <summary>
        /// 初始化字典
        /// </summary>
        private void InitAudioClipDic()
        {
            _clipDic = new Dictionary<AudioClip, int>();
            for (int i = 0; i < AudioClips.Count; i++)
            {
                ClipDic[AudioClips[i]] = i;
            }
        }
        /// <summary>
        /// 检测音效id是否合理
        /// </summary>
        /// <param name="audioStack">待检测的音效</param>
        /// <param name="index">要使用的id</param>
        /// <returns>合理的话返回true</returns>
        public bool CheckAudioIndexValid(AudioStack audioStack, int index)
        {
            //如果先前已经有这个id，并且id所指的对象和当前的不是同一个，那就不合理
            if (AudioDic.ContainsKey(index) && AudioDic[index] != audioStack)
            {
                return false;
            }

            return true;
        }
        /// <summary>
        /// 检测音效名称是否合理
        /// </summary>
        /// <param name="name">要使用的名称</param>
        /// <returns>合理的话返回true</returns>
        public bool CheckAudioNameValid(string name)
        {
            //如果先前已经有这个名称或者名称为空那就返回false
            if (string.IsNullOrEmpty(name) || AudioNameDic.ContainsKey(name))
            {
                return false;
            }

            return true;
        }
        /// <summary>
        /// 检测clip是否已在库中
        /// 如果没有的话那就添加进去
        /// </summary>
        /// <param name="clip">要检测的clip</param>
        public void EditorCheckAudioClip(AudioClip clip)
        {
            //如果为空那就直接返回
            if(clip == null) return;

            //如果没有的话那就添加到列表里面
            if (!ClipDic.ContainsKey(clip))
            {
                ClipDic[clip] = AudioClips.Count;
                AudioClips.Add(clip);
            }
        }

        #endregion
        
#endif
        
    }

}



