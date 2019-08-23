namespace HT.Framework
{
    /// <summary>
    /// 非常驻UI
    /// </summary>
    public abstract class UILogicTemporary : UILogicBase
    {
        /// <summary>
        /// 打开自己
        /// </summary>
        protected override void Open()
        {
            base.Open();

            Main.m_UI.OpenTemporaryUI(GetType());
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
