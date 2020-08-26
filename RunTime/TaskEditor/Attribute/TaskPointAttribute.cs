using System;
using System.Diagnostics;

namespace HT.Framework
{
    /// <summary>
    /// 任务点标记
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    [Conditional("UNITY_EDITOR")]
    public sealed class TaskPointAttribute : Attribute
    {
        public string Name;

        public TaskPointAttribute(string name)
        {
            Name = name;
        }

        public string GetLastName()
        {
            int index = Name.LastIndexOf("/");
            if (index >= 0)
            {
                return Name.Substring(index + 1);
            }
            else
            {
                return Name;
            }
        }
    }
}