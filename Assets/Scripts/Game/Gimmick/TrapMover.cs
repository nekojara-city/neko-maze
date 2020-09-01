using System;
using UnityEngine;

namespace Game.Gimmick
{
    /// <summary>
    /// ワナの動き
    /// </summary>
    internal class TrapMover : MonoBehaviour
    {
        [SerializeField] private float _cycle = 5;
        
        private Transform _transform;
        private float _time;
        private Vector3 _initLocalPosition;
        
        private void Awake()
        {
            _transform = transform;
        }

        private void Start()
        {
            _initLocalPosition = _transform.localPosition;
        }

        private void Update()
        {
            _time += Time.deltaTime;

            _transform.localPosition = _initLocalPosition + new Vector3(
                0, 
                0.5f * Mathf.Cos(2 * Mathf.PI * _time / _cycle) - 0.5f, 
                0);
        }
    }
}