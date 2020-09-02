using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(AudioManager))]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("https://wanderer.blog.csdn.net/article/details/89874351")]
    internal sealed class AudioManagerInspector : InternalModuleInspector<AudioManager>
    {
        private bool _backgroundAudioFoldout = true;
        private bool _singleAudioFoldout = true;
        private bool _multipleAudioFoldout = true;
        private bool _worldAudioFoldout = true;
        private IAudioHelper _audioHelper;

        protected override string Intro
        {
            get
            {
                return "Audio Manager, manage all audio playback, pause, stop, etc.";
            }
        }

        protected override Type HelperInterface
        {
            get
            {
                return typeof(IAudioHelper);
            }
        }

        protected override void OnRuntimeEnable()
        {
            base.OnRuntimeEnable();

            _audioHelper = Target.GetType().GetField("_helper", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Target) as IAudioHelper;
        }

        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUI.enabled = !EditorApplication.isPlaying;
            
            GUILayout.BeginHorizontal();
            Toggle(Target.MuteDefault, out Target.MuteDefault, "Mute");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            IntSlider(Target.BackgroundPriorityDefault, out Target.BackgroundPriorityDefault, 0, 256, "Background Priority");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            FloatSlider(Target.BackgroundVolumeDefault, out Target.BackgroundVolumeDefault, 0f, 1f, "Background Volume");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            IntSlider(Target.SinglePriorityDefault, out Target.SinglePriorityDefault, 0, 256, "Single Priority");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            FloatSlider(Target.SingleVolumeDefault, out Target.SingleVolumeDefault, 0f, 1f, "Single Volume");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            IntSlider(Target.MultiplePriorityDefault, out Target.MultiplePriorityDefault, 0, 256, "Multiple Priority");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            FloatSlider(Target.MultipleVolumeDefault, out Target.MultipleVolumeDefault, 0f, 1f, "Multiple Volume");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            IntSlider(Target.WorldPriorityDefault, out Target.WorldPriorityDefault, 0, 256, "WorldSound Priority");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            FloatSlider(Target.WorldVolumeDefault, out Target.WorldVolumeDefault, 0f, 1f, "WorldSound Volume");
            GUILayout.EndHorizontal();

            GUI.enabled = true;
        }

        protected override void OnInspectorRuntimeGUI()
        {
            base.OnInspectorRuntimeGUI();

            GUILayout.BeginHorizontal();
            Target.Mute = EditorGUILayout.Toggle("Mute", Target.Mute);
            GUILayout.EndHorizontal();

            #region Background Audio
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            _backgroundAudioFoldout = EditorGUILayout.Foldout(_backgroundAudioFoldout, "Background Audio", true);
            GUILayout.EndHorizontal();

            if (_backgroundAudioFoldout)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUI.enabled = _audioHelper.BackgroundAudio.clip;
                if (GUILayout.Button("Play", EditorStyles.miniButtonLeft))
                {
                    Main.m_Audio.PlayBackgroundMusic(_audioHelper.BackgroundAudio.clip);
                }
                if (GUILayout.Button("Pause", EditorStyles.miniButtonMid))
                {
                    Main.m_Audio.PauseBackgroundMusic();
                }
                if (GUILayout.Button("UnPause", EditorStyles.miniButtonMid))
                {
                    Main.m_Audio.UnPauseBackgroundMusic();
                }
                if (GUILayout.Button("Stop", EditorStyles.miniButtonRight))
                {
                    Main.m_Audio.StopBackgroundMusic();
                }
                GUI.enabled = true;
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                _audioHelper.BackgroundAudio.clip = EditorGUILayout.ObjectField("Clip:", _audioHelper.BackgroundAudio.clip, typeof(AudioClip), true) as AudioClip;
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label("Loop: " + _audioHelper.BackgroundAudio.loop);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                Target.BackgroundVolume = EditorGUILayout.Slider("Volume: ", Target.BackgroundVolume, 0f, 1f);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                Target.BackgroundPriority = EditorGUILayout.IntSlider("Priority: ", Target.BackgroundPriority, 0, 256);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label("Speed: " + _audioHelper.BackgroundAudio.pitch);
                GUILayout.EndHorizontal();
            }
            #endregion

            #region Single Audio
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            _singleAudioFoldout = EditorGUILayout.Foldout(_singleAudioFoldout, "Single Audio", true);
            GUILayout.EndHorizontal();

            if (_singleAudioFoldout)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUI.enabled = _audioHelper.SingleAudio.clip;
                if (GUILayout.Button("Play", EditorStyles.miniButtonLeft))
                {
                    Main.m_Audio.PlaySingleSound(_audioHelper.SingleAudio.clip);
                }
                if (GUILayout.Button("Pause", EditorStyles.miniButtonMid))
                {
                    Main.m_Audio.PauseSingleSound();
                }
                if (GUILayout.Button("UnPause", EditorStyles.miniButtonMid))
                {
                    Main.m_Audio.UnPauseSingleSound();
                }
                if (GUILayout.Button("Stop", EditorStyles.miniButtonRight))
                {
                    Main.m_Audio.StopSingleSound();
                }
                GUI.enabled = true;
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                _audioHelper.SingleAudio.clip = EditorGUILayout.ObjectField("Clip:", _audioHelper.SingleAudio.clip, typeof(AudioClip), true) as AudioClip;
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label("Loop: " + _audioHelper.SingleAudio.loop);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                Target.SingleVolume = EditorGUILayout.Slider("Volume: ", Target.SingleVolume, 0f, 1f);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                Target.SinglePriority = EditorGUILayout.IntSlider("Priority: ", Target.SinglePriority, 0, 256);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label("Speed: " + _audioHelper.SingleAudio.pitch);
                GUILayout.EndHorizontal();
            }
            #endregion

            #region Multiple Audio
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            _multipleAudioFoldout = EditorGUILayout.Foldout(_multipleAudioFoldout, "Multiple Audio", true);
            GUILayout.EndHorizontal();

            if (_multipleAudioFoldout)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                if (GUILayout.Button("Clear", EditorStyles.miniButtonLeft))
                {
                    Main.m_Audio.ClearIdleMultipleAudioSource();
                }
                if (GUILayout.Button("Stop All", EditorStyles.miniButtonRight))
                {
                    Main.m_Audio.StopAllMultipleSound();
                }
                GUILayout.EndHorizontal();

                int mplayingCount = 0;
                int mstopedCount = 0;
                for (int i = 0; i < _audioHelper.MultipleAudios.Count; i++)
                {
                    if (_audioHelper.MultipleAudios[i].isPlaying)
                    {
                        mplayingCount += 1;
                    }
                    else
                    {
                        mstopedCount += 1;
                    }
                }

                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                Target.MultipleVolume = EditorGUILayout.Slider("Volume: ", Target.MultipleVolume, 0f, 1f);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                Target.MultiplePriority = EditorGUILayout.IntSlider("Priority: ", Target.MultiplePriority, 0, 256);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label("Source: " + _audioHelper.MultipleAudios.Count);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label("Playing: " + mplayingCount);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label("Stoped: " + mstopedCount);
                GUILayout.EndHorizontal();
            }
            #endregion

            #region World Audio
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            _worldAudioFoldout = EditorGUILayout.Foldout(_worldAudioFoldout, "World Audio", true);
            GUILayout.EndHorizontal();

            if (_worldAudioFoldout)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                if (GUILayout.Button("Clear", EditorStyles.miniButtonLeft))
                {
                    Main.m_Audio.ClearIdleWorldAudioSource();
                }
                if (GUILayout.Button("Stop All", EditorStyles.miniButtonRight))
                {
                    Main.m_Audio.StopAllWorldSound();
                }
                GUILayout.EndHorizontal();

                int wplayingCount = 0;
                int wstopedCount = 0;
                foreach (KeyValuePair<GameObject, AudioSource> audio in _audioHelper.WorldAudios)
                {
                    if (audio.Value.isPlaying)
                    {
                        wplayingCount += 1;
                    }
                    else
                    {
                        wstopedCount += 1;
                    }
                }

                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                Target.WorldVolume = EditorGUILayout.Slider("Volume: ", Target.WorldVolume, 0f, 1f);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                Target.WorldPriority = EditorGUILayout.IntSlider("Priority: ", Target.WorldPriority, 0, 256);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label("Source: " + _audioHelper.WorldAudios.Count);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label("Playing: " + wplayingCount);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label("Stoped: " + wstopedCount);
                GUILayout.EndHorizontal();
            }
            #endregion
        }
    }
}