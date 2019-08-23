namespace HT.Framework
{
    /// <summary>
    /// 常驻UI
    /// </summary>
    public abstract class UILogicResident : UILogicBase
    {
        /// <summary>
        /// 置顶UI
        /// </summary>
        public virtual void OnPlaceTop()
        { }

        /// <summary>
        /// 打开自己
        /// </summary>
        protected override void Open()
        {
            base.Open();

            Main.m_UI.OpenResidentUI(GetType());
        }

        /// <summary>
        /// 关闭自己
        /// </summary>
        protected override void Close()
        {
            base.Close();

            Main.m_UI.CloseUI(GetType());
        }
    }
}
