using System;
using System.Collections.Generic;
using Game.Avatar;
using UnityEngine;
using UniRx;
using Zenject;

namespace Game.Gimmick
{
    /// <summary>
    /// ゴールギミックの全体管理
    /// </summary>
    internal class GoalManager : MonoBehaviour
    {
        [SerializeField] private AvatarSpawner _avatarSpawner;
        [SerializeField] private CircularTimer _timer;

        [Inject] private GameStateManager _gameStateManager;

        private readonly List<GoalZone> _goals = new List<GoalZone>();
        private readonly List<GoalZone> _enteringGoals = new List<GoalZone>();

        private readonly Subject<Unit> _onCompleteGoal = new Subject<Unit>();

        public IObservable<Unit> OnCompleteGoal => _onCompleteGoal;

        private void Awake()
        {
            _onCompleteGoal.Subscribe(_ =>
            {
                _gameStateManager.SetGoalEffect();
                _timer.PauseTimer();
            });

            _gameStateManager
                .OnGoalEffectEnd
                .Subscribe(_ =>
                {
                    _goals.Clear();
                    _enteringGoals.Clear();
                    _avatarSpawner.SpawnAvatar();
                });
        }

        public void AddGoal(GoalZone obj)
        {
            _goals.Add(obj);
        }

        public void RemoveGoal(GoalZone obj)
        {
            _goals.Remove(obj);
        }

        public void EnterGoal(GoalZone obj)
        {
            _enteringGoals.Add(obj);

            if (_enteringGoals.Count == _goals.Count)
                _onCompleteGoal.OnNext(Unit.Default);
        }

        public void ExitGoal(GoalZone obj)
        {
            _enteringGoals.Remove(obj);
        }
    }
}