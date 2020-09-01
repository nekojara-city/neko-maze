using System;
using Cysharp.Threading.Tasks;
using GameOver.Ranking;
using UnityEngine;

namespace GameOver
{
    /// <summary>
    /// ランキングウィンドウ
    /// </summary>
    internal class RankingWindow : MonoBehaviour
    {
        [SerializeField] private RankingScrollView _scrollViewScore;
        [SerializeField] private RankingScrollView _scrollViewLevel;

        [SerializeField] private Animator _animator;

        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _soundOpen;
        [SerializeField] private AudioClip _soundClose;

        public bool IsOpening { get; private set; }

        private bool _isLocking;
        private bool _isChanged;

        /// <summary>
        /// ウィンドウを開く
        /// </summary>
        public void OpenWindow()
        {
            if (IsOpening || _isLocking) return;

            gameObject.SetActive(true);

            if (_isChanged)
            {
                _scrollViewScore.Initialize(true);
                _scrollViewLevel.Initialize(true);
                _isChanged = false;
            }

            _audioSource.PlayOneShot(_soundOpen);

            LockWindow().Forget();
            IsOpening = true;
        }

        /// <summary>
        /// ウィンドウを閉じる
        /// </summary>
        public void CloseWindow()
        {
            if (!IsOpening || _isLocking) return;

            CloseWindowAsync().Forget();

            _audioSource.PlayOneShot(_soundClose);

            LockWindow().Forget();
            IsOpening = false;
        }

        public void RefreshContent()
        {
            _isChanged = true;
        }

        private async UniTaskVoid CloseWindowAsync()
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Open"))
                _animator.Play("Close");

            await UniTask.WaitWhile(() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1);

            gameObject.SetActive(false);
        }

        private async UniTaskVoid LockWindow()
        {
            _isLocking = true;
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
            _isLocking = false;
        }
    }
}