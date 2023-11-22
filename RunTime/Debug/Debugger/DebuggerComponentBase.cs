using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

        private Dictionary<string, bool> _drawer = new Dictionary<string, bool>();

        public abstract void OnEnable();
        public abstract void OnDebuggerGUI();
        public virtual void Reset()
        {
            Target = null;
            _drawer.Clear();
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
        /// 创建一个Color字段
        /// </summary>
        /// <param name="name">字段名称</param>
        /// <param name="value">旧的值</param>
        /// <param name="nameLength">名称长度</param>
        /// <returns>新的值</returns>
        protected Color ColorField(string name, Color value, int nameLength = 120, params GUILayoutOption[] options)
        {
            if (!_drawer.ContainsKey(name)) _drawer.Add(name, false);

            GUILayout.BeginHorizontal();
            GUILayout.Label(name, GUILayout.Width(nameLength));
            Color lastColor = GUI.color;
            GUI.color = value;
            _drawer[name] = GUILayout.Toggle(_drawer[name], "COLOR", "Button", options);
            GUI.color = lastColor;
            GUILayout.EndHorizontal();

            if (_drawer[name])
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(nameLength);
                GUILayout.Label("R", GUILayout.Width(20));
                value.r = GUILayout.HorizontalSlider(value.r, 0, 1);
                bool lastEnabled = GUI.enabled;
                GUI.enabled = false;
                GUILayout.TextField(value.r.ToString(), GUILayout.Width(40));
                GUI.enabled = lastEnabled;
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(nameLength);
                GUILayout.Label("G", GUILayout.Width(20));
                value.g = GUILayout.HorizontalSlider(value.g, 0, 1);
                lastEnabled = GUI.enabled;
                GUI.enabled = false;
                GUILayout.TextField(value.g.ToString(), GUILayout.Width(40));
                GUI.enabled = lastEnabled;
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(nameLength);
                GUILayout.Label("B", GUILayout.Width(20));
                value.b = GUILayout.HorizontalSlider(value.b, 0, 1);
                lastEnabled = GUI.enabled;
                GUI.enabled = false;
                GUILayout.TextField(value.b.ToString(), GUILayout.Width(40));
                GUI.enabled = lastEnabled;
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(nameLength);
                GUILayout.Label("A", GUILayout.Width(20));
                value.a = GUILayout.HorizontalSlider(value.a, 0, 1);
                lastEnabled = GUI.enabled;
                GUI.enabled = false;
                GUILayout.TextField(value.a.ToString(), GUILayout.Width(40));
                GUI.enabled = lastEnabled;
                GUILayout.EndHorizontal();
            }

            return value;
        }
        /// <summary>
        /// 创建一个枚举字段
        /// </summary>
        /// <param name="name">字段名称</param>
        /// <param name="value">旧的值</param>
        /// <param name="nameLength">名称长度</param>
        /// <returns>新的值</returns>
        protected Enum EnumField(string name, Enum value, int nameLength = 120, params GUILayoutOption[] options)
        {
            if (!_drawer.ContainsKey(name)) _drawer.Add(name, false);

            GUILayout.BeginHorizontal();
            GUILayout.Label(name, GUILayout.Width(nameLength));
            _drawer[name] = GUILayout.Toggle(_drawer[name], value.ToString(), "Button", options);
            GUILayout.EndHorizontal();

            if (_drawer[name])
            {
                foreach (Enum e in Enum.GetValues(value.GetType()))
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(nameLength);
                    if (GUILayout.Toggle(value.Equals(e), e.ToString()))
                    {
                        value = e;
                    }
                    GUILayout.EndHorizontal();
                }
            }

            return value;
        }
        /// <summary>
        /// 创建一个UnityEvent字段
        /// </summary>
        /// <param name="name">字段名称</param>
        /// <param name="unityEvent">值</param>
        /// <param name="nameLength">名称长度</param>
        protected void UnityEventField(string name, UnityEvent unityEvent, int nameLength = 120, params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(name, GUILayout.Width(nameLength));
            int count = unityEvent.GetPersistentEventCount();
            GUILayout.Label($"[{count}]");
            bool lastEnabled = GUI.enabled;
            GUI.enabled = count > 0;
            if (GUILayout.Button("Invoke", options))
            {
                unityEvent.Invoke();
            }
            GUI.enabled = lastEnabled;
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 创建一个Vector3字段（只读）
        /// </summary>
        /// <param name="name">字段名称</param>
        /// <param name="value">旧的值</param>
        /// <param name="nameLength">名称长度</param>
        protected void Vector3FieldReadOnly(string name, Vector3 value, int nameLength = 120, params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(name, GUILayout.Width(nameLength));
            bool lastEnabled = GUI.enabled;
            GUI.enabled = false;
            GUILayout.TextField(value.x.ToString(), options);
            GUILayout.TextField(value.y.ToString(), options);
            GUILayout.TextField(value.z.ToString(), options);
            GUI.enabled = lastEnabled;
            GUILayout.EndHorizontal();
        }
        /// <summary>
        /// 创建一个Vector2字段（只读）
        /// </summary>
        /// <param name="name">字段名称</param>
        /// <param name="value">旧的值</param>
        /// <param name="nameLength">名称长度</param>
        protected void Vector2FieldReadOnly(string name, Vector2 value, int nameLength = 120, params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(name, GUILayout.Width(nameLength));
            bool lastEnabled = GUI.enabled;
            GUI.enabled = false;
            GUILayout.TextField(value.x.ToString(), options);
            GUILayout.TextField(value.y.ToString(), options);
            GUI.enabled = lastEnabled;
            GUILayout.EndHorizontal();
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
            bool lastEnabled = GUI.enabled;
            GUI.enabled = false;
            GUILayout.TextField(value, options);
            GUI.enabled = lastEnabled;
            GUILayout.EndHorizontal();
        }
        /// <summary>
        /// 创建一个Bool字段（只读）
        /// </summary>
        /// <param name="name">字段名称</param>
        /// <param name="value">旧的值</param>
        /// <param name="nameLength">名称长度</param>
        protected void BoolFieldReadOnly(string name, bool value, int nameLength = 120, params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(name, GUILayout.Width(nameLength));
            bool lastEnabled = GUI.enabled;
            GUI.enabled = false;
            GUILayout.Toggle(value, "", options);
            GUI.enabled = lastEnabled;
            GUILayout.EndHorizontal();
        }
        /// <summary>
        /// 创建一个float字段（只读）
        /// </summary>
        /// <param name="name">字段名称</param>
        /// <param name="value">旧的值</param>
        /// <param name="nameLength">名称长度</param>
        protected void FloatFieldReadOnly(string name, float value, int nameLength = 120, params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(name, GUILayout.Width(nameLength));
            bool lastEnabled = GUI.enabled;
            GUI.enabled = false;
            GUILayout.TextField(value.ToString(), options);
            GUI.enabled = lastEnabled;
            GUILayout.EndHorizontal();
        }
        /// <summary>
        /// 创建一个int字段（只读）
        /// </summary>
        /// <param name="name">字段名称</param>
        /// <param name="value">旧的值</param>
        /// <param name="nameLength">名称长度</param>
        protected void IntFieldReadOnly(string name, int value, int nameLength = 120, params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(name, GUILayout.Width(nameLength));
            bool lastEnabled = GUI.enabled;
            GUI.enabled = false;
            GUILayout.TextField(value.ToString(), options);
            GUI.enabled = lastEnabled;
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
            bool lastEnabled = GUI.enabled;
            GUI.enabled = false;
            GUILayout.TextField(value != null ? $"{value.name} ({value.GetType().Name})" : "<None>", options);
            GUI.enabled = lastEnabled;
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
            bool lastEnabled = GUI.enabled;
            GUI.enabled = false;
            GUILayout.TextField(materials.Length.ToString());
            GUI.enabled = lastEnabled;
            GUILayout.EndHorizontal();

            for (int i = 0; i < materials.Length; i++)
            {
                bool hasColor = materials[i] != null && materials[i].HasColor("_Color");
                Color color = hasColor ? materials[i].GetColor("_Color") : Color.white;

                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label($"Element {i}", GUILayout.Width(nameLength - 20));
                lastEnabled = GUI.enabled;
                GUI.enabled = false;
                GUILayout.TextField(materials[i] != null ? materials[i].name : "<None>", options);
                GUI.enabled = lastEnabled;
                if (hasColor)
                {
                    if (!_drawer.ContainsKey(materials[i].name)) _drawer.Add(materials[i].name, false);

                    Color lastColor = GUI.color;
                    GUI.color = color;
                    _drawer[materials[i].name] = GUILayout.Toggle(_drawer[materials[i].name], "COLOR", "Button", GUILayout.Width(60));
                    GUI.color = lastColor;
                }
                GUILayout.EndHorizontal();

                if (hasColor && _drawer[materials[i].name])
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(nameLength);
                    GUILayout.Label("R", GUILayout.Width(20));
                    color.r = GUILayout.HorizontalSlider(color.r, 0, 1);
                    lastEnabled = GUI.enabled;
                    GUI.enabled = false;
                    GUILayout.TextField(color.r.ToString(), GUILayout.Width(40));
                    GUI.enabled = lastEnabled;
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(nameLength);
                    GUILayout.Label("G", GUILayout.Width(20));
                    color.g = GUILayout.HorizontalSlider(color.g, 0, 1);
                    lastEnabled = GUI.enabled;
                    GUI.enabled = false;
                    GUILayout.TextField(color.g.ToString(), GUILayout.Width(40));
                    GUI.enabled = lastEnabled;
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(nameLength);
                    GUILayout.Label("B", GUILayout.Width(20));
                    color.b = GUILayout.HorizontalSlider(color.b, 0, 1);
                    lastEnabled = GUI.enabled;
                    GUI.enabled = false;
                    GUILayout.TextField(color.b.ToString(), GUILayout.Width(40));
                    GUI.enabled = lastEnabled;
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(nameLength);
                    GUILayout.Label("A", GUILayout.Width(20));
                    color.a = GUILayout.HorizontalSlider(color.a, 0, 1);
                    lastEnabled = GUI.enabled;
                    GUI.enabled = false;
                    GUILayout.TextField(color.a.ToString(), GUILayout.Width(40));
                    GUI.enabled = lastEnabled;
                    GUILayout.EndHorizontal();

                    if (GUI.changed)
                    {
                        materials[i].SetColor("_Color", color);
                    }
                }
            }
        }
    }
}