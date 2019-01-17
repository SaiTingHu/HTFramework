using UnityEngine;
using UnityEditor;

namespace HT.Framework
{
    [CustomEditor(typeof(ExceptionHandler))]
    public sealed class ExceptionHandlerInspector : Editor
    {
        private ExceptionHandler _target;

        private void OnEnable()
        {
            _target = target as ExceptionHandler;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Exception handler, When an exception occurs, he catches it!", MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUI.color = _target.IsHandler ? Color.white : Color.gray;
            _target.IsHandler = GUILayout.Toggle(_target.IsHandler, "Is Handler");
            GUI.color = Color.white;
            GUILayout.EndHorizontal();

            if (_target.IsHandler)
            {
                GUILayout.BeginHorizontal();
                _target.IsQuitWhenException = GUILayout.Toggle(_target.IsQuitWhenException, "Is Quit When Exception");
                GUILayout.EndHorizontal();
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(_target);
            }
        }
    }
}
