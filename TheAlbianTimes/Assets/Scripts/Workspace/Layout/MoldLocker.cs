using System.Collections;
using Countries;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Utility;

namespace Workspace.Layout
{
    public class MoldLocker : InteractableRectTransform
    {
        private const string LOCK_LOCKER_SOUND = "LockMoldLocker";
        private const string UNLOCK_LOCKER_SOUND = "UnlockMoldLocker";
        
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

            if (_newsPaperMold.IsDraggable())
            {
                AudioManager.Instance.Play3DSound(UNLOCK_LOCKER_SOUND, 5, 100, transform.position);
                Vector2 sizeDelta = rectTransform.sizeDelta;
                rectTransform.sizeDelta = new Vector2(sizeDelta.x, sizeDelta.y * pressedHeightMultiplier);
            }
            else
            {
                AudioManager.Instance.Play3DSound(LOCK_LOCKER_SOUND, 5, 100, transform.position);
                Vector2 sizeDelta = rectTransform.sizeDelta;
                rectTransform.sizeDelta = new Vector2(sizeDelta.x, sizeDelta.y / pressedHeightMultiplier);
            }

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