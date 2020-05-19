using System;
using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// YieldInstruction创建者
    /// </summary>
    public static class YieldInstructioner
    {
        private static WaitForEndOfFrame _waitForEndOfFrame = new WaitForEndOfFrame();
        private static WaitForFixedUpdate _waitForFixedUpdate = new WaitForFixedUpdate();
        private static Dictionary<string, WaitForSeconds> _waitForSeconds = new Dictionary<string, WaitForSeconds>();
        private static Dictionary<string, WaitForSecondsRealtime> _waitForSecondsRealtime = new Dictionary<string, WaitForSecondsRealtime>();

        /// <summary>
        /// 获取WaitForEndOfFrame对象
        /// </summary>
        /// <returns>WaitForEndOfFrame对象</returns>
        public static WaitForEndOfFrame GetWaitForEndOfFrame()
        {
            return _waitForEndOfFrame;
        }
        /// <summary>
        /// 获取WaitForFixedUpdate对象
        /// </summary>
        /// <returns>WaitForFixedUpdate对象</returns>
        public static WaitForFixedUpdate GetWaitForFixedUpdate()
        {
            return _waitForFixedUpdate;
        }
        /// <summary>
        /// 获取WaitForSeconds对象
        /// </summary>
        /// <param name="second">等待的秒数</param>
        /// <returns>WaitForSeconds对象</returns>
        public static WaitForSeconds GetWaitForSeconds(float second)
        {
            string secondStr = second.ToString("F2");
            if (!_waitForSeconds.ContainsKey(secondStr))
            {
                _waitForSeconds.Add(secondStr, new WaitForSeconds(second));
            }
            return _waitForSeconds[secondStr];
        }
        /// <summary>
        /// 获取WaitForSecondsRealtime对象
        /// </summary>
        /// <param name="second">等待的秒数</param>
        /// <returns>WaitForSecondsRealtime对象</returns>
        public static WaitForSecondsRealtime GetWaitForSecondsRealtime(float second)
        {
            string secondStr = second.ToString("F2");
            if (!_waitForSecondsRealtime.ContainsKey(secondStr))
            {
                _waitForSecondsRealtime.Add(secondStr, new WaitForSecondsRealtime(second));
            }
            return _waitForSecondsRealtime[secondStr];
        }
        /// <summary>
        /// 获取WaitUntil对象
        /// </summary>
        /// <param name="predicate">判断的委托</param>
        /// <returns>WaitUntil对象</returns>
        public static WaitUntil GetWaitUntil(Func<bool> predicate)
        {
            return new WaitUntil(predicate);
        }
        /// <summary>
        /// 获取WaitWhile对象
        /// </summary>
        /// <param name="predicate">判断的委托</param>
        /// <returns>WaitWhile对象</returns>
        public static WaitWhile GetWaitWhile(Func<bool> predicate)
        {
            return new WaitWhile(predicate);
        }
    }
}