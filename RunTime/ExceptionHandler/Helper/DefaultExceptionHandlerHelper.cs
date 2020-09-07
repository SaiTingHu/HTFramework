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
    public sealed class DefaultExceptionHandlerHelper : IExceptionHandlerHelper
    {
        //异常处理器
        private ExceptionHandler _module;
        //异常日志Builder
        private StringBuilder _logInfoBuilder = new StringBuilder();
        //异常日志保存路径
        private string _logPath;
        //邮件发送者
        private EmailSender _sender;
        //回发邮件缓冲计时器
        private float _reportBufferTimer = 0;

        /// <summary>
        /// 异常处理器
        /// </summary>
        public InternalModuleBase Module { get; set; }
        /// <summary>
        /// 当前捕获的所有异常信息
        /// </summary>
        public List<ExceptionInfo> ExceptionInfos { get; private set; } = new List<ExceptionInfo>();
        
        /// <summary>
        /// 初始化助手
        /// </summary>
        public void OnInitialization()
        {
            _module = Module as ExceptionHandler;

#if UNITY_EDITOR
            _module.IsHandler = false;
#endif

#if UNITY_STANDALONE_WIN
            _module.FeedbackProgramPath = GlobalTools.GetDirectorySameLevelOfAssets(_module.FeedbackProgramPath);
            _logPath = GlobalTools.GetDirectorySameLevelOfAssets("/Log");
            if (!Directory.Exists(_logPath))
            {
                Directory.CreateDirectory(_logPath);
            }
#endif
            if (_module.IsHandler)
            {
                Application.logMessageReceived += Handler;

                if (_module.IsEnableMailReport)
                {
                    _sender = new EmailSender(_module.SendMailbox, _module.SendMailboxPassword, _module.ReceiveMailbox, _module.Host, _module.Port);
                }
            }
        }
        /// <summary>
        /// 助手准备工作
        /// </summary>
        public void OnPreparatory()
        {

        }
        /// <summary>
        /// 刷新助手
        /// </summary>
        public void OnRefresh()
        {
            if (_reportBufferTimer > 0)
            {
                _reportBufferTimer -= Time.deltaTime;
            }
        }
        /// <summary>
        /// 终结助手
        /// </summary>
        public void OnTermination()
        {
            if (_module.IsHandler)
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
        public void OnUnPause()
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
            {
                return;
            }
            _reportBufferTimer = _module.ReportBufferTime;

            if (_module.IsHandler)
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
                _logInfoBuilder.Append("[time]:" + DateTime.Now.ToString() + "\r\n\r\n");
                _logInfoBuilder.Append("[type]:" + type.ToString() + "\r\n\r\n");
                _logInfoBuilder.Append("[message]:" + logString + "\r\n\r\n");
                _logInfoBuilder.Append("[stack trace]:" + stackTrace + "\r\n\r\n");

#if UNITY_STANDALONE_WIN
                string logPath = _logPath + "/" + DateTime.Now.ToString("yyyy_MM_dd HH_mm_ss_fff") + ".log";
                File.AppendAllText(logPath, _logInfoBuilder.ToString());

                if (_module.IsEnableFeedback)
                {
                    if (File.Exists(_module.FeedbackProgramPath))
                    {
                        ProcessStartInfo process = new ProcessStartInfo();
                        process.FileName = _module.FeedbackProgramPath;
                        process.Arguments = "\"" + logPath + "\"";
                        Process pro = new Process();
                        pro.StartInfo = process;
                        pro.Start();
                    }
                    else
                    {
                        File.AppendAllText(logPath, "[feedback]:Doesn't find feedback program!path: " + _module.FeedbackProgramPath + "\r\n");
                    }
                    Application.Quit();
                }
#endif
                if (_module.IsEnableMailReport)
                {
                    ReportMail(Application.productName + ".Exception." + DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), _logInfoBuilder.ToString());
                }
            }
        }
        private void OnException(string logString, string stackTrace, LogType type)
        {
            Main.m_Event.Throw(this, Main.m_ReferencePool.Spawn<EventException>().Fill(logString, stackTrace, type));
        }
    }
}