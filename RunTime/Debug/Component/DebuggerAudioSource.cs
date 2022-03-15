using UnityEngine;

namespace HT.Framework
{
    [CustomDebugger(typeof(AudioSource))]
    internal sealed class DebuggerAudioSource : DebuggerComponentBase
    {
        private AudioSource _target;

        public override void OnEnable()
        {
            _target = Target as AudioSource;
        }
        public override void OnDebuggerGUI()
        {
            GUI.contentColor = _target.enabled ? Color.white : Color.gray;

            _target.enabled = BoolField("Enabled", _target.enabled);
            ObjectFieldReadOnly("Clip", _target.clip);
            _target.mute = BoolField("Mute", _target.mute);
            _target.playOnAwake = BoolField("Play On Awake", _target.playOnAwake);
            _target.loop = BoolField("Loop", _target.loop);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Volume", GUILayout.Width(120));
            _target.volume = GUILayout.HorizontalSlider(_target.volume, 0, 1);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("3D Blend", GUILayout.Width(120));
            _target.spatialBlend = GUILayout.HorizontalSlider(_target.spatialBlend, 0, 1);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(_target.isPlaying ? "Pause" : "Play"))
            {
                if (_target.isPlaying)
                    _target.Pause();
                else
                    _target.UnPause();
            }
            if (GUILayout.Button("Replay"))
            {
                _target.Stop();
                _target.Play();
            }
            if (GUILayout.Button("Stop"))
            {
                _target.Stop();
            }
            GUILayout.EndHorizontal();
        }
    }
}