using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Pause
{
    /// <summary>
    /// スクリプトのポーズ
    /// </summary>
    internal class Pauser : MonoBehaviour
    {
        // ポーズ対象のスクリプト
        private Behaviour[] _targets;

        /// <summary>
        /// ポーズ実施
        /// </summary>
        public void Pause()
        {
            _targets = Array.FindAll(
                GetComponentsInChildren<Behaviour>(),
                x => x.enabled && !(x is UIBehaviour));

            for (var i = 0; i < _targets.Length; ++i)
            {
                _targets[i].enabled = false;
            }
        }

        /// <summary>
        /// ポーズ解除
        /// </summary>
        public void Resume()
        {
            if (_targets == null) return;

            for (var i = 0; i < _targets.Length; ++i)
            {
                _targets[i].enabled = true;
            }

            _targets = null;
        }
    }
}