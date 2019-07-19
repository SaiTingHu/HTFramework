using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 异常处理模块
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ExceptionHandler : ModuleManager
    {
        /// <summary>
        /// 是否开启异常处理监听【只在Inspector面板设置有效，代码中设置无效】
        /// </summary>
        public bool IsHandler = false;
        /// <summary>
        /// 是否启用异常反馈程序【只在Inspector面板设置有效，代码中设置无效】
        /// </summary>
        public bool IsEnableFeedback = false;
        /// <summary>
        /// 是否启用邮件回发机制【只在Inspector面板设置有效，代码中设置无效】
        /// </summary>
        public bool IsEnableMailReport = false;
        /// <summary>
        /// 反馈程序路径
        /// </summary>
        public string FeedbackProgramPath = "/Feedback.exe";
        /// <summary>
        /// 回发邮件的发送邮箱
        /// </summary>
        public string SendMailbox = "hutao_123456@sina.com";
        /// <summary>
        /// 回发邮件的发送邮箱密码
        /// </summary>
        public string SendMailboxPassword = "";
        /// <summary>
        /// 回发邮件的目标邮箱
        /// </summary>
        public string ReceiveMailbox = "";
        /// <summary>
        /// 邮件服务器Host
        /// </summary>
        public string Host = "smtp.sina.com";
        /// <summary>
        /// 邮件服务器端口
        /// </summary>
        public int Port = 25;
        /// <summary>
        /// 回发邮件缓冲时间
        /// </summary>
        public float ReportBufferTime = 5;

        //异常信息
        private List<ExceptionInfo> _exceptionInfos = new List<ExceptionInfo>();
        //异常日志保存路径
        private string _logPath;
        //邮件发送者
        private EmailSender _sender;
        //回发邮件缓冲计时器
        private float _reportBufferTimer = 0;

        public override void Initialization()
        {
            base.Initialization();

#if UNITY_EDITOR
            IsHandler = false;
#endif

#if UNITY_STANDALONE_WIN
            FeedbackProgramPath = GlobalTools.GetDirectorySameLevelOfAssets(FeedbackProgramPath);
            _logPath = GlobalTools.GetDirectorySameLevelOfAssets("/Log");
            if (!Directory.Exists(_logPath))
            {
                Directory.CreateDirectory(_logPath);
            }
#endif
            if (IsHandler)
            {
                Application.logMessageReceived += Handler;
                _sender = new EmailSender(SendMailbox, SendMailboxPassword, ReceiveMailbox, Host, Port);
            }
        }

        public override void Termination()
        {
            base.Termination();

            if (IsHandler)
            {
                Application.logMessageReceived -= Handler;
                _sender = null;
            }
        }

        public override void Refresh()
        {
            base.Refresh();

            if (_reportBufferTimer > 0)
            {
                _reportBufferTimer -= Time.deltaTime;
            }
        }

        /// <summary>
        /// 当前捕获的所有异常信息
        /// </summary>
        public List<ExceptionInfo> ExceptionInfos
        {
            get
            {
                return _exceptionInfos;
            }
        }

        /// <summary>
        /// 回发邮件
        /// </summary>
        public void ReportMail(string subject, string body)
        {
            if (_reportBufferTimer > 0)
            {
                return;
            }
            _reportBufferTimer = ReportBufferTime;

            _sender.Send(subject, body);
        }

        /// <summary>
        /// 清理所有异常信息
        /// </summary>
        public void ClearExceptionInfos()
        {
            Main.m_ReferencePool.Despawns(_exceptionInfos);
        }

        private void Handler(string logString, string stackTrace, LogType type)
        {
            if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
            {
                _exceptionInfos.Add(Main.m_ReferencePool.Spawn<ExceptionInfo>().Fill(logString, stackTrace, type));

                OnException(logString, stackTrace, type);

                string logContent = string.Format("[time]:{0}\r\n\r\n[type]:{1}\r\n\r\n[message]:{2}\r\n\r\n[stack trace]:{3}\r\n\r\n", DateTime.Now.ToString(), type.ToString(), logString, stackTrace);

#if UNITY_STANDALONE_WIN
                string logPath = string.Format("{0}/{1}.log", _logPath, DateTime.Now.ToString("yyyy_MM_dd HH_mm_ss_fff"));
                File.AppendAllText(logPath, logContent);

                if (IsEnableFeedback)
                {
                    if (File.Exists(FeedbackProgramPath))
                    {
                        ProcessStartInfo process = new ProcessStartInfo();
                        process.FileName = FeedbackProgramPath;
                        process.Arguments = "\"" + logPath + "\"";
                        Process pro = new Process();
                        pro.StartInfo = process;
                        pro.Start();
                    }
                    else
                    {
                        File.AppendAllText(logPath, string.Format("[feedback]:Doesn't find feedback program!path: {0}\r\n", FeedbackProgramPath));
                    }
                    Application.Quit();
                }
#endif
                if (IsEnableMailReport)
                {
                    ReportMail(string.Format("{0}.Exception.{1}", Application.productName, DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss")), logContent);
                }
            }
        }

        private void OnException(string logString, string stackTrace, LogType type)
        {
            Main.m_Event.Throw(this, Main.m_ReferencePool.Spawn<EventException>().Fill(logString, stackTrace, type));
        }
    }
}