using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 异常处理模块
    /// </summary>
    [DisallowMultipleComponent]
    [InternalModule(HTFrameworkModule.ExceptionHandler)]
    public sealed class ExceptionHandler : InternalModuleBase
    {
        /// <summary>
        /// 是否开启异常处理监听【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal bool IsHandler = false;
        /// <summary>
        /// 是否启用异常反馈程序【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal bool IsEnableFeedback = false;
        /// <summary>
        /// 是否启用邮件回发机制【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal bool IsEnableMailReport = false;
        /// <summary>
        /// 反馈程序路径【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal string FeedbackProgramPath = "/Feedback.exe";
        /// <summary>
        /// 回发邮件的发送邮箱【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal string SendMailbox = "hutao_123456@sina.com";
        /// <summary>
        /// 回发邮件的发送邮箱密码【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal string SendMailboxPassword = "";
        /// <summary>
        /// 回发邮件的目标邮箱【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal string ReceiveMailbox = "";
        /// <summary>
        /// 邮件服务器Host【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal string Host = "smtp.sina.com";
        /// <summary>
        /// 邮件服务器端口【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal int Port = 25;
        /// <summary>
        /// 回发邮件缓冲时间【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal float ReportBufferTime = 5;

        //异常信息
        private List<ExceptionInfo> _exceptionInfos = new List<ExceptionInfo>();
        //异常日志Builder
        private StringBuilder _logInfoBuilder = new StringBuilder();
        //异常日志保存路径
        private string _logPath;
        //邮件发送者
        private EmailSender _sender;
        //回发邮件缓冲计时器
        private float _reportBufferTimer = 0;

        internal override void OnInitialization()
        {
            base.OnInitialization();

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

                if (IsEnableMailReport)
                {
                    _sender = new EmailSender(SendMailbox, SendMailboxPassword, ReceiveMailbox, Host, Port);
                }
            }
        }

        internal override void OnTermination()
        {
            base.OnTermination();

            if (IsHandler)
            {
                Application.logMessageReceived -= Handler;
                if (_sender != null)
                {
                    _sender.Dispose();
                    _sender = null;
                }
            }
        }

        internal override void OnRefresh()
        {
            base.OnRefresh();

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
        /// <param name="subject">邮件标题</param>
        /// <param name="body">邮件内容</param>
        public void ReportMail(string subject, string body)
        {
            if (_reportBufferTimer > 0)
            {
                return;
            }
            _reportBufferTimer = ReportBufferTime;

            if (IsHandler)
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
            Main.m_ReferencePool.Despawns(_exceptionInfos);
        }

        private void Handler(string logString, string stackTrace, LogType type)
        {
            if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
            {
                _exceptionInfos.Add(Main.m_ReferencePool.Spawn<ExceptionInfo>().Fill(logString, stackTrace, type));

                OnException(logString, stackTrace, type);

                _logInfoBuilder.Clear();
                _logInfoBuilder.Append("[time]:" + DateTime.Now.ToString() + "\r\n\r\n");
                _logInfoBuilder.Append("[type]:" + type.ToString() + "\r\n\r\n");
                _logInfoBuilder.Append("[message]:" + logString + "\r\n\r\n");
                _logInfoBuilder.Append("[stack trace]:" + stackTrace + "\r\n\r\n");

#if UNITY_STANDALONE_WIN
                string logPath = _logPath + "/" + DateTime.Now.ToString("yyyy_MM_dd HH_mm_ss_fff") + ".log";
                File.AppendAllText(logPath, _logInfoBuilder.ToString());

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
                        File.AppendAllText(logPath, "[feedback]:Doesn't find feedback program!path: " + FeedbackProgramPath + "\r\n");
                    }
                    Application.Quit();
                }
#endif
                if (IsEnableMailReport)
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