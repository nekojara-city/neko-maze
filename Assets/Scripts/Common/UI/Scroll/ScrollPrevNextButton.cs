using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Common.UI.Scroll
{
    internal class ScrollPrevNextButton : MonoBehaviour
    {
        [SerializeField] private ScrollElementManager _scrollElementManager;
        
        [SerializeField] private Button _button;

        private enum Type
        {
            Prev,
            Next,
        }

        [SerializeField] private Type _type;

        private void Awake()
        {
            switch (_type)
            {
                case Type.Prev:
                    _button.onClick.AddListener(OnClickPrev);
                    break;
                
                case Type.Next:
                    _button.onClick.AddListener(OnClickNext);
                    break;
            }

            _scrollElementManager
                .OnCurrentChanged
                .Subscribe(OnCurrentChanged);
        }

        private void OnClickPrev()
        {
            _scrollElementManager.MovePrev();
        }

        private void OnClickNext()
        {
            _scrollElementManager.MoveNext();
        }

        private void OnCurrentChanged(int index)
        {
            switch (_type)
            {
                case Type.Prev:
                    gameObject.SetActive(index > 0);
                    break;
                
                case Type.Next:
                    gameObject.SetActive(index < _scrollElementManager.Count - 1);
                    break;
            }
        }
    }
}