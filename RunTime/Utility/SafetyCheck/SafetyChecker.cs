using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 性能及安全性检查程序
    /// </summary>
    public static class SafetyChecker
    {
        /// <summary>
        /// 安全性警告前缀
        /// </summary>
        private const string WarningPrefix = "<b><color=yellow>[Safety Warning]</color></b> ";
        /// <summary>
        /// 检查的目标总数量
        /// </summary>
        public static int CheckCount { get; private set; }
        /// <summary>
        /// 检查通过的目标数量
        /// </summary>
        public static int PassCount { get; private set; }

        /// <summary>
        /// 执行性能及安全性检查程序
        /// </summary>
        /// <param name="target">检查目标</param>
        /// <param name="args">参数</param>
        public static void DoSafetyCheck(ISafetyCheckTarget target, params object[] args)
        {
            CheckCount += 1;
            bool pass = target.OnSafetyCheck(args);
            if (pass)
            {
                PassCount += 1;
            }
        }
        /// <summary>
        /// 执行安全性警告
        /// </summary>
        /// <param name="content">警告内容</param>
        public static void DoSafetyWarning(string content)
        {
            Debug.LogWarning(WarningPrefix + content);
        }
    }
}