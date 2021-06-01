using System;

namespace HT.Framework
{
    /// <summary>
    /// 【自动化任务】数据绑定标记，将UGUI控件双向绑定至target目标数据字段
    /// （仅可用于 EntityLogicBase、UILogicBase、HTBehaviour、FiniteStateBase、FSMDataBase 子类中定义的非静态、UIBehaviour 类型字段）
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class DataBindingAttribute : Attribute
    {
        public string Target { get; private set; }

        public DataBindingAttribute(string target)
        {
            Target = target;
        }
    }
}