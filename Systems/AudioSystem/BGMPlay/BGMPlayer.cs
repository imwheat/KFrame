//****************** 代码文件申明 ************************
//* 文件：BGMPlayer                                       
//* 作者：wheat
//* 创建时间：2024/01/20 10:19:57 星期六
//* 描述：背景音乐播放器
//*****************************************************
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine.Audio;
using KFrame.Tools;

namespace KFrame.Systems
{
    public class BGMPlayer : MonoBehaviour
    {

        #region 播放器字段

        [LabelText("播放BGM的预制体"), TabGroup("设置")] public GameObject BGMPlayPrefab;
        [LabelText("混音器"), TabGroup("设置")] public AudioMixer AudioMixer;
        [LabelText("混音器的快照"), TabGroup("设置")] public AudioMixerSnapshot AudioMixerSnapshot;
        [LabelText("音轨列表"),TabGroup("设置")] public List<AudioMixerGroup> SoundTrackList;
        [LabelText("音轨启用所需时间"),TabGroup("设置")] public float SoundTrackEnableTranlationDuration = 2f;
        [LabelText("音轨禁用所需时间"),TabGroup("设置")] public float SoundTrackDisableTranlationDuration = 5f;
        [LabelText("BGM切换所需时间"),TabGroup("设置")] public float BGMTranslationDuration = 5f;
        [LabelText("BGM切换音量曲线"),TabGroup("设置")] public AnimationCurve BGMTranslationVolumeCurve;
        [ShowInInspector, LabelText("播放中的音轨字典"), TabGroup("逻辑"), Tooltip("int是音轨id，float是当前分轨音量")] 
        public Dictionary<int,float> PlayingTrackDic;
        [SerializeField ,LabelText("当前的BGMStack"), TabGroup("逻辑")] public BGMStack PlayingBGMStack;
        [SerializeField ,LabelText("播放中的BGM列表"), TabGroup("逻辑")] public List<BGMPlay> BGMPlayingList;
        [SerializeField ,LabelText("持续播放的BGM列表"), TabGroup("逻辑")] private List<BGMPlay> BGMPlayingPersistList;
        [ShowInInspector ,LabelText("正在播放的音轨List"), TabGroup("逻辑")] public HashSet<int> PlayingList;
        [ShowInInspector, LabelText("正在启用播放的音轨List"), TabGroup("逻辑")] private List<int> enablingPlayList;
        [ShowInInspector, LabelText("正在禁用播放的音轨List"), TabGroup("逻辑")] private List<int> disablingPlayList;
        [SerializeField ,LabelText("bgm切换音量的写成"), TabGroup("逻辑")] private Coroutine bgmTranslationCoroutine;
        [SerializeField ,LabelText("音轨切换音量的写成"), TabGroup("逻辑")] private Coroutine soundtrackTranslationCoroutine;

        #endregion

        #region 静态字段

        public static readonly string TrackBGMTranslationVolume = "BGMTranslationVolume";
        public static readonly string TrackBGM01Volume = "BGM01Volume";
        public static readonly string TrackBGM02Volume = "BGM02Volume";
        public static readonly string TrackBGM03Volume = "BGM03Volume";
        public static readonly string TrackBGM04Volume = "BGM04Volume";
        public static readonly string TrackBGM05Volume = "BGM05Volume";
        public static readonly string TrackBGM06Volume = "BGM06Volume";

        #endregion

        #region 生命周期

        private void Awake()
        {
            //初始化
            //BGMPlay的Prefab
            if(BGMPlayPrefab == null)
            {
                BGMPlayPrefab = ResSystem.LoadAsset<GameObject>("BGMPlay");
            }
            //混音器
            if(AudioMixer == null)
            {
                AudioMixer = ResSystem.LoadAsset<AudioMixer>("AudioMixer");
            }
            //音轨列表
            if(SoundTrackList==null)
            {
                SoundTrackList = new List<AudioMixerGroup>();
            }
            if(SoundTrackList.Count == 0)
            {
                AudioMixerGroup[] bgmGroups = AudioMixer.FindMatchingGroups("Master/BGM/");

                SoundTrackList.AddRange(bgmGroups);
            }

            //列表字典初始化
            PlayingTrackDic = new Dictionary<int,float>();
            BGMPlayingList = new List<BGMPlay>();
            BGMPlayingPersistList = new List<BGMPlay>();
            enablingPlayList = new List<int>();
            disablingPlayList = new List<int>();
            PlayingList = new HashSet<int>();

#if UNITY_EDITOR

            //编辑器快速响应更新
            if(AudioMixerSnapshot!=null)
            {
                AudioMixerSnapshot.TransitionTo(0f);
            }

#endif

        }
        private void OnDestroy()
        {


        }
        private void Start()
        {
            //先将音量都调零
            for (int i = 1; i <= 6; i++)
            {
                SetTrackVolume(i, 0f);
            }
        }
        private void LateUpdate()
        {
            //如果有BGM停止播放了就去除
            for (int i = 0; i < BGMPlayingList.Count; i++)
            {
                if (BGMPlayingList[i].MyAudioSource.isPlaying == false)
                {
                    BGMPlayingList[i].EndPlay();
                    BGMPlayingList.RemoveAt(i);
                    i--;
                }
            }
            for (int i = 0; i < BGMPlayingPersistList.Count; i++)
            {
                if (BGMPlayingPersistList[i].MyAudioSource.isPlaying == false)
                {
                    BGMPlayingPersistList[i].EndPlay();
                    BGMPlayingPersistList.RemoveAt(i);
                    i--;
                }
            }
        }

        #endregion

        #region 方法
        /// <summary>
        /// 播放BGM
        /// </summary>
        /// <param name="stack">BGM的Stack</param>
        public void PlayBGM(BGMStack stack)
        {
            //如果要播放的和现在的一样那就返回
            if (stack == PlayingBGMStack) return;

            //如果是没有BGM在播放的情况
            if(PlayingList.Count == 0)
            {
                //更新stack
                PlayingBGMStack = stack;

                //先把BGM切换的音量调零
                SetTrackVolume(0, 0f);

                //因为没有BGM在播放默认开启1和2的播放
                for (int i = 1; i <= 6; i++)
                {
                    InitSoundTrack(i);
                }

                //将音轨1和2的音量调为1f
                SetTrackVolume(1, 1f);
                SetTrackVolume(2, 1f);
                PlayingList.Add(2);

                //开始BGM渐入
                StartBGMFadeIn();
            }
            //如果现在在播放其他的BGM
            else
            {
                //启用协程来切换BGM
                StartChangeBGM(stack);
            }
        }
        /// <summary>
        /// 播放BGM
        /// </summary>
        /// <param name="id">BGM的id</param>
        public void PlayBGM(int id)
        {
            //先获取stack
            BGMStack stack = AudioDic.GetBGMStack(id);

            //如果没有那就返回
            if(stack == null) return;

            //开始播放
            PlayBGM(stack);
        }
        /// <summary>
        /// 停止BGM播放
        /// </summary>
        /// <param name="immediate">是否立即停止</param>
        [Button("停止播放BGM", 30), PropertySpace(5f, 5f), FoldoutGroup("测试按钮")]
        public void StopBGMPlay(bool immediate)
        {

#if UNITY_EDITOR

            if (Application.isPlaying == false)
            {
                Debug.Log("请在游戏启动的时候再使用此功能");
                return;
            }
#endif

            //如果立即停止
            if (immediate)
            {
                //那就马上停止
                StopBGMImmediate();
            }
            //不是的话那就渐出然后停止
            else
            {
                StartBGMStopCoroutine();
            }
        }
        /// <summary>
        /// 立即停止BGM播放
        /// </summary>
        private void StopBGMImmediate()
        {
            PlayingBGMStack = null;

            //把先前播放的BGM给关了
            foreach (var bgmPlay in BGMPlayingPersistList)
            {
                bgmPlay.EndPlay();
            }
            foreach (var bgmPlay in BGMPlayingList)
            {
                bgmPlay.EndPlay();
            }
            BGMPlayingPersistList.Clear();
            BGMPlayingList.Clear();
            PlayingList.Clear();

        }
        /// <summary>
        /// 切换播放的音轨
        /// </summary>
        /// <param name="trackId">音轨id</param>
        [Button("切换播放BGM的音轨", 30), PropertySpace(5f, 5f), FoldoutGroup("测试按钮")]
        public void ChangeSoundTrack(int trackId)
        {

#if UNITY_EDITOR

            if (Application.isPlaying == false)
            {
                Debug.Log("请在游戏启动的时候再使用此功能");
                return;
            }
#endif

            //如果已经在播放了或者在启用了就返回
            if (PlayingList.Contains(trackId)||enablingPlayList.Contains(trackId)) return;
            //如果是1直接播放1
            if(trackId == 1)
            {
                PlaySoundTrack(1);
                return;
            }
            //参数不正确直接返回
            else if (trackId <=0||trackId>=SoundTrackList.Count)
            {
                return;
            }

            //如果禁用队列里面有对应音轨那就去掉
            if(disablingPlayList.Contains(trackId))
            {
                disablingPlayList.Remove(trackId);
            }

            //先把正在播放的音轨的id加到禁用播放的里面
            foreach (var i in PlayingList)
            {
                //防止重复添加(id为1的是持续播放的不用加到里面)
                if (disablingPlayList.Contains(i) == false&&i!=1 && i != trackId)
                    disablingPlayList.Add(i);
            }
            foreach (var i in enablingPlayList)
            {
                //防止重复添加(id为1的是持续播放的不用加到里面)
                if (disablingPlayList.Contains(i) == false&&i!=1 && i!=trackId)
                    disablingPlayList.Add(i);
            }
            //然后清空列表
            PlayingList.Clear();
            enablingPlayList.Clear();

            //把新的要播放的音轨id添加到启用中的列表中
            enablingPlayList.Add(trackId);
            //先将音轨音量设为0f
            SetTrackVolume(trackId, 0f);

            //如果已经在切换先把之前的终止
            if (soundtrackTranslationCoroutine != null)
            {
                StopCoroutine(soundtrackTranslationCoroutine);
            }

            //开始新的切换
            soundtrackTranslationCoroutine = StartCoroutine(SoundTrackTranslationCoroutine());
        }
        /// <summary>
        /// 播放指定音轨的BGM
        /// 直接播放音量为1f
        /// </summary>
        /// <param name="trackId">音轨id</param>
        [Button("播放指定BGM的音轨", 30), PropertySpace(5f, 5f), FoldoutGroup("测试按钮")]
        public void PlaySoundTrack(int trackId)
        {

#if UNITY_EDITOR

            if (Application.isPlaying == false)
            {
                Debug.Log("请在游戏启动的时候再使用此功能");
                return;
            }
#endif

            //如果没有bgm的stack就返回
            if (PlayingBGMStack == null) return;
            //如果已经在播放了就返回
            if(PlayingList.Contains(trackId)) return;
            //添加到播放中的音轨列表中
            PlayingList.Add(trackId);
            PlayingTrackDic.TryAdd(trackId, 1f);

            //查找当前BGM中对应音轨的BGM
            foreach (var clip in PlayingBGMStack.Clips)
            {
                if (clip.SoundTrackIndex == trackId)
                {
                    GameObject obj = PoolSystem.GetOrNewGameObject(BGMPlayPrefab, transform);
                    BGMPlay bgmPlay = obj.GetComponent<BGMPlay>();
                    bgmPlay.PlayBGM(clip, SoundTrackList[clip.SoundTrackIndex]);

                    if(trackId==1)
                    {
                        BGMPlayingPersistList.Add(bgmPlay);
                    }
                    else
                    {
                        BGMPlayingList.Add(bgmPlay);
                    }
                }
            }
        }
        /// <summary>
        /// 初始化指定音轨的BGM
        /// </summary>
        /// <param name="trackId">音轨id</param>
        private void InitSoundTrack(int trackId)
        {
            //如果没有bgm的stack就返回
            if (PlayingBGMStack == null) return;

            SetTrackVolume(trackId, 0f);

            //查找当前BGM中对应音轨的BGM
            foreach (var clip in PlayingBGMStack.Clips)
            {
                if (clip.SoundTrackIndex == trackId)
                {
                    GameObject obj = PoolSystem.GetOrNewGameObject(BGMPlayPrefab, transform);
                    BGMPlay bgmPlay = obj.GetComponent<BGMPlay>();
                    bgmPlay.PlayBGM(clip, SoundTrackList[clip.SoundTrackIndex]);

                    if (trackId == 1)
                    {
                        BGMPlayingPersistList.Add(bgmPlay);
                    }
                    else
                    {
                        BGMPlayingList.Add(bgmPlay);
                    }
                }
            }
        }
        /// <summary>
        /// 切换当前BGM的Stack换为另一个
        /// </summary>
        public void ChangePlayingBGMStack(BGMStack stack)
        {
            //如果是空的，或者和现在的一样就返回
            if(stack == null || stack == PlayingBGMStack) return;
            PlayingBGMStack = stack;

            //先把先前播放的BGM给关了
            foreach (var bgmPlay in BGMPlayingPersistList)
            {
                bgmPlay.EndPlay();
            }
            foreach (var bgmPlay in BGMPlayingList)
            {
                bgmPlay.EndPlay();
            }
            BGMPlayingPersistList.Clear();
            BGMPlayingList.Clear();

            //然后根据之前的播放的音轨id替换对应的BGM开始播放
            for (int i = 1; i <= 6; i++)
            {
                InitSoundTrack(i);
            }
            //更新音量
            foreach (var trackId in PlayingList)
            {
                SetTrackVolume(trackId , 1);
            }
        }

        #endregion

        #region 调参

        /// <summary>
        /// 将0~1的音量换算成对应的音量
        /// </summary>
        /// <param name="x">0~1的音量</param>
        /// <returns>-80f~0f的音量</returns>
        public float GetVolume(float x)
        {
            if(BGMTranslationVolumeCurve==null)
            {
                return Mathf.Clamp(- Mathf.Pow(x - 1, 2) * 80f, -80f, 0f);
            }
            else
            {
                x = Mathf.Clamp01(x);
                return BGMTranslationVolumeCurve.Evaluate(x);

            }
        }
        /// <summary>
        /// 设置某个音轨的音量
        /// </summary>
        /// <param name="trackId">音轨id</param>
        /// <param name="volume">0~1f的音量</param>
        private void SetTrackVolume(int trackId, float volume)
        {
            //防止越界
            if (trackId >= SoundTrackList.Count || trackId < 0) return;

            //获取路径string
            string track = "";
            switch (trackId)
            {
                case 0:
                    track = TrackBGMTranslationVolume;
                    break;
                case 1:
                    track = TrackBGM01Volume;
                    break;
                case 2:
                    track = TrackBGM02Volume;
                    break;
                case 3:
                    track = TrackBGM03Volume;
                    break;
                case 4:
                    track = TrackBGM04Volume;
                    break;
                case 5:
                    track = TrackBGM05Volume;
                    break;
                case 6:
                    track = TrackBGM06Volume;
                    break;
                default:
                    return;
            }

            //设置音量
            AudioMixer.SetFloat(track, GetVolume(volume));

            //更新字典里面的音量
            PlayingTrackDic[trackId] = volume;
        }

        #endregion

        #region 协程

        /// <summary>
        /// 开启BGM渐入
        /// </summary>
        public void StartBGMFadeIn()
        {
            //如果已经在切换了那就把先前的终止
            if (bgmTranslationCoroutine != null)
            {
                StopCoroutine(bgmTranslationCoroutine);
            }

            //开始渐入
            bgmTranslationCoroutine = StartCoroutine(FadeInBGMTranslation());
        }

        /// <summary>
        /// BGM渐入将BGMTranslation的音量慢慢调到1
        /// </summary>
        /// <returns></returns>
        private IEnumerator FadeInBGMTranslation()
        {
            //获取当前音量，要是字典里面没有那就取0f
            float v = PlayingTrackDic.ContainsKey(0) ? PlayingTrackDic[0] : 0f;
            float speed = 1f / BGMTranslationDuration;

            while(v<1f)
            {
                v += speed * Time.fixedDeltaTime;

                //更新
                SetTrackVolume(0, v);
                yield return CoroutineTool.WaitForFixedUpdate();
            }

            //把v规整为1f后再更新一下音量
            v = 1f;
            SetTrackVolume(0, v);
            yield return CoroutineTool.WaitForFixedUpdate();

            //清空协程
            bgmTranslationCoroutine = null;
        }
        /// <summary>
        /// 开启BGM渐出
        /// </summary>
        public void StartBGMFadeOut()
        {
            //如果已经在切换了那就把先前的终止
            if (bgmTranslationCoroutine != null)
            {
                StopCoroutine(bgmTranslationCoroutine);
            }

            //开始渐出
            bgmTranslationCoroutine = StartCoroutine(FadeOutBGMTranslation());
        }
        /// <summary>
        /// BGM渐出将BGMTranslation的音量慢慢调到0
        /// </summary>
        /// <returns></returns>
        private IEnumerator FadeOutBGMTranslation()
        {
            //获取当前音量，要是字典里面没有那就取1f
            float v = PlayingTrackDic.ContainsKey(0) ? PlayingTrackDic[0] : 1f;
            float speed = 1f / BGMTranslationDuration;

            while (v > 0f)
            {
                v -= speed * Time.fixedDeltaTime;

                //更新
                SetTrackVolume(0, v);
                yield return CoroutineTool.WaitForFixedUpdate();
            }

            //把v规整为0f后再更新一下音量
            v = 0f;
            SetTrackVolume(0, v);
            yield return CoroutineTool.WaitForFixedUpdate();

            //清空协程
            bgmTranslationCoroutine = null;
        }
        public void StartChangeBGM(BGMStack stack)
        {
            //如果stack和当前的一样那就返回
            if (stack == PlayingBGMStack) return;

            //如果已经在切换了那就把先前的终止
            if (bgmTranslationCoroutine != null)
            {
                StopCoroutine(bgmTranslationCoroutine);
            }

            //开始切换
            bgmTranslationCoroutine = StartCoroutine(ChangeBGMCoroutine(stack));
        }
        /// <summary>
        /// 切换BGM协程
        /// </summary>
        /// <returns></returns>
        private IEnumerator ChangeBGMCoroutine(BGMStack stack)
        {
            //如果现在已经有在播放的BGM了
            if(PlayingBGMStack != null)
            {
                //那就先渐出
                yield return FadeOutBGMTranslation();
            }

            //然后更新stack
            PlayingBGMStack = stack;

            //如果是空的那就说明直接不播放BGM了
            if(PlayingBGMStack == null)
            {
                //结束
                bgmTranslationCoroutine = null;
                yield break;
            }
            //不是空的
            else
            {
                //切换播放的BGM
                ChangePlayingBGMStack(stack);

                //开始渐入
                yield return FadeInBGMTranslation();

                //结束
                bgmTranslationCoroutine = null;
            }
        }
        /// <summary>
        /// 停止播放BGM
        /// </summary>
        public void StartBGMStopCoroutine()
        {
            //如果已经在切换了那就把先前的终止
            if (bgmTranslationCoroutine != null)
            {
                StopCoroutine(bgmTranslationCoroutine);
            }

            bgmTranslationCoroutine = StartCoroutine(StopBGMCoroutine());
        }
        /// <summary>
        /// 停止BGM的协程
        /// </summary>
        /// <returns></returns>
        private IEnumerator StopBGMCoroutine()
        {

            //渐出BGM
            yield return FadeOutBGMTranslation();

            //清空参数
            StopBGMImmediate();

            //清空
            bgmTranslationCoroutine = null;
        }
        /// <summary>
        /// 音轨切换的协程
        /// </summary>
        private IEnumerator SoundTrackTranslationCoroutine()
        {
            //计算速度
            float speedEnable = 1f / SoundTrackEnableTranlationDuration;
            float speedDisable = 1f / SoundTrackDisableTranlationDuration;

            //如果还有在启用或者关闭的音轨那就继续循环
            while(enablingPlayList.Count + disablingPlayList.Count >0)
            {
                for (int i = 0; i < enablingPlayList.Count; i++)
                {
                    //获取音量
                    float v = PlayingTrackDic.ContainsKey(enablingPlayList[i]) ? PlayingTrackDic[enablingPlayList[i]] : 0f;

                    //更新音量
                    v += speedEnable * Time.fixedDeltaTime;
                    SetTrackVolume(enablingPlayList[i], v);

                    //如果音量到达目标了，那就去掉当前音轨
                    if (v >= 1f)
                    {
                        v = 1f;
                        SetTrackVolume(enablingPlayList[i], v);
                        //添加到播放中
                        PlayingList.Add(enablingPlayList[i]);
                        //从启动中的去掉
                        enablingPlayList.RemoveAt(i);

                        i--;
                    }
                }

                for (int i = 0; i < disablingPlayList.Count; i++)
                {
                    //获取音量
                    float v = PlayingTrackDic.ContainsKey(disablingPlayList[i]) ? PlayingTrackDic[disablingPlayList[i]] : 0f;

                    //更新音量
                    v -= speedDisable * Time.fixedDeltaTime;
                    SetTrackVolume(disablingPlayList[i], v);

                    //如果音量到达目标了，那就去掉当前音轨
                    if (v <= 0f)
                    {
                        v = 0f;
                        SetTrackVolume(disablingPlayList[i], v);
                        disablingPlayList.RemoveAt(i);
                        i--;
                    }
                }

                yield return CoroutineTool.WaitForFixedUpdate();

            }

            //结束
            soundtrackTranslationCoroutine = null;

        }

        #endregion

    }
}

