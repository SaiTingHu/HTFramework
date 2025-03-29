using System;

namespace HT.Framework
{
    /// <summary>
    /// 【自动化任务】依赖注入（组件模式），将根据设置的组件类型，以脚本实体为根目标，查找子对象上的组件并注入依赖
    /// 【支持类型】EntityLogicBase、UILogicBase、HTBehaviour、FiniteStateBase、FSMDataBase、FSMArgsBase 子类中定义的非静态字段
    /// 【前提条件】类型必须启用自动化任务 IsAutomate（FiniteStateBase、FSMArgsBase除外）
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class InjectComponentAttribute : Attribute
    {
        
    }
}