namespace HT.Framework
{
    /// <summary>
    /// Json工具箱
    /// </summary>
    public static class JsonToolkit
    {
        /// <summary>
        /// Json对象转换为字符串
        /// </summary>
        /// <param name="json">Json对象</param>
        /// <returns>字符串</returns>
        public static string JsonToString(JsonData json)
        {
            if (json == null)
            {
                return null;
            }
            return json.ToJson();
        }
        /// <summary>
        /// Json对象转换为字符串
        /// </summary>
        /// <param name="json">Json对象</param>
        /// <returns>字符串</returns>
        public static string JsonToString(object json)
        {
            if (json == null)
            {
                return null;
            }
            return JsonMapper.ToJson(json);
        }
        /// <summary>
        /// 字符串转换为Json对象
        /// </summary>
        /// <param name="value">字符串</param>
        /// <returns>Json对象</returns>
        public static JsonData StringToJson(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            else
            {
                JsonData jsonData = null;
                try
                {
                    jsonData = JsonMapper.ToObject(value);
                }
                catch
                {
                    jsonData = null;
                }
                return jsonData;
            }
        }
        /// <summary>
        /// 字符串转换为Json对象
        /// </summary>
        /// <param name="value">字符串</param>
        /// <returns>Json对象</returns>
        public static T StringToJson<T>(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return default;
            }
            else
            {
                T jsonData = default;
                try
                {
                    jsonData = JsonMapper.ToObject<T>(value);
                }
                catch
                {
                    jsonData = default;
                }
                return jsonData;
            }
        }
        /// <summary>
        /// 在安全模式下获取Json值
        /// </summary>
        /// <param name="json">json数据</param>
        /// <param name="key">键</param>
        /// <param name="defaultValue">缺省值</param>
        /// <returns>获取到的键对应的值</returns>
        public static string GetValueInSafe(this JsonData json, string key, string defaultValue = null)
        {
            if (json.Keys.Contains(key))
            {
                if (json[key] != null)
                {
                    return json[key].ToString();
                }
                else
                {
                    return defaultValue;
                }
            }
            else
            {
                return defaultValue;
            }
        }
    }
}