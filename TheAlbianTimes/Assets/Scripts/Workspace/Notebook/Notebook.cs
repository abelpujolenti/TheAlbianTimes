using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utility;

namespace Workspace.Notebook
{
    public class Notebook : InteractableRectTransform
    {
        private const float PAGE_OPEN_TIME = 0.5f;
        private const float PAGE_CLOSE_TIME = 0.25f;
        private const float PAGE_FLIP_TIME = 0.6f;
        private const float PULL_UP_BOOK_TIME = 0.5f;
        private const float PULL_DOWN_BOOK_TIME = 0.4f;
        private const float AUTO_CLOSE_THRESHOLD = -6f;
        
        [SerializeField] private GameObject rightPage;
        [SerializeField] private GameObject leftPage;
        [SerializeField] private GameObject flipPage;
        [SerializeField] private Transform pageMarkerParent;
        [SerializeField] private Transform pageMarkerActiveParent;
        
        private float boundX = 10f;

        private List<NotebookPage> pages;
        private int currentPage = 0;

        private Vector3 initialPosition;
        private Vector3 dragVector;

        private Coroutine flipCoroutine;

        private bool open = false;

        protected override void Setup()
        {
            base.Setup();
            initialPosition = transform.position;
        }

        protected override void Drag(BaseEventData data)
        {
            Vector3 prevPosition = transform.position;
            base.Drag(data);

            float x = Mathf.Max(-boundX, Mathf.Min(boundX, transform.position.x));
            float y = Mathf.Max(initialPosition.y, Mathf.Min(6.33f, transform.position.y));
            transform.position = new Vector3(x, y, transform.position.z);

            Vector3 currPosition = transform.position;
            dragVector = currPosition - prevPosition;

        }

        protected override void EndDrag(BaseEventData data)
        {
            base.EndDrag(data);
            if (open && (transform.position.y <= AUTO_CLOSE_THRESHOLD || transform.position.y <= AUTO_CLOSE_THRESHOLD * 0.5f && dragVector.y < -0.04f))
            {
                Close();
            }
            else if (!open)
            {
                if (transform.position.y > AUTO_CLOSE_THRESHOLD) Open(false);
                else StartCoroutine(MoveDownCoroutine(PULL_DOWN_BOOK_TIME * (Vector2.Distance(transform.position, initialPosition) / 7f)));
            }
        }

        protected override void PointerClick(BaseEventData data)
        {
            base.PointerClick(data);
            if (held) return;
            Open();
        }

        public void ClickFromBooknote(int page)
        {
            currentPage = page;
            if (!open)
            {
                Open();
            }
            else
            {
                FlipPage();
            }
        }

        public void Open(bool move = true)
        {
            if (open) return;
            StartCoroutine(OpenCoroutine(move));
        }

        private IEnumerator OpenCoroutine(bool move)
        {
            EnableCover();
            if (move)
            {
                yield return MoveUpCoroutine(PULL_UP_BOOK_TIME);
            }
            yield return StartCoroutine(RotatePageCoroutine((RectTransform)leftPage.transform, PAGE_OPEN_TIME, 179.9f, 0f, 0.5f, DisableCover));
            open = true;
        }

        public void Close()
        {
            if (!open) return;
            if (flipCoroutine != null) StopCoroutine(flipCoroutine);
            StartCoroutine(CloseCoroutine());
        }

        private IEnumerator CloseCoroutine()
        {
            float speed = Mathf.Max(0.4f, (Vector2.Distance(transform.position, initialPosition) / 8f));
            DisableCover();
            StartCoroutine(RotatePageCoroutine((RectTransform)leftPage.transform, PAGE_CLOSE_TIME * speed, 0.01f, 180f, 0.5f, EnableCover));
            yield return MoveDownCoroutine(PULL_DOWN_BOOK_TIME * speed);

            for (int i = 0; i < pageMarkerActiveParent.childCount; i++)
            {
                pageMarkerActiveParent.GetChild(i).SetParent(pageMarkerParent);
            }

            open = false;
        }

        private IEnumerator MoveUpCoroutine(float t)
        {
            yield return StartCoroutine(TransformUtility.SetPositionCoroutine(transform, transform.position, new Vector3(transform.position.x, -.5f, transform.position.z), PULL_UP_BOOK_TIME));
        }

        private IEnumerator MoveDownCoroutine(float t)
        {
            yield return StartCoroutine(TransformUtility.SetPositionCoroutine(transform, transform.position, initialPosition, t));
        }

        public void FlipPage()
        {
            flipCoroutine = StartCoroutine(FlipPageCoroutine());
        }

        private IEnumerator FlipPageCoroutine()
        {
            flipPage.SetActive(true);

            Image flipImage = flipPage.transform.Find("Background").GetComponent<Image>();
            float imageBrightness = ColorUtil.GetBrightness(flipImage.color);
            yield return StartCoroutine(RotatePageCoroutine((RectTransform)flipPage.transform, PAGE_FLIP_TIME / 2f, 0f, 90f, 0.5f));
            StartCoroutine(ShadePageCoroutine(flipImage, PAGE_FLIP_TIME / 2f, 0.3f, imageBrightness, 2f));
            yield return StartCoroutine(RotatePageCoroutine((RectTransform)flipPage.transform, PAGE_FLIP_TIME / 2f, 90f, 180f, 2f));
            flipPage.SetActive(false);
        }

        private void EnableCover()
        {
            leftPage.transform.Find("Background").GetComponent<Image>().color = new Color(.5f, .25f, .3f);
        }

        private void DisableCover()
        {
            leftPage.transform.Find("Background").GetComponent<Image>().color = new Color(.95f, .95f, .93f);
        }

        private IEnumerator ShadePageCoroutine(Image image, float t, float start, float end, float exp = 0.5f)
        {
            image.color = ColorUtil.SetBrightness(image.color, start);
            float elapsedT = 0f;
            while (elapsedT <= t)
            {
                float b = Mathf.Lerp(start, end, Mathf.Pow((elapsedT / t), exp));
                image.color = ColorUtil.SetBrightness(image.color, b);

                yield return new WaitForFixedUpdate();
                elapsedT += Time.fixedDeltaTime;
            }
            image.color = ColorUtil.SetBrightness(image.color, end);
        }

        private IEnumerator RotatePageCoroutine(RectTransform rt, float t, float start, float end, float exp = 0.5f, Action midCallback = null)
        {
            rt.rotation = Quaternion.Euler(new Vector3(0f, start, 0f));
            float elapsedT = 0f;
            float absDiff = Mathf.Abs(end - start);
            bool callbackActivated = false;
            while (elapsedT <= t)
            {
                float yRotation = Mathf.LerpAngle(start, end, Mathf.Pow((elapsedT / t), exp));
                rt.rotation = Quaternion.Euler(new Vector3(0f, yRotation, 0f));

                if (!callbackActivated && midCallback != null && Mathf.Abs(yRotation - start) > absDiff / 2f)
                {
                    midCallback();
                    callbackActivated = true;
                }

                yield return new WaitForFixedUpdate();
                elapsedT += Time.fixedDeltaTime;
            }
            rt.rotation = Quaternion.Euler(new Vector3(0f, end, 0f));
        }

        public bool IsOpen()
        {
            return open;
        }
    }
}
