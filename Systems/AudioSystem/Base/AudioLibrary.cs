//****************** 代码文件申明 ************************
//* 文件：AudioLibrary                                       
//* 作者：wheat
//* 创建时间：2023/10/01 10:04:00 星期日
//* 描述：负责存储和管理AudioStack
//*****************************************************
using UnityEngine;
using System.Collections.Generic;
using KFrame.Attributes;
using Sirenix.OdinInspector;
using UnityEngine.Audio;
using KFrame.Utilities;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace KFrame.Systems
{
    [KGlobalConfigPath(GlobalPathType.Assets, typeof(AudioLibrary), true)]
    public class AudioLibrary : GlobalConfigBase<AudioLibrary>
    {
        [field: SerializeField, LabelText("音效库"), FoldoutGroup("信息预览")] public List<AudioStack> Audioes { get; private set; }
        [field: SerializeField, LabelText("BGM库"), FoldoutGroup("信息预览")] public List<BGMStack> BGMs { get; private set; }
        [field: SerializeField, LabelText("分组列表"), FoldoutGroup("参数设置")] public List<AudioGroup> AudioGroups { get; private set; }

        [field: SerializeField, LabelText("音效音源库"), FoldoutGroup("参数设置"), ListDrawerSettings(ShowIndexLabels = true)]
        public List<AudioClip> AudioClips { get; private set; }
        [field: SerializeField, LabelText("BGM音源库"), FoldoutGroup("参数设置"), ListDrawerSettings(ShowIndexLabels = true)] 
        public List<AudioClip> BGMClips { get; private set; }
        [field: SerializeField, LabelText("混音器"), FoldoutGroup("参数设置")] public AudioMixer AudioMixer { get; private set; }
        [field: SerializeField, LabelText("混音器分组库"), FoldoutGroup("参数设置")] public List<AudioMixerGroup> AudioMixerGroups { get; private set; }
        
        #if UNITY_EDITOR
        
        /// <summary>
        /// 音效id的起始数
        /// </summary>
        private const int InitAudioIndex = 200000;
        
        /// <summary>
        /// BGMid的起始数
        /// </summary>
        private const int InitBGMIndex = 300000;
        /// <summary>
        /// 当前库中最大的音效id
        /// </summary>
        private static int maxAudioIndex = InitAudioIndex;
        /// <summary>
        /// 当前库中最大的音效id
        /// </summary>
        public static int MaxAudioIndex
        {
            get
            {
                //等于InitAudioIndex说明还没初始化那就更新一下
                if (maxAudioIndex == InitAudioIndex)
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
        /// 当前库中最大的BGMid
        /// </summary>
        private static int maxBGMIndex = InitBGMIndex;
        /// <summary>
        /// 当前库中最大的BGMid
        /// </summary>
        public static int MaxBGMIndex
        {
            get
            {
                //等于InitBGMIndex说明还没初始化那就更新一下
                if (maxBGMIndex == InitBGMIndex)
                {
                    foreach (BGMStack audioStack in Instance.BGMs)
                    {
                        if (maxAudioIndex < audioStack.BGMIndex)
                        {
                            maxAudioIndex = audioStack.BGMIndex;
                        }
                    }
                }
                
                return maxBGMIndex;
            }
            set => maxBGMIndex = value;
        }
        
        #endif
        
        #if UNITY_EDITOR

        #region 编辑器相关

        /// <summary>
        /// 添加一个新的AudioStack
        /// </summary>
        public static void AddAudioStack(AudioStack stack)
        {
            //防空
            if(stack == null) return;
            
            //添加到库
            Instance.Audioes.Add(stack);
            //更新字典
            AudioDic.SetAudioStack(stack, stack.AudioIndex);
        }
        /// <summary>
        /// 删除BGMStack
        /// </summary>
        /// <param name="stack"></param>
        public static void DeleteAudioStack(AudioStack stack)
        {
            //防空
            if(stack == null) return;
            
            //从库里删除
            Instance.Audioes.Remove(stack);
            //更新字典
            AudioDic.RemoveAudioStack(stack);
        }
        /// <summary>
        /// 添加一个新的BGMStack
        /// </summary>
        public static void AddBGMStack(BGMStack stack)
        {
            //防空
            if(stack == null) return;
            
            //添加到库
            Instance.BGMs.Add(stack);
            //更新字典
            AudioDic.SetBGMStack(stack, stack.BGMIndex);
        }
        /// <summary>
        /// 删除BGMStack
        /// </summary>
        /// <param name="stack"></param>
        public static void DeleteBGMStack(BGMStack stack)
        {
            //防空
            if(stack == null) return;
            
            //从库里删除
            Instance.BGMs.Remove(stack);
            //更新字典
            AudioDic.RemoveBGMStack(stack);
        }
        /// <summary>
        /// 递归查找每个Group的子集
        /// </summary>
        public void FindChildGroup(Dictionary<AudioMixerGroup, AudioGroup> groupDic,
            AudioMixer mixer,string path, HashSet<string> seen)
        {
            //标记已经设置过了
            if (seen.Contains(path))
            {
                return;
            }
            seen.Add(path);
            
            //先根据路径查找子集，如果只有1个那就说明到底了
            AudioMixerGroup[] groups = mixer.FindMatchingGroups(path);
            if (groups.Length == 1 && !string.IsNullOrEmpty(path))
            {
                //查找它的父级然后绑定
                AudioGroup nGroup = groupDic[groups[0]];

                AudioGroup parent =
                    groupDic[mixer.FindMatchingGroups(path.Substring(0, path.Length - ($"/{nGroup.GroupName}").Length))[0]];
                nGroup.SetParentGroup(parent);
            }
            else
            {
                //如果还找到多个Group，那就继续查询
                foreach (AudioMixerGroup group in groups)
                {
                    FindChildGroup(groupDic, mixer, path + $"/{group.name}", seen);
                }
            }
        }
        /// <summary>
        /// 更新AudioGroups
        /// </summary>
        public void UpdateAudioGroups()
        {
            if (AudioMixer == null)
            {
                EditorUtility.DisplayDialog("错误", "AudioMixer为空，无法更新", "确认");
                return;
            }
            
            //清空AudioGroup，然后递归查询每个Group的子集
            AudioGroups = new List<AudioGroup>();
            AudioMixerGroups = new List<AudioMixerGroup>();
            AudioMixerGroup[] groups = AudioMixer.FindMatchingGroups("");
            Dictionary<AudioMixerGroup, AudioGroup> groupDic = new Dictionary<AudioMixerGroup, AudioGroup>();
            foreach (AudioMixerGroup group in groups)
            {
                AudioGroup newGroup = new AudioGroup(group.name, AudioGroups.Count);
                groupDic[group] = newGroup;
                AudioGroups.Add(newGroup);
                AudioMixerGroups.Add(group);
            }
            FindChildGroup(groupDic, AudioMixer, AudioGroups[0].GroupName, new HashSet<string>());
                
            //保存
            EditorUtility.SetDirty(this);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

       
        /// <summary>
        /// 检测音效id是否合理
        /// </summary>
        /// <param name="audioStack">待检测的音效</param>
        /// <param name="index">要使用的id</param>
        /// <returns>合理的话返回true</returns>
        public bool CheckAudioIndexValid(AudioStack audioStack, int index)
        {
            //如果太小了也不合理
            if (index <= InitAudioIndex) return false;
            
            AudioStack findStack = AudioDic.GetAudioStack(index);
            //如果先前已经有这个id，并且id所指的对象和当前的不是同一个，那就不合理
            if (findStack != null && findStack != audioStack)
            {
                return false;
            }

            return true;
        }
        /// <summary>
        /// 检测BGMid是否合理
        /// </summary>
        /// <param name="bgmStack">待检测的BGM</param>
        /// <param name="index">要使用的id</param>
        /// <returns>合理的话返回true</returns>
        public bool CheckBGMIndexValid(BGMStack bgmStack, int index)
        {
            //如果太小了也不合理
            if (index <= InitBGMIndex) return false;
            
            BGMStack findStack = AudioDic.GetBGMStack(index);
            //如果先前已经有这个id，并且id所指的对象和当前的不是同一个，那就不合理
            if (findStack != null && findStack != bgmStack)
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
            if (string.IsNullOrEmpty(name))
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
            int findIndex = AudioDic.GetAudioClipIndex(clip);
            if (findIndex == -1)
            {
                AudioDic.SetAudioClipIndex(clip,AudioClips.Count);
                AudioClips.Add(clip);
            }
        }
        /// <summary>
        /// 检测clip是否已在库中
        /// 如果没有的话那就添加进去
        /// </summary>
        /// <param name="clip">要检测的clip</param>
        public void EditorCheckBGMClip(AudioClip clip)
        {
            //如果为空那就直接返回
            if(clip == null) return;

            //如果没有的话那就添加到列表里面
            int findIndex = AudioDic.GetBGMClipIndex(clip);
            if (findIndex == -1)
            {
                AudioDic.SetBGMClipIndex(clip,BGMClips.Count);
                BGMClips.Add(clip);
            }
        }

        #endregion
        
#endif
        
    }

}



