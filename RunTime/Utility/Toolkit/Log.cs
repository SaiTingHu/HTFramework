using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 日志工具箱
    /// </summary>
    public static class Log
    {
        private static readonly string InfoPrefix = "<b><color=cyan>[HTFramework.Info]</color></b> ";
        private static readonly string WarningPrefix = "<b><color=yellow>[HTFramework.Warning]</color></b> ";
        private static readonly string ErrorPrefix = "<b><color=red>[HTFramework.Error]</color></b> ";

        /// <summary>
        /// 打印信息日志
        /// </summary>
        /// <param name="content">日志内容</param>
        public static void Info(this string content)
        {
#if UNITY_EDITOR
            Debug.Log(InfoPrefix + content);
#else
            if (Main.Current.IsEnabledLogInfo)
            {
                Debug.Log(content);
            }
#endif
        }
        /// <summary>
        /// 打印警告日志
        /// </summary>
        /// <param name="content">日志内容</param>
        public static void Warning(this string content)
        {
#if UNITY_EDITOR
            Debug.LogWarning(WarningPrefix + content);
#else
            if (Main.Current.IsEnabledLogWarning)
            {
                Debug.LogWarning(content);
            }
#endif
        }
        /// <summary>
        /// 打印错误日志
        /// </summary>
        /// <param name="content">日志内容</param>
        public static void Error(this string content)
        {
#if UNITY_EDITOR
            Debug.LogError(ErrorPrefix + content);
#else
            if (Main.Current.IsEnabledLogError)
            {
                Debug.LogError(content);
            }
#endif
        }
    }
}