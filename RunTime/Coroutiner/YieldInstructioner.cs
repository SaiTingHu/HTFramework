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
        private static WaitForEndOfFrame CurrentWaitForEndOfFrame = new WaitForEndOfFrame();
        private static WaitForFixedUpdate CurrentWaitForFixedUpdate = new WaitForFixedUpdate();
        private static Dictionary<string, WaitForSeconds> AllWaitForSeconds = new Dictionary<string, WaitForSeconds>();
        private static Dictionary<string, WaitForSecondsRealtime> AllWaitForSecondsRealtimes = new Dictionary<string, WaitForSecondsRealtime>();

        /// <summary>
        /// 获取WaitForEndOfFrame对象
        /// </summary>
        /// <returns>WaitForEndOfFrame对象</returns>
        public static WaitForEndOfFrame GetWaitForEndOfFrame()
        {
            return CurrentWaitForEndOfFrame;
        }
        /// <summary>
        /// 获取WaitForFixedUpdate对象
        /// </summary>
        /// <returns>WaitForFixedUpdate对象</returns>
        public static WaitForFixedUpdate GetWaitForFixedUpdate()
        {
            return CurrentWaitForFixedUpdate;
        }
        /// <summary>
        /// 获取WaitForSeconds对象
        /// </summary>
        /// <param name="second">等待的秒数</param>
        /// <returns>WaitForSeconds对象</returns>
        public static WaitForSeconds GetWaitForSeconds(float second)
        {
            string secondStr = second.ToString("F2");
            if (!AllWaitForSeconds.ContainsKey(secondStr))
            {
                AllWaitForSeconds.Add(secondStr, new WaitForSeconds(second));
            }
            return AllWaitForSeconds[secondStr];
        }
        /// <summary>
        /// 获取WaitForSecondsRealtime对象
        /// </summary>
        /// <param name="second">等待的秒数</param>
        /// <returns>WaitForSecondsRealtime对象</returns>
        public static WaitForSecondsRealtime GetWaitForSecondsRealtime(float second)
        {
            string secondStr = second.ToString("F2");
            if (!AllWaitForSecondsRealtimes.ContainsKey(secondStr))
            {
                AllWaitForSecondsRealtimes.Add(secondStr, new WaitForSecondsRealtime(second));
            }
            return AllWaitForSecondsRealtimes[secondStr];
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
        /// <summary>
        /// 清空缓存池
        /// </summary>
        public static void ClearPool()
        {
            AllWaitForSeconds.Clear();
            AllWaitForSecondsRealtimes.Clear();
        }
    }
}