using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 默认的音频管理器助手
    /// </summary>
    internal sealed class DefaultAudioHelper : IAudioHelper
    {
        private Transform _audioSourceRoot;
        private AudioSource _backgroundSource;
        private AudioSource _singleSource;
        private List<AudioSource> _multipleSources = new List<AudioSource>();
        private Dictionary<GameObject, AudioSource> _worldSources  = new Dictionary<GameObject, AudioSource>();
        private AudioSource _oneShootSource;
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
        /// 所属的内置模块
        /// </summary>
        public IModuleManager Module { get; set; }
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
                    _backgroundSource.mute = _isMute;
                    _singleSource.mute = _isMute;
                    for (int i = 0; i < _multipleSources.Count; i++)
                    {
                        _multipleSources[i].mute = _isMute;
                    }
                    foreach (var audio in _worldSources)
                    {
                        audio.Value.mute = _isMute;
                    }
                    _oneShootSource.mute = _isMute;
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
                return _backgroundSource.isPlaying;
            }
        }
        /// <summary>
        /// 单通道音效是否播放中
        /// </summary>
        public bool IsSinglePlaying
        {
            get
            {
                return _singleSource.isPlaying;
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
                    _backgroundSource.priority = _backgroundPriority;
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
                    _singleSource.priority = _singlePriority;
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
                    for (int i = 0; i < _multipleSources.Count; i++)
                    {
                        _multipleSources[i].priority = _multiplePriority;
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
                    foreach (var audio in _worldSources)
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
                    _oneShootSource.priority = _oneShootPriority;
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
                    _backgroundSource.volume = _backgroundVolume;
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
                    _singleSource.volume = _singleVolume;
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
                    for (int i = 0; i < _multipleSources.Count; i++)
                    {
                        _multipleSources[i].volume = _multipleVolume;
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
                    foreach (var audio in _worldSources)
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
                    _oneShootSource.volume = _oneShootVolume;
                }
            }
        }
        /// <summary>
        /// 当前的背景音乐剪辑
        /// </summary>
        public AudioClip BackgroundMusicClip
        {
            get
            {
                return _backgroundSource.clip;
            }
        }
        /// <summary>
        /// 当前的单通道音效剪辑
        /// </summary>
        public AudioClip SingleSoundClip
        {
            get
            {
                return _singleSource.clip;
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
        
        }
        /// <summary>
        /// 助手准备工作
        /// </summary>
        public void OnReady()
        { 
        
        }
        /// <summary>
        /// 刷新助手
        /// </summary>
        public void OnUpdate()
        {
            if (_singlePlayDetector)
            {
                if (!_singleSource.isPlaying)
                {
                    _singlePlayDetector = false;
                    SingleSoundEndOfPlayEvent?.Invoke(_singleSource.clip != null ? _singleSource.clip.name : null);
                }
            }
        }
        /// <summary>
        /// 终结助手
        /// </summary>
        public void OnTerminate()
        { 
        
        }
        /// <summary>
        /// 暂停助手
        /// </summary>
        public void OnPause()
        {

        }
        /// <summary>
        /// 恢复助手
        /// </summary>
        public void OnResume()
        {

        }

        /// <summary>
        /// 设置音源根节点
        /// </summary>
        /// <param name="transform">音源根节点</param>
        public void SetAudioSourceRoot(Transform transform)
        {
            _audioSourceRoot = transform;
            _backgroundSource = AudioToolkit.CreateAudioSource("BackgroundSource", _audioSourceRoot);
            _singleSource = AudioToolkit.CreateAudioSource("SingleSource", _audioSourceRoot);
            _oneShootSource = AudioToolkit.CreateAudioSource("OneShootSource", _audioSourceRoot);
            _backgroundSource.loop = true;
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

            _backgroundSource.DOKill();

            if (_backgroundSource.isPlaying)
            {
                _backgroundSource.Stop();
            }

            _backgroundSource.clip = clip;
            _backgroundSource.loop = isLoop;
            _backgroundSource.pitch = speed;
            _backgroundSource.volume = BackgroundVolume;
            _backgroundSource.Play();
        }
        /// <summary>
        /// 暂停播放背景音乐
        /// </summary>
        /// <param name="isGradual">是否渐进式</param>
        public void PauseBackgroundMusic(bool isGradual = true)
        {
            _backgroundSource.DOKill();

            if (isGradual)
            {
                _backgroundSource.DOFade(0, 2).OnComplete(() =>
                {
                    _backgroundSource.Pause();
                    _backgroundSource.volume = BackgroundVolume;
                });
            }
            else
            {
                _backgroundSource.Pause();
                _backgroundSource.volume = BackgroundVolume;
            }
        }
        /// <summary>
        /// 恢复播放背景音乐
        /// </summary>
        /// <param name="isGradual">是否渐进式</param>
        public void ResumeBackgroundMusic(bool isGradual = true)
        {
            _backgroundSource.DOKill();

            if (isGradual)
            {
                _backgroundSource.volume = 0;
                _backgroundSource.UnPause();
                _backgroundSource.DOFade(BackgroundVolume, 2);
            }
            else
            {
                _backgroundSource.UnPause();
                _backgroundSource.volume = BackgroundVolume;
            }
        }
        /// <summary>
        /// 停止播放背景音乐
        /// </summary>
        public void StopBackgroundMusic()
        {
            _backgroundSource.DOKill();

            if (_backgroundSource.isPlaying)
            {
                _backgroundSource.Stop();
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

            _singleSource.DOKill();

            if (_singleSource.isPlaying)
            {
                _singleSource.Stop();
            }

            _singleSource.clip = clip;
            _singleSource.loop = isLoop;
            _singleSource.pitch = speed;
            _singleSource.volume = SingleVolume;
            _singleSource.Play();
            _singlePlayDetector = true;
        }
        /// <summary>
        /// 暂停播放单通道音效
        /// </summary>
        /// <param name="isGradual">是否渐进式</param>
        public void PauseSingleSound(bool isGradual = true)
        {
            _singleSource.DOKill();

            if (isGradual)
            {
                _singleSource.DOFade(0, 2).OnComplete(() =>
                {
                    _singleSource.Pause();
                    _singleSource.volume = SingleVolume;
                });
            }
            else
            {
                _singleSource.Pause();
                _singleSource.volume = SingleVolume;
            }
        }
        /// <summary>
        /// 恢复播放单通道音效
        /// </summary>
        /// <param name="isGradual">是否渐进式</param>
        public void ResumeSingleSound(bool isGradual = true)
        {
            _singleSource.DOKill();

            if (isGradual)
            {
                _singleSource.volume = 0;
                _singleSource.UnPause();
                _singleSource.DOFade(SingleVolume, 2);
            }
            else
            {
                _singleSource.UnPause();
                _singleSource.volume = SingleVolume;
            }
        }
        /// <summary>
        /// 停止播放单通道音效
        /// </summary>
        public void StopSingleSound()
        {
            _singleSource.DOKill();

            if (_singleSource.isPlaying)
            {
                _singleSource.Stop();
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

            for (int i = 0; i < _multipleSources.Count; i++)
            {
                if (_multipleSources[i].isPlaying)
                {
                    if (_multipleSources[i].clip == clip)
                    {
                        _multipleSources[i].Stop();
                    }
                }
            }
        }
        /// <summary>
        /// 停止播放所有多通道音效
        /// </summary>
        public void StopAllMultipleSound()
        {
            for (int i = 0; i < _multipleSources.Count; i++)
            {
                if (_multipleSources[i].isPlaying)
                {
                    _multipleSources[i].Stop();
                }
            }
        }
        /// <summary>
        /// 销毁所有闲置中的多通道音效的音源
        /// </summary>
        public void ClearIdleMultipleAudioSource()
        {
            for (int i = 0; i < _multipleSources.Count; i++)
            {
                if (!_multipleSources[i].isPlaying)
                {
                    AudioSource audio = _multipleSources[i];
                    _multipleSources.RemoveAt(i);
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
        /// <param name="is3D">是否为3D模式，否则为2D模式</param>
        /// <param name="isLoop">是否循环</param>
        /// <param name="speed">播放速度</param>
        public void PlayWorldSound(GameObject attachTarget, AudioClip clip, bool is3D = true, bool isLoop = false, float speed = 1)
        {
            if (attachTarget == null || clip == null)
                return;

            if (_worldSources.ContainsKey(attachTarget))
            {
                AudioSource audio = _worldSources[attachTarget];
                audio.DOKill();
                if (audio.isPlaying)
                {
                    audio.Stop();
                }
                audio.clip = clip;
                audio.loop = isLoop;
                audio.pitch = speed;
                audio.volume = WorldVolume;
                audio.spatialBlend = is3D ? 1 : 0;
                audio.Play();
            }
            else
            {
                AudioSource audio = AudioToolkit.AttachAudioSource(attachTarget, WorldPriority, WorldVolume, 1, is3D ? 1 : 0, Mute);
                _worldSources.Add(attachTarget, audio);
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

            if (_worldSources.ContainsKey(attachTarget))
            {
                AudioSource audio = _worldSources[attachTarget];
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

            if (_worldSources.ContainsKey(attachTarget))
            {
                AudioSource audio = _worldSources[attachTarget];
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

            if (_worldSources.ContainsKey(attachTarget))
            {
                _worldSources[attachTarget].DOKill();
                if (_worldSources[attachTarget].isPlaying)
                {
                    _worldSources[attachTarget].Stop();
                }
            }
        }
        /// <summary>
        /// 停止播放所有世界音效
        /// </summary>
        public void StopAllWorldSound()
        {
            foreach (var audio in _worldSources)
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
            foreach (var audio in _worldSources)
            {
                if (!audio.Value.isPlaying)
                {
                    removeSet.Add(audio.Key);
                    Main.Kill(audio.Value);
                }
            }
            foreach (var item in removeSet)
            {
                _worldSources.Remove(item);
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

            _oneShootSource.PlayOneShot(clip, volumeScale);
        }

        /// <summary>
        /// 提取闲置中的多通道音源
        /// </summary>
        /// <returns>闲置中的多通道音源</returns>
        private AudioSource ExtractIdleMultipleAudioSource()
        {
            for (int i = 0; i < _multipleSources.Count; i++)
            {
                if (!_multipleSources[i].isPlaying)
                {
                    return _multipleSources[i];
                }
            }

            AudioSource audio = AudioToolkit.CreateAudioSource("MultipleAudio", MultiplePriority, MultipleVolume, 1, 0, Mute, _audioSourceRoot.transform);
            _multipleSources.Add(audio);
            return audio;
        }
    }
}