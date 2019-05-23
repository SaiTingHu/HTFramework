using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace HT.Framework
{
    public sealed class ReplaceFontColorBatch : EditorWindow
    {
        private Color _targetColor = Color.white;
        private GameObject _root;

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("将Root及以下所有Text组件的字体颜色替换为新的颜色", MessageType.Info);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Root：");
            _root = EditorGUILayout.ObjectField(_root, typeof(GameObject), true) as GameObject;
            GUILayout.Label("New Color：");
            _targetColor = EditorGUILayout.ColorField(_targetColor);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUI.enabled = _root;
            if (GUILayout.Button("Replace"))
            {
                int number = 0;
                Text[] ts = _root.GetComponentsInChildren<Text>(true);
                foreach (Text t in ts)
                {
                    if (t.color != _targetColor)
                    {
                        t.color = _targetColor;
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
