using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 音频管理器的助手接口
    /// </summary>
    public interface IAudioHelper : IInternalModuleHelper
    {
        /// <summary>
        /// 静音
        /// </summary>
        bool Mute { get; set; }
        /// <summary>
        /// 背景音乐是否播放中
        /// </summary>
        bool IsBackgroundPlaying { get; }
        /// <summary>
        /// 单通道音效是否播放中
        /// </summary>
        bool IsSinglePlaying { get; }
        /// <summary>
        /// 背景音乐优先级
        /// </summary>
        int BackgroundPriority { get; set; }
        /// <summary>
        /// 单通道音效优先级
        /// </summary>
        int SinglePriority { get; set; }
        /// <summary>
        /// 多通道音效优先级
        /// </summary>
        int MultiplePriority { get; set; }
        /// <summary>
        /// 世界音效优先级
        /// </summary>
        int WorldPriority { get; set; }
        /// <summary>
        /// OneShoot音效优先级
        /// </summary>
        int OneShootPriority { get; set; }
        /// <summary>
        /// 背景音乐音量
        /// </summary>
        float BackgroundVolume { get; set; }
        /// <summary>
        /// 单通道音效音量
        /// </summary>
        float SingleVolume { get; set; }
        /// <summary>
        /// 多通道音效音量
        /// </summary>
        float MultipleVolume { get; set; }
        /// <summary>
        /// 世界音效音量
        /// </summary>
        float WorldVolume { get; set; }
        /// <summary>
        /// OneShoot音效音量
        /// </summary>
        float OneShootVolume { get; set; }
        /// <summary>
        /// 当前的背景音乐剪辑
        /// </summary>
        AudioClip BackgroundMusicClip { get; }
        /// <summary>
        /// 当前的单通道音效剪辑
        /// </summary>
        AudioClip SingleSoundClip { get; }
        /// <summary>
        /// 单通道音效播放结束事件，参数为播放结束时的音频剪辑名称
        /// </summary>
        event HTFAction<string> SingleSoundEndOfPlayEvent;

        /// <summary>
        /// 设置音源根节点
        /// </summary>
        /// <param name="transform">音源根节点</param>
        void SetAudioSourceRoot(Transform transform);

        /// <summary>
        /// 播放背景音乐
        /// </summary>
        /// <param name="clip">音乐剪辑</param>
        /// <param name="isLoop">是否循环</param>
        /// <param name="speed">播放速度</param>
        void PlayBackgroundMusic(AudioClip clip, bool isLoop = true, float speed = 1);
        /// <summary>
        /// 暂停播放背景音乐
        /// </summary>
        /// <param name="isGradual">是否渐进式</param>
        void PauseBackgroundMusic(bool isGradual = true);
        /// <summary>
        /// 恢复播放背景音乐
        /// </summary>
        /// <param name="isGradual">是否渐进式</param>
        void ResumeBackgroundMusic(bool isGradual = true);
        /// <summary>
        /// 停止播放背景音乐
        /// </summary>
        void StopBackgroundMusic();

        /// <summary>
        /// 播放单通道音效
        /// </summary>
        /// <param name="clip">音乐剪辑</param>
        /// <param name="isLoop">是否循环</param>
        /// <param name="speed">播放速度</param>
        void PlaySingleSound(AudioClip clip, bool isLoop = false, float speed = 1);
        /// <summary>
        /// 暂停播放单通道音效
        /// </summary>
        /// <param name="isGradual">是否渐进式</param>
        void PauseSingleSound(bool isGradual = true);
        /// <summary>
        /// 恢复播放单通道音效
        /// </summary>
        /// <param name="isGradual">是否渐进式</param>
        void ResumeSingleSound(bool isGradual = true);
        /// <summary>
        /// 停止播放单通道音效
        /// </summary>
        void StopSingleSound();

        /// <summary>
        /// 播放多通道音效
        /// </summary>
        /// <param name="clip">音乐剪辑</param>
        /// <param name="isLoop">是否循环</param>
        /// <param name="speed">播放速度</param>
        void PlayMultipleSound(AudioClip clip, bool isLoop = false, float speed = 1);
        /// <summary>
        /// 停止播放指定的多通道音效
        /// </summary>
        /// <param name="clip">音乐剪辑</param>
        void StopMultipleSound(AudioClip clip);
        /// <summary>
        /// 停止播放所有多通道音效
        /// </summary>
        void StopAllMultipleSound();
        /// <summary>
        /// 销毁所有闲置中的多通道音效的音源
        /// </summary>
        void ClearIdleMultipleAudioSource();

        /// <summary>
        /// 播放世界音效
        /// </summary>
        /// <param name="attachTarget">附加目标</param>
        /// <param name="clip">音乐剪辑</param>
        /// <param name="is3D">是否为3D模式，否则为2D模式</param>
        /// <param name="isLoop">是否循环</param>
        /// <param name="speed">播放速度</param>
        void PlayWorldSound(GameObject attachTarget, AudioClip clip, bool is3D = true, bool isLoop = false, float speed = 1);
        /// <summary>
        /// 暂停播放指定的世界音效
        /// </summary>
        /// <param name="attachTarget">附加目标</param>
        /// <param name="isGradual">是否渐进式</param>
        void PauseWorldSound(GameObject attachTarget, bool isGradual = true);
        /// <summary>
        /// 恢复播放指定的世界音效
        /// </summary>
        /// <param name="attachTarget">附加目标</param>
        /// <param name="isGradual">是否渐进式</param>
        void ResumeWorldSound(GameObject attachTarget, bool isGradual = true);
        /// <summary>
        /// 停止播放指定的世界音效
        /// </summary>
        /// <param name="attachTarget">附加目标</param>
        void StopWorldSound(GameObject attachTarget);
        /// <summary>
        /// 停止播放所有世界音效
        /// </summary>
        void StopAllWorldSound();
        /// <summary>
        /// 销毁所有闲置中的世界音效的音源
        /// </summary>
        void ClearIdleWorldAudioSource();

        /// <summary>
        /// 播放OneShoot音效
        /// </summary>
        /// <param name="clip">音效剪辑</param>
        /// <param name="volumeScale">音量缩放比</param>
        void PlayOneShoot(AudioClip clip, float volumeScale = 1);
    }
}