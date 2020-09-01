using UnityEngine;

namespace Game.Gimmick
{
    /// <summary>
    /// ワナの効果音管理
    /// </summary>
    internal class TrapSound : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _damageSound;

        public void PlayDamageSound()
        {
            _audioSource.PlayOneShot(_damageSound);
        }
    }
}