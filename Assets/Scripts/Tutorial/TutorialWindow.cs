using System;
using Common.Audio;
using Common.Save;
using Common.UI.Scroll;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Zenject;

namespace Tutorial
{
    internal class TutorialWindow : MonoBehaviour
    {
        [SerializeField] private ScrollElementManager _scrollElementManager;

        [SerializeField] private Animator _animator;

        [SerializeField] private AudioClip _soundOpen;
        [SerializeField] private AudioClip _soundClose;
        [SerializeField] private AudioClip _soundScroll;

        [SerializeField] private GameObject _backPlane;

        [Inject] private SaveData _saveData;
        [Inject] private SoundManager _soundManager;

        public bool IsOpening { get; private set; }

        private bool _isLocking;

        private Subject<Unit> _onOpened = new Subject<Unit>();
        private Subject<Unit> _onClose = new Subject<Unit>();

        public IObservable<Unit> OnOpened => _onOpened;
        public IObservable<Unit> OnClose => _onClose;

        private void Awake()
        {
            this.UpdateAsObservable()
                .Where(_ => IsOpening)
                .Subscribe(_ => OnUpdate());

            _scrollElementManager
                .OnCurrentChanged
                .Skip(1)
                .Where(_ => IsOpening)
                .Subscribe(_ => _soundManager.Play(_soundScroll));
        }

        private void OnUpdate()
        {
            if (Input.GetButtonDown("Horizontal"))
            {
                var axis = Input.GetAxis("Horizontal");

                if (axis > 0)
                {
                    _scrollElementManager.MoveNext();
                }
                else
                {
                    _scrollElementManager.MovePrev();
                }
            }

            if (Input.GetButtonDown("Submit"))
            {
                if (_scrollElementManager.CurrentIndex < _scrollElementManager.Count - 1)
                {
                    _scrollElementManager.MoveNext();
                }
                else
                {
                    CloseWindow();
                }
            }
        }

        public void OpenWindow()
        {
            if (IsOpening || _isLocking) return;

            _scrollElementManager.CurrentIndex = 0;
            _scrollElementManager.Initialize();

            gameObject.SetActive(true);
            OpenWindowAsync().Forget();

            _soundManager.Play(_soundOpen);

            IsOpening = true;
        }

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

            await UniTask.WaitWhile(() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1);
            _onOpened.OnNext(Unit.Default);

            if (_backPlane != null)
                _backPlane.SetActive(true);

            _saveData.Data.isTutorialShown = true;
            _saveData.Save();

            _isLocking = false;
        }

        private async UniTaskVoid CloseWindowAsync()
        {
            _isLocking = true;

            if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Open"))
                _animator.Play("Close");

            _onClose.OnNext(Unit.Default);

            if (_backPlane != null)
                _backPlane.SetActive(false);

            await UniTask.WaitWhile(() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1);

            if (gameObject != null)
                gameObject.SetActive(false);

            _isLocking = false;
        }
    }
}