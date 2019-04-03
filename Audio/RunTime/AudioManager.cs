using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 音频管理者
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class AudioManager : ModuleManager
    {
        public int BackgroundPriority = 0;
        public int SinglePriority = 10;
        public int MultiplePriority = 20;
        public float BackgroundVolume = 0.6f;
        public float SingleVolume = 1;
        public float MultipleVolume = 1;
                         
        private AudioSource _backgroundAudio;
        private AudioSource _singleAudio;
        private List<AudioSource> _multipleAudio = new List<AudioSource>();

        public override void Initialization()
        {
            _backgroundAudio = CreateAudioSource("BackgroundAudio", BackgroundPriority, BackgroundVolume);
            _singleAudio = CreateAudioSource("SingleAudio", SinglePriority, SingleVolume);
        }

        /// <summary>
        /// 静音
        /// </summary>
        public bool Mute
        {
            get
            {
                return _backgroundAudio.mute;
            }
            set
            {
                _backgroundAudio.mute = value;
                _singleAudio.mute = value;
                for (int i = 0; i < _multipleAudio.Count; i++)
                {
                    _multipleAudio[i].mute = value;
                }
            }
        }

        /// <summary>
        /// 播放2D背景音乐
        /// </summary>
        public void Play2DBackgroundAudio(AudioClip clip, bool isLoop = true, float speed = 1)
        {
            if (_backgroundAudio.isPlaying)
            {
                _backgroundAudio.Stop();
            }

            _backgroundAudio.clip = clip;
            _backgroundAudio.loop = isLoop;
            _backgroundAudio.pitch = speed;
            _backgroundAudio.spatialBlend = 0;
            _backgroundAudio.Play();
        }
        /// <summary>
        /// 播放3D背景音乐
        /// </summary>
        public void Play3DBackgroundAudio(AudioClip clip, Vector3 audioSourcePosition, bool isLoop = true, float speed = 1)
        {
            if (_backgroundAudio.isPlaying)
            {
                _backgroundAudio.Stop();
            }

            _backgroundAudio.transform.position = audioSourcePosition;
            _backgroundAudio.clip = clip;
            _backgroundAudio.loop = isLoop;
            _backgroundAudio.pitch = speed;
            _backgroundAudio.spatialBlend = 1;
            _backgroundAudio.Play();
        }
        /// <summary>
        /// 暂停播放背景音乐
        /// </summary>
        public void PauseBackgroundAudio(bool isGradual = true)
        {
            if (_backgroundAudio.isPlaying)
            {
                if (isGradual)
                {
                    _backgroundAudio.DOFade(0, 2);
                }
                else
                {
                    _backgroundAudio.volume = 0;
                }
            }
        }
        /// <summary>
        /// 恢复播放背景音乐
        /// </summary>
        public void UnPauseBackgroundAudio(bool isGradual = true)
        {
            if (_backgroundAudio.isPlaying)
            {
                if (isGradual)
                {
                    _backgroundAudio.DOFade(BackgroundVolume, 2);
                }
                else
                {
                    _backgroundAudio.volume = BackgroundVolume;
                }
            }
        }
        /// <summary>
        /// 停止播放背景音乐
        /// </summary>
        public void StopBackgroundAudio()
        {
            if (_backgroundAudio.isPlaying)
            {
                _backgroundAudio.Stop();
            }
        }

        /// <summary>
        /// 播放2D单一的音效
        /// </summary>
        public void Play2DSingleAudio(AudioClip clip, bool isLoop = false, float speed = 1)
        {
            if (_singleAudio.isPlaying)
            {
                _singleAudio.Stop();
            }

            _singleAudio.clip = clip;
            _singleAudio.loop = isLoop;
            _singleAudio.pitch = speed;
            _singleAudio.spatialBlend = 0;
            _singleAudio.Play();
        }
        /// <summary>
        /// 播放3D单一的音效
        /// </summary>
        public void Play3DSingleAudio(AudioClip clip, Vector3 audioSourcePosition, bool isLoop = false, float speed = 1)
        {
            if (_singleAudio.isPlaying)
            {
                _singleAudio.Stop();
            }

            _singleAudio.transform.position = audioSourcePosition;
            _singleAudio.clip = clip;
            _singleAudio.loop = isLoop;
            _singleAudio.pitch = speed;
            _singleAudio.spatialBlend = 1;
            _singleAudio.Play();
        }
        /// <summary>
        /// 暂停播放单一的音乐
        /// </summary>
        public void PauseSingleAudio(bool isGradual = true)
        {
            if (_singleAudio.isPlaying)
            {
                if (isGradual)
                {
                    _singleAudio.DOFade(0, 2);
                }
                else
                {
                    _singleAudio.volume = 0;
                }
            }
        }
        /// <summary>
        /// 恢复播放单一的音乐
        /// </summary>
        public void UnPauseSingleAudio(bool isGradual = true)
        {
            if (_singleAudio.isPlaying)
            {
                if (isGradual)
                {
                    _singleAudio.DOFade(SingleVolume, 2);
                }
                else
                {
                    _singleAudio.volume = SingleVolume;
                }
            }
        }
        /// <summary>
        /// 停止播放单一的音效
        /// </summary>
        public void StopSingleAudio()
        {
            if (_singleAudio.isPlaying)
            {
                _singleAudio.Stop();
            }
        }

        /// <summary>
        /// 播放2D可重复的音效
        /// </summary>
        public void Play2DMultipleAudio(AudioClip clip, bool isLoop = false, float speed = 1)
        {
            AudioSource audio = ExtractIdleAudioSource();
            audio.clip = clip;
            audio.loop = isLoop;
            audio.pitch = speed;
            audio.spatialBlend = 0;
            audio.Play();
        }
        /// <summary>
        /// 播放3D可重复的音效
        /// </summary>
        public void Play3DMultipleAudio(AudioClip clip, Vector3 audioSourcePosition, bool isLoop = false, float speed = 1)
        {
            AudioSource audio = ExtractIdleAudioSource();
            audio.transform.position = audioSourcePosition;
            audio.clip = clip;
            audio.loop = isLoop;
            audio.pitch = speed;
            audio.spatialBlend = 1;
            audio.Play();
        }
        /// <summary>
        /// 停止播放指定的可重复的音效
        /// </summary>
        public void StopMultipleAudio(AudioClip clip)
        {
            for (int i = 0; i < _multipleAudio.Count; i++)
            {
                if (_multipleAudio[i].isPlaying)
                {
                    if (_multipleAudio[i].clip == clip)
                    {
                        _multipleAudio[i].Stop();
                    }
                }
            }
        }
        /// <summary>
        /// 停止播放所有可重复的音效
        /// </summary>
        public void StopAllMultipleAudio()
        {
            for (int i = 0; i < _multipleAudio.Count; i++)
            {
                if (_multipleAudio[i].isPlaying)
                {
                    _multipleAudio[i].Stop();
                }
            }
        }
        /// <summary>
        /// 销毁所有闲置中的可重复音效的音源
        /// </summary>
        public void ClearIdleAudioSource()
        {
            for (int i = 0; i < _multipleAudio.Count; i++)
            {
                if (!_multipleAudio[i].isPlaying)
                {
                    AudioSource audio = _multipleAudio[i];
                    _multipleAudio.RemoveAt(i);
                    i -= 1;
                    Main.Kill(audio.gameObject);
                }
            }
        }

        /// <summary>
        /// 创建一个音源
        /// </summary>
        private AudioSource CreateAudioSource(string name, int priority, float volume)
        {
            GameObject audioObj = new GameObject(name);
            audioObj.transform.SetParent(transform);
            audioObj.transform.localPosition = Vector3.zero;
            audioObj.transform.localRotation = Quaternion.identity;
            audioObj.transform.localScale = Vector3.one;
            AudioSource audio = audioObj.AddComponent<AudioSource>();
            audio.playOnAwake = false;
            audio.priority = priority;
            audio.volume = volume;
            return audio;
        }
        /// <summary>
        /// 提取闲置中的可重复播放音源
        /// </summary>
        private AudioSource ExtractIdleAudioSource()
        {
            for (int i = 0; i < _multipleAudio.Count; i++)
            {
                if (!_multipleAudio[i].isPlaying)
                {
                    return _multipleAudio[i];
                }
            }

            AudioSource audio = CreateAudioSource("MultipleAudio", MultiplePriority, MultipleVolume);
            _multipleAudio.Add(audio);
            return audio;
        }
    }
}
