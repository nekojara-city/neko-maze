using System;
using Game.Avatar;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game.Gimmick
{
    /// <summary>
    /// ワナ接触チェック
    /// </summary>
    internal class TrapChecker : MonoBehaviour
    {
        [SerializeField] private TrapMover _trapMover;
        [SerializeField] private TrapSound _trapSound;
        
        [Inject] private TrapEffectCamera _trapEffectCamera;
        [Inject] private GameStateManager _gameStateManager;

        private void OnTriggerEnter(Collider other)
        {
            _gameStateManager.SetTrapEffect();
            _trapEffectCamera.ChangeCameraWork(other.transform);

            _trapMover.enabled = false;

            var avatarAnim = other.GetComponentInChildren<AvatarAnimation>();
            if (avatarAnim != null)
            {
                avatarAnim.PlayDamageMotion();
            }
            
            _trapSound.PlayDamageSound();

            Observable
                .Timer(TimeSpan.FromSeconds(1.5f))
                .Subscribe(_ => _gameStateManager.SetGameOver())
                .AddTo(this);
        }
    }
}