using System;

namespace HT.Framework
{
    /// <summary>
    /// 热修复方法的标记，仅可标记热更新程序集中的静态方法，用以修复外部程序的指定方法
    /// 注意：参数 targetName 为热修复的目标方法全称，组成格式：目标方法的类名全称.目标方法名
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class HotfixMethodAttribute : Attribute
    {
        public string TargetName;
        
        public HotfixMethodAttribute(string targetName)
        {
            TargetName = targetName;
        }
    }
}