using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace HT.Framework
{
    internal sealed class LogIntercepter
    {
        private static LogIntercepter _current;
        private static LogIntercepter Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new LogIntercepter();
                }
                return _current;
            }
        }

        private Type _consoleWindowType;
        private FieldInfo _activeTextInfo;
        private FieldInfo _consoleWindowInfo;
        private MethodInfo _setActiveEntry;
        private object[] _setActiveEntryArgs;
        private object _consoleWindow;

        private LogIntercepter()
        {
            _consoleWindowType = Type.GetType("UnityEditor.ConsoleWindow,UnityEditor");
            _activeTextInfo = _consoleWindowType.GetField("m_ActiveText", BindingFlags.Instance | BindingFlags.NonPublic);
            _consoleWindowInfo = _consoleWindowType.GetField("ms_ConsoleWindow", BindingFlags.Static | BindingFlags.NonPublic);
            _setActiveEntry = _consoleWindowType.GetMethod("SetActiveEntry", BindingFlags.Instance | BindingFlags.NonPublic);
            _setActiveEntryArgs = new object[] { null };
        }

        [OnOpenAsset]
        private static bool OnOpenAsset(int instanceID, int line)
        {
            UnityEngine.Object instance = EditorUtility.InstanceIDToObject(instanceID);
            if (AssetDatabase.GetAssetOrScenePath(instance).EndsWith(".cs"))
            {
                return Current.OpenAsset();
            }
            return false;
        }

        private bool OpenAsset()
        {
            string stackTrace = GetStackTrace();
            if (stackTrace != "")
            {
                if (stackTrace.Contains("[HTFramework.Info]") || stackTrace.Contains("[HTFramework.Warning]") || stackTrace.Contains("[HTFramework.Error]"))
                {
                    string[] paths = stackTrace.Split('\n');
                    int index = 0;
                    for (int i = 0; i < paths.Length; i++)
                    {
                        if (paths[i].Contains(" (at "))
                        {
                            index += 1;

                            if (index == 2)
                            {
                                return OpenScriptAsset(paths[i]);
                            }
                        }
                    }
                }
            }
            return false;
        }

        private bool OpenScriptAsset(string path)
        {
            int startIndex = path.IndexOf(" (at ") + 5;
            int endIndex = path.IndexOf(".cs:") + 3;
            string filePath = path.Substring(startIndex, endIndex - startIndex);
            string lineStr = path.Substring(endIndex + 1, path.Length - endIndex - 2);

            TextAsset asset = AssetDatabase.LoadAssetAtPath<TextAsset>(filePath);
            if (asset != null)
            {
                int line = 0;
                if (int.TryParse(lineStr, out line))
                {
                    object consoleWindow = GetConsoleWindow();
                    _setActiveEntry.Invoke(consoleWindow, _setActiveEntryArgs);

                    EditorGUIUtility.PingObject(asset);
                    AssetDatabase.OpenAsset(asset, line);
                    return true;
                }
            }
            return false;
        }
        
        private string GetStackTrace()
        {
            object consoleWindow = GetConsoleWindow();

            if (consoleWindow != null)
            {
                if (consoleWindow == EditorWindow.focusedWindow.Cast<object>())
                {
                    object value = _activeTextInfo.GetValue(consoleWindow);
                    return value != null ? value.ToString() : "";
                }
            }
            return "";
        }

        private object GetConsoleWindow()
        {
            if (_consoleWindow == null)
            {
                _consoleWindow = _consoleWindowInfo.GetValue(null);
            }
            return _consoleWindow;
        }
    }
}