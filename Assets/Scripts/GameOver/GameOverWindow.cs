using System;
using System.Collections.Generic;
using Common.Scene;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UniRx;
using UnityEngine.EventSystems;

namespace GameOver
{
    /// <summary>
    /// ゲームオーバーウィンドウ
    /// </summary>
    internal class GameOverWindow : MonoBehaviour
    {
        [SerializeField] private Text _textScore;
        [SerializeField] private Text _textLevel;

        [SerializeField] private InputField _inputPlayerName;

        [SerializeField] private Animator _animator;

        [SerializeField] private ButtonChecker _buttonChecker;
        [SerializeField] private RankingWindow _rankingWindow;

        [SerializeField] private SceneTransition _transitionGame;
        [SerializeField] private SceneTransition _transitionTitle;

        [InjectOptional] private GameOverParam _gameOverParam;

        private HighScoreRequest _requestHighScore;

        private int _lastScore;
        private int _lastLevel;

        private int _newScore;
        private int _newLevel;

        private static readonly int ParamStar = Animator.StringToHash("Star");
        private static readonly int ParamNewRecord = Animator.StringToHash("NewRecord");
        private static readonly int ParamSend = Animator.StringToHash("Send");

        private const string ColumnScore = "score";
        private const string ColumnLevel = "level";

        private bool _isHighScore;

        private async UniTaskVoid Awake()
        {
            // UIの初期化
            var param = _gameOverParam ?? new GameOverParam
            {
                Score = 0,
                Level = 1,
            };

            _buttonChecker.enabled = false;

            _newScore = param.Score;
            _newLevel = param.Level;

            _textScore.text = _newScore.ToString();
            _textLevel.text = "Level " + _newLevel;

            // ハイスコア取得
            _requestHighScore = new HighScoreRequest("HighScore");
            var result = await _requestHighScore.GetScoreAsync(new[] { ColumnScore, ColumnLevel });

            if (result != null)
            {
                _lastScore = result[ColumnScore];
                _lastLevel = result[ColumnLevel];
            }

            // スコア更新判定
            if (_newScore > _lastScore || _newLevel > _lastLevel)
            {
                await UniTask.WaitUntil(
                    () => _animator.GetCurrentAnimatorStateInfo(0).shortNameHash == ParamStar);

                _animator.SetBool(ParamNewRecord, true);

                await UniTask.WaitUntil(
                    () => _animator.GetCurrentAnimatorStateInfo(0).shortNameHash == ParamNewRecord);
                
                await UniTask.WaitWhile(
                    () => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1);

                // 入力フォームにフォーカス
                _inputPlayerName.Select();
                _inputPlayerName.ActivateInputField();

                _isHighScore = true;
            }
            
            // キーボード操作の設定

            #region ランキングウィンドウが開いていない場合

            _buttonChecker
                .OnReturn
                .Where(_ => !_rankingWindow.IsOpening)
                .Where(_ => _isHighScore && _inputPlayerName.interactable)
                .Take(1)
                .Subscribe(_ => SendHighScore());
            
            _buttonChecker
                .OnSubmit
                .Where(_ => !_rankingWindow.IsOpening)
                .Where(_ => !_isHighScore || !_inputPlayerName.interactable)
                .Subscribe(_ => _transitionGame.PerformTransition());

            _buttonChecker
                .OnCancel
                .Where(_ => !_rankingWindow.IsOpening)
                .Subscribe(_ => _transitionTitle.PerformTransition());
            
            _buttonChecker
                .OnRanking
                .Where(_ => !_rankingWindow.IsOpening)
                .Where(_ => !_inputPlayerName.isFocused)
                .Subscribe(_ => _rankingWindow.OpenWindow());

            #endregion

            #region ランキングウィンドウが開いている場合

            _buttonChecker
                .OnSubmit
                .Where(_ => _rankingWindow.IsOpening)
                .Subscribe(_ => _rankingWindow.CloseWindow());

            _buttonChecker
                .OnCancel
                .Where(_ => _rankingWindow.IsOpening)
                .Subscribe(_ => _rankingWindow.CloseWindow());

            _buttonChecker
                .OnRanking
                .Where(_ => _rankingWindow.IsOpening)
                .Subscribe(_ => _rankingWindow.CloseWindow());

            #endregion

            #region 常に有効

            _buttonChecker
                .OnTwitter
                .Where(_ => !_inputPlayerName.isFocused)
                .Subscribe(_ => TweetResult());

            #endregion

            _buttonChecker.enabled = true;
        }

        public void SendHighScore()
        {
            SendHighScoreAsync().Forget();
        }

        public void TweetResult()
        {
            var header = "『Neko's Maze』をプレイしました✨🐈\n";
            var footer = $"到達レベル：{_newLevel}\nスコア：{_newScore}";
            string message;

            if (_newLevel < 5)
            {
                message = "子猫ちゃんはまた遊びたがっているご様子です♪";
            }
            else if (_newLevel < 10)
            {
                message = "子猫ちゃんはとっても楽しそうでした♪";
            }
            else if (_newLevel < 15)
            {
                message = "たくさんの子猫ちゃんが満足しているご様子です♪";
            }
            else if (_newLevel < 20)
            {
                message = "たくさんの子猫ちゃんが驚かれているご様子です！";
            }
            else if (_newLevel < 25)
            {
                message = "もうそこは子猫ちゃんの楽園です！";
            }
            else
            {
                message = "ここまでくるともう神の領域！子猫ちゃんはあなたの虜です！";
            }

            naichilab.UnityRoomTweet.Tweet(
                "neko_maze",
                header + "\n\n" + message + "\n\n" + footer + "\n\n",
                "unityroom", "unity1week");
        }

        private async UniTaskVoid SendHighScoreAsync()
        {
            var playerName = _inputPlayerName.text;
            if (string.IsNullOrEmpty(playerName))
                playerName = "No Name";

            _animator.SetBool(ParamSend, true);

            var sendData = new Dictionary<string, int>
            {
                [ColumnScore] = Mathf.Max(_newScore, _lastScore),
                [ColumnLevel] = Mathf.Max(_newLevel, _lastLevel)
            };

            await _requestHighScore.SendScoreAsync(playerName, sendData);

            // ランキングボードを開く
            _rankingWindow.RefreshContent();
            _rankingWindow.OpenWindow();
        }
    }
}