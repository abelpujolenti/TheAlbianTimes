using System.Collections;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utility;

namespace Workspace.Editorial
{
    public class Postit : ThrowableInteractableRectTransform
    {
        private const string GRAB_POST_IT_SOUND = "";
        private const string DROP_POST_IT_SOUND = "";
        
        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI _text;

        protected override void BeginDrag(BaseEventData data)
        {
            base.BeginDrag(data);
            AudioManager.Instance.Play3DSound(GRAB_POST_IT_SOUND, 5, 100, transform.position);
        }

        protected override void EndDrag(BaseEventData data)
        {
            base.EndDrag(data);
            AudioManager.Instance.Play3DSound(DROP_POST_IT_SOUND, 5, 100, transform.position);
        }

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
