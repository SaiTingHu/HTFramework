using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 异常处理器
    /// </summary>
    [InternalModule(HTFrameworkModule.Exception)]
    public sealed class ExceptionManager : InternalModuleBase<IExceptionHelper>
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
        [SerializeField] internal string FeedbackProgramPath = "Feedback.exe";
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
        
        /// <summary>
        /// 当前捕获的所有异常信息
        /// </summary>
        public List<ExceptionInfo> ExceptionInfos
        {
            get
            {
                return _helper.ExceptionInfos;
            }
        }
        
        /// <summary>
        /// 回发邮件
        /// </summary>
        /// <param name="subject">邮件标题</param>
        /// <param name="body">邮件内容</param>
        public void ReportMail(string subject, string body)
        {
            _helper.ReportMail(subject, body);
        }
        /// <summary>
        /// 清理所有异常信息
        /// </summary>
        public void ClearExceptionInfos()
        {
            _helper.ClearExceptionInfos();
        }
    }
}