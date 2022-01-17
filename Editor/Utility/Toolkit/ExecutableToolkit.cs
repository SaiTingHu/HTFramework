using System;
using System.Diagnostics;
using System.IO;

namespace HT.Framework
{
    /// <summary>
    /// 可执行程序工具箱
    /// </summary>
    internal static class ExecutableToolkit
    {
        /// <summary>
        /// 打开一个可执行程序
        /// </summary>
        /// <param name="fullPath">可执行程序完整路径</param>
        /// <param name="args">参数</param>
        /// <returns>是否成功打开</returns>
        public static bool Execute(string fullPath, string args = null)
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix
               || Environment.OSVersion.Platform == PlatformID.Xbox
               || Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                Log.Error("当前平台不支持打开外部可执行程序！");
                return false;
            }

            if (!File.Exists(fullPath))
                return false;
            
            Process process = new Process();
            process.StartInfo = new ProcessStartInfo(fullPath, args);
            process.Start();
            return true;
        }
        /// <summary>
        /// 打开一个资源管理器
        /// </summary>
        /// <param name="args">参数</param>
        /// <returns>是否成功打开</returns>
        public static bool ExecuteExplorer(string args)
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix
               || Environment.OSVersion.Platform == PlatformID.Xbox
               || Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                Log.Error("当前平台不支持打开外部可执行程序！");
                return false;
            }

            Process process = new Process();
            process.StartInfo = new ProcessStartInfo("Explorer.exe", args);
            process.Start();
            return true;
        }
    }
}