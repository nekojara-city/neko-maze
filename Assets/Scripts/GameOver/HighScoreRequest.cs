using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using NCMB;
using NCMB.Extensions;
using UnityEngine;

namespace GameOver
{
    /// <summary>
    /// ハイスコアのサーバーリクエスト
    /// </summary>
    internal class HighScoreRequest
    {
        private const string ObjectId = "objectId";
        private const string ColumnName = "name";

        private string _className;
        private string _objectid = null;
        private NCMBObject _ncmbRecord;

        private string ObjectID
        {
            get { return _objectid ?? (_objectid = PlayerPrefs.GetString(BoardIdPlayerPrefsKey, null)); }
            set
            {
                if (_objectid == value)
                    return;
                PlayerPrefs.SetString(BoardIdPlayerPrefsKey, _objectid = value);
            }
        }

        private string BoardIdPlayerPrefsKey => $"board_{_className}_{ObjectId}";

        public HighScoreRequest(string className)
        {
            _className = className;
        }

        public async UniTask<Dictionary<string, int>> GetScoreAsync(string[] columns)
        {
            var hiScoreCheck = new YieldableNcmbQuery<NCMBObject>(_className);
            hiScoreCheck.WhereEqualTo(ObjectId, ObjectID);
            await hiScoreCheck.FindAsync();

            if (hiScoreCheck.Count <= 0)
                return null;

            _ncmbRecord = hiScoreCheck.Result.First();

            var result = new Dictionary<string, int>();

            for (var i = 0; i < columns.Length; ++i)
            {
                var column = columns[i];
                var value = _ncmbRecord.ContainsKey(column) ? _ncmbRecord[column].ToString() : string.Empty;
                result.Add(column, int.Parse(value));
            }

            return result;
        }

        /// <summary>
        /// ハイスコア送信
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async UniTask SendScoreAsync(string name, Dictionary<string, int> data)
        {
            // ハイスコア送信
            if (_ncmbRecord == null)
            {
                _ncmbRecord = new NCMBObject(_className)
                {
                    ObjectId = ObjectID
                };
            }

            _ncmbRecord[ColumnName] = name;

            foreach (var row in data)
            {
                _ncmbRecord[row.Key] = row.Value;
            }

            NCMBException errorResult = null;

            await _ncmbRecord.YieldableSaveAsync(error => errorResult = error);

            if (errorResult != null)
            {
                _ncmbRecord.ObjectId = null;
                await _ncmbRecord.YieldableSaveAsync(error => errorResult = error);
            }

            ObjectID = _ncmbRecord.ObjectId;
        }
    }
}