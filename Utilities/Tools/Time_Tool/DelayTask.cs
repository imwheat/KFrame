using System;

namespace KFrame.Utilities
{
    /// <summary>
    /// 延迟任务
    /// </summary>
    public class DelayTask
    {
        /// <summary>
        /// 任务状态
        /// </summary>
        public enum State
        {
            Start,
            Pause,
            Finished
        }

        /// <summary>
        /// 计时任务结束时的回调
        /// </summary>
        private Action taskCallBack;

        /// <summary>
        /// 剩余时间
        /// </summary>
        private float timeRemaining;

        /// <summary>
        /// 延迟时间
        /// </summary>
        private float delayTime;

        /// <summary>
        /// 是否循环
        /// </summary>
        private bool isLoop;
        
        /// <summary>
        /// 任务状态
        /// </summary>
        private State _state = State.Finished;

        /// <summary>
        /// 是否完成任务
        /// </summary>
        public bool IsFinish => _state == State.Finished;

        // /// <summary>
        // /// 开始计时
        // /// </summary>
        // public void StartTask(float de)
        // {
        //     
        // }

    }
}