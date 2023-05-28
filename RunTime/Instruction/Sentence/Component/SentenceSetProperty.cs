using System;
using System.Reflection;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// #SetProperty 指令
    /// </summary>
    internal sealed class SentenceSetProperty : Sentence
    {
        public string ComType;
        public string PropertyName;
        public ArgsType Type;
        public string StringArgs;
        public bool BoolArgs;
        public int IntArgs;
        public float FloatArgs;
        public Vector2 Vector2Args;
        public Vector3 Vector3Args;

        private Type _type;
        private Component _component;
        private PropertyInfo _propertyInfo;

        public Type T
        {
            get
            {
                if (_type == null)
                {
                    _type = ReflectionToolkit.GetTypeInRunTimeAssemblies(ComType);
                }
                return _type;
            }
        }
        public Component C
        {
            get
            {
                if (_component == null)
                {
                    _component = Target.GetComponent(T);
                }
                return _component;
            }
        }
        public PropertyInfo Property
        {
            get
            {
                if (_propertyInfo == null)
                {
                    _propertyInfo = T.GetProperty(PropertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                }
                return _propertyInfo;
            }
        }

        public override void Execute()
        {
            if (C != null && Property != null)
            {
                switch (Type)
                {
                    case ArgsType.String:
                        if (Property.PropertyType == typeof(string))
                        {
                            Property.SetValue(C, StringArgs);
                        }
                        break;
                    case ArgsType.Bool:
                        if (Property.PropertyType == typeof(bool))
                        {
                            Property.SetValue(C, BoolArgs);
                        }
                        break;
                    case ArgsType.Int:
                        if (Property.PropertyType == typeof(int))
                        {
                            Property.SetValue(C, IntArgs);
                        }
                        break;
                    case ArgsType.Float:
                        if (Property.PropertyType == typeof(float))
                        {
                            Property.SetValue(C, FloatArgs);
                        }
                        break;
                    case ArgsType.Vector2:
                        if (Property.PropertyType == typeof(Vector2))
                        {
                            Property.SetValue(C, Vector2Args);
                        }
                        break;
                    case ArgsType.Vector3:
                        if (Property.PropertyType == typeof(Vector3))
                        {
                            Property.SetValue(C, Vector3Args);
                        }
                        break;
                }
            }
        }
        public override void Reset()
        {
            base.Reset();

            _type = null;
            _component = null;
            _propertyInfo = null;
        }
    }
}