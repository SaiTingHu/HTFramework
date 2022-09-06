using System;

namespace HT.Framework
{
    /// <summary>
    /// 【自动化任务】数据绑定，将UGUI控件双向绑定至数据模型的可绑定数据字段
    /// 【支持类型】EntityLogicBase、UILogicBase、HTBehaviour、FiniteStateBase、FSMDataBase、FSMArgsBase 子类中定义的非静态、UIBehaviour 类型字段
    /// 【前提条件】类型必须实现接口 IDataDriver，并启用自动化任务 IsAutomate（FiniteStateBase、FSMArgsBase除外）
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class DataBindingAttribute : Attribute
    {
        /// <summary>
        /// 目标数据模型
        /// </summary>
        public Type TargetType { get; private set; }
        /// <summary>
        /// 目标数据字段
        /// </summary>
        public string TargetField { get; private set; }

        public DataBindingAttribute(Type type, string field)
        {
            TargetType = type;
            TargetField = field;
        }
        public DataBindingAttribute(string type, string field)
        {
            TargetType = ReflectionToolkit.GetTypeInRunTimeAssemblies(type);
            TargetField = field;
        }
    }
}