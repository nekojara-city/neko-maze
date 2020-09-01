using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace Game
{
    /// <summary>
    /// ゲームイベント通知
    /// </summary>
    internal class GameEventDispatcher : MonoBehaviour
    {
        [Inject] private GameStateManager _gameStateManager;

        private Subject<Unit> _onUpdate = new Subject<Unit>();

        public IObservable<Unit> OnUpdate => _onUpdate;

        private void Awake()
        {
            this.UpdateAsObservable()
                .Where(_ => _gameStateManager.State == GameStateManager.GameState.Playing)
                .Subscribe(_ => _onUpdate.OnNext(Unit.Default));
        }
    }
}