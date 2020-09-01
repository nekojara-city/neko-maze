using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Common.UI.Scroll
{
    internal class ScrollGridSnap : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private Camera _camera;
        
        [SerializeField] private ScrollElementManager _scrollElementManager;
        [SerializeField] private Transform _scrollTransform;

        [SerializeField] private float _easePower = 1;
        [SerializeField, Range(0, 1)] private float _easeRatio;

        private readonly List<Vector3> _offsets = new List<Vector3>();
        private Vector3 _targetPosition;

        private bool _isLayoutChanging;

        private bool _isDragging;
        private Vector3 _basePosition;

        private void Awake()
        {
            _scrollElementManager
                .OnInitialized
                .Subscribe(_ => OnInitialized());

            _scrollElementManager
                .OnElementNumChanged
                .Subscribe(num => OnElementNumChanged(num).Forget());

            _scrollElementManager
                .OnCurrentChanged
                .Where(index => !_isLayoutChanging)
                .Subscribe(OnCurrentChanged);

            this.UpdateAsObservable()
                .Subscribe(_ => OnUpdate());
        }

        private void OnInitialized()
        {
            _scrollTransform.localPosition = _targetPosition;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _isDragging = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.localPosition += new Vector3(eventData.delta.x, 0);
            
            var index = GetClosestElement();
            if (index >= 0)
            {
                _scrollElementManager.CurrentIndex = index;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _isDragging = false;
        }

        private void OnUpdate()
        {
            if (!_isDragging)
            {
                var ratio = 1 - Mathf.Pow(1 - _easeRatio, _easePower * Time.deltaTime);

                _scrollTransform.localPosition = Vector3.Lerp(
                    _scrollTransform.localPosition,
                    _targetPosition,
                    ratio);
            }
        }

        private void OnCurrentChanged(int index)
        {
            _targetPosition = -_offsets[_scrollElementManager.CurrentIndex];
        }

        private async UniTaskVoid OnElementNumChanged(int num)
        {
            _offsets.Clear();

            var elements = _scrollElementManager.Elements;
            if (elements.Count <= 0) return;

            _isLayoutChanging = true;

            await UniTask.WaitForEndOfFrame();

            var basePosition = elements[0].localPosition;

            for (var i = 0; i < elements.Count; ++i)
            {
                _offsets.Add(elements[i].localPosition - basePosition);
            }

            _basePosition = _scrollTransform.localPosition + basePosition;

            _isLayoutChanging = false;
            _scrollElementManager.CurrentIndex = 0;
        }

        private int GetClosestElement()
        {
            var index = -1;
            var minSqrDistance = float.MaxValue;

            var elements = _scrollElementManager.Elements;
            for (var i = 0; i < elements.Count; ++i)
            {
                var elemPos = _scrollTransform.localPosition + elements[i].localPosition - _basePosition;

                var sqrDistance = elemPos.sqrMagnitude;
                if (sqrDistance < minSqrDistance)
                {
                    index = i;
                    minSqrDistance = sqrDistance;
                }
            }

            return index;
        }
    }
}