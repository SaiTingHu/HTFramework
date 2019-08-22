using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(AudioManager))]
    public sealed class AudioManagerInspector : HTFEditor<AudioManager>
    {
        private AudioSource _backgroundAudio;
        private AudioSource _singleAudio;
        private List<AudioSource> _multipleAudio;
        private Dictionary<GameObject, AudioSource> _worldAudio;
        
        protected override void OnRuntimeEnable()
        {
            base.OnRuntimeEnable();

            _backgroundAudio = Target.GetType().GetField("_backgroundAudio", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Target) as AudioSource;
            _singleAudio = Target.GetType().GetField("_singleAudio", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Target) as AudioSource;
            _multipleAudio = Target.GetType().GetField("_multipleAudio", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Target) as List<AudioSource>;
            _worldAudio = Target.GetType().GetField("_worldAudio", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Target) as Dictionary<GameObject, AudioSource>;
        }

        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Audio Manager, manage all audio playback, pause, stop, etc.", MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            IntSlider(Target.BackgroundPriority, out Target.BackgroundPriority, 0, 256, "Background Priority");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            FloatSlider(Target.BackgroundVolume, out Target.BackgroundVolume, 0f, 1f, "Background Volume");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            IntSlider(Target.SinglePriority, out Target.SinglePriority, 0, 256, "Single Priority");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            FloatSlider(Target.SingleVolume, out Target.SingleVolume, 0f, 1f, "Single Volume");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            IntSlider(Target.MultiplePriority, out Target.MultiplePriority, 0, 256, "Multiple Priority");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            FloatSlider(Target.MultipleVolume, out Target.MultipleVolume, 0f, 1f, "Multiple Volume");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            IntSlider(Target.WorldPriority, out Target.WorldPriority, 0, 256, "WorldSound Priority");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            FloatSlider(Target.WorldVolume, out Target.WorldVolume, 0f, 1f, "WorldSound Volume");
            GUILayout.EndHorizontal();
        }

        protected override void OnInspectorRuntimeGUI()
        {
            base.OnInspectorRuntimeGUI();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Mute: ");
            bool mute = EditorGUILayout.Toggle(Target.Mute);
            if (Target.Mute != mute)
            {
                Target.Mute = mute;
            }
            GUILayout.EndHorizontal();

            #region Background Audio
            GUILayout.BeginHorizontal();
            GUILayout.Label("Background Audio: ");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Pause", "minibuttonleft"))
            {
                Target.PauseBackgroundMusic();
            }
            if (GUILayout.Button("UnPause", "minibuttonmid"))
            {
                Target.UnPauseBackgroundMusic();
            }
            if (GUILayout.Button("Stop", "minibuttonright"))
            {
                Target.StopBackgroundMusic();
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
                Target.PauseSingleSound();
            }
            if (GUILayout.Button("UnPause", "minibuttonmid"))
            {
                Target.UnPauseSingleSound();
            }
            if (GUILayout.Button("Stop", "minibuttonright"))
            {
                Target.StopSingleSound();
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
                Target.ClearIdleMultipleAudioSource();
            }
            if (GUILayout.Button("Stop All", "minibuttonright"))
            {
                Target.StopAllMultipleSound();
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
                Target.ClearIdleWorldAudioSource();
            }
            if (GUILayout.Button("Stop All", "minibuttonright"))
            {
                Target.StopAllWorldSound();
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
        }
    }
}
