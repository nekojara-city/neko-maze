using System;
using Common;
using Common.Save;
using Common.Scene;
using Tutorial;
using UnityEngine;
using UniRx;
using UnityEngine.Playables;
using UnityEngine.UI;
using Zenject;

namespace Game
{
    /// <summary>
    /// ゲームの状態管理
    /// </summary>
    internal class GameStateManager : MonoBehaviour
    {
        public enum GameState
        {
            None,
            Tutorial,
            CountDown,
            Playing,
            GoalEffect,
            GameOver,
            Pausing,
        }

        private readonly ReactiveProperty<GameState> _state = new ReactiveProperty<GameState>();
        private readonly Subject<Unit> _onGoalEffectEnd = new Subject<Unit>();

        [SerializeField] private PlayableDirector _gameTimeline;

        [SerializeField] private CircularTimer _timer;
        [SerializeField] private SceneTransition _sceneTransition;

        [SerializeField] private Text _textScore;
        [SerializeField] private Text _textLevel;
        [SerializeField] private IntReactiveProperty _score = new IntReactiveProperty(0);
        [SerializeField] private IntReactiveProperty _level = new IntReactiveProperty(0);

        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _soundLevelUp;

        [SerializeField] private TutorialWindow _tutorialWindowPrefab;
        [SerializeField] private Transform _tutorialWindowRoot;
        [SerializeField] private GameObject _tutorialBack;

        [Inject] private DiContainer _container;
        [Inject] private SaveData _saveData;

        /// <summary>
        /// ゲーム状態
        /// </summary>
        public GameState State => _state.Value;

        /// <summary>
        /// 現在のレベル
        /// </summary>
        public int Level => _level.Value;

        /// <summary>
        /// ゲーム状態変更通知
        /// </summary>
        public IObservable<GameState> OnStateChanged => _state;

        /// <summary>
        /// ゴール到達演出終了通知
        /// </summary>
        public IObservable<Unit> OnGoalEffectEnd => _onGoalEffectEnd;

        private void Awake()
        {
            _score.Subscribe(x => _textScore.text = "SCORE : " + x);
            _level.Subscribe(x => _textLevel.text = "LEVEL : " + x);

            if (_saveData.Data.isTutorialShown)
            {
                // チュートリアル閲覧済みの場合は、カウントダウンから開始
                StartCountDown();
            }
            else
            {
                // チュートリアルを見ていない場合、チュートリアル表示
                _state.Value = GameState.Tutorial;

                Observable
                    .Timer(TimeSpan.FromSeconds(0.5f))
                    .Subscribe(_ => OnTutorialOpen())
                    .AddTo(this);
            }
        }
        
        /// <summary>
        /// ゲーム開始通知
        /// </summary>
        public void OnGameStart()
        {
            _state.Value = GameState.Playing;
            _timer.StartTimer();
        }

        /// <summary>
        /// ゴール演出に移行
        /// </summary>
        public void SetGoalEffect()
        {
            _state.Value = GameState.GoalEffect;

            Observable
                .Timer(TimeSpan.FromSeconds(1))
                .Subscribe(_ =>
                {
                    var remainTime = _timer.duration - _timer.CurrentTime;
                    _score.Value += _level.Value * Mathf.CeilToInt(remainTime);
                    ++_level.Value;

                    _onGoalEffectEnd.OnNext(Unit.Default);
                    _state.Value = GameState.Playing;
                })
                .AddTo(this);

            _audioSource.PlayOneShot(_soundLevelUp);
        }

        /// <summary>
        /// ワナに嵌った演出に移行
        /// </summary>
        public void SetTrapEffect()
        {
            _state.Value = GameState.GameOver;
        }

        /// <summary>
        /// ゲームオーバー演出に移行
        /// </summary>
        public void SetGameOver()
        {
            _state.Value = GameState.GameOver;

            Observable
                .Timer(TimeSpan.FromSeconds(0.5f))
                .Subscribe(_ =>
                {
                    _sceneTransition.PerformTransition(new []
                    {
                        new GameOverParam
                        {
                            Score = _score.Value,
                            Level = _level.Value,
                        }
                    });
                })
                .AddTo(this);
        }

        /// <summary>
        /// ポーズ状態に移行
        /// </summary>
        public void Pause()
        {
            _state.Value = GameState.Pausing;
        }

        /// <summary>
        /// ポーズ解除してゲーム実行状態に移行
        /// </summary>
        public void Resume()
        {
            _state.Value = GameState.Playing;
        }
        
        // チュートリアルウィンドウを開く
        private void OnTutorialOpen()
        {
            var window = _container.InstantiatePrefabForComponent<TutorialWindow>(
                _tutorialWindowPrefab,
                _tutorialWindowRoot);

            window.OnOpened.Subscribe(_ => _tutorialBack.SetActive(true));

            window.OnClose.Subscribe(_ =>
            {
                _tutorialBack.SetActive(false);
                StartCountDown();
            });

            window.OpenWindow();
        }

        // カウントダウン表示
        private void StartCountDown()
        {
            _state.Value = GameState.CountDown;
            _gameTimeline.Play();
        }
    }
}