//****************** 代码文件申明 ***********************
//* 文件：EditorCoroutine
//* 作者：wheat
//* 创建时间：2024/10/20 10:05:26 星期日
//* 描述：编辑器协程类，在编辑器使用的协程
//*******************************************************

using System.Collections;
using UnityEngine;

namespace KFrame.Editor
{
    public class EditorCoroutine
    {
        private readonly IEnumerator _routine;
        private object _current;

        public EditorCoroutine(IEnumerator routine)
        {
            this._routine = routine;
        }

        /// <summary>
        /// 执行协程的下一步
        /// </summary>
        /// <returns>如果没有下一步了那就返回false</returns>
        public bool MoveNext()
        {
            //如果协程为空直接返回false，执行完毕
            if (_routine == null) return false;

            // 检查是否等待完成
            if (_current is WaitForSeconds wait)
            {
                if (!wait.IsFinished())
                    return true; // 等待还没结束，继续等待
            }

            // 继续执行协程的下一步
            if (_routine.MoveNext())
            {
                _current = _routine.Current;
                return true;
            }

            //执行完毕返回false
            return false;
        }
    }
}

