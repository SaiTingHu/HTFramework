using System;
using System.Collections;
using System.Net;
using System.Net.Mail;
using System.Text;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 邮件发送者
    /// </summary>
    internal sealed class EmailSender : IDisposable
    {
        private MailMessage _mailMessage;
        private SmtpClient _smtpClient;
        private string _subject = "";
        private string _body = "";

        public EmailSender(string fromEmail, string fromEmailPassword, string toEmail, string host, int port)
        {
            _mailMessage = new MailMessage();
            _mailMessage.From = new MailAddress(fromEmail);
            _mailMessage.To.Add(new MailAddress(toEmail));
            _mailMessage.SubjectEncoding = Encoding.UTF8;
            _mailMessage.BodyEncoding = Encoding.UTF8;
            _mailMessage.IsBodyHtml = false;

            _smtpClient = new SmtpClient();
            _smtpClient.Host = host;
            _smtpClient.Port = port;
            _smtpClient.UseDefaultCredentials = false;
            _smtpClient.Credentials = new NetworkCredential(fromEmail, fromEmailPassword) as ICredentialsByHost;
            _smtpClient.EnableSsl = false;
            _smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
        }

        /// <summary>
        /// 添加目标邮件地址
        /// </summary>
        /// <param name="toEmail">目标邮件地址</param>
        public void AddToEmail(string toEmail)
        {
            _mailMessage.To.Add(new MailAddress(toEmail));
        }

        /// <summary>
        /// 设置邮件标题
        /// </summary>
        /// <param name="subject">邮件标题</param>
        public void SetSubject(string subject)
        {
            _subject = subject;
        }

        /// <summary>
        /// 设置邮件内容
        /// </summary>
        /// <param name="body">邮件内容</param>
        public void SetBody(string body)
        {
            _body = body;
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        public Coroutine Send()
        {
            _mailMessage.Subject = _subject;
            _mailMessage.Body = _body;
            return Main.Current.StartCoroutine(SendCoroutine());
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="subject">邮件标题</param>
        /// <param name="body">邮件内容</param>
        public Coroutine Send(string subject, string body)
        {
            _subject = subject;
            _body = body;
            _mailMessage.Subject = _subject;
            _mailMessage.Body = _body;
            return Main.Current.StartCoroutine(SendCoroutine());
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public void Dispose()
        {
            _mailMessage.Dispose();
            _smtpClient.Dispose();
        }

        private IEnumerator SendCoroutine()
        {
            yield return null;
            _smtpClient.Send(_mailMessage);
        }
    }
}