namespace HT.Framework
{
    /// <summary>
    /// 拥有管理员模式的窗口
    /// </summary>
    public interface IAdminLoginWindow
    {
        /// <summary>
        /// 是否是管理员模式
        /// </summary>
        bool IsAdminMode { get; set; }
        /// <summary>
        /// 管理员密码
        /// </summary>
        string Password { get; }

        /// <summary>
        /// 管理员身份验证
        /// </summary>
        /// <param name="password">密码</param>
        void AdminCheck(string password);
    }
}