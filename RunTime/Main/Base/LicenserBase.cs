using System.Collections;

namespace HT.Framework
{
    /// <summary>
    /// 授权者基类
    /// </summary>
    public abstract class LicenserBase
    {
        /// <summary>
        /// 是否授权通过
        /// </summary>
        public bool IsLicensePass { get; protected set; } = false;
        /// <summary>
        /// 授权失败提示
        /// </summary>
        public virtual string LicenseFailurePrompt
        {
            get
            {
                return "授权未通过！";
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void OnInitialization()
        { }
        /// <summary>
        /// 授权校验
        /// </summary>
        public abstract IEnumerator Checking();
    }
}