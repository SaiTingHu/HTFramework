﻿using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 默认的音频管理器助手
    /// </summary>
    public sealed class DefaultAudioHelper : IAudioHelper
    {
        private AudioManager _module;
        private bool _isMute = false;
        private int _backgroundPriority = 0;
        private int _singlePriority = 10;
        private int _multiplePriority = 20;
        private int _worldPriority = 30;
        private int _oneShootPriority = 40;
        private float _backgroundVolume = 0.6f;
        private float _singleVolume = 1;
        private float _multipleVolume = 1;
        private float _worldVolume = 1;
        private float _oneShootVolume = 1;
        private bool _singlePlayDetector = false;

        /// <summary>
        /// 音频管理器
        /// </summary>
        public IModuleManager Module { get; set; }
        /// <summary>
        /// 背景音乐音源
        /// </summary>
        public AudioSource BackgroundSource { get; private set; }
        /// <summary>
        /// 单通道音效音源
        /// </summary>
        public AudioSource SingleSource { get; private set; }
        /// <summary>
        /// 多通道音效音源
        /// </summary>
        public List<AudioSource> MultipleSources { get; private set; } = new List<AudioSource>();
        /// <summary>
        /// 世界音效音源
        /// </summary>
        public Dictionary<GameObject, AudioSource> WorldSources { get; private set; } = new Dictionary<GameObject, AudioSource>();
        /// <summary>
        /// OneShoot音源
        /// </summary>
        public AudioSource OneShootSource { get; private set; }
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
                    BackgroundSource.mute = _isMute;
                    SingleSource.mute = _isMute;
                    for (int i = 0; i < MultipleSources.Count; i++)
                    {
                        MultipleSources[i].mute = _isMute;
                    }
                    foreach (var audio in WorldSources)
                    {
                        audio.Value.mute = _isMute;
                    }
                    OneShootSource.mute = _isMute;
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
                return BackgroundSource.isPlaying;
            }
        }
        /// <summary>
        /// 单通道音效是否播放中
        /// </summary>
        public bool IsSinglePlaying
        {
            get
            {
                return SingleSource.isPlaying;
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
                    BackgroundSource.priority = _backgroundPriority;
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
                    SingleSource.priority = _singlePriority;
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
                    for (int i = 0; i < MultipleSources.Count; i++)
                    {
                        MultipleSources[i].priority = _multiplePriority;
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
                    foreach (var audio in WorldSources)
                    {
                        audio.Value.priority = _worldPriority;
                    }
                }
            }
        }
        /// <summary>
        /// OneShoot音效优先级
        /// </summary>
        public int OneShootPriority
        {
            get
            {
                return _oneShootPriority;
            }
            set
            {
                if (_oneShootPriority != value)
                {
                    _oneShootPriority = value;
                    OneShootSource.priority = _oneShootPriority;
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
                    BackgroundSource.volume = _backgroundVolume;
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
                    SingleSource.volume = _singleVolume;
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
                    for (int i = 0; i < MultipleSources.Count; i++)
                    {
                        MultipleSources[i].volume = _multipleVolume;
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
                    foreach (var audio in WorldSources)
                    {
                        audio.Value.volume = _worldVolume;
                    }
                }
            }
        }
        /// <summary>
        /// OneShoot音效音量
        /// </summary>
        public float OneShootVolume
        {
            get
            {
                return _oneShootVolume;
            }
            set
            {
                if (!_oneShootVolume.Approximately(value))
                {
                    _oneShootVolume = value;
                    OneShootSource.volume = _oneShootVolume;
                }
            }
        }
        /// <summary>
        /// 单通道音效播放结束事件，参数为播放结束时的音频剪辑名称
        /// </summary>
        public event HTFAction<string> SingleSoundEndOfPlayEvent;

        /// <summary>
        /// 初始化助手
        /// </summary>
        public void OnInit()
        {
            _module = Module as AudioManager;
            BackgroundSource = AudioToolkit.CreateAudioSource("BackgroundSource", _module.transform);
            SingleSource = AudioToolkit.CreateAudioSource("SingleSource", _module.transform);
            OneShootSource = AudioToolkit.CreateAudioSource("OneShootSource", _module.transform);
            BackgroundSource.loop = true;
        }
        /// <summary>
        /// 助手准备工作
        /// </summary>
        public void OnReady()
        { }
        /// <summary>
        /// 刷新助手
        /// </summary>
        public void OnUpdate()
        {
            if (_singlePlayDetector)
            {
                if (!SingleSource.isPlaying)
                {
                    _singlePlayDetector = false;
                    SingleSoundEndOfPlayEvent?.Invoke(SingleSource.clip != null ? SingleSource.clip.name : null);
                }
            }
        }
        /// <summary>
        /// 终结助手
        /// </summary>
        public void OnTerminate()
        { }
        /// <summary>
        /// 暂停助手
        /// </summary>
        public void OnPause()
        {
            Mute = true;
        }
        /// <summary>
        /// 恢复助手
        /// </summary>
        public void OnResume()
        {
            Mute = false;
        }

        /// <summary>
        /// 播放背景音乐
        /// </summary>
        /// <param name="clip">音乐剪辑</param>
        /// <param name="isLoop">是否循环</param>
        /// <param name="speed">播放速度</param>
        public void PlayBackgroundMusic(AudioClip clip, bool isLoop = true, float speed = 1)
        {
            if (clip == null)
                return;

            BackgroundSource.DOKill();

            if (BackgroundSource.isPlaying)
            {
                BackgroundSource.Stop();
            }

            BackgroundSource.clip = clip;
            BackgroundSource.loop = isLoop;
            BackgroundSource.pitch = speed;
            BackgroundSource.volume = BackgroundVolume;
            BackgroundSource.Play();
        }
        /// <summary>
        /// 暂停播放背景音乐
        /// </summary>
        /// <param name="isGradual">是否渐进式</param>
        public void PauseBackgroundMusic(bool isGradual = true)
        {
            BackgroundSource.DOKill();

            if (isGradual)
            {
                BackgroundSource.DOFade(0, 2).OnComplete(() =>
                {
                    BackgroundSource.Pause();
                    BackgroundSource.volume = BackgroundVolume;
                });
            }
            else
            {
                BackgroundSource.Pause();
                BackgroundSource.volume = BackgroundVolume;
            }
        }
        /// <summary>
        /// 恢复播放背景音乐
        /// </summary>
        /// <param name="isGradual">是否渐进式</param>
        public void ResumeBackgroundMusic(bool isGradual = true)
        {
            BackgroundSource.DOKill();

            if (isGradual)
            {
                BackgroundSource.volume = 0;
                BackgroundSource.UnPause();
                BackgroundSource.DOFade(BackgroundVolume, 2);
            }
            else
            {
                BackgroundSource.UnPause();
                BackgroundSource.volume = BackgroundVolume;
            }
        }
        /// <summary>
        /// 停止播放背景音乐
        /// </summary>
        public void StopBackgroundMusic()
        {
            BackgroundSource.DOKill();

            if (BackgroundSource.isPlaying)
            {
                BackgroundSource.Stop();
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
            if (clip == null)
                return;

            SingleSource.DOKill();

            if (SingleSource.isPlaying)
            {
                SingleSource.Stop();
            }

            SingleSource.clip = clip;
            SingleSource.loop = isLoop;
            SingleSource.pitch = speed;
            SingleSource.volume = SingleVolume;
            SingleSource.Play();
            _singlePlayDetector = true;
        }
        /// <summary>
        /// 暂停播放单通道音效
        /// </summary>
        /// <param name="isGradual">是否渐进式</param>
        public void PauseSingleSound(bool isGradual = true)
        {
            SingleSource.DOKill();

            if (isGradual)
            {
                SingleSource.DOFade(0, 2).OnComplete(() =>
                {
                    SingleSource.Pause();
                    SingleSource.volume = SingleVolume;
                });
            }
            else
            {
                SingleSource.Pause();
                SingleSource.volume = SingleVolume;
            }
        }
        /// <summary>
        /// 恢复播放单通道音效
        /// </summary>
        /// <param name="isGradual">是否渐进式</param>
        public void ResumeSingleSound(bool isGradual = true)
        {
            SingleSource.DOKill();

            if (isGradual)
            {
                SingleSource.volume = 0;
                SingleSource.UnPause();
                SingleSource.DOFade(SingleVolume, 2);
            }
            else
            {
                SingleSource.UnPause();
                SingleSource.volume = SingleVolume;
            }
        }
        /// <summary>
        /// 停止播放单通道音效
        /// </summary>
        public void StopSingleSound()
        {
            SingleSource.DOKill();

            if (SingleSource.isPlaying)
            {
                SingleSource.Stop();
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
            if (clip == null)
                return;

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
            if (clip == null)
                return;

            for (int i = 0; i < MultipleSources.Count; i++)
            {
                if (MultipleSources[i].isPlaying)
                {
                    if (MultipleSources[i].clip == clip)
                    {
                        MultipleSources[i].Stop();
                    }
                }
            }
        }
        /// <summary>
        /// 停止播放所有多通道音效
        /// </summary>
        public void StopAllMultipleSound()
        {
            for (int i = 0; i < MultipleSources.Count; i++)
            {
                if (MultipleSources[i].isPlaying)
                {
                    MultipleSources[i].Stop();
                }
            }
        }
        /// <summary>
        /// 销毁所有闲置中的多通道音效的音源
        /// </summary>
        public void ClearIdleMultipleAudioSource()
        {
            for (int i = 0; i < MultipleSources.Count; i++)
            {
                if (!MultipleSources[i].isPlaying)
                {
                    AudioSource audio = MultipleSources[i];
                    MultipleSources.RemoveAt(i);
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
            if (attachTarget == null || clip == null)
                return;

            if (WorldSources.ContainsKey(attachTarget))
            {
                AudioSource audio = WorldSources[attachTarget];
                audio.DOKill();
                if (audio.isPlaying)
                {
                    audio.Stop();
                }
                audio.clip = clip;
                audio.loop = isLoop;
                audio.pitch = speed;
                audio.volume = WorldVolume;
                audio.Play();
            }
            else
            {
                AudioSource audio = AudioToolkit.AttachAudioSource(attachTarget, WorldPriority, WorldVolume, 1, 1, Mute);
                WorldSources.Add(attachTarget, audio);
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
            if (attachTarget == null)
                return;

            if (WorldSources.ContainsKey(attachTarget))
            {
                AudioSource audio = WorldSources[attachTarget];
                audio.DOKill();
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
                    audio.volume = WorldVolume;
                }
            }
        }
        /// <summary>
        /// 恢复播放指定的世界音效
        /// </summary>
        /// <param name="attachTarget">附加目标</param>
        /// <param name="isGradual">是否渐进式</param>
        public void ResumeWorldSound(GameObject attachTarget, bool isGradual = true)
        {
            if (attachTarget == null)
                return;

            if (WorldSources.ContainsKey(attachTarget))
            {
                AudioSource audio = WorldSources[attachTarget];
                audio.DOKill();
                if (isGradual)
                {
                    audio.volume = 0;
                    audio.UnPause();
                    audio.DOFade(WorldVolume, 2);
                }
                else
                {
                    audio.UnPause();
                    audio.volume = WorldVolume;
                }
            }
        }
        /// <summary>
        /// 停止播放指定的世界音效
        /// </summary>
        /// <param name="attachTarget">附加目标</param>
        public void StopWorldSound(GameObject attachTarget)
        {
            if (attachTarget == null)
                return;

            if (WorldSources.ContainsKey(attachTarget))
            {
                WorldSources[attachTarget].DOKill();
                if (WorldSources[attachTarget].isPlaying)
                {
                    WorldSources[attachTarget].Stop();
                }
            }
        }
        /// <summary>
        /// 停止播放所有世界音效
        /// </summary>
        public void StopAllWorldSound()
        {
            foreach (var audio in WorldSources)
            {
                audio.Value.DOKill();
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
            foreach (var audio in WorldSources)
            {
                if (!audio.Value.isPlaying)
                {
                    removeSet.Add(audio.Key);
                    Main.Kill(audio.Value);
                }
            }
            foreach (var item in removeSet)
            {
                WorldSources.Remove(item);
            }
        }

        /// <summary>
        /// 播放OneShoot音效
        /// </summary>
        /// <param name="clip">音效剪辑</param>
        /// <param name="volumeScale">音量缩放比</param>
        public void PlayOneShoot(AudioClip clip, float volumeScale = 1)
        {
            if (clip == null)
                return;

            OneShootSource.PlayOneShot(clip, volumeScale);
        }

        /// <summary>
        /// 提取闲置中的多通道音源
        /// </summary>
        /// <returns>闲置中的多通道音源</returns>
        private AudioSource ExtractIdleMultipleAudioSource()
        {
            for (int i = 0; i < MultipleSources.Count; i++)
            {
                if (!MultipleSources[i].isPlaying)
                {
                    return MultipleSources[i];
                }
            }

            AudioSource audio = AudioToolkit.CreateAudioSource("MultipleAudio", MultiplePriority, MultipleVolume, 1, 0, Mute, _module.transform);
            MultipleSources.Add(audio);
            return audio;
        }
    }
}