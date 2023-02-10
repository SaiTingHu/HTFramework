using System;
using System.Collections.Generic;
using System.Text;
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
        /// 转换成Vector3，格式：x,y,z
        /// </summary>
        /// <param name="value">字符串</param>
        /// <returns>Vector3值</returns>
        public static Vector3 ToVector3(this string value)
        {
            value = value.Replace("f", "");
            string[] values = value.Split(',');
            if (values.Length == 3)
            {
                try
                {
                    return new Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
                }
                catch
                {
                    return Vector3.zero;
                }
            }

            return Vector3.zero;
        }
        #endregion
    }
}