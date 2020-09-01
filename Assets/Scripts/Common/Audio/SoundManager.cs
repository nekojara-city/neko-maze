using System;
using UnityEngine;

namespace Common.Audio
{
    /// <summary>
    /// 効果音管理
    /// </summary>
    internal class SoundManager : MonoBehaviour
    {
        private AudioSource _audioSource;
        
        private void Awake()
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        /// <summary>
        /// 効果音を再生する
        /// </summary>
        /// <param name="clip">再生するAudioClip</param>
        public void Play(AudioClip clip)
        {
            _audioSource.PlayOneShot(clip);
        }
    }
}