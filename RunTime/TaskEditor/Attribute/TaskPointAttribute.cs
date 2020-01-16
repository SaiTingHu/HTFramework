using System;

namespace HT.Framework
{
    /// <summary>
    /// 任务点标记
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class TaskPointAttribute : Attribute
    {
        public string Name;

        public TaskPointAttribute(string name)
        {
            Name = name;
        }
    }
}