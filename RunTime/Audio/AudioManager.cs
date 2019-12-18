using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 音频管理者
    /// </summary>
    [DisallowMultipleComponent]
    [InternalModule(HTFrameworkModule.Audio)]
    public sealed class AudioManager : InternalModuleBase
    {
        /// <summary>
        /// 是否静音初始值【请勿在代码中修改】
        /// </summary>
        public bool MuteDefault = false;
        /// <summary>
        /// 背景音乐优先级初始值【请勿在代码中修改】
        /// </summary>
        public int BackgroundPriorityDefault = 0;
        /// <summary>
        /// 单通道音效优先级初始值【请勿在代码中修改】
        /// </summary>
        public int SinglePriorityDefault = 10;
        /// <summary>
        /// 多通道音效优先级初始值【请勿在代码中修改】
        /// </summary>
        public int MultiplePriorityDefault = 20;
        /// <summary>
        /// 世界音效优先级初始值【请勿在代码中修改】
        /// </summary>
        public int WorldPriorityDefault = 30;
        /// <summary>
        /// 背景音乐音量初始值【请勿在代码中修改】
        /// </summary>
        public float BackgroundVolumeDefault = 0.6f;
        /// <summary>
        /// 单通道音效音量初始值【请勿在代码中修改】
        /// </summary>
        public float SingleVolumeDefault = 1;
        /// <summary>
        /// 多通道音效音量初始值【请勿在代码中修改】
        /// </summary>
        public float MultipleVolumeDefault = 1;
        /// <summary>
        /// 世界音效音量初始值【请勿在代码中修改】
        /// </summary>
        public float WorldVolumeDefault = 1;

        /// <summary>
        /// 单通道音效播放结束事件
        /// </summary>
        public event HTFAction SingleSoundEndOfPlayEvent;
        
        private AudioSource _backgroundAudio;
        private AudioSource _singleAudio;
        private List<AudioSource> _multipleAudios = new List<AudioSource>();
        private Dictionary<GameObject, AudioSource> _worldAudios = new Dictionary<GameObject, AudioSource>();
        private bool _singleSoundPlayDetector = false;
        private bool _isMute = false;
        private int _backgroundPriority = 0;
        private int _singlePriority = 10;
        private int _multiplePriority = 20;
        private int _worldPriority = 30;
        private float _backgroundVolume = 0.6f;
        private float _singleVolume = 1;
        private float _multipleVolume = 1;
        private float _worldVolume = 1;

        public override void OnInitialization()
        {
            base.OnInitialization();

            _backgroundAudio = CreateAudioSource("BackgroundAudio", BackgroundPriorityDefault, BackgroundVolumeDefault, 1, 0);
            _singleAudio = CreateAudioSource("SingleAudio", SinglePriorityDefault, SingleVolumeDefault, 1, 0);

            Mute = MuteDefault;
            BackgroundPriority = BackgroundPriorityDefault;
            SinglePriority = SinglePriorityDefault;
            MultiplePriority = MultiplePriorityDefault;
            WorldPriority = WorldPriorityDefault;
            BackgroundVolume = BackgroundVolumeDefault;
            SingleVolume = SingleVolumeDefault;
            MultipleVolume = MultipleVolumeDefault;
            WorldVolume = WorldVolumeDefault;
        }

        public override void OnTermination()
        {
            base.OnTermination();

            StopBackgroundMusic();
            StopSingleSound();
            StopAllMultipleSound();
            StopAllWorldSound();
        }

        public override void OnRefresh()
        {
            base.OnRefresh();

            if (_singleSoundPlayDetector)
            {
                if (!_singleAudio.isPlaying)
                {
                    _singleSoundPlayDetector = false;
                    SingleSoundEndOfPlayEvent?.Invoke();
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
                if (_isMute != value)
                {
                    _isMute = value;
                    _backgroundAudio.mute = _isMute;
                    _singleAudio.mute = _isMute;
                    for (int i = 0; i < _multipleAudios.Count; i++)
                    {
                        _multipleAudios[i].mute = _isMute;
                    }
                    foreach (var audio in _worldAudios)
                    {
                        audio.Value.mute = _isMute;
                    }
                }
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
                    _backgroundAudio.priority = _backgroundPriority;
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
                    _singleAudio.priority = _singlePriority;
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
                    for (int i = 0; i < _multipleAudios.Count; i++)
                    {
                        _multipleAudios[i].priority = _multiplePriority;
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
                    foreach (var audio in _worldAudios)
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
                    _backgroundAudio.volume = _backgroundVolume;
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
                    _singleAudio.volume = _singleVolume;
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
                    for (int i = 0; i < _multipleAudios.Count; i++)
                    {
                        _multipleAudios[i].volume = _multipleVolume;
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
                    foreach (var audio in _worldAudios)
                    {
                        audio.Value.volume = _worldVolume;
                    }
                }
            }
        }

        /// <summary>
        /// 播放背景音乐
        /// </summary>
        /// <param name="clip">音乐剪辑</param>
        /// <param name="isLoop">是否循环</param>
        /// <param name="speed">播放速度</param>
        public void PlayBackgroundMusic(AudioClip clip, bool isLoop = true, float speed = 1)
        {
            if (_backgroundAudio.isPlaying)
            {
                _backgroundAudio.Stop();
            }

            _backgroundAudio.clip = clip;
            _backgroundAudio.loop = isLoop;
            _backgroundAudio.pitch = speed;
            _backgroundAudio.Play();
        }
        /// <summary>
        /// 暂停播放背景音乐
        /// </summary>
        /// <param name="isGradual">是否渐进式</param>
        public void PauseBackgroundMusic(bool isGradual = true)
        {
            if (isGradual)
            {
                _backgroundAudio.DOFade(0, 2).OnComplete(() =>
                {
                    _backgroundAudio.volume = BackgroundVolume;
                    _backgroundAudio.Pause();
                });
            }
            else
            {
                _backgroundAudio.Pause();
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
                _backgroundAudio.UnPause();
                _backgroundAudio.volume = 0;
                _backgroundAudio.DOFade(BackgroundVolume, 2);
            }
            else
            {
                _backgroundAudio.UnPause();
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
        /// <param name="clip">音乐剪辑</param>
        /// <param name="isLoop">是否循环</param>
        /// <param name="speed">播放速度</param>
        public void PlaySingleSound(AudioClip clip, bool isLoop = false, float speed = 1)
        {
            if (_singleAudio.isPlaying)
            {
                _singleAudio.Stop();
            }

            _singleAudio.clip = clip;
            _singleAudio.loop = isLoop;
            _singleAudio.pitch = speed;
            _singleAudio.Play();
            _singleSoundPlayDetector = true;
        }
        /// <summary>
        /// 暂停播放单通道音效
        /// </summary>
        /// <param name="isGradual">是否渐进式</param>
        public void PauseSingleSound(bool isGradual = true)
        {
            if (isGradual)
            {
                _singleAudio.DOFade(0, 2).OnComplete(() =>
                {
                    _singleAudio.volume = SingleVolume;
                    _singleAudio.Pause();
                });
            }
            else
            {
                _singleAudio.Pause();
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
                _singleAudio.UnPause();
                _singleAudio.volume = 0;
                _singleAudio.DOFade(SingleVolume, 2);
            }
            else
            {
                _singleAudio.UnPause();
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
            for (int i = 0; i < _multipleAudios.Count; i++)
            {
                if (_multipleAudios[i].isPlaying)
                {
                    if (_multipleAudios[i].clip == clip)
                    {
                        _multipleAudios[i].Stop();
                    }
                }
            }
        }
        /// <summary>
        /// 停止播放所有多通道音效
        /// </summary>
        public void StopAllMultipleSound()
        {
            for (int i = 0; i < _multipleAudios.Count; i++)
            {
                if (_multipleAudios[i].isPlaying)
                {
                    _multipleAudios[i].Stop();
                }
            }
        }
        /// <summary>
        /// 销毁所有闲置中的多通道音效的音源
        /// </summary>
        public void ClearIdleMultipleAudioSource()
        {
            for (int i = 0; i < _multipleAudios.Count; i++)
            {
                if (!_multipleAudios[i].isPlaying)
                {
                    AudioSource audio = _multipleAudios[i];
                    _multipleAudios.RemoveAt(i);
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
            if (_worldAudios.ContainsKey(attachTarget))
            {
                AudioSource audio = _worldAudios[attachTarget];
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
                AudioSource audio = AttachAudioSource(attachTarget, WorldPriority, WorldVolume, 1, 1);
                _worldAudios.Add(attachTarget, audio);
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
            if (_worldAudios.ContainsKey(attachTarget))
            {
                AudioSource audio = _worldAudios[attachTarget];
                if (isGradual)
                {
                    audio.DOFade(0, 2).OnComplete(() =>
                    {
                        audio.volume = WorldVolume;
                        audio.Pause();
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
            if (_worldAudios.ContainsKey(attachTarget))
            {
                AudioSource audio = _worldAudios[attachTarget];
                if (isGradual)
                {
                    audio.UnPause();
                    audio.volume = 0;
                    audio.DOFade(WorldVolume, 2);
                }
                else
                {
                    audio.UnPause();
                }
            }
        }
        /// <summary>
        /// 停止播放所有的世界音效
        /// </summary>
        public void StopAllWorldSound()
        {
            foreach (var audio in _worldAudios)
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
            foreach (var audio in _worldAudios)
            {
                if (!audio.Value.isPlaying)
                {
                    removeSet.Add(audio.Key);
                    Main.Kill(audio.Value);
                }
            }
            foreach (var item in removeSet)
            {
                _worldAudios.Remove(item);
            }
        }

        //创建一个音源
        private AudioSource CreateAudioSource(string name, int priority, float volume, float speed, float spatialBlend)
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
            audio.pitch = speed;
            audio.spatialBlend = spatialBlend;
            audio.mute = _isMute;
            return audio;
        }
        //附加一个音源
        private AudioSource AttachAudioSource(GameObject target, int priority, float volume, float speed, float spatialBlend)
        {
            AudioSource audio = target.AddComponent<AudioSource>();
            audio.playOnAwake = false;
            audio.priority = priority;
            audio.volume = volume;
            audio.pitch = speed;
            audio.spatialBlend = spatialBlend;
            audio.mute = _isMute;
            return audio;
        }
        //提取闲置中的多通道音源
        private AudioSource ExtractIdleMultipleAudioSource()
        {
            for (int i = 0; i < _multipleAudios.Count; i++)
            {
                if (!_multipleAudios[i].isPlaying)
                {
                    return _multipleAudios[i];
                }
            }

            AudioSource audio = CreateAudioSource("MultipleAudio", MultiplePriority, MultipleVolume, 1, 0);
            _multipleAudios.Add(audio);
            return audio;
        }
    }
}