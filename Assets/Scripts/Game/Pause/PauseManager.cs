using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace Game.Pause
{
    /// <summary>
    /// ポーズ管理
    /// </summary>
    internal class PauseManager : MonoBehaviour
    {
        [SerializeField] private PauseWindow _pauseWindow;

        [Inject] private GameStateManager _gameStateManager;

        private Pauser[] _targets;

        private void Awake()
        {
            _targets = FindObjectsOfType<Pauser>();

            this.UpdateAsObservable()
                .Where(_ => _gameStateManager.State == GameStateManager.GameState.Playing)
                .Subscribe(_ => OnCheckPauseKey());

            this.UpdateAsObservable()
                .Where(_ => _gameStateManager.State == GameStateManager.GameState.Pausing)
                .Subscribe(_ => OnCheckPauseCloseKey());

            _pauseWindow
                .OnOpen
                .Subscribe(_ => OnPause())
                .AddTo(this);

            _pauseWindow
                .OnClosed
                .Subscribe(_ => OnResume())
                .AddTo(this);
        }

        private void OnCheckPauseKey()
        {
            if (Input.GetButtonDown("Cancel"))
            {
                _pauseWindow.OpenWindow();
            }
        }

        private void OnCheckPauseCloseKey()
        {
            if (Input.GetButtonDown("Cancel"))
            {
                _pauseWindow.CloseWindow();
            }
        }

        private void OnPause()
        {
            if (_targets != null)
            {
                for (var i = 0; i < _targets.Length; ++i)
                    _targets[i].Pause();
            }

            _gameStateManager.Pause();
        }

        private void OnResume()
        {
            if (_targets != null)
            {
                for (var i = 0; i < _targets.Length; ++i)
                    _targets[i].Resume();
            }

            _gameStateManager.Resume();
        }
    }
}