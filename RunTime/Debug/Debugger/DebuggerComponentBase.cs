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
        /// 创建一个Vector3字段
        /// </summary>
        /// <param name="name">字段名称</param>
        /// <param name="value">旧的值</param>
        /// <param name="nameLength">名称长度</param>
        /// <returns>新的值</returns>
        protected Vector3 Vector3Field(string name, Vector3 value, int nameLength = 120, params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(name, GUILayout.Width(nameLength));
            string valueX = value.x.ToString();
            string valueY = value.y.ToString();
            string valueZ = value.z.ToString();
            string newX = GUILayout.TextField(valueX, options);
            string newY = GUILayout.TextField(valueY, options);
            string newZ = GUILayout.TextField(valueZ, options);
            GUILayout.EndHorizontal();

            if (!string.Equals(newX, valueX) || !string.Equals(newY, valueY) || !string.Equals(newZ, valueZ))
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
        /// 创建一个Vector2字段
        /// </summary>
        /// <param name="name">字段名称</param>
        /// <param name="value">旧的值</param>
        /// <param name="nameLength">名称长度</param>
        /// <returns>新的值</returns>
        protected Vector3 Vector2Field(string name, Vector2 value, int nameLength = 120, params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(name, GUILayout.Width(nameLength));
            string valueX = value.x.ToString();
            string valueY = value.y.ToString();
            string newX = GUILayout.TextField(valueX, options);
            string newY = GUILayout.TextField(valueY, options);
            GUILayout.EndHorizontal();

            if (!string.Equals(newX, valueX) || !string.Equals(newY, valueY))
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
        /// 创建一个String字段
        /// </summary>
        /// <param name="name">字段名称</param>
        /// <param name="value">旧的值</param>
        /// <param name="nameLength">名称长度</param>
        /// <returns>新的值</returns>
        protected string StringField(string name, string value, int nameLength = 120, params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(name, GUILayout.Width(nameLength));
            string newS = GUILayout.TextField(value, options);
            GUILayout.EndHorizontal();

            if (newS != value)
            {
                return newS;
            }
            else
            {
                return value;
            }
        }
        /// <summary>
        /// 创建一个Bool字段
        /// </summary>
        /// <param name="name">字段名称</param>
        /// <param name="value">旧的值</param>
        /// <param name="nameLength">名称长度</param>
        /// <returns>新的值</returns>
        protected bool BoolField(string name, bool value, int nameLength = 120, params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(name, GUILayout.Width(nameLength));
            bool newB = GUILayout.Toggle(value, "", options);
            GUILayout.EndHorizontal();

            return newB;
        }
        /// <summary>
        /// 创建一个float字段
        /// </summary>
        /// <param name="name">字段名称</param>
        /// <param name="value">旧的值</param>
        /// <param name="nameLength">名称长度</param>
        /// <returns>新的值</returns>
        protected float FloatField(string name, float value, int nameLength = 120, params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(name, GUILayout.Width(nameLength));
            string valueF = value.ToString();
            string newF = GUILayout.TextField(valueF, options);
            GUILayout.EndHorizontal();

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
        /// 创建一个int字段
        /// </summary>
        /// <param name="name">字段名称</param>
        /// <param name="value">旧的值</param>
        /// <param name="nameLength">名称长度</param>
        /// <returns>新的值</returns>
        protected int IntField(string name, int value, int nameLength = 120, params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(name, GUILayout.Width(nameLength));
            string valueI = value.ToString();
            string newI = GUILayout.TextField(valueI, options);
            GUILayout.EndHorizontal();

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
        /// 创建一个枚举字段
        /// </summary>
        /// <param name="name">字段名称</param>
        /// <param name="value">旧的值</param>
        /// <returns>新的值</returns>
        protected Enum EnumField(string name, Enum value, params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(name);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            foreach (Enum e in Enum.GetValues(value.GetType()))
            {
                if (GUILayout.Toggle(value.Equals(e), e.ToString(), options))
                {
                    value = e;
                }
            }
            GUILayout.EndHorizontal();

            return value;
        }

        /// <summary>
        /// 创建一个String字段（只读）
        /// </summary>
        /// <param name="name">字段名称</param>
        /// <param name="value">字符串值</param>
        /// <param name="nameLength">名称长度</param>
        protected void StringFieldReadOnly(string name, string value, int nameLength = 120, params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(name, GUILayout.Width(nameLength));
            GUILayout.TextField(value, options);
            GUILayout.EndHorizontal();
        }
        /// <summary>
        /// 创建一个Object字段（只读）
        /// </summary>
        /// <param name="name">字段名称</param>
        /// <param name="value">对象值</param>
        /// <param name="nameLength">名称长度</param>
        protected void ObjectFieldReadOnly(string name, UnityEngine.Object value, int nameLength = 120, params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(name, GUILayout.Width(nameLength));
            GUILayout.TextField(value != null ? value.name : "<None>", options);
            GUILayout.EndHorizontal();
        }
        /// <summary>
        /// 创建一个材质数组字段（只读）
        /// </summary>
        /// <param name="name">字段名称</param>
        /// <param name="materials">材质数组</param>
        /// <param name="nameLength">名称长度</param>
        protected void MaterialsFieldReadOnly(string name, Material[] materials, int nameLength = 120, params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(name);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.Label("Size", GUILayout.Width(nameLength - 20));
            GUILayout.TextField(materials.Length.ToString(), options);
            GUILayout.EndHorizontal();

            for (int i = 0; i < materials.Length; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label($"Element {i}", GUILayout.Width(nameLength - 20));
                GUILayout.TextField(materials[i] != null ? materials[i].name : "<None>", options);
                GUILayout.EndHorizontal();
            }
        }
    }
}