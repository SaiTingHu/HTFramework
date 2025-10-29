using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.Callbacks;
using UObject = UnityEngine.Object;

namespace HT.Framework
{
    /// <summary>
    /// 打开资源回调监控工具箱
    /// </summary>
    internal static class OpenAssetToolkit
    {
        private static int _logFileID = 0;
        private static Type _consoleWindowType = null;
        private static FieldInfo _activeText = null;
        private static FieldInfo _consoleWindow = null;

        private static int LogFileID
        {
            get
            {
                if (_logFileID == 0)
                {
                    _logFileID = AssetDatabase.LoadAssetAtPath<MonoScript>("Assets/HTFramework/RunTime/Utility/Toolkit/Log.cs").GetInstanceID();
                }
                return _logFileID;
            }
        }
        private static Type ConsoleWindowType
        {
            get
            {
                if (_consoleWindowType == null)
                {
                    _consoleWindowType = Type.GetType("UnityEditor.ConsoleWindow,UnityEditor");
                }
                return _consoleWindowType;
            }
        }
        private static FieldInfo ActiveText
        {
            get
            {
                if (_activeText == null)
                {
                    _activeText = ConsoleWindowType.GetField("m_ActiveText", BindingFlags.Instance | BindingFlags.NonPublic);
                }
                return _activeText;
            }
        }
        private static FieldInfo ConsoleWindow
        {
            get
            {
                if (_consoleWindow == null)
                {
                    _consoleWindow = ConsoleWindowType.GetField("ms_ConsoleWindow", BindingFlags.Static | BindingFlags.NonPublic);
                }
                return _consoleWindow;
            }
        }

        [OnOpenAsset(-1)]
        private static bool OnOpenAsset(int instanceID, int line)
        {
            bool isOpened = OpenWithNotepad(instanceID);
            if (isOpened)
                return true;

            if (LogFileID == instanceID)
            {
                object activeText = ActiveText.GetValue(ConsoleWindow.GetValue(null));
                if (activeText != null)
                {
                    string text = activeText.ToString();
                    string[] texts = text.Split('\n');
                    if (IsDebugByLog(texts))
                    {
                        return OpenFirstLinkExcludeLog(texts);
                    }
                }
            }
            return false;
        }

        private static bool OpenWithNotepad(int instanceID)
        {
            string openWithNotepadFormat = EditorPrefs.GetString(EditorPrefsTable.OpenWithNotepadFormat, "");
            if (string.IsNullOrEmpty(openWithNotepadFormat))
                return false;

            UObject obj = EditorUtility.InstanceIDToObject(instanceID);
            if (obj != null)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path))
                {
                    string ext = Path.GetExtension(path).Trim() + ";";
                    if (openWithNotepadFormat.Contains(ext))
                    {
                        ExecutableToolkit.ExecuteNotepad(PathToolkit.ProjectPath + path);
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool IsDebugByLog(string[] texts)
        {
            for (int i = 0; i < texts.Length; i++)
            {
                if (IsStackTrace(texts[i]))
                {
                    string path;
                    int line;
                    SplitPathAndLine(texts[i], out path, out line);
                    if (!string.IsNullOrEmpty(path))
                    {
                        return path.EndsWith("/Log.cs");
                    }
                }
            }
            return false;
        }
        private static bool OpenFirstLinkExcludeLog(string[] texts)
        {
            for (int i = 0; i < texts.Length; i++)
            {
                if (IsStackTrace(texts[i]))
                {
                    string path;
                    int line;
                    SplitPathAndLine(texts[i], out path, out line);
                    if (string.IsNullOrEmpty(path))
                        continue;

                    if (!path.EndsWith("/Log.cs"))
                    {
                        UObject obj = AssetDatabase.LoadAssetAtPath(path, typeof(UObject));
                        if (obj != null)
                        {
                            AssetDatabase.OpenAsset(obj, line);
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        private static bool IsStackTrace(string text)
        {
            return text.Contains(" (at ");
        }
        private static void SplitPathAndLine(string text, out string path, out int line)
        {
            string[] texts = text.Split(" (at ");
            if (texts.Length == 2)
            {
                texts = texts[1].Split(':');
                if (texts.Length == 2)
                {
                    path = texts[0].Trim();
                    int.TryParse(texts[1].Replace(")", ""), out line);
                }
                else
                {
                    path = null;
                    line = 0;
                }
            }
            else
            {
                path = null;
                line = 0;
            }
        }
    }
}