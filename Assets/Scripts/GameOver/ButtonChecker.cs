using System;
using UniRx;
using UnityEngine;

namespace GameOver
{
    /// <summary>
    /// 各種ボタン状態監視
    /// </summary>
    internal class ButtonChecker : MonoBehaviour
    {
        private readonly Subject<Unit> _onSubmit = new Subject<Unit>();
        private readonly Subject<Unit> _onReturn = new Subject<Unit>();
        private readonly Subject<Unit> _onCancel = new Subject<Unit>();
        private readonly Subject<Unit> _onTwitter = new Subject<Unit>();
        private readonly Subject<Unit> _onRanking = new Subject<Unit>();

        public IObservable<Unit> OnSubmit => _onSubmit;
        public IObservable<Unit> OnReturn => _onReturn;
        public IObservable<Unit> OnCancel => _onCancel;
        public IObservable<Unit> OnTwitter => _onTwitter;
        public IObservable<Unit> OnRanking => _onRanking;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                _onReturn.OnNext(Unit.Default);
            }
            
            if (Input.GetButtonDown("Submit"))
            {
                _onSubmit.OnNext(Unit.Default);
            }
            else if (Input.GetButtonDown("Cancel"))
            {
                _onCancel.OnNext(Unit.Default);
            }
            else if (Input.GetKeyDown(KeyCode.T))
            {
                _onTwitter.OnNext(Unit.Default);
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                _onRanking.OnNext(Unit.Default);
            }
        }
    }
}