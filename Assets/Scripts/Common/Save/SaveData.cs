using System;
using UnityEngine;

namespace Common.Save
{
    /// <summary>
    /// セーブデータ管理
    /// </summary>
    internal class SaveData
    {
        /// <summary>
        /// セーブデータ構造
        /// </summary>
        [Serializable]
        public class Content
        {
            public bool isTutorialShown;
        }

        // セーブデータのPlayerPrefsキー
        private const string PrefsKey = "SaveData";

        /// <summary>
        /// セーブデータ
        /// </summary>
        public Content Data { get; private set; }

        /// <summary>
        /// セーブデータ読み込み
        /// </summary>
        public void Load()
        {
            var json = PlayerPrefs.GetString(PrefsKey);
            if (string.IsNullOrEmpty(json))
            {
                Data = new Content();
                return;
            }

            try
            {
                Data = JsonUtility.FromJson<Content>(json);
            }
            catch (UnityException)
            {
                Data = new Content();
            }
        }

        /// <summary>
        /// セーブデータ保存
        /// </summary>
        public void Save()
        {
            var json = JsonUtility.ToJson(Data);
            
            PlayerPrefs.SetString(PrefsKey, json);
            PlayerPrefs.Save();
        }
    }
}