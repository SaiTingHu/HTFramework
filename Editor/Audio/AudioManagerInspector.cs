using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(AudioManager))]
    public sealed class AudioManagerInspector : ModuleEditor
    {
        private AudioManager _target;

        protected override void OnEnable()
        {
            _target = target as AudioManager;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Audio Manager, manage all audio playback, pause, stop, etc.", MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            IntSlider(_target.BackgroundPriority, out _target.BackgroundPriority, 0, 256, "Background Priority");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            FloatSlider(_target.BackgroundVolume, out _target.BackgroundVolume, 0f, 1f, "Background Volume");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            IntSlider(_target.SinglePriority, out _target.SinglePriority, 0, 256, "Single Priority");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            FloatSlider(_target.SingleVolume, out _target.SingleVolume, 0f, 1f, "Single Volume");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            IntSlider(_target.MultiplePriority, out _target.MultiplePriority, 0, 256, "Multiple Priority");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            FloatSlider(_target.MultipleVolume, out _target.MultipleVolume, 0f, 1f, "Multiple Volume");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            IntSlider(_target.WorldPriority, out _target.WorldPriority, 0, 256, "WorldSound Priority");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            FloatSlider(_target.WorldVolume, out _target.WorldVolume, 0f, 1f, "WorldSound Volume");
            GUILayout.EndHorizontal();
        }
    }
}
