using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace Workspace.Editorial
{
    public class Postit : ThrowableInteractableRectTransform
    {
        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI _text;

        public Image GetImage()
        {
            return _image;
        }

        public void SetText(string text)
        {
            _text.text = text;
        }

        public void StartThrowAway()
        {
            if (!gameObject.activeSelf)
            {
                 return;   
            }
            StartCoroutine(ThrowAway());
        }

        private IEnumerator ThrowAway()
        {
            //// SET VALUES: DRAG_VELOCITY - VECTOR_OFFSET
            //dragVelocity
            //_vectorOffset
            
            yield return SlideCoroutine();
            
            gameObject.SetActive(false);
        }
    }
}
