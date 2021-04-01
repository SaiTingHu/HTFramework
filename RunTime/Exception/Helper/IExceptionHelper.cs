using System.Collections.Generic;

namespace HT.Framework
{
    /// <summary>
    /// 异常处理器的助手接口
    /// </summary>
    public interface IExceptionHelper : IInternalModuleHelper
    {
        /// <summary>
        /// 当前捕获的所有异常信息
        /// </summary>
        List<ExceptionInfo> ExceptionInfos { get; }
        
        /// <summary>
        /// 回发邮件
        /// </summary>
        /// <param name="subject">邮件标题</param>
        /// <param name="body">邮件内容</param>
        void ReportMail(string subject, string body);
        /// <summary>
        /// 清理所有异常信息
        /// </summary>
        void ClearExceptionInfos();
    }
}