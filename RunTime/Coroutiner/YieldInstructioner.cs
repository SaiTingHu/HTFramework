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

        public static WaitForEndOfFrame GetWaitForEndOfFrame()
        {
            return _waitForEndOfFrame;
        }
        public static WaitForFixedUpdate GetWaitForFixedUpdate()
        {
            return _waitForFixedUpdate;
        }
        public static WaitForSeconds GetWaitForSeconds(float second)
        {
            string secondStr = second.ToString("F2");
            if (!_waitForSeconds.ContainsKey(secondStr))
            {
                _waitForSeconds.Add(secondStr, new WaitForSeconds(second));
            }
            return _waitForSeconds[secondStr];
        }
        public static WaitForSecondsRealtime GetWaitForSecondsRealtime(float second)
        {
            string secondStr = second.ToString("F2");
            if (!_waitForSecondsRealtime.ContainsKey(secondStr))
            {
                _waitForSecondsRealtime.Add(secondStr, new WaitForSecondsRealtime(second));
            }
            return _waitForSecondsRealtime[secondStr];
        }
        public static WaitUntil GetWaitUntil(Func<bool> predicate)
        {
            return new WaitUntil(predicate);
        }
        public static WaitWhile GetWaitWhile(Func<bool> predicate)
        {
            return new WaitWhile(predicate);
        }
    }
}
