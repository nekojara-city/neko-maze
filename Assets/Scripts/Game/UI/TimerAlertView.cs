using UniRx;
using UnityEngine;

namespace Game.UI
{
    /// <summary>
    /// 残り時間UI表示管理
    /// </summary>
    internal class TimerAlertView : MonoBehaviour
    {
        [SerializeField] private CircularTimer _timer;

        [SerializeField] private int _warningRemainTime;
        [SerializeField] private int _alertRemainTime;

        [SerializeField] private Color _normalColor;
        [SerializeField] private Color _warningColor;
        [SerializeField] private Color _alertColor;

        [SerializeField] private Animator _animator;

        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _tickWarningSound;
        [SerializeField] private AudioClip _tickAlertSound;

        private float _fillAlpha;
        private float _backgroundAlpha;
        private float _textAlpha;

        private static readonly int ParamTickWarning = Animator.StringToHash("TickWarning");
        private static readonly int ParamTickAlert = Animator.StringToHash("TickAlert");

        private void Awake()
        {
            _fillAlpha = _timer.fillSettings.color.a;
            _backgroundAlpha = _timer.backgroundSettings.color.a;
            _textAlpha = _timer.textSettings.color.a;

            var duration = Mathf.CeilToInt(_timer.duration);

            this.ObserveEveryValueChanged(_ => Mathf.CeilToInt(duration - _timer.CurrentTime))
                .Subscribe(OnUpdateTick);
        }

        private void OnUpdateTick(int remainTime)
        {
            if (remainTime <= _alertRemainTime)
            {
                SetViewColor(_alertColor);
                
                _animator.Play(ParamTickAlert);
                _audioSource.PlayOneShot(_tickAlertSound);
            }
            else if (remainTime <= _warningRemainTime)
            {
                SetViewColor(_warningColor);

                _animator.Play(ParamTickWarning);
                _audioSource.PlayOneShot(_tickWarningSound);
            }
            else
            {
                SetViewColor(_normalColor);
            }
        }

        private void SetViewColor(Color color)
        {
            var fillColor = color;
            var backgroundColor = color;
            var textColor = color;

            fillColor.a = _fillAlpha;
            backgroundColor.a = _backgroundAlpha;
            textColor.a = _textAlpha;

            _timer.fillSettings.color = fillColor;
            _timer.backgroundSettings.color = backgroundColor;
            _timer.textSettings.color = textColor;
        }
    }
}