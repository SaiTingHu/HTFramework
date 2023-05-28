using System;
using System.Reflection;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// #SetField 指令
    /// </summary>
    internal sealed class SentenceSetField : Sentence
    {
        public string ComType;
        public string FieldName;
        public ArgsType Type;
        public string StringArgs;
        public bool BoolArgs;
        public int IntArgs;
        public float FloatArgs;
        public Vector2 Vector2Args;
        public Vector3 Vector3Args;

        private Type _type;
        private Component _component;
        private FieldInfo _fieldInfo;

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
        public FieldInfo Field
        {
            get
            {
                if (_fieldInfo == null)
                {
                    _fieldInfo = T.GetField(FieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                }
                return _fieldInfo;
            }
        }

        public override void Execute()
        {
            if (C != null && Field != null)
            {
                switch (Type)
                {
                    case ArgsType.String:
                        if (Field.FieldType == typeof(string))
                        {
                            Field.SetValue(C, StringArgs);
                        }
                        break;
                    case ArgsType.Bool:
                        if (Field.FieldType == typeof(bool))
                        {
                            Field.SetValue(C, BoolArgs);
                        }
                        break;
                    case ArgsType.Int:
                        if (Field.FieldType == typeof(int))
                        {
                            Field.SetValue(C, IntArgs);
                        }
                        break;
                    case ArgsType.Float:
                        if (Field.FieldType == typeof(float))
                        {
                            Field.SetValue(C, FloatArgs);
                        }
                        break;
                    case ArgsType.Vector2:
                        if (Field.FieldType == typeof(Vector2))
                        {
                            Field.SetValue(C, Vector2Args);
                        }
                        break;
                    case ArgsType.Vector3:
                        if (Field.FieldType == typeof(Vector3))
                        {
                            Field.SetValue(C, Vector3Args);
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
            _fieldInfo = null;
        }
    }
}