using System.Collections;
using Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using Utility;

namespace Workspace.Notebook
{
    public class NotebookBookmark : InteractableRectTransform
    {
        private Transform _pageMarkerActiveParent;

        private Coroutine moveUpCoroutine;
        private Coroutine moveDownCoroutine;

        private bool _rightSide = true;

        private int _page;

        public void SetPage(int page)
        {
            _page = page;
        }

        protected override void PointerClick(BaseEventData data)
        {
            base.PointerClick(data);
            NotebookManager.Instance.OnClickBookmark(this, _page);
            MoveDown();
        }
        protected override void PointerEnter(BaseEventData data)
        {
            base.PointerEnter(data);
            if (transform.parent != _pageMarkerActiveParent)
            {
                MoveUp();
            }
        }
        protected override void PointerExit(BaseEventData data)
        {
            base.PointerExit(data);
            MoveDown();
        }

        private void MoveUp()
        {
            if (moveUpCoroutine != null) StopCoroutine(moveUpCoroutine);
            if (moveDownCoroutine != null) StopCoroutine(moveDownCoroutine);
            moveUpCoroutine = StartCoroutine(MoveCoroutine(5f, 0.15f));
        }

        private void MoveDown()
        {
            if (transform.localPosition.y == 0f) return;
            if (moveUpCoroutine != null) StopCoroutine(moveUpCoroutine);
            if (moveDownCoroutine != null) StopCoroutine(moveDownCoroutine);
            moveDownCoroutine = StartCoroutine(MoveCoroutine(0f, 0.2f));
        }

        private IEnumerator MoveCoroutine(float end, float t)
        {
            float start = transform.localPosition.y;
            float elapsedT = 0f;
            while (elapsedT <= t)
            {
                Vector3 newPos = transform.localPosition;
                float sst = Mathf.Pow(Mathf.Min(1f, elapsedT / t), 2);
                newPos.y = Mathf.SmoothStep(start, end, sst);
                transform.localPosition = newPos;
                yield return new WaitForFixedUpdate();
                elapsedT += Time.fixedDeltaTime;
            }
            gameObjectToDrag.transform.localPosition = new Vector3(transform.localPosition.x, end, transform.localPosition.z);
        }

        public void SetIsOnRightSide(bool rightSide)
        {
            _rightSide = rightSide;
        }

        public bool IsOnRightSide()
        {
            return _rightSide;
        }

        public void SetPageMarkerActiveParent(Transform pageMarkerActiveParent)
        {
            _pageMarkerActiveParent = pageMarkerActiveParent;
        }
    }
}
