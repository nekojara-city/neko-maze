using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Common.UI.Scroll
{
    internal class ScrollDotPager : MonoBehaviour
    {
        [SerializeField] private ScrollElementManager _scrollElementManager;

        [SerializeField] private GameObject _dotPrefab;
        [SerializeField] private GameObject _dotSelectedPrefab;
        [SerializeField] private Transform _root;

        private GameObject _dotSelected;

        private void Awake()
        {
            _scrollElementManager
                .OnElementNumChanged
                .Subscribe(OnElementNumChanged);

            _scrollElementManager
                .OnCurrentChanged
                .Subscribe(OnCurrentChanged);
        }

        // ドットの配置
        private void OnElementNumChanged(int num)
        {
            foreach (Transform child in _root)
            {
                Destroy(child.gameObject);
            }

            _dotSelected = Instantiate(_dotSelectedPrefab, _root);

            for (var i = 0; i < num - 1; ++i)
            {
                var obj = Instantiate(_dotPrefab, _root);

                var button = obj.GetComponentInChildren<Button>();
                button.onClick.AddListener(() => OnClickDot(obj.transform));
            }
        }

        // ページ変更
        private void OnCurrentChanged(int index)
        {
            if (_dotSelected == null) return;

            _dotSelected.transform.SetSiblingIndex(index);
        }

        // ドットクリック
        private void OnClickDot(Transform obj)
        {
            _scrollElementManager.CurrentIndex = obj.transform.GetSiblingIndex();
        }
    }
}