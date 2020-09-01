using System;
using Game.Avatar;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace Game.Gimmick
{
    /// <summary>
    /// ゴールゾーン
    /// </summary>
    internal class GoalZone : MonoBehaviour
    {
        [SerializeField] private GameObject _effectNormal;
        [SerializeField] private GameObject _effectEnter;
        [SerializeField] private GameObject _effectComplete;

        [Inject] private GoalManager _goalManager;

        private int _enterCount;
        private bool _isCompleted;

        private void Awake()
        {
            this.OnTriggerEnterAsObservable()
                .Where(_ => !_isCompleted)
                .Subscribe(OnEnterGoal);

            this.OnTriggerExitAsObservable()
                .Where(_ => !_isCompleted)
                .Subscribe(OnExitGoal);

            _goalManager
                .OnCompleteGoal
                .Where(_ => !_isCompleted)
                .Subscribe(_ => OnCompleteGoal())
                .AddTo(this);
        }

        private void OnEnable()
        {
            _goalManager.AddGoal(this);
        }

        private void OnDisable()
        {
            _goalManager.RemoveGoal(this);
        }

        private void OnEnterGoal(Collider collider)
        {
            if (++_enterCount == 1)
            {
                _effectNormal.SetActive(false);
                _effectEnter.SetActive(true);
                _effectComplete.SetActive(false);

                _goalManager.EnterGoal(this);
            }
        }

        private void OnExitGoal(Collider collider)
        {
            if (--_enterCount == 0)
            {
                _effectNormal.SetActive(true);
                _effectEnter.SetActive(false);
                _effectComplete.SetActive(false);

                _goalManager.ExitGoal(this);
            }
        }

        private void OnCompleteGoal()
        {
            _effectNormal.SetActive(false);
            _effectEnter.SetActive(true);
            _effectComplete.SetActive(true);

            _isCompleted = true;
        }
    }
}