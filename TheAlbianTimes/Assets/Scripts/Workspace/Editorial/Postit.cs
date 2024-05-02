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
            dragVelocity = new Vector3(Random.Range(-2f, .7f), 4f, 0f);
            _vectorOffset = new Vector3(0f, Random.Range(0f,-.1f), 0f);
            
            yield return SlideCoroutine();
            
            gameObject.SetActive(false);
        }
    }
}
