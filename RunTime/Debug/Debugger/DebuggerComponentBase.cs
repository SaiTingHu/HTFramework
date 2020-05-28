using System;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 调试器组件基类
    /// </summary>
    public abstract class DebuggerComponentBase : IReference
    {
        /// <summary>
        /// 组件
        /// </summary>
        public Component Target;

        public abstract void OnEnable();
        public abstract void OnDebuggerGUI();
        public virtual void Reset()
        {
            Target = null;
        }

        /// <summary>
        /// 创建一个Vector3字段GUI
        /// </summary>
        /// <param name="value">旧的值</param>
        /// <returns>新的值</returns>
        protected Vector3 Vector3Field(Vector3 value)
        {
            string valueX = value.x.ToString();
            string valueY = value.y.ToString();
            string valueZ = value.z.ToString();
            string newX = GUILayout.TextField(valueX);
            string newY = GUILayout.TextField(valueY);
            string newZ = GUILayout.TextField(valueZ);
            if (string.Equals(newX, valueX) || string.Equals(newY, valueY) || string.Equals(newZ, valueZ))
            {
                float x, y, z;
                if (float.TryParse(newX, out x) && float.TryParse(newY, out y) && float.TryParse(newZ, out z))
                {
                    value.Set(x, y, z);
                    return value;
                }
                else
                {
                    return value;
                }
            }
            else
            {
                return value;
            }
        }

        /// <summary>
        /// 创建一个Vector2字段GUI
        /// </summary>
        /// <param name="value">旧的值</param>
        /// <returns>新的值</returns>
        protected Vector3 Vector2Field(Vector2 value)
        {
            string valueX = value.x.ToString();
            string valueY = value.y.ToString();
            string newX = GUILayout.TextField(valueX);
            string newY = GUILayout.TextField(valueY);
            if (string.Equals(newX, valueX) || string.Equals(newY, valueY))
            {
                float x, y;
                if (float.TryParse(newX, out x) && float.TryParse(newY, out y))
                {
                    value.Set(x, y);
                    return value;
                }
                else
                {
                    return value;
                }
            }
            else
            {
                return value;
            }
        }

        /// <summary>
        /// 创建一个float字段GUI
        /// </summary>
        /// <param name="value">旧的值</param>
        /// <returns>新的值</returns>
        protected float FloatField(float value, params GUILayoutOption[] options)
        {
            string valueF = value.ToString();
            string newF = GUILayout.TextField(valueF, options);
            if (newF != valueF)
            {
                float f;
                if (float.TryParse(newF, out f))
                {
                    return f;
                }
                else
                {
                    return value;
                }
            }
            else
            {
                return value;
            }
        }

        /// <summary>
        /// 创建一个int字段GUI
        /// </summary>
        /// <param name="value">旧的值</param>
        /// <returns>新的值</returns>
        protected int IntField(int value, params GUILayoutOption[] options)
        {
            string valueI = value.ToString();
            string newI = GUILayout.TextField(valueI, options);
            if (newI != valueI)
            {
                int f;
                if (int.TryParse(newI, out f))
                {
                    return f;
                }
                else
                {
                    return value;
                }
            }
            else
            {
                return value;
            }
        }

        /// <summary>
        /// 创建一个枚举字段GUI
        /// </summary>
        /// <param name="value">旧的值</param>
        /// <returns>新的值</returns>
        protected Enum EnumField(Enum value, params GUILayoutOption[] options)
        {
            foreach (Enum e in Enum.GetValues(value.GetType()))
            {
                if (GUILayout.Toggle(value.Equals(e), e.ToString(), options))
                {
                    value = e;
                }
            }
            return value;
        }
    }
}