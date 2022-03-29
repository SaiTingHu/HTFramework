using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 数学计算工具箱
    /// </summary>
    public static class MathfToolkit
    {
        /// <summary>
        /// 是否约等于另一个浮点数
        /// </summary>
        /// <param name="sourceValue">源浮点数</param>
        /// <param name="targetValue">目标浮点数</param>
        /// <returns>是否约等于</returns>
        public static bool Approximately(this float sourceValue, float targetValue)
        {
            return Mathf.Approximately(sourceValue, targetValue);
        }
        /// <summary>
        /// 从数组中随机获取一个值
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="values">值数组</param>
        /// <returns>随机获取的值</returns>
        public static T RandomValue<T>(this T[] values)
        {
            if (values == null || values.Length <= 0)
                return default;

            int index = UnityEngine.Random.Range(0, values.Length);
            return values[index];
        }
        /// <summary>
        /// 从集合中随机获取一个值
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="values">值数组</param>
        /// <returns>随机获取的值</returns>
        public static T RandomValue<T>(this List<T> values)
        {
            if (values == null || values.Count <= 0)
                return default;

            int index = UnityEngine.Random.Range(0, values.Count);
            return values[index];
        }
        /// <summary>
        /// 随机执行一个Action
        /// </summary>
        /// <param name="action1">Action1</param>
        /// <param name="action2">Action2</param>
        public static void RandomExecute(HTFAction action1, HTFAction action2)
        {
            int value = UnityEngine.Random.Range(0, 10);
            if (value < 5)
            {
                action1?.Invoke();
            }
            else
            {
                action2?.Invoke();
            }
        }
        /// <summary>
        /// 随机执行一个Action
        /// </summary>
        /// <param name="action1">Action1</param>
        /// <param name="action2">Action2</param>
        /// <param name="action3">Action3</param>
        public static void RandomExecute(HTFAction action1, HTFAction action2, HTFAction action3)
        {
            int value = UnityEngine.Random.Range(0, 9);
            if (value < 3)
            {
                action1?.Invoke();
            }
            else if (value < 6)
            {
                action2?.Invoke();
            }
            else
            {
                action3?.Invoke();
            }
        }
        /// <summary>
        /// 随机执行一个Action
        /// </summary>
        /// <param name="values">Action数组</param>
        public static void RandomExecute(params HTFAction[] values)
        {
            if (values == null || values.Length <= 0)
                return;

            int index = UnityEngine.Random.Range(0, values.Length);
            values[index]?.Invoke();
        }
        /// <summary>
        /// MD5算法加密
        /// </summary>
        /// <param name="value">字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string MD5Encrypt(this string value)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(value);
            byte[] toData = md5.ComputeHash(fromData);
            md5.Dispose();
            return Convert.ToBase64String(toData);
        }
        /// <summary>
        /// 转换成四元素
        /// </summary>
        /// <param name="value">Vector3值</param>
        /// <returns>四元素</returns>
        public static Quaternion ToQuaternion(this Vector3 value)
        {
            return Quaternion.Euler(value);
        }
        /// <summary>
        /// 限制目标值在最小值与最大值之间
        /// </summary>
        /// <param name="value">目标值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns>目标值</returns>
        public static Vector2 Clamp(this Vector2 value, Vector2 min, Vector2 max)
        {
            value.x = Mathf.Clamp(value.x, min.x, max.x);
            value.y = Mathf.Clamp(value.y, min.y, max.y);
            return value;
        }
        /// <summary>
        /// 限制目标值在最小值与最大值之间
        /// </summary>
        /// <param name="value">目标值</param>
        /// <param name="minX">X最小值</param>
        /// <param name="minY">Y最小值</param>
        /// <param name="maxX">X最大值</param>
        /// <param name="maxY">Y最大值</param>
        /// <returns>目标值</returns>
        public static Vector2 Clamp(this Vector2 value, float minX, float minY, float maxX, float maxY)
        {
            value.x = Mathf.Clamp(value.x, minX, maxX);
            value.y = Mathf.Clamp(value.y, minY, maxY);
            return value;
        }
        /// <summary>
        /// 限制目标值在最小值与最大值之间
        /// </summary>
        /// <param name="value">目标值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns>目标值</returns>
        public static Vector3 Clamp(this Vector3 value, Vector3 min, Vector3 max)
        {
            value.x = Mathf.Clamp(value.x, min.x, max.x);
            value.y = Mathf.Clamp(value.y, min.y, max.y);
            value.z = Mathf.Clamp(value.z, min.z, max.z);
            return value;
        }
        /// <summary>
        /// 限制目标值在最小值与最大值之间
        /// </summary>
        /// <param name="value">目标值</param>
        /// <param name="minX">X最小值</param>
        /// <param name="minY">Y最小值</param>
        /// <param name="minZ">Z最小值</param>
        /// <param name="maxX">X最大值</param>
        /// <param name="maxY">Y最大值</param>
        /// <param name="maxZ">Z最大值</param>
        /// <returns>目标值</returns>
        public static Vector3 Clamp(this Vector3 value, float minX, float minY, float minZ, float maxX, float maxY, float maxZ)
        {
            value.x = Mathf.Clamp(value.x, minX, maxX);
            value.y = Mathf.Clamp(value.y, minY, maxY);
            value.z = Mathf.Clamp(value.z, minZ, maxZ);
            return value;
        }
    }
}