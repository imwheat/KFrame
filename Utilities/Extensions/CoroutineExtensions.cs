using System;
using System.Collections;
using UnityEngine;

namespace KFrame.Utilities
{
    /// <summary>
    /// 提供在协程中使用的工具方法。
    /// <para>
    /// 特点：
    /// 1. 通过一个单例返回实例来避免频繁的垃圾回收（GC）。
    /// 2. 性能改进：使用 YieldInstruction 作为 yield 指令，减少内存和性能开销。
    /// 3. 代码简洁：预先创建的 yield 指令实例可确保代码更加简洁。
    /// 4. 潜在问题：预先创建的 YieldInstruction 实例可能导致意外行为，因为共享同一实例。其他代码修改实例可能影响其他协程。
    /// </para>
    /// </summary>
    public static class CoroutineExtensions
    {
        private struct WaitForFrameStruct : IEnumerator
        {
            public object Current => null;

			public bool MoveNext() => false;

            public void Reset() { }
        }

        private static WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
        private static WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

        public static WaitForEndOfFrame WaitForEndOfFrame() => waitForEndOfFrame;
        public static WaitForFixedUpdate WaitForFixedUpdate() => waitForFixedUpdate;


        /// <summary>
        /// 创建一个等待指定时间的协程。
        /// </summary>
        /// <param name="time">要等待的时间（以秒为单位）。</param>
        /// <returns>等待的协程。</returns>
        public static IEnumerator WaitForSeconds(float time)
        {
            float currTime = 0;
            while (currTime < time)
            {
                currTime += Time.deltaTime;
                yield return new WaitForFrameStruct();
            }
        }

        /// <summary>
        /// 创建一个等待指定时间的协程，事件到后调用回调函数
        /// </summary>
        /// <param name="time">要等待的时间（以秒为单位）。</param>
        /// <param name="callback">回调</param>
        /// <returns>等待的协程。</returns>
        public static IEnumerator WaitForSecondsCallback(float time, Action callback)
        {
            float currTime = 0;
            while (currTime < time)
            {
                currTime += Time.deltaTime;
                yield return new WaitForFrameStruct();
            }
            
            callback?.Invoke();
        }
        /// <summary>
        /// 创建一个在实际时间上等待指定时间的协程。
        /// </summary>
        /// <param name="time">要等待的时间（以秒为单位）。</param>
        /// <returns>等待的协程。</returns>
        public static IEnumerator WaitForSecondsRealtime(float time)
        {
            float currTime = 0;
            while (currTime < time)
            {
                currTime += Time.unscaledDeltaTime;
                yield return new WaitForFrameStruct();
            }
        }

        /// <summary>
        /// 创建一个在实际时间上等待指定时间的协程，时间到后调用回调
        /// </summary>
        /// <param name="time">要等待的时间（以秒为单位）。</param>
        /// <param name="callback">回调</param>
        /// <returns>等待的协程。</returns>
        public static IEnumerator WaitForSecondsRealtimeCallback(float time, Action callback)
        {
            float currTime = 0;
            while (currTime < time)
            {
                currTime += Time.unscaledDeltaTime;
                yield return new WaitForFrameStruct();
            }
            
            callback?.Invoke();
        }
        /// <summary>
        /// 创建一个等待一帧的协程
        /// </summary>
        /// <returns></returns>
        public static IEnumerator WaitForFrame()
        {
            yield return new WaitForFrameStruct();
        }

        /// <summary>
        /// 创建一个在指定帧数内等待的协程。
        /// </summary>
        /// <param name="count">要等待的帧数。</param>
        /// <returns>等待的协程。</returns>
        public static IEnumerator WaitForFrames(int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                yield return new WaitForFrameStruct();
            }
        }
    }
}