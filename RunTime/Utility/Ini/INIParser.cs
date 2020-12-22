using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// INI配置文件读取器
    /// </summary>
    public sealed class INIParser
    {
        /// <summary>
        /// 配置文件完整路径
        /// </summary>
        public string FileName { get; private set; } = null;
        /// <summary>
        /// 配置文件内容
        /// </summary>
        public string IniString { get; private set; } = null;

        private object _lock = new object();
        private bool _autoFlush = false;
        private Dictionary<string, Dictionary<string, string>> _sections = new Dictionary<string, Dictionary<string, string>>();
        private Dictionary<string, Dictionary<string, string>> _modified = new Dictionary<string, Dictionary<string, string>>();
        private bool _cacheModified = false;

        /// <summary>
        /// 写入配置数据
        /// </summary>
        /// <param name="section">配置单元</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="path">配置文件路径</param>
        public static void WriteIniData(string section, string key, string value, string path)
        {
            INIParser ini = new INIParser();
            ini.Open(path);
            ini.WriteValue(section, key, value);
            ini.Close();
        }

        /// <summary>
        /// 读取配置数据
        /// </summary>
        /// <param name="section">配置单元</param>
        /// <param name="key">键</param>
        /// <param name="defaultvalue">缺省值</param>
        /// <param name="path">配置文件路径</param>
        /// <returns>读取到的值</returns>
        public static string ReadIniData(string section, string key, string defaultvalue, string path)
        {
            INIParser ini = new INIParser();
            ini.Open(path);
            string value = ini.ReadValue(section, key, defaultvalue);
            ini.Close();
            return value;
        }

        /// <summary>
        /// 打开配置文件
        /// </summary>
        /// <param name="path">配置文件路径</param>
        public void Open(string path)
        {
            FileName = path;

            if (File.Exists(FileName))
            {
                IniString = File.ReadAllText(FileName);
            }
            else
            {
                FileStream temp = File.Create(FileName);
                temp.Close();
                IniString = "";
            }

            Initialize(IniString, false);
        }

        /// <summary>
        /// 打开配置文件
        /// </summary>
        /// <param name="file">配置文件</param>
        public void Open(TextAsset file)
        {
            if (file == null)
            {
                IniString = "";
                FileName = null;
                Initialize(IniString, false);
            }
            else
            {
                FileName = Application.persistentDataPath + "/" + file.name;
                if (File.Exists(FileName))
                {
                    IniString = File.ReadAllText(FileName);
                }
                else
                {
                    IniString = file.text;
                }
                Initialize(IniString, false);
            }
        }

        /// <summary>
        /// 打开配置文件，通过配置数据
        /// </summary>
        /// <param name="str">配置数据</param>
        public void OpenFromString(string str)
        {
            FileName = null;
            Initialize(str, false);
        }

        /// <summary>
        /// 关闭配置文件
        /// </summary>
        public void Close()
        {
            lock (_lock)
            {
                PerformFlush();

                FileName = null;
                IniString = null;
            }
        }

        /// <summary>
        /// 读取配置
        /// </summary>
        /// <param name="sectionName">配置单元名称</param>
        /// <param name="key">键</param>
        /// <param name="defaultValue">缺省值</param>
        /// <returns>值</returns>
        public string ReadValue(string sectionName, string key, string defaultValue)
        {
            lock (_lock)
            {
                Dictionary<string, string> section;
                if (!_sections.TryGetValue(sectionName, out section)) return defaultValue;

                string value;
                if (!section.TryGetValue(key, out value)) return defaultValue;

                return value;
            }
        }

        /// <summary>
        /// 读取配置
        /// </summary>
        /// <param name="sectionName">配置单元名称</param>
        /// <param name="key">键</param>
        /// <param name="defaultValue">缺省值</param>
        /// <returns>值</returns>
        public bool ReadValue(string sectionName, string key, bool defaultValue)
        {
            string stringValue = ReadValue(sectionName, key, defaultValue.ToString(CultureInfo.InvariantCulture));
            int Value;
            if (int.TryParse(stringValue, out Value)) return (Value != 0);
            return defaultValue;
        }

        /// <summary>
        /// 读取配置
        /// </summary>
        /// <param name="sectionName">配置单元名称</param>
        /// <param name="key">键</param>
        /// <param name="defaultValue">缺省值</param>
        /// <returns>值</returns>
        public int ReadValue(string sectionName, string key, int defaultValue)
        {
            string stringValue = ReadValue(sectionName, key, defaultValue.ToString(CultureInfo.InvariantCulture));
            int Value;
            if (int.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out Value)) return Value;
            return defaultValue;
        }

        /// <summary>
        /// 读取配置
        /// </summary>
        /// <param name="sectionName">配置单元名称</param>
        /// <param name="key">键</param>
        /// <param name="defaultValue">缺省值</param>
        /// <returns>值</returns>
        public long ReadValue(string sectionName, string key, long defaultValue)
        {
            string stringValue = ReadValue(sectionName, key, defaultValue.ToString(CultureInfo.InvariantCulture));
            long Value;
            if (long.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out Value)) return Value;
            return defaultValue;
        }

        /// <summary>
        /// 读取配置
        /// </summary>
        /// <param name="sectionName">配置单元名称</param>
        /// <param name="key">键</param>
        /// <param name="defaultValue">缺省值</param>
        /// <returns>值</returns>
        public double ReadValue(string sectionName, string key, double defaultValue)
        {
            string stringValue = ReadValue(sectionName, key, defaultValue.ToString(CultureInfo.InvariantCulture));
            double Value;
            if (double.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out Value)) return Value;
            return defaultValue;
        }

        /// <summary>
        /// 读取配置
        /// </summary>
        /// <param name="sectionName">配置单元名称</param>
        /// <param name="key">键</param>
        /// <param name="defaultValue">缺省值</param>
        /// <returns>值</returns>
        public byte[] ReadValue(string sectionName, string key, byte[] defaultValue)
        {
            string stringValue = ReadValue(sectionName, key, EncodeByteArray(defaultValue));
            try
            {
                return DecodeByteArray(stringValue);
            }
            catch (FormatException)
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 读取配置
        /// </summary>
        /// <param name="sectionName">配置单元名称</param>
        /// <param name="key">键</param>
        /// <param name="defaultValue">缺省值</param>
        /// <returns>值</returns>
        public DateTime ReadValue(string sectionName, string key, DateTime defaultValue)
        {
            string stringValue = ReadValue(sectionName, key, defaultValue.ToString(CultureInfo.InvariantCulture));
            DateTime Value;
            if (DateTime.TryParse(stringValue, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.NoCurrentDateDefault | DateTimeStyles.AssumeLocal, out Value)) return Value;
            return defaultValue;
        }

        /// <summary>
        /// 写入配置
        /// </summary>
        /// <param name="sectionName">配置单元名称</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void WriteValue(string sectionName, string key, string value)
        {
            lock (_lock)
            {
                _cacheModified = true;

                Dictionary<string, string> section;
                if (!_sections.TryGetValue(sectionName, out section))
                {
                    section = new Dictionary<string, string>();
                    _sections.Add(sectionName, section);
                }

                if (section.ContainsKey(key)) section.Remove(key);
                section.Add(key, value);

                if (!_modified.TryGetValue(sectionName, out section))
                {
                    section = new Dictionary<string, string>();
                    _modified.Add(sectionName, section);
                }

                if (section.ContainsKey(key)) section.Remove(key);
                section.Add(key, value);

                if (_autoFlush) PerformFlush();
            }
        }

        /// <summary>
        /// 写入配置
        /// </summary>
        /// <param name="sectionName">配置单元名称</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void WriteValue(string sectionName, string key, bool value)
        {
            WriteValue(sectionName, key, (value) ? ("1") : ("0"));
        }

        /// <summary>
        /// 写入配置
        /// </summary>
        /// <param name="sectionName">配置单元名称</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void WriteValue(string sectionName, string key, int value)
        {
            WriteValue(sectionName, key, value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// 写入配置
        /// </summary>
        /// <param name="sectionName">配置单元名称</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void WriteValue(string sectionName, string key, long value)
        {
            WriteValue(sectionName, key, value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// 写入配置
        /// </summary>
        /// <param name="sectionName">配置单元名称</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void WriteValue(string sectionName, string key, double value)
        {
            WriteValue(sectionName, key, value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// 写入配置
        /// </summary>
        /// <param name="sectionName">配置单元名称</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void WriteValue(string sectionName, string key, byte[] value)
        {
            WriteValue(sectionName, key, EncodeByteArray(value));
        }

        /// <summary>
        /// 写入配置
        /// </summary>
        /// <param name="sectionName">配置单元名称</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void WriteValue(string sectionName, string key, DateTime value)
        {
            WriteValue(sectionName, key, value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// 是否存在指定的数据单元名称
        /// </summary>
        /// <param name="sectionName">数据单元名称</param>
        /// <returns>是否存在</returns>
        public bool IsSectionExists(string sectionName)
        {
            return _sections.ContainsKey(sectionName);
        }

        /// <summary>
        /// 是否存在指定的键名称
        /// </summary>
        /// <param name="sectionName">数据单元名称</param>
        /// <param name="key">键名称</param>
        /// <returns>是否存在</returns>
        public bool IsKeyExists(string sectionName, string key)
        {
            Dictionary<string, string> Section;

            if (_sections.ContainsKey(sectionName))
            {
                _sections.TryGetValue(sectionName, out Section);

                return Section.ContainsKey(key);
            }
            else return false;
        }

        /// <summary>
        /// 删除数据单元
        /// </summary>
        /// <param name="sectionName">数据单元名称</param>
        public void SectionDelete(string sectionName)
        {
            if (IsSectionExists(sectionName))
            {
                lock (_lock)
                {
                    _cacheModified = true;
                    _sections.Remove(sectionName);
                    _modified.Remove(sectionName);
                    if (_autoFlush) PerformFlush();
                }
            }
        }

        /// <summary>
        /// 删除键
        /// </summary>
        /// <param name="sectionName">数据单元名称</param>
        /// <param name="key">键名称</param>
        public void KeyDelete(string sectionName, string key)
        {
            Dictionary<string, string> section;

            if (IsKeyExists(sectionName, key))
            {
                lock (_lock)
                {
                    _cacheModified = true;
                    _sections.TryGetValue(sectionName, out section);
                    section.Remove(key);

                    if (_modified.TryGetValue(sectionName, out section)) section.Remove(sectionName);

                    if (_autoFlush) PerformFlush();
                }
            }
        }

        public override string ToString()
        {
            return IniString;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="iniString">配置数据</param>
        /// <param name="autoFlush">是否自动覆盖旧的</param>
        private void Initialize(string iniString, bool autoFlush)
        {
            IniString = iniString;
            _autoFlush = autoFlush;
            Refresh();
        }
        
        /// <summary>
        /// 解析配置单元的名称
        /// </summary>
        /// <param name="line">数据行</param>
        /// <returns>配置单元的名称</returns>
        private string ParseSectionName(string line)
        {
            if (!line.StartsWith("[")) return null;
            if (!line.EndsWith("]")) return null;
            if (line.Length < 3) return null;
            return line.Substring(1, line.Length - 2);
        }

        /// <summary>
        /// 解析键值
        /// </summary>
        /// <param name="line">数据行</param>
        /// <param name="key">输出键</param>
        /// <param name="value">输出值</param>
        /// <returns>是否解析成功</returns>
        private bool ParseKeyValuePair(string line, ref string key, ref string value)
        {
            int i = line.IndexOf('=');
            if (i <= 0) return false;

            int j = line.Length - i - 1;
            key = line.Substring(0, i).Trim();
            if (key.Length <= 0) return false;

            value = (j > 0) ? (line.Substring(i + 1, j).Trim()) : ("");
            return true;
        }

        /// <summary>
        /// 是否是注释
        /// </summary>
        /// <param name="line">数据行</param>
        /// <returns>是否是注释</returns>
        private bool IsComment(string line)
        {
            string tmpKey = null, tmpValue = null;
            if (ParseSectionName(line) != null) return false;
            if (ParseKeyValuePair(line, ref tmpKey, ref tmpValue)) return false;
            return true;
        }

        /// <summary>
        /// 刷新
        /// </summary>
        private void Refresh()
        {
            lock (_lock)
            {
                StringReader sr = null;
                try
                {
                    _sections.Clear();
                    _modified.Clear();

                    sr = new StringReader(IniString);

                    Dictionary<string, string> currentSection = null;
                    string s;
                    string sectionName;
                    string key = null;
                    string value = null;
                    while ((s = sr.ReadLine()) != null)
                    {
                        s = s.Trim();

                        sectionName = ParseSectionName(s);
                        if (sectionName != null)
                        {
                            if (_sections.ContainsKey(sectionName))
                            {
                                currentSection = null;
                            }
                            else
                            {
                                currentSection = new Dictionary<string, string>();
                                _sections.Add(sectionName, currentSection);
                            }
                        }
                        else if (currentSection != null)
                        {
                            if (ParseKeyValuePair(s, ref key, ref value))
                            {
                                if (!currentSection.ContainsKey(key))
                                {
                                    currentSection.Add(key, value);
                                }
                            }
                        }
                    }
                }
                finally
                {
                    if (sr != null) sr.Close();
                    sr = null;
                }
            }
        }

        /// <summary>
        /// 执行配置数据覆盖
        /// </summary>
        private void PerformFlush()
        {
            if (!_cacheModified) return;
            _cacheModified = false;

            StringWriter sw = new StringWriter();

            try
            {
                Dictionary<string, string> currentSection = null;
                Dictionary<string, string> currentSection2 = null;
                StringReader sr = null;
                try
                {
                    sr = new StringReader(IniString);

                    string s;
                    string sectionName;
                    string key = null;
                    string value = null;
                    bool unmodified;
                    bool reading = true;

                    bool deleted = false;
                    string key2 = null;
                    string value2 = null;

                    StringBuilder sb_temp;

                    while (reading)
                    {
                        s = sr.ReadLine();
                        reading = (s != null);

                        if (reading)
                        {
                            unmodified = true;
                            s = s.Trim();
                            sectionName = ParseSectionName(s);
                        }
                        else
                        {
                            unmodified = false;
                            sectionName = null;
                        }

                        if ((sectionName != null) || (!reading))
                        {
                            if (currentSection != null)
                            {
                                if (currentSection.Count > 0)
                                {
                                    sb_temp = sw.GetStringBuilder();
                                    while ((sb_temp[sb_temp.Length - 1] == '\n') || (sb_temp[sb_temp.Length - 1] == '\r'))
                                    {
                                        sb_temp.Length = sb_temp.Length - 1;
                                    }
                                    sw.WriteLine();

                                    foreach (string fkey in currentSection.Keys)
                                    {
                                        if (currentSection.TryGetValue(fkey, out value))
                                        {
                                            sw.Write(fkey);
                                            sw.Write('=');
                                            sw.WriteLine(value);
                                        }
                                    }
                                    sw.WriteLine();
                                    currentSection.Clear();
                                }
                            }

                            if (reading)
                            {
                                if (!_modified.TryGetValue(sectionName, out currentSection))
                                {
                                    currentSection = null;
                                }
                            }
                        }
                        else if (currentSection != null)
                        {
                            if (ParseKeyValuePair(s, ref key, ref value))
                            {
                                if (currentSection.TryGetValue(key, out value))
                                {
                                    unmodified = false;
                                    currentSection.Remove(key);

                                    sw.Write(key);
                                    sw.Write('=');
                                    sw.WriteLine(value);
                                }
                            }
                        }

                        if (unmodified)
                        {
                            if (sectionName != null)
                            {
                                if (!_sections.ContainsKey(sectionName))
                                {
                                    deleted = true;
                                    currentSection2 = null;
                                }
                                else
                                {
                                    deleted = false;
                                    _sections.TryGetValue(sectionName, out currentSection2);
                                }

                            }
                            else if (currentSection2 != null)
                            {
                                if (ParseKeyValuePair(s, ref key2, ref value2))
                                {
                                    if (!currentSection2.ContainsKey(key2)) deleted = true;
                                    else deleted = false;
                                }
                            }
                        }


                        if (unmodified)
                        {
                            if (IsComment(s)) sw.WriteLine(s);
                            else if (!deleted) sw.WriteLine(s);
                        }
                    }

                    sr.Close();
                    sr = null;
                }
                finally
                {
                    if (sr != null) sr.Close();
                    sr = null;
                }

                foreach (KeyValuePair<string, Dictionary<string, string>> SectionPair in _modified)
                {
                    currentSection = SectionPair.Value;
                    if (currentSection.Count > 0)
                    {
                        sw.WriteLine();

                        sw.Write('[');
                        sw.Write(SectionPair.Key);
                        sw.WriteLine(']');

                        foreach (KeyValuePair<string, string> ValuePair in currentSection)
                        {
                            sw.Write(ValuePair.Key);
                            sw.Write('=');
                            sw.WriteLine(ValuePair.Value);
                        }
                        currentSection.Clear();
                    }
                }
                _modified.Clear();

                IniString = sw.ToString();
                sw.Close();
                sw = null;

                if (FileName != null)
                {
                    File.WriteAllText(FileName, IniString);
                }
            }
            finally
            {
                if (sw != null) sw.Close();
                sw = null;
            }
        }
        
        /// <summary>
        /// 编码字节数组
        /// </summary>
        /// <param name="value">字节数组</param>
        /// <returns>值</returns>
        private string EncodeByteArray(byte[] value)
        {
            if (value == null) return null;

            StringBuilder sb = new StringBuilder();
            foreach (byte b in value)
            {
                string hex = Convert.ToString(b, 16);
                int l = hex.Length;
                if (l > 2)
                {
                    sb.Append(hex.Substring(l - 2, 2));
                }
                else
                {
                    if (l < 2) sb.Append("0");
                    sb.Append(hex);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 解码字节数组
        /// </summary>
        /// <param name="value">值</param>
        /// <returns>字节数组</returns>
        private byte[] DecodeByteArray(string value)
        {
            if (value == null) return null;

            int l = value.Length;
            if (l < 2) return new byte[] { };

            l /= 2;
            byte[] result = new byte[l];
            for (int i = 0; i < l; i++) result[i] = Convert.ToByte(value.Substring(i * 2, 2), 16);
            return result;
        }
    }
}