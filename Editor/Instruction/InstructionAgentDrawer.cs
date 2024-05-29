using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomPropertyDrawer(typeof(InstructionAgent))]
    internal sealed class InstructionAgentDrawer : PropertyDrawer
    {
        private bool _isReadOnly = false;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty code = property.FindPropertyRelative("Code");
            _isReadOnly = !GUI.enabled;

            Rect sub = position;
            sub.Set(position.x, position.y, position.width, 20);
            property.isExpanded = EditorGUI.Foldout(sub, property.isExpanded, label, true);

            if (property.isExpanded)
            {
                sub.Set(position.x, position.y + 22, position.width, 100);
                code.stringValue = EditorGUI.TextArea(sub, code.stringValue);

                sub.Set(position.x + position.width - 220, position.y + 124, 80, 20);
                if (GUI.Button(sub, "New Code", EditorGlobalTools.Styles.ButtonLeft))
                {
                    GUI.FocusControl(null);
                    string[] keywords = File.ReadAllLines(Application.dataPath + "/HTFramework/Editor/Instruction/InstructionKeywords.txt");
                    GenericMenu gm = new GenericMenu();
                    for (int i = 0; i < keywords.Length; i++)
                    {
                        string keyword = keywords[i];
                        string key = keyword.Split(' ')[0];
                        gm.AddItem(new GUIContent(key), false, () =>
                        {
                            code.stringValue += $"\r\n{keyword}";
                            code.serializedObject.ApplyModifiedProperties();
                        });
                    }
                    gm.ShowAsContext();
                }
                sub.Set(position.x + position.width - 140, position.y + 124, 80, 20);
                if (GUI.Button(sub, "Clear Code", EditorGlobalTools.Styles.ButtonMid))
                {
                    GUI.FocusControl(null);
                    code.stringValue = "";
                }
                sub.Set(position.x + position.width - 60, position.y + 124, 60, 20);
                GUI.enabled = EditorApplication.isPlaying && !_isReadOnly;
                if (GUI.Button(sub, "Execute", EditorGlobalTools.Styles.ButtonRight))
                {
                    MethodInfo execute = fieldInfo.FieldType.GetMethod("Execute", BindingFlags.Public | BindingFlags.Instance);
                    execute.Invoke(fieldInfo.GetValue(code.serializedObject.targetObject), null);
                }
                GUI.enabled = !_isReadOnly;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return property.isExpanded ? 146 : 22;
        }
    }
}