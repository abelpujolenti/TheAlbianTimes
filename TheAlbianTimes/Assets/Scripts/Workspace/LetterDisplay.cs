using System;
using Managers;
using UnityEngine;

namespace Workspace
{
    public class LetterDisplay : MonoBehaviour
    {
        [SerializeField] private GameObject _background;
        
        [SerializeField] private Letter _letter;

        private void Start()
        {
            EventsManager.OnClickLetter += DisplayLetter;
        }

        private void OnDestroy()
        {
            EventsManager.OnClickLetter -= DisplayLetter;
        }

        private void DisplayLetter(String letterText)
        {
            _background.SetActive(true);
            _letter.gameObject.SetActive(true);
            _letter.SetText(letterText);
        }

        public void HideLetter()
        {
            _background.SetActive(false);
            _letter.gameObject.SetActive(false);
        }
    }
}
