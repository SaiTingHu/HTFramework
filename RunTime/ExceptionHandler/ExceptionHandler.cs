using UnityEngine;
using System;
using System.IO;
using System.Diagnostics;
using System.Net.Mail;
using System.Text;
using System.Net;

namespace HT.Framework
{
    /// <summary>
    /// 异常处理模块
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ExceptionHandler : ModuleManager
    {
        /// <summary>
        /// 是否开启异常处理监听
        /// </summary>
        public bool IsHandler = false;
        /// <summary>
        /// 是否退出程序当异常发生时
        /// </summary>
        public bool IsQuitWhenException = false;
        /// <summary>
        /// 是否回发邮件当异常发生时
        /// </summary>
        public bool IsReportMailWhenException = false;
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
        /// 回发邮件缓冲时间
        /// </summary>
        public float ReportBufferTime = 5;

        //异常日志保存路径（文件夹）
        private string _logPath;
        //Bug反馈程序的启动路径
        private string _bugExePath;
        //Host
        private string _host = "smtp.sina.com";
        //回发邮件缓冲计时器
        private float _reportBufferTimer = 0;

        public override void Initialization()
        {
            base.Initialization();

            _logPath = GlobalTools.GetDirectorySameLevelOfAssets("/Log");
            _bugExePath = GlobalTools.GetDirectorySameLevelOfAssets("/Bug.exe");

#if !UNITY_EDITOR
            if (!Directory.Exists(_logPath))
            {
                Directory.CreateDirectory(_logPath);
            }

            //注册异常处理委托
            if (IsHandler)
            {
                Application.logMessageReceived += Handler;
            }
#endif
        }

        public override void Termination()
        {
            base.Termination();

#if !UNITY_EDITOR
            //取消注册异常处理委托
            if (IsHandler)
            {
                Application.logMessageReceived -= Handler;
            }
#endif
        }

        public override void Refresh()
        {
            base.Refresh();

            if (_reportBufferTimer > 0)
            {
                _reportBufferTimer -= Time.deltaTime;
            }
        }

        private void Handler(string logString, string stackTrace, LogType type)
        {
            if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
            {
                OnException(logString, stackTrace, type);

                string logPath = _logPath + "\\" + DateTime.Now.ToString("yyyy_MM_dd HH_mm_ss") + ".log";
                File.AppendAllText(logPath, "[time]:" + DateTime.Now.ToString() + "\r\n");
                File.AppendAllText(logPath, "[type]:" + type.ToString() + "\r\n");
                File.AppendAllText(logPath, "[exception message]:" + logString + "\r\n");
                File.AppendAllText(logPath, "[stack trace]:" + stackTrace + "\r\n");
                
                if (File.Exists(_bugExePath))
                {
                    ProcessStartInfo pros = new ProcessStartInfo();
                    pros.FileName = _bugExePath;
                    pros.Arguments = "\"" + logPath + "\"";
                    Process pro = new Process();
                    pro.StartInfo = pros;
                    pro.Start();
                }
                else
                {
                    File.AppendAllText(logPath, "[bug exe]:Doesn't find bug exe!path: " + _bugExePath + "\r\n");
                }

                if (IsReportMailWhenException)
                {
                    string logContent = "";
                    logContent += ("[time]:" + DateTime.Now.ToString() + "\r\n");
                    logContent += ("[type]:" + type.ToString() + "\r\n");
                    logContent += ("[exception message]:" + logString + "\r\n");
                    logContent += ("[stack trace]:" + stackTrace + "\r\n");

                    ReportMail(string.Format("{0}.Exception.{1}", Application.productName, DateTime.Now.ToString("yyyy_MM_dd HH_mm_ss")), logContent);
                }

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

            try
            {
                MailMessage mailMsg = new MailMessage();
                mailMsg.From = new MailAddress(SendMailbox);
                mailMsg.To.Add(new MailAddress(ReceiveMailbox));
                mailMsg.Subject = subject;
                mailMsg.SubjectEncoding = Encoding.UTF8;
                mailMsg.Body = body;
                mailMsg.BodyEncoding = Encoding.UTF8;
                mailMsg.IsBodyHtml = true;

                SmtpClient client = new SmtpClient();
                client.Host = _host;
                client.Port = 25;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(SendMailbox, SendMailboxPassword) as ICredentialsByHost;
                client.EnableSsl = false;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Send(mailMsg);
            }
            catch (Exception e)
            {
                GlobalTools.LogError(e.Message);
            }
        }
    }
}