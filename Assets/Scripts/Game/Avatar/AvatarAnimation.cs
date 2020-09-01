using System;
using ECM.Components;
using ECM.Controllers;
using UniRx.Triggers;
using UniRx;
using UnityEngine;

namespace Game.Avatar
{
    /// <summary>
    /// アバターのアニメーション
    /// </summary>
    [RequireComponent(typeof(Animator))]
    internal class AvatarAnimation : MonoBehaviour
    {
        [SerializeField] private CharacterMovement _characterMovement;
        [SerializeField] private BaseCharacterController _characterController;

        [SerializeField] private SkinnedMeshRenderer _faceRenderer;
        [SerializeField] private Material _damagedFaceMaterial;

        private Animator _animator;

        private static readonly int ParamAnimation = Animator.StringToHash("Animation");
        private static readonly int ParamSpeed = Animator.StringToHash("Speed");

        private bool _isDamaged;
        private Transform _avatarTransform;

        private const float TurnAngleSpeed = 360f;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _avatarTransform = _characterMovement.transform;

            this.UpdateAsObservable()
                .Where(_ => _characterMovement.isGrounded)
                .Where(_ => !_isDamaged)
                .Subscribe(_ => OnMove());

            this.UpdateAsObservable()
                .Where(_ => _characterController.isJumping)
                .Where(_ => !_isDamaged)
                .Subscribe(_ => OnJump());

            this.UpdateAsObservable()
                .Where(_ => _isDamaged)
                .Subscribe(_ => OnDamageUpdate());
        }

        /// <summary>
        /// 移動する速さ
        /// </summary>
        public float MoveSpeed
        {
            set
            {
                if (value < 0.001f)
                {
                    _animator.SetInteger(ParamAnimation, 1);
                }
                else
                {
                    _animator.SetInteger(ParamAnimation, 18);
                }

                _animator.SetFloat(ParamSpeed, value);
            }
        }

        /// <summary>
        /// ワナに嵌ったときのアニメーション再生
        /// </summary>
        public void PlayDamageMotion()
        {
            _animator.SetInteger(ParamAnimation, 3);
            _faceRenderer.material = _damagedFaceMaterial;

            _isDamaged = true;
        }
        
        private void OnMove()
        {
            MoveSpeed = _characterMovement.forwardSpeed;
        }

        private void OnJump()
        {
            _animator.SetInteger(ParamAnimation, 9);
        }

        private void OnDamageUpdate()
        {
            var diffAngle = Vector3.SignedAngle(_avatarTransform.forward, Vector3.back, Vector3.up);
            var turnDelta = TurnAngleSpeed * Time.deltaTime;

            if (diffAngle < 0)
                turnDelta = -turnDelta;
            
            if (Mathf.Abs(turnDelta) < Mathf.Abs(diffAngle))
            {
                _avatarTransform.localRotation *= Quaternion.AngleAxis(turnDelta, Vector3.up);
            }
            else
            {
                _avatarTransform.localRotation *= Quaternion.AngleAxis(diffAngle, Vector3.up);
            }
        }
    }
}