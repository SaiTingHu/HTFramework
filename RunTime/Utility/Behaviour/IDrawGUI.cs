namespace HT.Framework
{
    /// <summary>
    /// 行为接口：绘制用户图形界面（仅限于HTBehaviour子类实现）
    /// </summary>
    public interface IDrawGUI
    {
        /// <summary>
        /// 绘制用户图形界面
        /// </summary>
        void OnDrawGUI();
    }
}