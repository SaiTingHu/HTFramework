using System.Collections;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 延时处理工具箱
    /// </summary>
    public static class DelayToolkit
    {
        /// <summary>
        /// 延时执行
        /// </summary>
        /// <param name="behaviour">执行者</param>
        /// <param name="action">执行的代码</param>
        /// <param name="delaySeconds">延时的秒数</param>
        /// <returns>延时的协程</returns>
        public static Coroutine DelayExecute(this MonoBehaviour behaviour, HTFAction action, float delaySeconds)
        {
            if (behaviour == null || action == null)
                return null;

            Coroutine coroutine = behaviour.StartCoroutine(DelayExecute(action, delaySeconds));
            return coroutine;
        }
        private static IEnumerator DelayExecute(HTFAction action, float delaySeconds)
        {
            yield return YieldInstructioner.GetWaitForSeconds(delaySeconds);
            action();
        }

        /// <summary>
        /// 下一帧执行
        /// </summary>
        /// <param name="behaviour">执行者</param>
        /// <param name="action">执行的代码</param>
        /// <returns>延时的协程</returns>
        public static Coroutine NextFrameExecute(this MonoBehaviour behaviour, HTFAction action)
        {
            if (behaviour == null || action == null)
                return null;

            Coroutine coroutine = behaviour.StartCoroutine(NextFrameExecute(action));
            return coroutine;
        }
        private static IEnumerator NextFrameExecute(HTFAction action)
        {
            yield return null;
            action();
        }

        /// <summary>
        /// 等待执行
        /// </summary>
        /// <param name="behaviour">执行者</param>
        /// <param name="action">执行的代码</param>
        /// <param name="waitUntil">等待的WaitUntil</param>
        /// <returns>等待的协程</returns>
        public static Coroutine WaitExecute(this MonoBehaviour behaviour, HTFAction action, WaitUntil waitUntil)
        {
            if (behaviour == null || action == null)
                return null;

            Coroutine coroutine = behaviour.StartCoroutine(WaitExecute(action, waitUntil));
            return coroutine;
        }
        private static IEnumerator WaitExecute(HTFAction action, WaitUntil waitUntil)
        {
            yield return waitUntil;
            action();
        }
    }
}