using System;
using ECM.Components;
using ECM.Controllers;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace Game.Avatar
{
    /// <summary>
    /// アバターの効果音管理
    /// </summary>
    internal class AvatarSound : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSourceWalk;
        [SerializeField] private AudioSource _audioSourceJump;

        [SerializeField] private BaseCharacterController _characterController;
        [SerializeField] private CharacterMovement _characterMovement;

        private AudioClip _playingSound = null;

        private void Awake()
        {
            this.UpdateAsObservable()
                .Where(_ => _characterMovement.isGrounded)
                .Subscribe(_ => OnMove());

            this.ObserveEveryValueChanged(_ => _characterController.isJumping)
                .Where(x => x)
                .Subscribe(_ => OnJump());
        }

        private void OnMove()
        {
            if (_characterMovement.forwardSpeed > 0.2f)
            {
                if (!_audioSourceWalk.isPlaying)
                    _audioSourceWalk.Play();
            }
            else
            {
                _audioSourceWalk.Stop();
            }
        }

        private void OnJump()
        {
            _audioSourceJump.Play();
        }
    }
}