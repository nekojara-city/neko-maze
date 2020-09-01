using UnityEngine;
using UnityEngine.Playables;

namespace Game.Timeline
{
    internal class SectionPauser : MonoBehaviour
    {
        [SerializeField] private PlayableDirector _playableDirector;

        public void OnSectionPause()
        {
            _playableDirector.Pause();
        }
    }
}