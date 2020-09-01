using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Common.UI.Scroll
{
    /// <summary>
    /// スクロールビューの要素管理
    /// </summary>
    internal class ScrollElementManager : MonoBehaviour
    {
        private Transform _transform;
        private readonly List<Transform> _children = new List<Transform>();

        private Subject<Unit> _onInitialized = new Subject<Unit>();
        private IntReactiveProperty _childrenCount = new IntReactiveProperty();
        private IntReactiveProperty _currentIndex = new IntReactiveProperty();

        private void Awake()
        {
            _transform = transform;
            RefreshChildren();
        }

        public void Initialize()
        {
            _onInitialized.OnNext(Unit.Default);
        }

        public void RefreshChildren()
        {
            _children.Clear();

            foreach (Transform child in _transform)
            {
                _children.Add(child);
            }

            _childrenCount.Value = _children.Count;
            _currentIndex.SetValueAndForceNotify(0);
        }

        public int Count => _children.Count;

        public int CurrentIndex
        {
            get => _currentIndex.Value;
            set => _currentIndex.Value = value;
        }

        public List<Transform> Elements => _children;
        public Transform Current => _children[CurrentIndex];

        public IObservable<Unit> OnInitialized=> _onInitialized;
        public IObservable<int> OnCurrentChanged => _currentIndex.Where(x => x >= 0 && x < _children.Count);
        public IObservable<int> OnElementNumChanged => _childrenCount;

        public Transform GetContent(int index) => _children[index];

        public void MovePrev()
        {
            if (_currentIndex.Value <= 0) return;
            --_currentIndex.Value;
        }

        public void MoveNext()
        {
            if (_currentIndex.Value >= _children.Count - 1) return;
            ++_currentIndex.Value;
        }
    }
}