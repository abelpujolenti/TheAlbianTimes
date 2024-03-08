using Managers;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using Utility;

namespace Workspace.Layout
{
    public class MoldLocker : InteractableRectTransform
    {
        [SerializeField] private NewspaperMold _newsPaperMold;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Publisher _publisher;

        public bool blink = false;

        protected override void PointerClick(BaseEventData data)
        {
            if (!LayoutManager.Instance.IsMoldInPlace())
            {
                return;
            }
            _newsPaperMold.SetDraggable(!_newsPaperMold.IsDraggable());
            _publisher.scrolling = !_publisher.scrolling;
            blink = false;

            float pressedHeightMultiplier = .75f;
            rectTransform.sizeDelta = _newsPaperMold.IsDraggable() ? new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y * pressedHeightMultiplier) : new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y / pressedHeightMultiplier);
        }

        private void Update()
        {
            if (blink)
            {
                float a = Mathf.Abs(Mathf.Sin(Time.time * 2f));
                Color blinkColor = _text.color.WithAlpha(a);
                _text.color = blinkColor;
            }
            else
            {
                _text.color = _text.color.WithAlpha(1f);
            }
        }
    }
}