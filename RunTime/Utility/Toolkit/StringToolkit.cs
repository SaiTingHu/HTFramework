using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 字符串工具箱
    /// </summary>
    public static class StringToolkit
    {
        #region 不重复命名
        private static HashSet<string> NoRepeatNames = new HashSet<string>();

        /// <summary>
        /// 开始不重复命名
        /// </summary>
        public static void BeginNoRepeatNaming()
        {
            NoRepeatNames.Clear();
        }
        /// <summary>
        /// 获取不重复命名（自动加工原名，以防止重复）
        /// </summary>
        /// <param name="rawName">原名</param>
        /// <returns>不重复命名</returns>
        public static string GetNoRepeatName(string rawName)
        {
            if (NoRepeatNames.Contains(rawName))
            {
                int index = 0;
                string noRepeatName = $"{rawName} {index}";
                while (NoRepeatNames.Contains(noRepeatName))
                {
                    index += 1;
                    noRepeatName = $"{rawName} {index}";
                }

                NoRepeatNames.Add(noRepeatName);
                return noRepeatName;
            }
            else
            {
                NoRepeatNames.Add(rawName);
                return rawName;
            }
        }
        #endregion

        #region 字符串拼接
        private static StringBuilder StringInstance = new StringBuilder();

        /// <summary>
        /// 字符串拼接
        /// </summary>
        /// <param name="str">待拼接的字符串</param>
        /// <returns>拼接成功的字符串</returns>
        public static string Concat(params string[] str)
        {
            StringInstance.Clear();
            for (int i = 0; i < str.Length; i++)
            {
                StringInstance.Append(str[i]);
            }
            return StringInstance.ToString();
        }
        /// <summary>
        /// 字符串拼接
        /// </summary>
        /// <param name="str">待拼接的字符串</param>
        /// <returns>拼接成功的字符串</returns>
        public static string Concat(List<string> str)
        {
            if (str == null || str.Count <= 0)
                return null;

            StringInstance.Clear();
            for (int i = 0; i < str.Count; i++)
            {
                StringInstance.Append(str[i]);
            }
            return StringInstance.ToString();
        }
        #endregion

        #region 字符串转换
        /// <summary>
        /// 转换成枚举
        /// </summary>
        /// <typeparam name="EnumType">枚举类型</typeparam>
        /// <param name="value">字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>枚举值</returns>
        public static EnumType ToEnum<EnumType>(this string value, EnumType defaultValue)
        {
            if (!string.IsNullOrEmpty(value))
            {
                try
                {
                    return (EnumType)Enum.Parse(typeof(EnumType), value);
                }
                catch
                {
                    return defaultValue;
                }
            }
            return defaultValue;
        }
        /// <summary>
        /// 转换成Vector2，格式：Vector2(x,y)
        /// </summary>
        /// <param name="value">字符串</param>
        /// <param name="defaultValue">转换失败时的默认值</param>
        /// <returns>Vector2值</returns>
        public static Vector2 ToVector2(this string value, Vector2 defaultValue = default)
        {
            if (value.StartsWith("Vector2("))
            {
                value = value.Replace("Vector2(", "");
                value = value.Replace(")", "");
                value = value.Replace("f", "");
                string[] vector2 = value.Split(',');
                float x = 0;
                float y = 0;
                if (vector2.Length > 0) float.TryParse(vector2[0], out x);
                if (vector2.Length > 1) float.TryParse(vector2[1], out y);
                return new Vector2(x, y);
            }
            return defaultValue;
        }
        /// <summary>
        /// 转换成Vector3，格式：Vector3(x,y,z)
        /// </summary>
        /// <param name="value">字符串</param>
        /// <param name="defaultValue">转换失败时的默认值</param>
        /// <returns>Vector3值</returns>
        public static Vector3 ToVector3(this string value, Vector3 defaultValue = default)
        {
            if (value.StartsWith("Vector3("))
            {
                value = value.Replace("Vector3(", "");
                value = value.Replace(")", "");
                value = value.Replace("f", "");
                string[] vector3 = value.Split(',');
                float x = 0;
                float y = 0;
                float z = 0;
                if (vector3.Length > 0) float.TryParse(vector3[0], out x);
                if (vector3.Length > 1) float.TryParse(vector3[1], out y);
                if (vector3.Length > 2) float.TryParse(vector3[2], out z);
                return new Vector3(x, y, z);
            }
            return defaultValue;
        }
        #endregion

        #region 正则表达式
        /// <summary>
        /// 是否为整型内容
        /// </summary>
        /// <param name="value">字符串值</param>
        /// <returns>是否为整型内容</returns>
        public static bool IsInt(this string value)
        {
            return Regex.IsMatch(value, "^([-]?[0-9]{1,})$");
        }
        /// <summary>
        /// 是否为浮点型内容
        /// </summary>
        /// <param name="value">字符串值</param>
        /// <returns>是否为浮点型内容</returns>
        public static bool IsFloat(this string value)
        {
            return Regex.IsMatch(value, "^([-]?[0-9]{1,}[.][0-9]*)$");
        }
        #endregion
    }
}