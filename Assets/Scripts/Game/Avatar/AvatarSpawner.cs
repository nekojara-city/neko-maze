using System.Collections.Generic;
using Cinemachine;
using Game.Stage;
using UnityEngine;
using Zenject;

namespace Game.Avatar
{
    /// <summary>
    /// アバター生成
    /// </summary>
    internal class AvatarSpawner : MonoBehaviour
    {
        [SerializeField] private CircularTimer _timer;

        [SerializeField] private AnimationCurve _spawnNumCurve;
        [SerializeField] private AnimationCurve _spawnTrapNumCurve;

        [Inject] private MazeGenerator _mazeGenerator;

        private int _spawnCount = 0;

        private void Start()
        {
            SpawnAvatar(false);
        }

        /// <summary>
        /// アバター配置
        /// </summary>
        /// <param name="autoStartTimer">タイマーを作動させるかどうか</param>
        public void SpawnAvatar(bool autoStartTimer = true)
        {
            // 2回インクリメントして正しくない挙動ですが、
            // 運用中のランキングに影響出るので仕様としました＞＜
            var nextNum = (int)_spawnNumCurve.Evaluate(++_spawnCount);
            var nextTrapNum = (int)_spawnTrapNumCurve.Evaluate(++_spawnCount);

            do
            {
                // ダンジョン自動生成
                _mazeGenerator.GenerateMaze();

                // 各種オブジェクト配置
                _mazeGenerator.ClearObjects();
                _mazeGenerator.SpawnGoal(nextNum);
                _mazeGenerator.SpawnAvatar(nextNum);
                _mazeGenerator.SpawnTrap(nextTrapNum);

                // 迷路が狭すぎてアバターを配置できない場合は、生成からやり直す
                var spawnedObjects = _mazeGenerator.GetSpawnedObjects();
                if (spawnedObjects.Exists(x => x.GetComponent<AvatarControlChecker>() != null))
                    break;
            } while (true);

            _timer.StopTimer();
            if (autoStartTimer)
                _timer.StartTimer();
        }
    }
}