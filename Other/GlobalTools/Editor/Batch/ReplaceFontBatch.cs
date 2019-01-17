using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace HT.Framework
{
    public sealed class ReplaceFontBatch : EditorWindow
    {
        private Font _targetFont;
        private GameObject _root;

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("将Root及以下所有Text组件的字体替换为新的字体", MessageType.Info);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Root：");
            _root = EditorGUILayout.ObjectField(_root, typeof(GameObject), true) as GameObject;
            GUILayout.Label("New Font：");
            _targetFont = EditorGUILayout.ObjectField(_targetFont, typeof(Font), false) as Font;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUI.enabled = _root && _targetFont;
            if (GUILayout.Button("Replace"))
            {
                int number = 0;
                Text[] ts = _root.GetComponentsInChildren<Text>(true);
                foreach (Text t in ts)
                {
                    if (t.font != _targetFont)
                    {
                        t.font = _targetFont;
                        number += 1;
                    }
                }
                GlobalTools.LogInfo("[" + _root.name + "] 替换完成！共替换了 " + number + " 处！");
            }
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
        }
    }
}
