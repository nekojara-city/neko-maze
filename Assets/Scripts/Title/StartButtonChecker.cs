using System;
using Tutorial;
using UnityEngine;
using UnityEngine.UI;

namespace Title
{
    internal class StartButtonChecker : MonoBehaviour
    {
        [SerializeField] private SceneTransition _transition;
        [SerializeField] private TutorialWindow _tutorialWindow;

        private bool _transit = false;

        private void Update()
        {
            if (_tutorialWindow.IsOpening)
            {
                if (Input.GetButtonDown("Cancel"))
                    _tutorialWindow.CloseWindow();
            }
            else if (Input.GetButtonDown("Submit") && !_transit)
            {
                _transition.PerformTransition();
                _transit = true;
            }
        }
    }
}