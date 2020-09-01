using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NCMB;
using NCMB.Extensions;
using UnityEngine;

namespace GameOver.Ranking
{
    /// <summary>
    /// ランキング一覧取得リクエスト
    /// </summary>
    internal class RankingRequest
    {
        private string _className;

        public RankingRequest(string className)
        {
            _className = className;
        }

        public async UniTask<List<NCMBObject>> GetRankingAsync(string column)
        {
            var so = new YieldableNcmbQuery<NCMBObject>(_className)
            {
                Limit = 30
            };

            so.OrderByDescending(column);

            await so.FindAsync();

            return so.Error != null ? null : so.Result;
        }
    }
}