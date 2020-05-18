using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 音频工具箱
    /// </summary>
    public static class AudioToolkit
    {
        /// <summary>
        /// 创建一个音源
        /// </summary>
        /// <param name="name">音源名称</param>
        /// <param name="priority">优先级</param>
        /// <param name="volume">音量</param>
        /// <param name="speed">速度</param>
        /// <param name="spatialBlend">2D/3D</param>
        /// <param name="mute">是否静音</param>
        /// <param name="parent">音源父级</param>
        /// <returns>音源</returns>
        public static AudioSource CreateAudioSource(string name, int priority, float volume, float speed, float spatialBlend, bool mute, Transform parent = null)
        {
            GameObject audioObj = new GameObject(name);
            if (parent != null)
            {
                audioObj.transform.SetParent(parent);
            }
            audioObj.transform.localPosition = Vector3.zero;
            audioObj.transform.localRotation = Quaternion.identity;
            audioObj.transform.localScale = Vector3.one;
            AudioSource audio = audioObj.AddComponent<AudioSource>();
            audio.playOnAwake = false;
            audio.priority = priority;
            audio.volume = volume;
            audio.pitch = speed;
            audio.spatialBlend = spatialBlend;
            audio.mute = mute;
            return audio;
        }
        /// <summary>
        /// 附加一个音源
        /// </summary>
        /// <param name="target">附加的目标</param>
        /// <param name="priority">优先级</param>
        /// <param name="volume">音量</param>
        /// <param name="speed">速度</param>
        /// <param name="spatialBlend">2D/3D</param>
        /// <param name="mute">是否静音</param>
        /// <returns>音源</returns>
        public static AudioSource AttachAudioSource(GameObject target, int priority, float volume, float speed, float spatialBlend, bool mute)
        {
            AudioSource audio = target.AddComponent<AudioSource>();
            audio.playOnAwake = false;
            audio.priority = priority;
            audio.volume = volume;
            audio.pitch = speed;
            audio.spatialBlend = spatialBlend;
            audio.mute = mute;
            return audio;
        }
    }
}