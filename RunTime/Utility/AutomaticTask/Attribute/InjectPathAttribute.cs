using System;

namespace HT.Framework
{
    /// <summary>
    /// 【自动化任务】依赖注入（路径模式），将根据设置的路径，以脚本实体为根目标，查找子对象并注入依赖
    /// 【支持类型】EntityLogicBase、UILogicBase、HTBehaviour、FiniteStateBase、FSMDataBase、FSMArgsBase 子类中定义的非静态字段
    /// 【前提条件】类型必须启用自动化任务 IsAutomate（FiniteStateBase、FSMArgsBase除外）
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class InjectPathAttribute : Attribute
    {
        /// <summary>
        /// 路径
        /// </summary>
        public string Path { get; private set; }

        public InjectPathAttribute(string path)
        {
            Path = path;
        }
    }
}