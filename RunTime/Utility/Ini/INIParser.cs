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
        public int Error = 0;
        public string FileName { get; private set; } = null;
        public string IniString { get; private set; } = null;

        private object _lock = new object();
        private bool _autoFlush = false;
        private Dictionary<string, Dictionary<string, string>> _sections = new Dictionary<string, Dictionary<string, string>>();
        private Dictionary<string, Dictionary<string, string>> _modified = new Dictionary<string, Dictionary<string, string>>();
        private bool _cacheModified = false;

        /// <summary>
        /// 写入配置数据
        /// </summary>
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
        public static string ReadIniData(string section, string key, string defaultvalue, string path)
        {
            INIParser ini = new INIParser();
            ini.Open(path);
            string value = ini.ReadValue(section, key, defaultvalue);
            ini.Close();
            return value;
        }

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

        public void Open(TextAsset name)
        {
            if (name == null)
            {
                Error = 1;
                IniString = "";
                FileName = null;
                Initialize(IniString, false);
            }
            else
            {
                FileName = Application.persistentDataPath + name.name;

                if (File.Exists(FileName))
                {
                    IniString = File.ReadAllText(FileName);
                }
                else IniString = name.text;
                Initialize(IniString, false);
            }
        }

        public void OpenFromString(string str)
        {
            FileName = null;
            Initialize(str, false);
        }

        public override string ToString()
        {
            return IniString;
        }

        private void Initialize(string iniString, bool AutoFlush)
        {
            IniString = iniString;
            _autoFlush = AutoFlush;
            Refresh();
        }

        public void Close()
        {
            lock (_lock)
            {
                PerformFlush();

                FileName = null;
                IniString = null;
            }
        }

        private string ParseSectionName(string Line)
        {
            if (!Line.StartsWith("[")) return null;
            if (!Line.EndsWith("]")) return null;
            if (Line.Length < 3) return null;
            return Line.Substring(1, Line.Length - 2);
        }

        private bool ParseKeyValuePair(string Line, ref string Key, ref string Value)
        {
            int i;
            if ((i = Line.IndexOf('=')) <= 0) return false;

            int j = Line.Length - i - 1;
            Key = Line.Substring(0, i).Trim();
            if (Key.Length <= 0) return false;

            Value = (j > 0) ? (Line.Substring(i + 1, j).Trim()) : ("");
            return true;
        }

        private bool IsComment(string Line)
        {
            string tmpKey = null, tmpValue = null;
            if (ParseSectionName(Line) != null) return false;
            if (ParseKeyValuePair(Line, ref tmpKey, ref tmpValue)) return false;
            return true;
        }

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

                    Dictionary<string, string> CurrentSection = null;
                    string s;
                    string SectionName;
                    string Key = null;
                    string Value = null;
                    while ((s = sr.ReadLine()) != null)
                    {
                        s = s.Trim();

                        SectionName = ParseSectionName(s);
                        if (SectionName != null)
                        {
                            if (_sections.ContainsKey(SectionName))
                            {
                                CurrentSection = null;
                            }
                            else
                            {
                                CurrentSection = new Dictionary<string, string>();
                                _sections.Add(SectionName, CurrentSection);
                            }
                        }
                        else if (CurrentSection != null)
                        {
                            if (ParseKeyValuePair(s, ref Key, ref Value))
                            {
                                if (!CurrentSection.ContainsKey(Key))
                                {
                                    CurrentSection.Add(Key, Value);
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

        private void PerformFlush()
        {
            if (!_cacheModified) return;
            _cacheModified = false;

            StringWriter sw = new StringWriter();

            try
            {
                Dictionary<string, string> CurrentSection = null;
                Dictionary<string, string> CurrentSection2 = null;
                StringReader sr = null;
                try
                {
                    sr = new StringReader(IniString);

                    string s;
                    string SectionName;
                    string Key = null;
                    string Value = null;
                    bool Unmodified;
                    bool Reading = true;

                    bool Deleted = false;
                    string Key2 = null;
                    string Value2 = null;

                    StringBuilder sb_temp;

                    while (Reading)
                    {
                        s = sr.ReadLine();
                        Reading = (s != null);

                        if (Reading)
                        {
                            Unmodified = true;
                            s = s.Trim();
                            SectionName = ParseSectionName(s);
                        }
                        else
                        {
                            Unmodified = false;
                            SectionName = null;
                        }

                        if ((SectionName != null) || (!Reading))
                        {
                            if (CurrentSection != null)
                            {
                                if (CurrentSection.Count > 0)
                                {
                                    sb_temp = sw.GetStringBuilder();
                                    while ((sb_temp[sb_temp.Length - 1] == '\n') || (sb_temp[sb_temp.Length - 1] == '\r'))
                                    {
                                        sb_temp.Length = sb_temp.Length - 1;
                                    }
                                    sw.WriteLine();

                                    foreach (string fkey in CurrentSection.Keys)
                                    {
                                        if (CurrentSection.TryGetValue(fkey, out Value))
                                        {
                                            sw.Write(fkey);
                                            sw.Write('=');
                                            sw.WriteLine(Value);
                                        }
                                    }
                                    sw.WriteLine();
                                    CurrentSection.Clear();
                                }
                            }

                            if (Reading)
                            {
                                if (!_modified.TryGetValue(SectionName, out CurrentSection))
                                {
                                    CurrentSection = null;
                                }
                            }
                        }
                        else if (CurrentSection != null)
                        {
                            if (ParseKeyValuePair(s, ref Key, ref Value))
                            {
                                if (CurrentSection.TryGetValue(Key, out Value))
                                {
                                    Unmodified = false;
                                    CurrentSection.Remove(Key);

                                    sw.Write(Key);
                                    sw.Write('=');
                                    sw.WriteLine(Value);
                                }
                            }
                        }

                        if (Unmodified)
                        {
                            if (SectionName != null)
                            {
                                if (!_sections.ContainsKey(SectionName))
                                {
                                    Deleted = true;
                                    CurrentSection2 = null;
                                }
                                else
                                {
                                    Deleted = false;
                                    _sections.TryGetValue(SectionName, out CurrentSection2);
                                }

                            }
                            else if (CurrentSection2 != null)
                            {
                                if (ParseKeyValuePair(s, ref Key2, ref Value2))
                                {
                                    if (!CurrentSection2.ContainsKey(Key2)) Deleted = true;
                                    else Deleted = false;
                                }
                            }
                        }


                        if (Unmodified)
                        {
                            if (IsComment(s)) sw.WriteLine(s);
                            else if (!Deleted) sw.WriteLine(s);
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
                    CurrentSection = SectionPair.Value;
                    if (CurrentSection.Count > 0)
                    {
                        sw.WriteLine();

                        sw.Write('[');
                        sw.Write(SectionPair.Key);
                        sw.WriteLine(']');

                        foreach (KeyValuePair<string, string> ValuePair in CurrentSection)
                        {
                            sw.Write(ValuePair.Key);
                            sw.Write('=');
                            sw.WriteLine(ValuePair.Value);
                        }
                        CurrentSection.Clear();
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

        public bool IsSectionExists(string SectionName)
        {
            return _sections.ContainsKey(SectionName);
        }

        public bool IsKeyExists(string SectionName, string Key)
        {
            Dictionary<string, string> Section;

            if (_sections.ContainsKey(SectionName))
            {
                _sections.TryGetValue(SectionName, out Section);

                return Section.ContainsKey(Key);
            }
            else return false;
        }

        public void SectionDelete(string SectionName)
        {
            if (IsSectionExists(SectionName))
            {
                lock (_lock)
                {
                    _cacheModified = true;
                    _sections.Remove(SectionName);

                    _modified.Remove(SectionName);

                    if (_autoFlush) PerformFlush();
                }
            }
        }

        public void KeyDelete(string SectionName, string Key)
        {
            Dictionary<string, string> Section;

            if (IsKeyExists(SectionName, Key))
            {
                lock (_lock)
                {
                    _cacheModified = true;
                    _sections.TryGetValue(SectionName, out Section);
                    Section.Remove(Key);

                    if (_modified.TryGetValue(SectionName, out Section)) Section.Remove(SectionName);

                    if (_autoFlush) PerformFlush();
                }
            }

        }

        public string ReadValue(string SectionName, string Key, string DefaultValue)
        {
            lock (_lock)
            {
                Dictionary<string, string> Section;
                if (!_sections.TryGetValue(SectionName, out Section)) return DefaultValue;

                string Value;
                if (!Section.TryGetValue(Key, out Value)) return DefaultValue;

                return Value;
            }
        }

        public void WriteValue(string SectionName, string Key, string Value)
        {
            lock (_lock)
            {
                _cacheModified = true;

                Dictionary<string, string> Section;
                if (!_sections.TryGetValue(SectionName, out Section))
                {
                    Section = new Dictionary<string, string>();
                    _sections.Add(SectionName, Section);
                }

                if (Section.ContainsKey(Key)) Section.Remove(Key);
                Section.Add(Key, Value);

                if (!_modified.TryGetValue(SectionName, out Section))
                {
                    Section = new Dictionary<string, string>();
                    _modified.Add(SectionName, Section);
                }

                if (Section.ContainsKey(Key)) Section.Remove(Key);
                Section.Add(Key, Value);

                if (_autoFlush) PerformFlush();
            }
        }

        private string EncodeByteArray(byte[] Value)
        {
            if (Value == null) return null;

            StringBuilder sb = new StringBuilder();
            foreach (byte b in Value)
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

        private byte[] DecodeByteArray(string Value)
        {
            if (Value == null) return null;

            int l = Value.Length;
            if (l < 2) return new byte[] { };

            l /= 2;
            byte[] Result = new byte[l];
            for (int i = 0; i < l; i++) Result[i] = Convert.ToByte(Value.Substring(i * 2, 2), 16);
            return Result;
        }

        public bool ReadValue(string SectionName, string Key, bool DefaultValue)
        {
            string StringValue = ReadValue(SectionName, Key, DefaultValue.ToString(System.Globalization.CultureInfo.InvariantCulture));
            int Value;
            if (int.TryParse(StringValue, out Value)) return (Value != 0);
            return DefaultValue;
        }

        public int ReadValue(string SectionName, string Key, int DefaultValue)
        {
            string StringValue = ReadValue(SectionName, Key, DefaultValue.ToString(CultureInfo.InvariantCulture));
            int Value;
            if (int.TryParse(StringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out Value)) return Value;
            return DefaultValue;
        }

        public long ReadValue(string SectionName, string Key, long DefaultValue)
        {
            string StringValue = ReadValue(SectionName, Key, DefaultValue.ToString(CultureInfo.InvariantCulture));
            long Value;
            if (long.TryParse(StringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out Value)) return Value;
            return DefaultValue;
        }

        public double ReadValue(string SectionName, string Key, double DefaultValue)
        {
            string StringValue = ReadValue(SectionName, Key, DefaultValue.ToString(CultureInfo.InvariantCulture));
            double Value;
            if (double.TryParse(StringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out Value)) return Value;
            return DefaultValue;
        }

        public byte[] ReadValue(string SectionName, string Key, byte[] DefaultValue)
        {
            string StringValue = ReadValue(SectionName, Key, EncodeByteArray(DefaultValue));
            try
            {
                return DecodeByteArray(StringValue);
            }
            catch (FormatException)
            {
                return DefaultValue;
            }
        }

        public DateTime ReadValue(string SectionName, string Key, DateTime DefaultValue)
        {
            string StringValue = ReadValue(SectionName, Key, DefaultValue.ToString(CultureInfo.InvariantCulture));
            DateTime Value;
            if (DateTime.TryParse(StringValue, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.NoCurrentDateDefault | DateTimeStyles.AssumeLocal, out Value)) return Value;
            return DefaultValue;
        }

        public void WriteValue(string SectionName, string Key, bool Value)
        {
            WriteValue(SectionName, Key, (Value) ? ("1") : ("0"));
        }

        public void WriteValue(string SectionName, string Key, int Value)
        {
            WriteValue(SectionName, Key, Value.ToString(CultureInfo.InvariantCulture));
        }

        public void WriteValue(string SectionName, string Key, long Value)
        {
            WriteValue(SectionName, Key, Value.ToString(CultureInfo.InvariantCulture));
        }

        public void WriteValue(string SectionName, string Key, double Value)
        {
            WriteValue(SectionName, Key, Value.ToString(CultureInfo.InvariantCulture));
        }

        public void WriteValue(string SectionName, string Key, byte[] Value)
        {
            WriteValue(SectionName, Key, EncodeByteArray(Value));
        }

        public void WriteValue(string SectionName, string Key, DateTime Value)
        {
            WriteValue(SectionName, Key, Value.ToString(CultureInfo.InvariantCulture));
        }
    }
}