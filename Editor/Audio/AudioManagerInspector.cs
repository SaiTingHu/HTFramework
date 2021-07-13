using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(AudioManager))]
    [GiteeURL("https://gitee.com/SaiTingHu/HTFramework")]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("https://wanderer.blog.csdn.net/article/details/89874351")]
    internal sealed class AudioManagerInspector : InternalModuleInspector<AudioManager, IAudioHelper>
    {
        private bool _backgroundAudioFoldout = true;
        private bool _singleAudioFoldout = true;
        private bool _multipleAudioFoldout = true;
        private bool _worldAudioFoldout = true;
        private bool _oneShootAudioFoldout = true;

        protected override string Intro
        {
            get
            {
                return "Audio Manager, manage all audio playback, pause, stop, etc.";
            }
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

            GUILayout.BeginHorizontal();
            IntSlider(Target.OneShootPriorityDefault, out Target.OneShootPriorityDefault, 0, 256, "OneShoot Priority");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            FloatSlider(Target.OneShootVolumeDefault, out Target.OneShootVolumeDefault, 0f, 1f, "OneShoot Volume");
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
                GUI.enabled = _helper.BackgroundSource.clip;
                if (GUILayout.Button("Play", EditorStyles.miniButtonLeft))
                {
                    Target.PlayBackgroundMusic(_helper.BackgroundSource.clip);
                }
                if (GUILayout.Button("Pause", EditorStyles.miniButtonMid))
                {
                    Target.PauseBackgroundMusic();
                }
                if (GUILayout.Button("Resume", EditorStyles.miniButtonMid))
                {
                    Target.ResumeBackgroundMusic();
                }
                if (GUILayout.Button("Stop", EditorStyles.miniButtonRight))
                {
                    Target.StopBackgroundMusic();
                }
                GUI.enabled = true;
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                _helper.BackgroundSource.clip = EditorGUILayout.ObjectField("Clip:", _helper.BackgroundSource.clip, typeof(AudioClip), true) as AudioClip;
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label("Loop: " + _helper.BackgroundSource.loop);
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
                GUILayout.Label("Speed: " + _helper.BackgroundSource.pitch);
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
                GUI.enabled = _helper.SingleSource.clip;
                if (GUILayout.Button("Play", EditorStyles.miniButtonLeft))
                {
                    Target.PlaySingleSound(_helper.SingleSource.clip);
                }
                if (GUILayout.Button("Pause", EditorStyles.miniButtonMid))
                {
                    Target.PauseSingleSound();
                }
                if (GUILayout.Button("Resume", EditorStyles.miniButtonMid))
                {
                    Target.ResumeSingleSound();
                }
                if (GUILayout.Button("Stop", EditorStyles.miniButtonRight))
                {
                    Target.StopSingleSound();
                }
                GUI.enabled = true;
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                _helper.SingleSource.clip = EditorGUILayout.ObjectField("Clip:", _helper.SingleSource.clip, typeof(AudioClip), true) as AudioClip;
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label("Loop: " + _helper.SingleSource.loop);
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
                GUILayout.Label("Speed: " + _helper.SingleSource.pitch);
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
                    Target.ClearIdleMultipleAudioSource();
                }
                if (GUILayout.Button("Stop All", EditorStyles.miniButtonRight))
                {
                    Target.StopAllMultipleSound();
                }
                GUILayout.EndHorizontal();

                int mplayingCount = 0;
                int mstopedCount = 0;
                for (int i = 0; i < _helper.MultipleSources.Count; i++)
                {
                    if (_helper.MultipleSources[i].isPlaying)
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
                GUILayout.Label("Source: " + _helper.MultipleSources.Count);
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
                    Target.ClearIdleWorldAudioSource();
                }
                if (GUILayout.Button("Stop All", EditorStyles.miniButtonRight))
                {
                    Target.StopAllWorldSound();
                }
                GUILayout.EndHorizontal();

                int wplayingCount = 0;
                int wstopedCount = 0;
                foreach (var audio in _helper.WorldSources)
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
                GUILayout.Label("Source: " + _helper.WorldSources.Count);
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

            #region OneShoot Audio
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            _oneShootAudioFoldout = EditorGUILayout.Foldout(_oneShootAudioFoldout, "OneShoot Audio", true);
            GUILayout.EndHorizontal();

            if (_oneShootAudioFoldout)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                Target.OneShootVolume = EditorGUILayout.Slider("Volume: ", Target.OneShootVolume, 0f, 1f);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                Target.OneShootPriority = EditorGUILayout.IntSlider("Priority: ", Target.OneShootPriority, 0, 256);
                GUILayout.EndHorizontal();
            }
            #endregion
        }
    }
}