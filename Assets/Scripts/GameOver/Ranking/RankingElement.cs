using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameOver.Ranking
{
    /// <summary>
    /// ランキングコンテンツのプレイヤー要素
    /// </summary>
    internal class RankingElement : MonoBehaviour
    {
        [SerializeField] private Text _textRank;
        [SerializeField] private Text _textValue;
        [SerializeField] private Text _textPlayerName;
        [SerializeField] private Image _imgMedal;
        [SerializeField] private Image _imgRankBack;

        [Serializable]
        private struct RankColor
        {
            public int rank;
            public Color text;
            public Color back;
        }

        [SerializeField] private RankColor[] _rankColors;
        [SerializeField] private RankColor _rankColorDefault;

        [SerializeField] private Sprite _sprMedal1st;
        [SerializeField] private Sprite _sprMedal2nd;
        [SerializeField] private Sprite _sprMedal3rd;

        public void Initialize(int rank, int value, string playerName)
        {
            _textRank.text = rank.ToString();
            _textValue.text = value.ToString();
            _textPlayerName.text = playerName;

            switch (rank)
            {
                case 1:
                    _imgMedal.enabled = true;
                    _imgMedal.sprite = _sprMedal1st;
                    break;
                
                case 2:
                    _imgMedal.enabled = true;
                    _imgMedal.sprite = _sprMedal2nd;
                    break;
                
                case 3:
                    _imgMedal.enabled = true;
                    _imgMedal.sprite = _sprMedal3rd;
                    break;

                default:
                    _imgMedal.enabled = false;
                    break;
            }

            var index = Array.FindIndex(_rankColors, x => x.rank == rank);
            var rankColor = index >= 0 ? _rankColors[index] : _rankColorDefault;

            _textRank.color = rankColor.text;
            _imgRankBack.color = rankColor.back;
        }
    }
}