using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 音频管理器
    /// </summary>
    [DisallowMultipleComponent]
    [InternalModule(HTFrameworkModule.Audio)]
    public sealed class AudioManager : InternalModuleBase
    {
        /// <summary>
        /// 是否静音初始值【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal bool MuteDefault = false;
        /// <summary>
        /// 背景音乐优先级初始值【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal int BackgroundPriorityDefault = 0;
        /// <summary>
        /// 单通道音效优先级初始值【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal int SinglePriorityDefault = 10;
        /// <summary>
        /// 多通道音效优先级初始值【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal int MultiplePriorityDefault = 20;
        /// <summary>
        /// 世界音效优先级初始值【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal int WorldPriorityDefault = 30;
        /// <summary>
        /// 背景音乐音量初始值【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal float BackgroundVolumeDefault = 0.6f;
        /// <summary>
        /// 单通道音效音量初始值【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal float SingleVolumeDefault = 1;
        /// <summary>
        /// 多通道音效音量初始值【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal float MultipleVolumeDefault = 1;
        /// <summary>
        /// 世界音效音量初始值【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal float WorldVolumeDefault = 1;

        /// <summary>
        /// 单通道音效播放结束事件
        /// </summary>
        public event HTFAction SingleSoundEndOfPlayEvent;
        /// <summary>
        /// 静音
        /// </summary>
        public bool Mute
        {
            get
            {
                return _helper.Mute;
            }
            set
            {
                _helper.Mute = value;
            }
        }
        /// <summary>
        /// 背景音乐优先级
        /// </summary>
        public int BackgroundPriority
        {
            get
            {
                return _helper.BackgroundPriority;
            }
            set
            {
                _helper.BackgroundPriority = value;
            }
        }
        /// <summary>
        /// 单通道音效优先级
        /// </summary>
        public int SinglePriority
        {
            get
            {
                return _helper.SinglePriority;
            }
            set
            {
                _helper.SinglePriority = value;
            }
        }
        /// <summary>
        /// 多通道音效优先级
        /// </summary>
        public int MultiplePriority
        {
            get
            {
                return _helper.MultiplePriority;
            }
            set
            {
                _helper.MultiplePriority = value;
            }
        }
        /// <summary>
        /// 世界音效优先级
        /// </summary>
        public int WorldPriority
        {
            get
            {
                return _helper.WorldPriority;
            }
            set
            {
                _helper.WorldPriority = value;
            }
        }
        /// <summary>
        /// 背景音乐音量
        /// </summary>
        public float BackgroundVolume
        {
            get
            {
                return _helper.BackgroundVolume;
            }
            set
            {
                _helper.BackgroundVolume = value;
            }
        }
        /// <summary>
        /// 单通道音效音量
        /// </summary>
        public float SingleVolume
        {
            get
            {
                return _helper.SingleVolume;
            }
            set
            {
                _helper.SingleVolume = value;
            }
        }
        /// <summary>
        /// 多通道音效音量
        /// </summary>
        public float MultipleVolume
        {
            get
            {
                return _helper.MultipleVolume;
            }
            set
            {
                _helper.MultipleVolume = value;
            }
        }
        /// <summary>
        /// 世界音效音量
        /// </summary>
        public float WorldVolume
        {
            get
            {
                return _helper.WorldVolume;
            }
            set
            {
                _helper.WorldVolume = value;
            }
        }

        private IAudioHelper _helper;

        internal override void OnInitialization()
        {
            base.OnInitialization();

            _helper = Helper as IAudioHelper;
            _helper.BackgroundAudio = AudioToolkit.CreateAudioSource("BackgroundAudio", BackgroundPriorityDefault, BackgroundVolumeDefault, 1, 0, MuteDefault, transform);
            _helper.SingleAudio = AudioToolkit.CreateAudioSource("SingleAudio", SinglePriorityDefault, SingleVolumeDefault, 1, 0, MuteDefault, transform);

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
        internal override void OnRefresh()
        {
            base.OnRefresh();

            if (_helper.SingleSoundPlayDetector)
            {
                if (!_helper.SingleAudio.isPlaying)
                {
                    _helper.SingleSoundPlayDetector = false;
                    SingleSoundEndOfPlayEvent?.Invoke();
                }
            }
        }
        internal override void OnTermination()
        {
            base.OnTermination();

            StopBackgroundMusic();
            StopSingleSound();
            StopAllMultipleSound();
            StopAllWorldSound();
        }
        
        /// <summary>
        /// 播放背景音乐
        /// </summary>
        /// <param name="clip">音乐剪辑</param>
        /// <param name="isLoop">是否循环</param>
        /// <param name="speed">播放速度</param>
        public void PlayBackgroundMusic(AudioClip clip, bool isLoop = true, float speed = 1)
        {
            _helper.PlayBackgroundMusic(clip, isLoop, speed);
        }
        /// <summary>
        /// 暂停播放背景音乐
        /// </summary>
        /// <param name="isGradual">是否渐进式</param>
        public void PauseBackgroundMusic(bool isGradual = true)
        {
            _helper.PauseBackgroundMusic(isGradual);
        }
        /// <summary>
        /// 恢复播放背景音乐
        /// </summary>
        /// <param name="isGradual">是否渐进式</param>
        public void UnPauseBackgroundMusic(bool isGradual = true)
        {
            _helper.UnPauseBackgroundMusic(isGradual);
        }
        /// <summary>
        /// 停止播放背景音乐
        /// </summary>
        public void StopBackgroundMusic()
        {
            _helper.StopBackgroundMusic();
        }

        /// <summary>
        /// 播放单通道音效
        /// </summary>
        /// <param name="clip">音乐剪辑</param>
        /// <param name="isLoop">是否循环</param>
        /// <param name="speed">播放速度</param>
        public void PlaySingleSound(AudioClip clip, bool isLoop = false, float speed = 1)
        {
            _helper.PlaySingleSound(clip, isLoop, speed);
        }
        /// <summary>
        /// 暂停播放单通道音效
        /// </summary>
        /// <param name="isGradual">是否渐进式</param>
        public void PauseSingleSound(bool isGradual = true)
        {
            _helper.PauseSingleSound(isGradual);
        }
        /// <summary>
        /// 恢复播放单通道音效
        /// </summary>
        /// <param name="isGradual">是否渐进式</param>
        public void UnPauseSingleSound(bool isGradual = true)
        {
            _helper.UnPauseSingleSound(isGradual);
        }
        /// <summary>
        /// 停止播放单通道音效
        /// </summary>
        public void StopSingleSound()
        {
            _helper.StopSingleSound();
        }

        /// <summary>
        /// 播放多通道音效
        /// </summary>
        /// <param name="clip">音乐剪辑</param>
        /// <param name="isLoop">是否循环</param>
        /// <param name="speed">播放速度</param>
        public void PlayMultipleSound(AudioClip clip, bool isLoop = false, float speed = 1)
        {
            _helper.PlayMultipleSound(clip, isLoop, speed);
        }
        /// <summary>
        /// 停止播放指定的多通道音效
        /// </summary>
        /// <param name="clip">音乐剪辑</param>
        public void StopMultipleSound(AudioClip clip)
        {
            _helper.StopMultipleSound(clip);
        }
        /// <summary>
        /// 停止播放所有多通道音效
        /// </summary>
        public void StopAllMultipleSound()
        {
            _helper.StopAllMultipleSound();
        }
        /// <summary>
        /// 销毁所有闲置中的多通道音效的音源
        /// </summary>
        public void ClearIdleMultipleAudioSource()
        {
            _helper.ClearIdleMultipleAudioSource();
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
            _helper.PlayWorldSound(attachTarget, clip, isLoop, speed);
        }
        /// <summary>
        /// 暂停播放指定的世界音效
        /// </summary>
        /// <param name="attachTarget">附加目标</param>
        /// <param name="isGradual">是否渐进式</param>
        public void PauseWorldSound(GameObject attachTarget, bool isGradual = true)
        {
            _helper.PauseWorldSound(attachTarget, isGradual);
        }
        /// <summary>
        /// 恢复播放指定的世界音效
        /// </summary>
        /// <param name="attachTarget">附加目标</param>
        /// <param name="isGradual">是否渐进式</param>
        public void UnPauseWorldSound(GameObject attachTarget, bool isGradual = true)
        {
            _helper.UnPauseWorldSound(attachTarget, isGradual);
        }
        /// <summary>
        /// 停止播放指定的世界音效
        /// </summary>
        /// <param name="attachTarget">附加目标</param>
        public void StopWorldSound(GameObject attachTarget)
        {
            _helper.StopWorldSound(attachTarget);
        }
        /// <summary>
        /// 停止播放所有世界音效
        /// </summary>
        public void StopAllWorldSound()
        {
            _helper.StopAllWorldSound();
        }
        /// <summary>
        /// 销毁所有闲置中的世界音效的音源
        /// </summary>
        public void ClearIdleWorldAudioSource()
        {
            _helper.ClearIdleWorldAudioSource();
        }
    }
}