using Cinemachine;
using UnityEngine;

namespace Game.Gimmick
{
    /// <summary>
    /// ワナに嵌った演出用のカメラ
    /// </summary>
    internal class TrapEffectCamera : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera _gameCamera;
        [SerializeField] private CinemachineVirtualCamera _effectCamera;

        /// <summary>
        /// ワナに嵌ったアバターのカメラ演出に切り替える
        /// </summary>
        /// <param name="target"></param>
        public void ChangeCameraWork(Transform target)
        {
            // ワナに嵌ったアバターをフォーカス対象に設定
            _effectCamera.Follow = target;
            _effectCamera.LookAt = target;
            
            // ゲームカメラからワナ演出用カメラに切り替え
            _effectCamera.Priority = _gameCamera.Priority + 1;
        }
    }
}