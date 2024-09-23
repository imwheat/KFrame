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
using KFrame.Utilities;
using UnityEngine.Serialization;

namespace KFrame.Systems
{
    public class BGMPlayer : MonoBehaviour
    {

        #region 播放器字段

        [LabelText("播放BGM的预制体"), TabGroup("设置")] public GameObject BGMPlayPrefab;
        [LabelText("BGM切换所需时间"),TabGroup("设置")] public float BGMTranslationDuration = 5f;
        [LabelText("BGM切换音量曲线"),TabGroup("设置")] public AnimationCurve BGMTranslationVolumeCurve;
        [SerializeField ,LabelText("当前的BGMStack"), TabGroup("逻辑")] public BGMStack PlayingBGMStack;
        [SerializeField ,LabelText("播放中的BGM列表"), TabGroup("逻辑")] public List<BGMPlay> BGMPlayingList;
        [ShowInInspector, LabelText("正在启用播放的音轨List"), TabGroup("逻辑")] private List<BGMPlay> enablingPlayList;
        [ShowInInspector, LabelText("正在禁用播放的音轨List"), TabGroup("逻辑")] private List<BGMPlay> disablingPlayList;
        [SerializeField ,LabelText("bgm切换音量的协程"), TabGroup("逻辑")] private Coroutine bgmTranslationCoroutine;
        [SerializeField ,LabelText("bgm切换音量的进度"), TabGroup("逻辑")] private float bgmTranslationProgress;

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

            //列表初始化
            BGMPlayingList = new List<BGMPlay>();
            enablingPlayList = new List<BGMPlay>();
            disablingPlayList = new List<BGMPlay>();

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

        }

        #endregion

        #region 方法
        
        /// <summary>
        /// 创建获取一个BGM播放器
        /// </summary>
        /// <param name="stack">要播放的BGM</param>
        public BGMPlay GetBGMPlay(BGMStack stack)
        {
            //如果为空返回空
            if (stack == null) return null;
            
            //创建Gameobject然后设置播放的BGM
            GameObject bgmPlayObj = PoolSystem.GetOrNewGameObject(BGMPlayPrefab, transform);
            BGMPlay bgmPlay = bgmPlayObj.GetComponent<BGMPlay>();
            bgmPlay.PlayBGM(stack, AudioSystem.BGMGroup.GetMixerGroup());
            
            //返回BGMPlay
            return bgmPlay;
        }
        /// <summary>
        /// 播放BGM
        /// </summary>
        /// <param name="stack">BGM的Stack</param>
        public void PlayBGM(BGMStack stack)
        {
            //如果要播放的和现在的一样那就返回
            if (stack == PlayingBGMStack) return;

            //如果是没有BGM在播放的情况
            if(BGMPlayingList.Count == 0)
            {
                //更新stack
                PlayingBGMStack = stack;
                
                //添加入正在启用播放的列表
                enablingPlayList.Add(GetBGMPlay(stack));
                
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
            foreach (var bgmPlay in BGMPlayingList)
            {
                bgmPlay.EndPlay();
            }
            BGMPlayingList.Clear();

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
            //获取当前音量，要是差不多为1f那就取0f
            if (Mathf.Approximately(1f, bgmTranslationProgress))
            {
                bgmTranslationProgress = 0f;
            }
            float speed = 1f / BGMTranslationDuration;

            while(bgmTranslationProgress<1f)
            {
                bgmTranslationProgress += speed * Time.fixedDeltaTime;
                if (bgmTranslationProgress > 1f) bgmTranslationProgress = 1f;

                //更新
                for (int i = 0; i < enablingPlayList.Count; i++)
                {
                    enablingPlayList[i].UpdateModifyVolume(BGMTranslationVolumeCurve.Evaluate(bgmTranslationProgress));
                }
                yield return CoroutineExtensions.WaitForFixedUpdate();
            }

            //把v规整为1f后再更新一下音量
            bgmTranslationProgress = 1f;
            //放入正在播放列表
            for (int i = 0; i < enablingPlayList.Count; i++)
            {
                BGMPlayingList.Add(enablingPlayList[i]);
            }
            enablingPlayList.Clear();
            yield return CoroutineExtensions.WaitForFixedUpdate();

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
            //把正在启用和播放的BGM添加入禁用的列表
            disablingPlayList.AddRange(enablingPlayList);
            disablingPlayList.AddRange(BGMPlayingList);
            BGMPlayingList.Clear();
            enablingPlayList.Clear();
            
            //获取当前音量，要是差不多为0f那就取1f
            if (Mathf.Approximately(0f, bgmTranslationProgress))
            {
                bgmTranslationProgress = 1f;
            }
            float speed = 1f / BGMTranslationDuration;

            while (bgmTranslationProgress > 0f)
            {
                bgmTranslationProgress -= speed * Time.fixedDeltaTime;
                if (bgmTranslationProgress < 0f) bgmTranslationProgress = 0f;

                //更新
                for (int i = 0; i < disablingPlayList.Count; i++)
                {
                    disablingPlayList[i].UpdateModifyVolume(BGMTranslationVolumeCurve.Evaluate(bgmTranslationProgress));
                }
                yield return CoroutineExtensions.WaitForFixedUpdate();
            }

            //结束播放
            for (int i = 0; i < disablingPlayList.Count; i++)
            {
                disablingPlayList[i].EndPlay();
            }
            //清空播放列表
            disablingPlayList.Clear();
            yield return CoroutineExtensions.WaitForFixedUpdate();

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
            }
            //不是空的
            else
            {
                //添加入正在启用播放的列表
                enablingPlayList.Add(GetBGMPlay(stack));
                
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


        #endregion

    }
}

