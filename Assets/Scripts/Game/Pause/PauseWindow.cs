using System;
using Common.Audio;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Pause
{
    /// <summary>
    /// ポーズウィンドウ
    /// </summary>
    internal class PauseWindow : MonoBehaviour
    {
        [SerializeField] private Animator _animator;

        [SerializeField] private Text _textLevel;
        
        [SerializeField] private AudioClip _soundOpen;
        [SerializeField] private AudioClip _soundClose;

        [SerializeField] private GameObject _backPlane;

        [Inject] private GameStateManager _gameStateManager;
        [Inject] private SoundManager _soundManager;


        private bool _isLocking;
        
        private readonly Subject<Unit> _onOpen = new Subject<Unit>();
        private readonly Subject<Unit> _onClosed = new Subject<Unit>();

        public bool IsOpening { get; private set; }

        public IObservable<Unit> OnOpen => _onOpen;
        public IObservable<Unit> OnClosed => _onClosed;

        /// <summary>
        /// ウィンドウを開く
        /// </summary>
        public void OpenWindow()
        {
            if (IsOpening || _isLocking) return;
            
            _textLevel.text = "Level " + _gameStateManager.Level;

            gameObject.SetActive(true);
            OpenWindowAsync().Forget();

            _soundManager.Play(_soundOpen);

            IsOpening = true;
        }

        /// <summary>
        /// ウィンドウを閉じる
        /// </summary>
        public void CloseWindow()
        {
            if (!IsOpening || _isLocking) return;

            CloseWindowAsync().Forget();

            _soundManager.Play(_soundClose);

            IsOpening = false;
        }

        private async UniTaskVoid OpenWindowAsync()
        {
            _isLocking = true;

            _onOpen.OnNext(Unit.Default);
            
            await UniTask.WaitWhile(() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1);

            if (_backPlane != null)
                _backPlane.SetActive(true);

            _isLocking = false;
        }

        private async UniTaskVoid CloseWindowAsync()
        {
            _isLocking = true;

            if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Open"))
                _animator.Play("Close");

            if (_backPlane != null)
                _backPlane.SetActive(false);

            await UniTask.WaitWhile(() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1);

            _onClosed.OnNext(Unit.Default);

            gameObject.SetActive(false);
            
            _isLocking = false;
        }
    }
}