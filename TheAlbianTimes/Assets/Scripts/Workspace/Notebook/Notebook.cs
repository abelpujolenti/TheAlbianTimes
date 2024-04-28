using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utility;

namespace Workspace.Notebook
{
    public class Notebook : InteractableRectTransform
    {
        private const string OPEN_NOTEBOOK = "Open Notebook";
        private const string CLOSE_NOTEBOOK = "Close Notebook";
        private const string GRAB_NOTEBOOK = "Grab Notebook";
        
        [SerializeField] private float PAGE_OPEN_TIME = 0.5f;
        [SerializeField] private float PAGE_CLOSE_TIME = 0.25f;
        [SerializeField] private float PAGE_FLIP_TIME = 0.6f;
        [SerializeField] private float PULL_UP_BOOK_TIME = 0.5f;
        [SerializeField] private float PULL_DOWN_BOOK_TIME = 0.4f;
        [SerializeField] private float AUTO_CLOSE_THRESHOLD = -6f;
        
        [SerializeField] private GameObject rightPage;
        [SerializeField] private GameObject leftPage;
        [SerializeField] private GameObject flipPage;
        [SerializeField] private Transform pageMarkerParent;
        [SerializeField] private Transform pageMarkerActiveParent;

        [SerializeField] private Image _leftPageBackground;
        [SerializeField] private Image _flipPageBackground;
        
        private float boundX = 10f;

        private List<NotebookPage> pages;

        private Vector3 initialPosition;
        private Vector3 dragVector;
        private float imageBrightness;

        private Coroutine flipCoroutine;

        private bool open;

        protected override void Setup()
        {
            base.Setup();
            initialPosition = transform.position;
        }

        private void Start()
        {
            NotebookManager.Instance.SetNotebook(this);
            imageBrightness = ColorUtil.GetBrightness(_flipPageBackground.color);
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
            
            NotebookManager.Instance.EndDragNotebook(transform.position.y, dragVector.y, AUTO_CLOSE_THRESHOLD);
        }

        protected override void PointerClick(BaseEventData data)
        {
            base.PointerClick(data);
            if (held) return;
            if (open)
            {
                PointerEventData pointerData = (PointerEventData)data;
                Vector3 positionInBook = Vector3.zero;
                RectTransformUtility.ScreenPointToWorldPointInRectangle((RectTransform)transform, pointerData.position, Camera.main, out positionInBook);
                positionInBook -= transform.position;

                if (positionInBook.x >= 0)
                {
                    NotebookManager.Instance.NextPage();
                    return;
                }
                
                NotebookManager.Instance.PreviousPage();
                return;
            }
            NotebookManager.Instance.OpenNotebook();
        }

        public void Open(Action open, bool move)
        {
            StartCoroutine(OpenCoroutine(move, open));
        }

        private IEnumerator OpenCoroutine(bool move, Action open)
        {
            if (move)
            {
                AudioManager.Instance.Play2DSound(GRAB_NOTEBOOK);
                yield return MoveUpCoroutine(PULL_UP_BOOK_TIME);
            }
            AudioManager.Instance.Play2DSound(OPEN_NOTEBOOK);
            yield return StartCoroutine(RotatePageCoroutine((RectTransform)leftPage.transform, PAGE_OPEN_TIME, 179.9f, 0f, 0.5f, open));
            
            this.open = true;
        }

        public void Close(Action close)
        {
            if (flipCoroutine != null) StopCoroutine(flipCoroutine);
            StartCoroutine(CloseCoroutine(close));
        }

        private IEnumerator CloseCoroutine(Action close)
        {
            float speed = Mathf.Max(0.4f, (Vector2.Distance(transform.position, initialPosition) / 8f));
            StartCoroutine(RotatePageCoroutine((RectTransform)leftPage.transform, PAGE_CLOSE_TIME * speed, 0.01f, 180f, 0.5f, close));
            yield return MoveDownCoroutine(PULL_DOWN_BOOK_TIME * speed);

            for (int i = 0; i < pageMarkerActiveParent.childCount; i++)
            {
                pageMarkerActiveParent.GetChild(i).SetParent(pageMarkerParent);
            }
            AudioManager.Instance.Play2DSound(CLOSE_NOTEBOOK);
            open = false;
        }

        private IEnumerator MoveUpCoroutine(float t)
        {
            yield return StartCoroutine(TransformUtility.SetPositionCoroutine(transform, transform.position, new Vector3(transform.position.x, -.5f, transform.position.z), PULL_UP_BOOK_TIME));
        }

        public void StartMoveDownCoroutine()
        {
            StartCoroutine(MoveDownCoroutine(PULL_DOWN_BOOK_TIME * (Vector2.Distance(transform.position, initialPosition) / 7f)));
        }

        private IEnumerator MoveDownCoroutine(float t)
        {
            yield return StartCoroutine(TransformUtility.SetPositionCoroutine(transform, transform.position, initialPosition, t));
        }

        public void FlipPageLeft(Action midPointFlip, Action endFlip)
        {
            if (flipCoroutine != null) StopCoroutine(flipCoroutine);
            flipCoroutine = StartCoroutine(FlipPageLeftCoroutine(midPointFlip, endFlip));
        }

        public void FlipPageRight(Action midPointFlip, Action endFlip)
        {
            if (flipCoroutine != null) StopCoroutine(flipCoroutine);
            flipCoroutine = StartCoroutine(FlipPageRightCoroutine(midPointFlip, endFlip));
        }

        private IEnumerator FlipPageLeftCoroutine(Action midPointFlip, Action endFlip)
        {
            flipPage.SetActive(true);

            yield return StartCoroutine(RotatePageCoroutine((RectTransform)flipPage.transform, PAGE_FLIP_TIME / 2f, 0f, 90f, 0.5f));
            midPointFlip();
            
            Image pageImage = _flipPageBackground.transform.GetChild(1).GetComponent<Image>();
            StartCoroutine(ShadePageCoroutine(pageImage, PAGE_FLIP_TIME / 2f, 0.3f, ColorUtil.GetBrightness(pageImage.color), 2f));
            yield return StartCoroutine(RotatePageCoroutine((RectTransform)flipPage.transform, PAGE_FLIP_TIME / 2f, 90f, 180f, 2f));
            endFlip();
            flipPage.SetActive(false);
        }

        private IEnumerator FlipPageRightCoroutine(Action midPointFlip, Action endFlip)
        {
            flipPage.SetActive(true);

            Image pageImage = _flipPageBackground.transform.GetChild(0).GetComponent<Image>();

            StartCoroutine(ShadePageCoroutine(pageImage, PAGE_FLIP_TIME / 2f, ColorUtil.GetBrightness(pageImage.color), 0.3f, 2f));
            yield return StartCoroutine(RotatePageCoroutine((RectTransform)flipPage.transform, PAGE_FLIP_TIME / 2f, 180f, 90f, 0.5f));
            midPointFlip();
            yield return StartCoroutine(RotatePageCoroutine((RectTransform)flipPage.transform, PAGE_FLIP_TIME / 2f, 90f, 0f, 2f));
            endFlip();
            flipPage.SetActive(false);
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
