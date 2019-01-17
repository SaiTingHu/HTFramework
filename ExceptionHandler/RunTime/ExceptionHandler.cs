using UnityEngine;
using System;
using System.IO;
using System.Diagnostics;

namespace HT.Framework
{
    /// <summary>
    /// 异常处理模块
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ExceptionHandler : ModuleManager
    {
        /// <summary>
        /// 是否作为异常处理者
        /// </summary>
        public bool IsHandler = false;
        /// <summary>
        /// 是否退出程序当异常发生时
        /// </summary>
        public bool IsQuitWhenException = true;
        
        //异常日志保存路径（文件夹）
        private string LogPath;
        //Bug反馈程序的启动路径
        private string BugExePath;

        public override void Initialization()
        {
            LogPath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/")) + "/Log";
            BugExePath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/")) + "/Bug.exe";

#if !UNITY_EDITOR
            if (!Directory.Exists(LogPath))
            {
                Directory.CreateDirectory(LogPath);
            }

            //注册异常处理委托
            if (IsHandler)
            {
                Application.logMessageReceived += Handler;
            }
#endif
        }

        private void Handler(string logString, string stackTrace, LogType type)
        {
            if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
            {
                OnException(logString, stackTrace, type);

                string logPath = LogPath + "\\" + DateTime.Now.ToString("yyyy_MM_dd HH_mm_ss") + ".log";
                //打印日志
                File.AppendAllText(logPath, "[time]:" + DateTime.Now.ToString() + "\r\n");
                File.AppendAllText(logPath, "[type]:" + type.ToString() + "\r\n");
                File.AppendAllText(logPath, "[exception message]:" + logString + "\r\n");
                File.AppendAllText(logPath, "[stack trace]:" + stackTrace + "\r\n");

                //启动bug反馈程序
                if (File.Exists(BugExePath))
                {
                    ProcessStartInfo pros = new ProcessStartInfo();
                    pros.FileName = BugExePath;
                    pros.Arguments = "\"" + logPath + "\"";
                    Process pro = new Process();
                    pro.StartInfo = pros;
                    pro.Start();
                }
                else
                {
                    File.AppendAllText(logPath, "[bug exe]:Doesn't find bug exe!path: " + BugExePath);
                }

                //退出程序，bug反馈程序重启主程序
                if (IsQuitWhenException)
                {
                    Application.Quit();
                }
            }
        }

        private void OnException(string logString, string stackTrace, LogType type)
        {
            Main.m_Event.Throw(this, Main.m_ReferencePool.Spawn<ExceptionEvent>().Fill(logString, stackTrace, type));
        }
    }
}