using System;

namespace HT.Framework
{
    /// <summary>
    /// 标注ECS的系统关注哪些组件
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class StarComponentAttribute : Attribute
    {
        public Type[] StarComponents;

        public StarComponentAttribute(params Type[] components)
        {
            StarComponents = components;
        }
    }
}