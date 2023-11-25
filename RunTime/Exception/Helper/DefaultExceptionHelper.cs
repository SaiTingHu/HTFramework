using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 默认的异常处理器助手
    /// </summary>
    internal sealed class DefaultExceptionHelper : IExceptionHelper
    {
        /// <summary>
        /// 异常处理器
        /// </summary>
        private ExceptionManager _exceptionManager;
        /// <summary>
        /// 异常日志Builder
        /// </summary>
        private StringBuilder _logInfoBuilder = new StringBuilder();
        /// <summary>
        /// 异常日志保存路径
        /// </summary>
        private string _logPath;
        /// <summary>
        /// 邮件发送者
        /// </summary>
        private EmailSender _sender;
        /// <summary>
        /// 回发邮件缓冲计时器
        /// </summary>
        private float _reportBufferTimer = 0;

        /// <summary>
        /// 所属的内置模块
        /// </summary>
        public IModuleManager Module { get; set; }
        /// <summary>
        /// 当前捕获的所有异常信息
        /// </summary>
        public List<ExceptionInfo> ExceptionInfos { get; private set; } = new List<ExceptionInfo>();
        
        /// <summary>
        /// 初始化助手
        /// </summary>
        public void OnInit()
        {
            _exceptionManager = Module as ExceptionManager;

#if UNITY_EDITOR
            _exceptionManager.IsHandler = false;
#endif

#if UNITY_STANDALONE_WIN
            _exceptionManager.FeedbackProgramPath = PathToolkit.ProjectPath + _exceptionManager.FeedbackProgramPath;
            _logPath = PathToolkit.ProjectPath + "Logs";
            if (!Directory.Exists(_logPath))
            {
                Directory.CreateDirectory(_logPath);
            }
#endif
            if (_exceptionManager.IsHandler)
            {
                Application.logMessageReceived += Handler;

                if (_exceptionManager.IsEnableMailReport)
                {
                    _sender = new EmailSender(_exceptionManager.SendMailbox, _exceptionManager.SendMailboxPassword, _exceptionManager.ReceiveMailbox, _exceptionManager.Host, _exceptionManager.Port);
                }
            }
        }
        /// <summary>
        /// 助手准备工作
        /// </summary>
        public void OnReady()
        {

        }
        /// <summary>
        /// 刷新助手
        /// </summary>
        public void OnUpdate()
        {
            if (_reportBufferTimer > 0)
            {
                _reportBufferTimer -= Time.deltaTime;
            }
        }
        /// <summary>
        /// 终结助手
        /// </summary>
        public void OnTerminate()
        {
            if (_exceptionManager.IsHandler)
            {
                Application.logMessageReceived -= Handler;

                if (_sender != null)
                {
                    _sender.Dispose();
                    _sender = null;
                }
            }
        }
        /// <summary>
        /// 暂停助手
        /// </summary>
        public void OnPause()
        {

        }
        /// <summary>
        /// 恢复助手
        /// </summary>
        public void OnResume()
        {

        }

        /// <summary>
        /// 回发邮件
        /// </summary>
        /// <param name="subject">邮件标题</param>
        /// <param name="body">邮件内容</param>
        public void ReportMail(string subject, string body)
        {
            if (_reportBufferTimer > 0)
                return;

            _reportBufferTimer = _exceptionManager.ReportBufferTime;

            if (_exceptionManager.IsHandler)
            {
                if (_sender != null)
                {
                    _sender.Send(subject, body);
                }
            }
        }
        /// <summary>
        /// 清理所有异常信息
        /// </summary>
        public void ClearExceptionInfos()
        {
            Main.m_ReferencePool.Despawns(ExceptionInfos);
        }

        private void Handler(string logString, string stackTrace, LogType type)
        {
            if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
            {
                ExceptionInfos.Add(Main.m_ReferencePool.Spawn<ExceptionInfo>().Fill(logString, stackTrace, type));

                OnException(logString, stackTrace, type);

                _logInfoBuilder.Clear();
                _logInfoBuilder.Append($"[time]:{DateTime.Now}\r\n\r\n");
                _logInfoBuilder.Append($"[type]:{type}\r\n\r\n");
                _logInfoBuilder.Append($"[message]:{logString}\r\n\r\n");
                _logInfoBuilder.Append($"[stack trace]:{stackTrace}\r\n\r\n");

#if UNITY_STANDALONE_WIN
                string logPath = $"{_logPath}/{DateTime.Now.ToString("yyyy_MM_dd HH_mm_ss_fff")}.log";
                File.AppendAllText(logPath, _logInfoBuilder.ToString(), Encoding.UTF8);

                if (_exceptionManager.IsEnableFeedback)
                {
                    if (File.Exists(_exceptionManager.FeedbackProgramPath))
                    {
                        ProcessStartInfo process = new ProcessStartInfo();
                        process.FileName = _exceptionManager.FeedbackProgramPath;
                        process.Arguments = $"\"{logPath}\"";
                        Process pro = new Process();
                        pro.StartInfo = process;
                        pro.Start();
                    }
                    else
                    {
                        File.AppendAllText(logPath, $"[feedback]:Doesn't find feedback program!path: {_exceptionManager.FeedbackProgramPath}\r\n", Encoding.UTF8);
                    }
                    Application.Quit();
                }
#endif
                if (_exceptionManager.IsEnableMailReport)
                {
                    ReportMail($"{Application.productName}.Exception.{DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss")}", _logInfoBuilder.ToString());
                }
            }
        }
        private void OnException(string logString, string stackTrace, LogType type)
        {
            Main.m_Event.Throw(Main.m_ReferencePool.Spawn<EventException>().Fill(logString, stackTrace, type));
        }
    }
}