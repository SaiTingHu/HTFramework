using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(AudioManager))]
    public sealed class AudioManagerInspector : Editor
    {
        private AudioManager _target;

        private void OnEnable()
        {
            _target = target as AudioManager;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Audio Manager, manage all audio playback, pause, stop, etc.", MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Background Priority", GUILayout.Width(120));
            _target.BackgroundPriority = EditorGUILayout.IntSlider(_target.BackgroundPriority, 0, 256);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Background Volume", GUILayout.Width(120));
            _target.BackgroundVolume = EditorGUILayout.Slider(_target.BackgroundVolume, 0f, 1f);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Single Priority", GUILayout.Width(120));
            _target.SinglePriority = EditorGUILayout.IntSlider(_target.SinglePriority, 0, 256);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Single Volume", GUILayout.Width(120));
            _target.SingleVolume = EditorGUILayout.Slider(_target.SingleVolume, 0f, 1f);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Multiple Priority", GUILayout.Width(120));
            _target.MultiplePriority = EditorGUILayout.IntSlider(_target.MultiplePriority, 0, 256);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Multiple Volume", GUILayout.Width(120));
            _target.MultipleVolume = EditorGUILayout.Slider(_target.MultipleVolume, 0f, 1f);
            GUILayout.EndHorizontal();
        }
    }
}
