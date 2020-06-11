using System;

namespace HT.Framework
{
    /// <summary>
    /// 任务内容标记
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
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