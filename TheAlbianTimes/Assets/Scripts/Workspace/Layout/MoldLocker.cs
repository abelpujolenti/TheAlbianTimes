using System.Collections;
using Managers;
using TMPro;
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

        private bool _blink = false;
        private bool _nudged = false;

        protected override void PointerClick(BaseEventData data)
        {
            if (!LayoutManager.Instance.IsMoldInPlace())
            {
                return;
            }
            _newsPaperMold.SetDraggable(!_newsPaperMold.IsDraggable());
            _publisher.SetIsScrolling(!_publisher.IsScrolling());
            _blink = false;

            float pressedHeightMultiplier = .75f;
            rectTransform.sizeDelta = _newsPaperMold.IsDraggable() ? new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y * pressedHeightMultiplier) : new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y / pressedHeightMultiplier);

            if (GameManager.Instance.GetRound() != 0 || _nudged)
            {
                return;
            }

            _nudged = true;
            _newsPaperMold.Nudge();
        }

        public void SetBlink(bool blink)
        {
            _blink = blink;
            if (!_blink)
            {
                return;
            }
            StartCoroutine(Blink());
        }

        private IEnumerator Blink()
        {
            while (_blink)
            {
                float alpha = Mathf.Abs(Mathf.Sin(Time.time * 2f));
                Color blinkColor = ColorUtil.Alpha(_text.color, alpha);
                _text.color = blinkColor;
                yield return null;
            }
            _text.color = ColorUtil.Alpha(_text.color, 1f);
        }
    }
}