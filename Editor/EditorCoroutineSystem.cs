//****************** 代码文件申明 ***********************
//* 文件：EditorCoroutineSystem
//* 作者：wheat
//* 创建时间：2024/10/20 09:59:06 星期日
//* 描述：在编辑器使用的Coroutine
//*******************************************************

using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace KFrame.Editor
{
    [InitializeOnLoad]
    public static class EditorCoroutineSystem
    {
        #region 参数

        /// <summary>
        /// 正在运作的协程
        /// </summary>
        private static readonly List<EditorCoroutine> Coroutines = new();
        /// <summary>
        /// 等待字典
        /// </summary>
        private static readonly Dictionary<WaitForSeconds, float> WaitTimeDic = new();

        #endregion

        #region 反射引用

        /// <summary>
        /// 等待时间的FieldInfo
        /// </summary>
        private static readonly FieldInfo WaitTimeSecondFieldInfo =
            typeof(WaitForSeconds).GetField("m_Seconds", BindingFlags.NonPublic | BindingFlags.Instance);

        #endregion
        
        #region 初始化

        static EditorCoroutineSystem()
        {
            EditorApplication.update += Update;
        }

        #endregion

        #region 生命周期

        private static void Update()
        {
            //遍历每个协程
            for (int i = Coroutines.Count - 1; i >= 0; i--)
            {
                //然后进行下一步迭代
                if (!Coroutines[i].MoveNext())
                {
                    //如果结束了，那就从列表移除
                    Coroutines.RemoveAt(i);
                }
            }
        }

        #endregion

        #region 协程控制
        
        /// <summary>
        /// 开启协程
        /// </summary>
        /// <param name="enumerator"></param>
        public static EditorCoroutine StartCoroutine(IEnumerator enumerator)
        {
            //新建协程，然后加入队列
            EditorCoroutine coroutine = new EditorCoroutine(enumerator);
            Coroutines.Add(coroutine);

            //返回协程
            return coroutine;
        }
        /// <summary>
        /// 结束协程
        /// </summary>
        /// <param name="coroutine">要结束的协程</param>
        public static void StopCoroutine(EditorCoroutine coroutine)
        {
            Coroutines.Remove(coroutine);
        }
        /// <summary>
        /// 结束所有协程
        /// </summary>
        public static void StopAllCoroutines()
        {
            Coroutines.Clear();
        }

        #endregion
        
        #region 协程判断

        /// <summary>
        /// 判断等待是否完成了
        /// </summary>
        /// <param name="wait"></param>
        /// <returns></returns>
        internal static bool IsFinished(this WaitForSeconds wait)
        {
            //如果有记录数据了
            if (WaitTimeDic.TryGetValue(wait, out float startTime))
            {
                //那就判断检测是否已经结束
                if (Time.realtimeSinceStartup - startTime >= (float)WaitTimeSecondFieldInfo.GetValue(wait))
                {
                    //移除记录数
                    WaitTimeDic.Remove(wait);
                    
                    //完成了那就返回true
                    return true;
                }
                //没结束就继续等待
                else
                {
                    return false;
                }
            }
            //没有记录数据
            else
            {
                //那就记录当前的时间
                WaitTimeDic[wait] = Time.realtimeSinceStartup;
                return false;
            }
            
        }

        #endregion
    }
}

