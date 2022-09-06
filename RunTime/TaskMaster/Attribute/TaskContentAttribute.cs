using System;
using System.Diagnostics;

namespace HT.Framework
{
    /// <summary>
    /// 任务内容标记
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    [Conditional("UNITY_EDITOR")]
    public sealed class TaskContentAttribute : Attribute
    {
        public string Name;

        public TaskContentAttribute(string name)
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