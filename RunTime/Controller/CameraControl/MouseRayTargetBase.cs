namespace HT.Framework
{
    /// <summary>
    /// 鼠标射线可捕获的目标
    /// </summary>
    public abstract class MouseRayTargetBase : HTBehaviour
    {
        /// <summary>
        /// 目标显示的名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 目标是否为步骤目标，若为步骤目标，则在步骤流程中点错会触发错误事件
        /// </summary>
        public bool IsStepTarget = false;
        /// <summary>
        /// 是否开启提示，当目标被射线捕获时
        /// </summary>
        public bool IsOpenPrompt = true;
        /// <summary>
        /// 是否开启高亮，当目标被射线捕获时
        /// </summary>
        public bool IsOpenHighlight = true;
    }
}