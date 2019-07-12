using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(AudioManager))]
    public sealed class AudioManagerInspector : ModuleEditor
    {
        private AudioManager _target;

        private AudioSource _backgroundAudio;
        private AudioSource _singleAudio;
        private List<AudioSource> _multipleAudio;
        private Dictionary<GameObject, AudioSource> _worldAudio;

        protected override void OnEnable()
        {
            _target = target as AudioManager;

            base.OnEnable();
        }

        protected override void OnPlayingEnable()
        {
            base.OnPlayingEnable();

            _backgroundAudio = _target.GetType().GetField("_backgroundAudio", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(_target) as AudioSource;
            _singleAudio = _target.GetType().GetField("_singleAudio", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(_target) as AudioSource;
            _multipleAudio = _target.GetType().GetField("_multipleAudio", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(_target) as List<AudioSource>;
            _worldAudio = _target.GetType().GetField("_worldAudio", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(_target) as Dictionary<GameObject, AudioSource>;
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

            base.OnInspectorGUI();
        }

        protected override void OnPlayingInspectorGUI()
        {
            base.OnPlayingInspectorGUI();

            GUILayout.BeginVertical("Helpbox");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Runtime Data", "BoldLabel");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Mute: ");
            bool mute = EditorGUILayout.Toggle(_target.Mute);
            if (_target.Mute != mute)
            {
                _target.Mute = mute;
            }
            GUILayout.EndHorizontal();

            #region Background Audio
            GUILayout.BeginHorizontal();
            GUILayout.Label("Background Audio: ");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Pause", "minibuttonleft"))
            {
                _target.PauseBackgroundMusic();
            }
            if (GUILayout.Button("UnPause", "minibuttonmid"))
            {
                _target.UnPauseBackgroundMusic();
            }
            if (GUILayout.Button("Stop", "minibuttonright"))
            {
                _target.StopBackgroundMusic();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.Label("Clip: " + (_backgroundAudio.clip != null ? _backgroundAudio.clip.name : "<None>"));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.Label("Loop: " + _backgroundAudio.loop);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.Label("Volume: " + _backgroundAudio.volume);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.Label("Speed: " + _backgroundAudio.pitch);
            GUILayout.EndHorizontal();
            #endregion

            #region Single Audio
            GUILayout.BeginHorizontal();
            GUILayout.Label("Single Audio: ");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Pause", "minibuttonleft"))
            {
                _target.PauseSingleSound();
            }
            if (GUILayout.Button("UnPause", "minibuttonmid"))
            {
                _target.UnPauseSingleSound();
            }
            if (GUILayout.Button("Stop", "minibuttonright"))
            {
                _target.StopSingleSound();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.Label("Clip: " + (_singleAudio.clip != null ? _singleAudio.clip.name : "<None>"));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.Label("Loop: " + _singleAudio.loop);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.Label("Volume: " + _singleAudio.volume);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.Label("Speed: " + _singleAudio.pitch);
            GUILayout.EndHorizontal();
            #endregion

            #region Multiple Audio
            GUILayout.BeginHorizontal();
            GUILayout.Label("Multiple Audio: ");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Clear", "minibuttonleft"))
            {
                _target.ClearIdleMultipleAudioSource();
            }
            if (GUILayout.Button("Stop All", "minibuttonright"))
            {
                _target.StopAllMultipleSound();
            }
            GUILayout.EndHorizontal();

            int playingCount = 0;
            int stopedCount = 0;
            for (int i = 0; i < _multipleAudio.Count; i++)
            {
                if (_multipleAudio[i].isPlaying)
                {
                    playingCount += 1;
                }
                else
                {
                    stopedCount += 1;
                }
            }

            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.Label("Source: " + _multipleAudio.Count);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.Label("Playing: " + playingCount);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.Label("Stoped: " + stopedCount);
            GUILayout.EndHorizontal();
            #endregion

            #region World Audio
            GUILayout.BeginHorizontal();
            GUILayout.Label("World Audio: ");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Clear", "minibuttonleft"))
            {
                _target.ClearIdleWorldAudioSource();
            }
            if (GUILayout.Button("Stop All", "minibuttonright"))
            {
                _target.StopAllWorldSound();
            }
            GUILayout.EndHorizontal();

            playingCount = 0;
            stopedCount = 0;
            foreach(KeyValuePair<GameObject, AudioSource> audio in _worldAudio)
            {
                if (audio.Value.isPlaying)
                {
                    playingCount += 1;
                }
                else
                {
                    stopedCount += 1;
                }
            }

            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.Label("Source: " + _worldAudio.Count);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.Label("Playing: " + playingCount);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.Label("Stoped: " + stopedCount);
            GUILayout.EndHorizontal();
            #endregion
            
            GUILayout.EndVertical();
        }
    }
}
