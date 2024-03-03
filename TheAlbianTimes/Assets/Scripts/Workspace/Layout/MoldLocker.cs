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

        public bool blink = false;

        protected override void PointerClick(BaseEventData data)
        {
            if (!LayoutManager.Instance.IsMoldInPlace())
            {
                return;
            }
            _newsPaperMold.SetDraggable(!_newsPaperMold.IsDraggable());
            blink = false;
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