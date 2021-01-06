using UnityEngine;

namespace HT.Framework
{
    [CustomDebugger(typeof(AudioManager))]
    internal sealed class DebuggerAudioManager : DebuggerComponentBase
    {
        private AudioManager _target;

        public override void OnEnable()
        {
            _target = Target as AudioManager;
        }
        public override void OnDebuggerGUI()
        {
            GUILayout.BeginHorizontal();
            _target.Mute = GUILayout.Toggle(_target.Mute, "Mute");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Background Volume: ", GUILayout.Width(180));
            _target.BackgroundVolume = GUILayout.HorizontalSlider(_target.BackgroundVolume, 0f, 1f);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Single Volume: ", GUILayout.Width(180));
            _target.SingleVolume = GUILayout.HorizontalSlider(_target.SingleVolume, 0f, 1f);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Multiple Volume: ", GUILayout.Width(180));
            _target.MultipleVolume = GUILayout.HorizontalSlider(_target.MultipleVolume, 0f, 1f);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("World Volume: ", GUILayout.Width(180));
            _target.WorldVolume = GUILayout.HorizontalSlider(_target.WorldVolume, 0f, 1f);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("OneShoot Volume: ", GUILayout.Width(180));
            _target.OneShootVolume = GUILayout.HorizontalSlider(_target.OneShootVolume, 0f, 1f);
            GUILayout.EndHorizontal();
        }
    }
}