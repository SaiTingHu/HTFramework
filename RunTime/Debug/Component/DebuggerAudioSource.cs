using UnityEngine;

namespace HT.Framework
{
    [CustomDebugger(typeof(AudioSource))]
    public sealed class DebuggerAudioSource : DebuggerComponentBase
    {
        private AudioSource _target;

        public override void OnEnable()
        {
            _target = Target as AudioSource;
        }

        public override void OnDebuggerGUI()
        {
            GUI.contentColor = _target.enabled ? Color.white : Color.gray;
            _target.enabled = GUILayout.Toggle(_target.enabled, "Enabled");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Clip: ");
            GUILayout.Label(_target.clip ? _target.clip.name : "None");
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

            GUILayout.BeginHorizontal();
            _target.mute = GUILayout.Toggle(_target.mute, "Mute");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _target.loop = GUILayout.Toggle(_target.loop, "Loop");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Volume: ");
            _target.volume = FloatField(_target.volume);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _target.volume = GUILayout.HorizontalSlider(_target.volume, 0, 1);
            GUILayout.EndHorizontal();
        }
    }
}