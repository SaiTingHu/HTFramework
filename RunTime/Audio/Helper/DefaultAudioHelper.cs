using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 默认的音频管理器助手
    /// </summary>
    public sealed class DefaultAudioHelper : IAudioHelper
    {
        private bool _isMute = false;
        private int _backgroundPriority = 0;
        private int _singlePriority = 10;
        private int _multiplePriority = 20;
        private int _worldPriority = 30;
        private float _backgroundVolume = 0.6f;
        private float _singleVolume = 1;
        private float _multipleVolume = 1;
        private float _worldVolume = 1;

        /// <summary>
        /// 音频管理器
        /// </summary>
        public InternalModuleBase Module { get; set; }
        /// <summary>
        /// 背景音乐音源
        /// </summary>
        public AudioSource BackgroundAudio { get; set; }
        /// <summary>
        /// 单通道音效音源
        /// </summary>
        public AudioSource SingleAudio { get; set; }
        /// <summary>
        /// 多通道音效音源
        /// </summary>
        public List<AudioSource> MultipleAudios { get; set; } = new List<AudioSource>();
        /// <summary>
        /// 世界音效音源
        /// </summary>
        public Dictionary<GameObject, AudioSource> WorldAudios { get; set; } = new Dictionary<GameObject, AudioSource>();
        /// <summary>
        /// 静音
        /// </summary>
        public bool Mute
        {
            get
            {
                return _isMute;
            }
            set
            {
                if (_isMute != value)
                {
                    _isMute = value;
                    BackgroundAudio.mute = _isMute;
                    SingleAudio.mute = _isMute;
                    for (int i = 0; i < MultipleAudios.Count; i++)
                    {
                        MultipleAudios[i].mute = _isMute;
                    }
                    foreach (var audio in WorldAudios)
                    {
                        audio.Value.mute = _isMute;
                    }
                }
            }
        }
        /// <summary>
        /// 背景音乐是否播放中
        /// </summary>
        public bool IsBackgroundPlaying
        {
            get
            {
                return BackgroundAudio.isPlaying;
            }
        }
        /// <summary>
        /// 单通道音效是否播放中
        /// </summary>
        public bool IsSinglePlaying
        {
            get
            {
                return SingleAudio.isPlaying;
            }
        }
        /// <summary>
        /// 背景音乐优先级
        /// </summary>
        public int BackgroundPriority
        {
            get
            {
                return _backgroundPriority;
            }
            set
            {
                if (_backgroundPriority != value)
                {
                    _backgroundPriority = value;
                    BackgroundAudio.priority = _backgroundPriority;
                }
            }
        }
        /// <summary>
        /// 单通道音效优先级
        /// </summary>
        public int SinglePriority
        {
            get
            {
                return _singlePriority;
            }
            set
            {
                if (_singlePriority != value)
                {
                    _singlePriority = value;
                    SingleAudio.priority = _singlePriority;
                }
            }
        }
        /// <summary>
        /// 多通道音效优先级
        /// </summary>
        public int MultiplePriority
        {
            get
            {
                return _multiplePriority;
            }
            set
            {
                if (_multiplePriority != value)
                {
                    _multiplePriority = value;
                    for (int i = 0; i < MultipleAudios.Count; i++)
                    {
                        MultipleAudios[i].priority = _multiplePriority;
                    }
                }
            }
        }
        /// <summary>
        /// 世界音效优先级
        /// </summary>
        public int WorldPriority
        {
            get
            {
                return _worldPriority;
            }
            set
            {
                if (_worldPriority != value)
                {
                    _worldPriority = value;
                    foreach (var audio in WorldAudios)
                    {
                        audio.Value.priority = _worldPriority;
                    }
                }
            }
        }
        /// <summary>
        /// 背景音乐音量
        /// </summary>
        public float BackgroundVolume
        {
            get
            {
                return _backgroundVolume;
            }
            set
            {
                if (!_backgroundVolume.Approximately(value))
                {
                    _backgroundVolume = value;
                    BackgroundAudio.volume = _backgroundVolume;
                }
            }
        }
        /// <summary>
        /// 单通道音效音量
        /// </summary>
        public float SingleVolume
        {
            get
            {
                return _singleVolume;
            }
            set
            {
                if (!_singleVolume.Approximately(value))
                {
                    _singleVolume = value;
                    SingleAudio.volume = _singleVolume;
                }
            }
        }
        /// <summary>
        /// 多通道音效音量
        /// </summary>
        public float MultipleVolume
        {
            get
            {
                return _multipleVolume;
            }
            set
            {
                if (!_multipleVolume.Approximately(value))
                {
                    _multipleVolume = value;
                    for (int i = 0; i < MultipleAudios.Count; i++)
                    {
                        MultipleAudios[i].volume = _multipleVolume;
                    }
                }
            }
        }
        /// <summary>
        /// 世界音效音量
        /// </summary>
        public float WorldVolume
        {
            get
            {
                return _worldVolume;
            }
            set
            {
                if (!_worldVolume.Approximately(value))
                {
                    _worldVolume = value;
                    foreach (var audio in WorldAudios)
                    {
                        audio.Value.volume = _worldVolume;
                    }
                }
            }
        }
        /// <summary>
        /// 单通道音效播放标记
        /// </summary>
        public bool SingleSoundPlayDetector { get; set; } = false;

        /// <summary>
        /// 播放背景音乐
        /// </summary>
        /// <param name="clip">音乐剪辑</param>
        /// <param name="isLoop">是否循环</param>
        /// <param name="speed">播放速度</param>
        public void PlayBackgroundMusic(AudioClip clip, bool isLoop = true, float speed = 1)
        {
            if (BackgroundAudio.isPlaying)
            {
                BackgroundAudio.Stop();
            }

            BackgroundAudio.clip = clip;
            BackgroundAudio.loop = isLoop;
            BackgroundAudio.pitch = speed;
            BackgroundAudio.Play();
        }
        /// <summary>
        /// 暂停播放背景音乐
        /// </summary>
        /// <param name="isGradual">是否渐进式</param>
        public void PauseBackgroundMusic(bool isGradual = true)
        {
            if (isGradual)
            {
                BackgroundAudio.DOFade(0, 2).OnComplete(() =>
                {
                    BackgroundAudio.Pause();
                    BackgroundAudio.volume = BackgroundVolume;
                });
            }
            else
            {
                BackgroundAudio.Pause();
            }
        }
        /// <summary>
        /// 恢复播放背景音乐
        /// </summary>
        /// <param name="isGradual">是否渐进式</param>
        public void UnPauseBackgroundMusic(bool isGradual = true)
        {
            if (isGradual)
            {
                BackgroundAudio.volume = 0;
                BackgroundAudio.UnPause();
                BackgroundAudio.DOFade(BackgroundVolume, 2);
            }
            else
            {
                BackgroundAudio.UnPause();
            }
        }
        /// <summary>
        /// 停止播放背景音乐
        /// </summary>
        public void StopBackgroundMusic()
        {
            if (BackgroundAudio.isPlaying)
            {
                BackgroundAudio.Stop();
            }
        }

        /// <summary>
        /// 播放单通道音效
        /// </summary>
        /// <param name="clip">音乐剪辑</param>
        /// <param name="isLoop">是否循环</param>
        /// <param name="speed">播放速度</param>
        public void PlaySingleSound(AudioClip clip, bool isLoop = false, float speed = 1)
        {
            if (SingleAudio.isPlaying)
            {
                SingleAudio.Stop();
            }

            SingleAudio.clip = clip;
            SingleAudio.loop = isLoop;
            SingleAudio.pitch = speed;
            SingleAudio.Play();
            SingleSoundPlayDetector = true;
        }
        /// <summary>
        /// 暂停播放单通道音效
        /// </summary>
        /// <param name="isGradual">是否渐进式</param>
        public void PauseSingleSound(bool isGradual = true)
        {
            if (isGradual)
            {
                SingleAudio.DOFade(0, 2).OnComplete(() =>
                {
                    SingleAudio.Pause();
                    SingleAudio.volume = SingleVolume;
                });
            }
            else
            {
                SingleAudio.Pause();
            }
        }
        /// <summary>
        /// 恢复播放单通道音效
        /// </summary>
        /// <param name="isGradual">是否渐进式</param>
        public void UnPauseSingleSound(bool isGradual = true)
        {
            if (isGradual)
            {
                SingleAudio.volume = 0;
                SingleAudio.UnPause();
                SingleAudio.DOFade(SingleVolume, 2);
            }
            else
            {
                SingleAudio.UnPause();
            }
        }
        /// <summary>
        /// 停止播放单通道音效
        /// </summary>
        public void StopSingleSound()
        {
            if (SingleAudio.isPlaying)
            {
                SingleAudio.Stop();
            }
        }

        /// <summary>
        /// 播放多通道音效
        /// </summary>
        /// <param name="clip">音乐剪辑</param>
        /// <param name="isLoop">是否循环</param>
        /// <param name="speed">播放速度</param>
        public void PlayMultipleSound(AudioClip clip, bool isLoop = false, float speed = 1)
        {
            AudioSource audio = ExtractIdleMultipleAudioSource();
            audio.clip = clip;
            audio.loop = isLoop;
            audio.pitch = speed;
            audio.Play();
        }
        /// <summary>
        /// 停止播放指定的多通道音效
        /// </summary>
        /// <param name="clip">音乐剪辑</param>
        public void StopMultipleSound(AudioClip clip)
        {
            for (int i = 0; i < MultipleAudios.Count; i++)
            {
                if (MultipleAudios[i].isPlaying)
                {
                    if (MultipleAudios[i].clip == clip)
                    {
                        MultipleAudios[i].Stop();
                    }
                }
            }
        }
        /// <summary>
        /// 停止播放所有多通道音效
        /// </summary>
        public void StopAllMultipleSound()
        {
            for (int i = 0; i < MultipleAudios.Count; i++)
            {
                if (MultipleAudios[i].isPlaying)
                {
                    MultipleAudios[i].Stop();
                }
            }
        }
        /// <summary>
        /// 销毁所有闲置中的多通道音效的音源
        /// </summary>
        public void ClearIdleMultipleAudioSource()
        {
            for (int i = 0; i < MultipleAudios.Count; i++)
            {
                if (!MultipleAudios[i].isPlaying)
                {
                    AudioSource audio = MultipleAudios[i];
                    MultipleAudios.RemoveAt(i);
                    i -= 1;
                    Main.Kill(audio.gameObject);
                }
            }
        }

        /// <summary>
        /// 播放世界音效
        /// </summary>
        /// <param name="attachTarget">附加目标</param>
        /// <param name="clip">音乐剪辑</param>
        /// <param name="isLoop">是否循环</param>
        /// <param name="speed">播放速度</param>
        public void PlayWorldSound(GameObject attachTarget, AudioClip clip, bool isLoop = false, float speed = 1)
        {
            if (WorldAudios.ContainsKey(attachTarget))
            {
                AudioSource audio = WorldAudios[attachTarget];
                if (audio.isPlaying)
                {
                    audio.Stop();
                }
                audio.clip = clip;
                audio.loop = isLoop;
                audio.pitch = speed;
                audio.Play();
            }
            else
            {
                AudioSource audio = AudioToolkit.AttachAudioSource(attachTarget, WorldPriority, WorldVolume, 1, 1, Mute);
                WorldAudios.Add(attachTarget, audio);
                audio.clip = clip;
                audio.loop = isLoop;
                audio.pitch = speed;
                audio.Play();
            }
        }
        /// <summary>
        /// 暂停播放指定的世界音效
        /// </summary>
        /// <param name="attachTarget">附加目标</param>
        /// <param name="isGradual">是否渐进式</param>
        public void PauseWorldSound(GameObject attachTarget, bool isGradual = true)
        {
            if (WorldAudios.ContainsKey(attachTarget))
            {
                AudioSource audio = WorldAudios[attachTarget];
                if (isGradual)
                {
                    audio.DOFade(0, 2).OnComplete(() =>
                    {
                        audio.Pause();
                        audio.volume = WorldVolume;
                    });
                }
                else
                {
                    audio.Pause();
                }
            }
        }
        /// <summary>
        /// 恢复播放指定的世界音效
        /// </summary>
        /// <param name="attachTarget">附加目标</param>
        /// <param name="isGradual">是否渐进式</param>
        public void UnPauseWorldSound(GameObject attachTarget, bool isGradual = true)
        {
            if (WorldAudios.ContainsKey(attachTarget))
            {
                AudioSource audio = WorldAudios[attachTarget];
                if (isGradual)
                {
                    audio.volume = 0;
                    audio.UnPause();
                    audio.DOFade(WorldVolume, 2);
                }
                else
                {
                    audio.UnPause();
                }
            }
        }
        /// <summary>
        /// 停止播放指定的世界音效
        /// </summary>
        /// <param name="attachTarget">附加目标</param>
        public void StopWorldSound(GameObject attachTarget)
        {
            if (WorldAudios.ContainsKey(attachTarget))
            {
                if (WorldAudios[attachTarget].isPlaying)
                {
                    WorldAudios[attachTarget].Stop();
                }
            }
        }
        /// <summary>
        /// 停止播放所有世界音效
        /// </summary>
        public void StopAllWorldSound()
        {
            foreach (var audio in WorldAudios)
            {
                if (audio.Value.isPlaying)
                {
                    audio.Value.Stop();
                }
            }
        }
        /// <summary>
        /// 销毁所有闲置中的世界音效的音源
        /// </summary>
        public void ClearIdleWorldAudioSource()
        {
            HashSet<GameObject> removeSet = new HashSet<GameObject>();
            foreach (var audio in WorldAudios)
            {
                if (!audio.Value.isPlaying)
                {
                    removeSet.Add(audio.Key);
                    Main.Kill(audio.Value);
                }
            }
            foreach (var item in removeSet)
            {
                WorldAudios.Remove(item);
            }
        }

        //提取闲置中的多通道音源
        private AudioSource ExtractIdleMultipleAudioSource()
        {
            for (int i = 0; i < MultipleAudios.Count; i++)
            {
                if (!MultipleAudios[i].isPlaying)
                {
                    return MultipleAudios[i];
                }
            }

            AudioSource audio = AudioToolkit.CreateAudioSource("MultipleAudio", MultiplePriority, MultipleVolume, 1, 0, Mute, Module.transform);
            MultipleAudios.Add(audio);
            return audio;
        }
    }
}