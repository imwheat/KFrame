using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using KFrame;
using KFrame.Systems;

namespace KFrame.Utilities
{
    //基于协程的定时器

    public partial class GameTimeTool
    {
        public static Coroutine WaitTime(float time, UnityAction callBack)
        {
            return MonoSystem.Start_Coroutine(TimeCoroutine(time, callBack));
        }

        public static void CancelWait(ref Coroutine coroutine)
        {
            if (coroutine == null && MonoSystem.Instance is null) return;
            MonoSystem.Stop_Coroutine(coroutine);
            coroutine = null;
        }

        private static IEnumerator TimeCoroutine(float time, UnityAction callBack)
        {
            yield return CoroutineExtensions.WaitForSeconds(time);
            callBack?.Invoke();
        }
    }
}