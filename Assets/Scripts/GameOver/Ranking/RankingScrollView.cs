using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GameOver.Ranking
{
    /// <summary>
    /// ランキングコンテンツのスクロールビュー
    /// </summary>
    internal class RankingScrollView : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private RectTransform _scrollContent;
        [SerializeField] private CanvasGroup _canvasGroup;

        [SerializeField] private string _dbColumn;

        [SerializeField] private RankingElement _prefabElement;
        private static readonly int ParamShow = Animator.StringToHash("Show");

        private const string COLUMN_NAME = "name";

        private bool _isInitialized = false;

        private void Start()
        {
            Initialize();
        }

        public void Initialize(bool isForce = false)
        {
            if (_isInitialized && !isForce) return;

            InitializeAsync().Forget();
        }

        private async UniTaskVoid InitializeAsync()
        {
            // 表示を一旦消す
            _animator.enabled = false;
            _canvasGroup.alpha = 0;

            // ランキング情報の取得
            var request = new RankingRequest("HighScore");
            var result = await request.GetRankingAsync(_dbColumn);

            // ランキング一覧の配置
            foreach (Transform elem in _scrollContent)
            {
                Destroy(elem.gameObject);
            }

            for (var i = 0; i < result.Count; ++i)
            {
                var row = result[i];

                var rank = i + 1;
                var value = int.Parse(row[_dbColumn].ToString());
                var playerName = row[COLUMN_NAME].ToString();

                var elem = Instantiate(_prefabElement, _scrollContent);

                elem.Initialize(rank, value, playerName);
            }

            // スクロール位置初期化
            await UniTask.WaitForEndOfFrame();

            var pos = _scrollContent.anchoredPosition;
            pos.y = 0;
            _scrollContent.anchoredPosition = pos;

            // コンテンツ表示
            _animator.enabled = true;
            _animator.SetBool(ParamShow, true);

            _isInitialized = true;
        }
    }
}