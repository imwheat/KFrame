using Sirenix.OdinInspector;
using UnityEngine;

namespace KFrame.Utilities
{
    /// <summary>
    /// 性能数据的显示模式
    /// </summary>
    public enum DisplayMode
    {
        FPS,
        MS,
    }

    /// <summary>
    /// 性能数据显示工具 支持显示最好值与最差值 并可以显示FPS和MS
    /// 左Ctrl+F 显示FPS 左Ctrl+M 显示MS
    /// </summary>
    public class FrameRateHelp : MonoBehaviour
    {
        #region 序列化参数

        /// <summary>
        /// 更新显示帧率的时间间隔
        /// </summary>
        [SerializeField, Range(0.1f, 2f)] float sampleDuration = 1f;

        [SerializeField, Tooltip("是否使用键盘快捷键Ctrl+F/M/H/C 等进行打包后操作")] private bool isUserShortCutKey = true;


        /// <summary>
        /// 是否隐藏
        /// </summary>
        [SerializeField] private bool isHide;

        [SerializeField] DisplayMode displayMode = DisplayMode.FPS;

        private GUIStyle style = new GUIStyle();

        /// <summary>
        /// 是否控制目标帧率
        /// </summary>
        [SerializeField] private bool isControlTargetFrameRate = false;

        /// <summary>
        /// 限制最大帧率
        /// 默认100
        /// </summary>
        [SerializeField] private int maxFrameRate = 100;

        [SerializeField] private int showFontSize = 13;

        #endregion

        /// <summary>
        /// 帧间间隔
        /// </summary>
        private float duration;

        /// <summary>
        /// 最好帧间隔
        /// </summary>
        private float bestDuration = float.MaxValue;

        /// <summary>
        /// 最差帧间隔
        /// </summary>
        private float worstDuration = 0;

        /// <summary>
        /// 帧数
        /// </summary>
        private int frames = 0;

        /// <summary>
        /// 平均帧数
        /// </summary>
        private float curValue = 0;

        /// <summary>
        /// 最好帧数
        /// </summary>
        private float bestValue;

        /// <summary>
        /// 最差帧数
        /// </summary>
        private float worstValue;


        private void Awake()
        {
            Application.targetFrameRate = isControlTargetFrameRate ? maxFrameRate : -1;
        }

        private void Start()
        {
            style.fontSize = showFontSize;
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = Color.red;
        }

        private void Update()
        {
            //打包后使用键盘控制模式切换
            if (isUserShortCutKey)
            {
                if (Input.GetKeyDown(KeyCode.F) && Input.GetKey(KeyCode.LeftControl))
                {
                    displayMode = DisplayMode.FPS;
                }

                if (Input.GetKeyDown(KeyCode.M) && Input.GetKey(KeyCode.LeftControl))
                {
                    displayMode = DisplayMode.MS;
                }

                if (Input.GetKeyDown(KeyCode.C) && Input.GetKey(KeyCode.LeftControl))
                {
                    isControlTargetFrameRate = !isControlTargetFrameRate;
                }

                if (Input.GetKeyDown(KeyCode.H) && Input.GetKey(KeyCode.LeftControl))
                {
                    isHide = !isHide;
                }
            }


            //帧时间
            float frameDuration = Time.unscaledDeltaTime;
            //更新帧
            frames++;
            duration += frameDuration;

            if (frameDuration < bestDuration)
            {
                bestDuration = frameDuration;
            }

            if (frameDuration > worstDuration)
            {
                worstDuration = frameDuration;
            }

            //每隔sampleDuration刷新帧数数值
            if (duration >= sampleDuration)
            {
                if (displayMode == DisplayMode.FPS)
                {
                    bestValue = 1f / bestDuration;
                    curValue = frames / duration;
                    worstValue = 1f / worstDuration;
                }
                else
                {
                    bestValue = 1000f * bestDuration;
                    curValue = 1000f * duration / frames;
                    worstValue = 1000f * worstDuration;
                }

                frames = 0;
                duration = 0f;
                bestDuration = float.MaxValue;
                worstDuration = 0f;
            }
        }

        [Button("是否锁定帧率观察")]
        private void IsControlTargetFrameRate(int targetMaxFrameRate = 0)
        {
            if (targetMaxFrameRate != 0)
            {
                maxFrameRate = targetMaxFrameRate;
            }

            isControlTargetFrameRate = !isControlTargetFrameRate;
            Application.targetFrameRate = isControlTargetFrameRate ? maxFrameRate : -1;
        }

        private void OnGUI()
        {
            if (!isHide)
            {
                GUILayout.BeginVertical("box");
                if (displayMode == DisplayMode.FPS)
                {
                    GUILayout.Label("Best  : " + bestValue, style);
                    GUILayout.Label("FPS   : " + curValue, style);
                    GUILayout.Label("Worst: " + worstValue, style);
                }
                else
                {
                    GUILayout.Label("Best  : " + bestValue, style);
                    GUILayout.Label("MS    : " + curValue, style);
                    GUILayout.Label("Worst: " + worstValue, style);
                }

                GUILayout.EndVertical();
            }
        }
    }
}