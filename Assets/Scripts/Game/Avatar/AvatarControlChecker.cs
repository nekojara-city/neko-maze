using System;
using ECM.Controllers;
using UnityEngine;
using Zenject;
using UniRx;

namespace Game.Avatar
{
    /// <summary>
    /// アバター操作チェック
    /// </summary>
    internal class AvatarControlChecker : MonoBehaviour
    {
        [SerializeField] private BaseCharacterController _characterController;
        [SerializeField] private Rigidbody _rigidbody;

        [Inject] private GameStateManager _gameStateManager;

        private void Awake()
        {
            UpdateState(_gameStateManager.State);
            
            _gameStateManager
                .OnStateChanged
                .Subscribe(UpdateState)
                .AddTo(this);
        }

        private void UpdateState(GameStateManager.GameState state)
        {
            _characterController.enabled = state == GameStateManager.GameState.Playing;

            if (!_characterController.enabled)
            {
                _rigidbody.velocity = Vector3.zero;
            }
        }
    }
}