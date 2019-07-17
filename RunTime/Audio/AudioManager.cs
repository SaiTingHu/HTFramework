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
        /// <summary>
        /// 背景音乐优先级
        /// </summary>
        public int BackgroundPriority = 0;
        /// <summary>
        /// 单通道音效优先级
        /// </summary>
        public int SinglePriority = 10;
        /// <summary>
        /// 多通道音效优先级
        /// </summary>
        public int MultiplePriority = 20;
        /// <summary>
        /// 世界音效优先级
        /// </summary>
        public int WorldPriority = 30;
        /// <summary>
        /// 背景音乐音量
        /// </summary>
        public float BackgroundVolume = 0.6f;
        /// <summary>
        /// 单通道音效音量
        /// </summary>
        public float SingleVolume = 1;
        /// <summary>
        /// 多通道音效音量
        /// </summary>
        public float MultipleVolume = 1;
        /// <summary>
        /// 世界音效音量
        /// </summary>
        public float WorldVolume = 1;

        /// <summary>
        /// 单通道音效播放结束事件
        /// </summary>
        public event HTFAction SingleSoundEndOfPlayEvent;

        private int _backgroundPriorityCache;
        private int _singlePriorityCache;
        private int _multiplePriorityCache;
        private int _worldPriorityCache;
        private float _backgroundVolumeCache;
        private float _singleVolumeCache;
        private float _multipleVolumeCache;
        private float _worldVolumeCache;
        private AudioSource _backgroundAudio;
        private AudioSource _singleAudio;
        private List<AudioSource> _multipleAudio = new List<AudioSource>();
        private Dictionary<GameObject, AudioSource> _worldAudio = new Dictionary<GameObject, AudioSource>();
        private bool _isMute = false;
        private bool _singleSoundPlayDetector = false;

        public override void Initialization()
        {
            base.Initialization();

            _backgroundAudio = CreateAudioSource("BackgroundAudio", BackgroundPriority, BackgroundVolume);
            _singleAudio = CreateAudioSource("SingleAudio", SinglePriority, SingleVolume);
        }

        public override void Termination()
        {
            base.Termination();

            StopBackgroundMusic();
            StopSingleSound();
            StopAllMultipleSound();
            StopAllWorldSound();
        }

        public override void Refresh()
        {
            base.Refresh();

            if (_singleSoundPlayDetector)
            {
                if (!_singleAudio.isPlaying)
                {
                    _singleSoundPlayDetector = false;
                    SingleSoundEndOfPlayEvent?.Invoke();
                }
            }

            if (_backgroundPriorityCache != BackgroundPriority)
            {
                _backgroundPriorityCache = BackgroundPriority;
                _backgroundAudio.priority = _backgroundPriorityCache;
            }
            if (_singlePriorityCache != SinglePriority)
            {
                _singlePriorityCache = SinglePriority;
                _singleAudio.priority = _singlePriorityCache;
            }
            if (_multiplePriorityCache != MultiplePriority)
            {
                _multiplePriorityCache = MultiplePriority;
                for (int i = 0; i < _multipleAudio.Count; i++)
                {
                    _multipleAudio[i].priority = _multiplePriorityCache;
                }
            }
            if (_worldPriorityCache != WorldPriority)
            {
                _worldPriorityCache = WorldPriority;
                foreach (KeyValuePair<GameObject, AudioSource> audio in _worldAudio)
                {
                    audio.Value.priority = _worldPriorityCache;
                }
            }

            if (!Mathf.Approximately(_backgroundVolumeCache, BackgroundVolume))
            {
                _backgroundVolumeCache = BackgroundVolume;
                _backgroundAudio.volume = _backgroundVolumeCache;
            }
            if (!Mathf.Approximately(_singleVolumeCache, SingleVolume))
            {
                _singleVolumeCache = SingleVolume;
                _singleAudio.volume = _singleVolumeCache;
            }
            if (!Mathf.Approximately(_multipleVolumeCache, MultipleVolume))
            {
                _multipleVolumeCache = MultipleVolume;
                for (int i = 0; i < _multipleAudio.Count; i++)
                {
                    _multipleAudio[i].volume = _multipleVolumeCache;
                }
            }
            if (!Mathf.Approximately(_worldVolumeCache, WorldVolume))
            {
                _worldVolumeCache = WorldVolume;
                foreach (KeyValuePair<GameObject, AudioSource> audio in _worldAudio)
                {
                    audio.Value.volume = _worldVolumeCache;
                }
            }
        }

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
                _isMute = value;
                _backgroundAudio.mute = value;
                _singleAudio.mute = value;
                for (int i = 0; i < _multipleAudio.Count; i++)
                {
                    _multipleAudio[i].mute = value;
                }
                foreach (KeyValuePair<GameObject, AudioSource> audio in _worldAudio)
                {
                    audio.Value.mute = value;
                }
            }
        }

        /// <summary>
        /// 播放背景音乐
        /// </summary>
        public void PlayBackgroundMusic(AudioClip clip, bool isLoop = true, float speed = 1)
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
        /// 暂停播放背景音乐
        /// </summary>
        public void PauseBackgroundMusic(bool isGradual = true)
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
        public void UnPauseBackgroundMusic(bool isGradual = true)
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
        public void StopBackgroundMusic()
        {
            if (_backgroundAudio.isPlaying)
            {
                _backgroundAudio.Stop();
            }
        }

        /// <summary>
        /// 播放单通道音效
        /// </summary>
        public void PlaySingleSound(AudioClip clip, bool isLoop = false, float speed = 1)
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
            _singleSoundPlayDetector = true;
        }
        /// <summary>
        /// 暂停播放单通道音效
        /// </summary>
        public void PauseSingleSound(bool isGradual = true)
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
        /// 恢复播放单通道音效
        /// </summary>
        public void UnPauseSingleSound(bool isGradual = true)
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
        /// 停止播放单通道音效
        /// </summary>
        public void StopSingleSound()
        {
            if (_singleAudio.isPlaying)
            {
                _singleAudio.Stop();
            }
        }

        /// <summary>
        /// 播放多通道音效
        /// </summary>
        public void PlayMultipleSound(AudioClip clip, bool isLoop = false, float speed = 1)
        {
            AudioSource audio = ExtractIdleMultipleAudioSource();
            audio.clip = clip;
            audio.loop = isLoop;
            audio.pitch = speed;
            audio.spatialBlend = 0;
            audio.Play();
        }
        /// <summary>
        /// 停止播放指定的多通道音效
        /// </summary>
        public void StopMultipleSound(AudioClip clip)
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
        /// 停止播放所有多通道音效
        /// </summary>
        public void StopAllMultipleSound()
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
        /// 销毁所有闲置中的多通道音效的音源
        /// </summary>
        public void ClearIdleMultipleAudioSource()
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
        /// 播放世界音效
        /// </summary>
        public void PlayWorldSound(GameObject attachTarget, AudioClip clip, bool isLoop = false, float speed = 1)
        {
            if (_worldAudio.ContainsKey(attachTarget))
            {
                AudioSource audio = _worldAudio[attachTarget];
                if (audio.isPlaying)
                {
                    audio.Stop();
                }
                audio.clip = clip;
                audio.loop = isLoop;
                audio.pitch = speed;
                audio.spatialBlend = 1;
                audio.Play();
            }
            else
            {
                AudioSource audio = AttachAudioSource(attachTarget, WorldPriority, WorldVolume);
                _worldAudio.Add(attachTarget, audio);
                audio.clip = clip;
                audio.loop = isLoop;
                audio.pitch = speed;
                audio.spatialBlend = 1;
                audio.Play();
            }
        }
        /// <summary>
        /// 暂停播放指定的世界音效
        /// </summary>
        public void PauseWorldSound(GameObject attachTarget, bool isGradual = true)
        {
            if (_worldAudio.ContainsKey(attachTarget))
            {
                AudioSource audio = _worldAudio[attachTarget];
                if (audio.isPlaying)
                {
                    if (isGradual)
                    {
                        audio.DOFade(0, 2);
                    }
                    else
                    {
                        audio.volume = 0;
                    }
                }
            }
        }
        /// <summary>
        /// 恢复播放指定的世界音效
        /// </summary>
        public void UnPauseWorldSound(GameObject attachTarget, bool isGradual = true)
        {
            if (_worldAudio.ContainsKey(attachTarget))
            {
                AudioSource audio = _worldAudio[attachTarget];
                if (audio.isPlaying)
                {
                    if (isGradual)
                    {
                        audio.DOFade(WorldVolume, 2);
                    }
                    else
                    {
                        audio.volume = WorldVolume;
                    }
                }
            }
        }
        /// <summary>
        /// 停止播放所有的世界音效
        /// </summary>
        public void StopAllWorldSound()
        {
            foreach (KeyValuePair<GameObject, AudioSource> audio in _worldAudio)
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
            foreach (KeyValuePair<GameObject, AudioSource> audio in _worldAudio)
            {
                if (!audio.Value.isPlaying)
                {
                    removeSet.Add(audio.Key);
                    Main.Kill(audio.Value);
                }
            }
            foreach (GameObject item in removeSet)
            {
                _worldAudio.Remove(item);
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
            audio.mute = _isMute;
            return audio;
        }
        /// <summary>
        /// 附加一个音源
        /// </summary>
        private AudioSource AttachAudioSource(GameObject target, int priority, float volume)
        {
            AudioSource audio = target.AddComponent<AudioSource>();
            audio.playOnAwake = false;
            audio.priority = priority;
            audio.volume = volume;
            audio.mute = _isMute;
            return audio;
        }
        /// <summary>
        /// 提取闲置中的多通道音源
        /// </summary>
        private AudioSource ExtractIdleMultipleAudioSource()
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
