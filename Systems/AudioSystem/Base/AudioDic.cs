//****************** 代码文件申明 ************************
//* 文件：AudioDic                                       
//* 作者：wheat
//* 创建时间：2023/11/25 18:47:06 星期六
//* 描述：音效字典
//*****************************************************
using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Audio;

namespace KFrame.Systems
{
    public static class AudioDic
    {
        private static AudioLibrary audioLibrary;
        private static bool inited;

        #region 字典

        /// <summary>
        /// 音效字典
        /// </summary>
        private static Dictionary<int, AudioStack> audioDic = new Dictionary<int, AudioStack>();

        /// <summary>
        /// 音效clip字典
        /// </summary>
        private static Dictionary<int, AudioClip> audioClipDic = new Dictionary<int, AudioClip>();
        
        /// <summary>
        /// 音效分组字典
        /// </summary>
        private static Dictionary<int, AudioGroup> groupDic = new Dictionary<int, AudioGroup>();
        
        /// <summary>
        /// 音效混音器分组字典
        /// </summary>
        private static Dictionary<int, AudioMixerGroup> mixerGroupDic = new Dictionary<int, AudioMixerGroup>();

        /// <summary>
        /// BGM字典
        /// </summary>
        private static Dictionary<int, BGMStack> bgmDic = new Dictionary<int, BGMStack>();

        /// <summary>
        /// BGMClip字典
        /// </summary>
        private static Dictionary<int, AudioClip> bgmClipDic = new Dictionary<int, AudioClip>();

        #endregion

        #region 编辑器相关

#if UNITY_EDITOR
        
        /// <summary>
        /// 音效分组字典
        /// </summary>
        private static readonly Dictionary<AudioMixerGroup, AudioGroup>
            groupDic2 = new Dictionary<AudioMixerGroup, AudioGroup>();
        /// <summary>
        /// 音效Clip对id的字典(编辑器使用)
        /// </summary>
        private static Dictionary<AudioClip, int> audioClipIndexDic = new Dictionary<AudioClip, int>();
        /// <summary>
        /// BGMClip对id的字典(编辑器使用)
        /// </summary>
        private static Dictionary<AudioClip, int> bgmClipIndexDic = new Dictionary<AudioClip, int>();

        static AudioDic()
        {
            Init();
        }

#endif

        #endregion

        #region 初始化

        /// <summary>
        /// 同步初始化
        /// </summary>
        public static void Init()
        {
            if (inited) return;

            var load = ResSystem.LoadAsset<AudioLibrary>(nameof(AudioLibrary));
            LoadResult(load);
        }

        private static void LoadResult(AudioLibrary audioLib)
        {
            //如果已经初始化过了那就返回
            if (inited) return;

            try
            {
                // 资源加载成功，可以在这里进行实例化或其他操作
                audioLibrary = audioLib;

                //注册字典
                foreach (var cp in audioLib.Audioes)
                {
                    audioDic.Add(cp.AudioIndex, cp);
                }

                foreach (var bgm in audioLib.BGMs)
                {
                    bgmDic.Add(bgm.BGMIndex, bgm);
                }

                for (int i = 0; i < audioLib.AudioMixerGroups.Count; i++)
                {
                    mixerGroupDic[i] = audioLib.AudioMixerGroups[i];
                }

                for (int i = 0; i < audioLib.AudioClips.Count; i++)
                {
                    audioClipDic[i] = audioLib.AudioClips[i];

#if UNITY_EDITOR
                    audioClipIndexDic[audioLib.AudioClips[i]] = i;
#endif
                    
                }

                for (int i = 0; i < audioLib.BGMClips.Count; i++)
                {
                    bgmClipDic[i] = audioLib.BGMClips[i];
                    
#if UNITY_EDITOR
                    bgmClipIndexDic[audioLib.BGMClips[i]] = i;
#endif
                }
                
                foreach (var group in audioLib.AudioGroups)
                {
                    groupDic.Add(group.GroupIndex, group);
#if UNITY_EDITOR
                    groupDic2[GetAudioMixerGroup(group.GroupIndex)] = group;
#endif
                }

                inited = true;
            }
            catch (Exception ex)
            {
                Debug.LogWarning("加载音效库失败: " + ex);
            }
        }

        #endregion

        #region 数据获取

         /// <summary>
        /// 获取音效信息文件
        /// </summary>
        /// <param name="index">音效id</param>
        public static AudioStack GetAudioStack(int index)
        {
            if (audioDic.TryGetValue(index, out var audioStack))
            {
                return audioStack;
            }
            else
            {
                Debug.Log("无法找到对应id的音效:" + index);
                return null;
            }
        }

        /// <summary>
        /// 获取音效分组信息
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static AudioGroup GetAudioGroup(int index)
        {
            if (groupDic.TryGetValue(index, out var group))
            {
                return group;
            }
            else
            {
                Debug.Log("无法找到对应id的音效分组:" + index);
                return null;
            }
        }
        
        /// <summary>
        /// 获取音效混音器分组
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static AudioMixerGroup GetAudioMixerGroup(int index)
        {
            if (groupDic.ContainsKey(index))
            {
                return mixerGroupDic[index];
            }
            else
            {
                Debug.Log("无法找到对应id的音效分组:" + index);
                return null;
            }
        }

        /// <summary>
        /// 获取所有的AudioGroup
        /// </summary>
        internal static List<AudioGroup> GetAllAudioGroup()
        {
            List<AudioGroup> res = new List<AudioGroup>(groupDic.Values);

            return res;
        }

        /// <summary>
        /// 获取BGM的信息文件
        /// </summary>
        /// <param name="index">音效id</param>
        public static BGMStack GetBGMStack(int index)
        {
            //如果字典中有的话就返回
            if (bgmDic.TryGetValue(index, out var stack))
            {
                return stack;
            }
            else
            {
                Debug.Log("无法找到对应id的BGM:" + index);
                return null;
            }
        }

        /// <summary>
        /// 获取音效Clip
        /// </summary>
        /// <param name="index">音效clip的id</param>
        public static AudioClip GetAudioClip(int index)
        {
            //如果字典中有的话就返回
            return audioClipDic.GetValueOrDefault(index);
        }

        /// <summary>
        /// 获取BGMClip
        /// </summary>
        /// <param name="index">音效clip的id</param>
        public static AudioClip GetBGMClip(int index)
        {
            //如果字典中有的话就返回
            return bgmClipDic.GetValueOrDefault(index);
        }

        #endregion

        #region 播放操作

        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="id">音效id</param>
        /// <param name="pos">位置</param>
        /// <param name="parent">父级</param>
        /// <param name="callBack">回调事件</param>
        public static AudioPlay PlayAudio(int id, Vector3 pos, Transform parent = null, Action callBack = null)
        {
            //id为0表示没声音
            if (id == 0) return null;

            //从字典中获取对应的音效
            AudioStack audioStack = GetAudioStack(id);

            //如果无法获取就返回
            if (audioStack == null) return null;

            //播放音效
            return AudioSystem.PlayAudio(audioStack, pos, parent, callBack);
        }

        /// <summary>
        /// 播放BGM
        /// </summary>
        /// <param name="id">bgm id</param>
        public static void PlayBGM(int id)
        {
            //id为0代表无
            if (id == 0) return;

            //查找BGM
            BGMStack bgmStack = GetBGMStack(id);

            //如果为空直接返回
            if (bgmStack == null) return;

            //播放BGM
            AudioSystem.PlayBGM(bgmStack);
        }

        #endregion

        #region 编辑器配置

        
#if UNITY_EDITOR

        /// <summary>
        /// 通过indexes列表查找一个个clip放入clips列表
        /// </summary>
        /// <param name="indexes">音效id列表</param>
        /// <param name="clips">Clip列表</param>
        public static void FindAudioClipsByIndexes(List<int> indexes, List<AudioClip> clips)
        {
            //遍历查找，然后一个个添加
            foreach (int index in indexes)
            {
                clips.Add(GetAudioClip(index));
            }
        }
        /// <summary>
        /// 通过clips列表查找一个个Index放入indexes列表
        /// </summary>
        /// <param name="indexes">音效id列表</param>
        /// <param name="clips">Clip列表</param>
        public static void FindIndexesByAudioClips(List<int> indexes, List<AudioClip> clips)
        {
            //遍历查找，然后一个个添加
            foreach (AudioClip clip in clips)
            {
                indexes.Add(GetAudioClipIndex(clip));
            }
        }

        /// <summary>
        /// 查找AudioClip的id(编辑器使用)
        /// </summary>
        /// <returns>返回Clip下标，找不到返回-1</returns>
        public static int GetAudioClipIndex(AudioClip clip)
        {
            //如果字典中有的话就返回
            //找不到返回-1
            return audioClipIndexDic.GetValueOrDefault(clip, -1);
        }
        /// <summary>
        /// 查找AudioClip的id(编辑器使用)
        /// </summary>
        /// <returns>返回Clip下标，找不到返回-1</returns>
        public static int GetBGMClipIndex(AudioClip clip)
        {
            //如果字典中有的话就返回
            //找不到返回-1
            return bgmClipIndexDic.GetValueOrDefault(clip, -1);
        }

        /// <summary>
        /// 更新AudioClip库(编辑器使用)
        /// </summary>
        public static void SetAudioClipIndex(AudioClip clip, int id)
        {
            audioClipIndexDic[clip] = id;
            audioClipDic[id] = clip;
        }
        /// <summary>
        /// 更新BGMClip库(编辑器使用)
        /// </summary>
        public static void SetBGMClipIndex(AudioClip clip, int id)
        {
            bgmClipIndexDic[clip] = id;
            bgmClipDic[id] = clip;
        }
        /// <summary>
        /// 更新AudioStack库(编辑器使用)
        /// </summary>
        public static void SetAudioStack(AudioStack stack, int id)
        {
            audioDic[id] = stack;
        }
        /// <summary>
        /// 更新BGMStack库(编辑器使用)
        /// </summary>
        public static void SetBGMStack(BGMStack stack, int id)
        {
            bgmDic[id] = stack;
        }
        /// <summary>
        /// 从库中删除Audio(编辑器使用)
        /// </summary>
        public static void RemoveAudioStack(AudioStack stack)
        {
            audioDic.Remove(stack.AudioIndex);
        }
        /// <summary>
        /// 从库中删除BGM(编辑器使用)
        /// </summary>
        public static void RemoveBGMStack(BGMStack stack)
        {
            bgmDic.Remove(stack.BGMIndex);
        }
        /// <summary>
        /// 查找AudioGroup(编辑器使用)
        /// </summary>
        public static int GetAudioGroupIndex(AudioMixerGroup mixerGroup)
        {
            if (mixerGroup == null) return 0;
            
            //如果字典中有的话就返回
            if (groupDic2.TryGetValue(mixerGroup, out var value))
            {
                return value.GroupIndex;
            }
            else
            {
                //找不到就返回0
                return 0;
            }
        }

#endif

        #endregion

    }
    
}

