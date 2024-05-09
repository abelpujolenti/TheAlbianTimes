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
        private const string OPEN_NOTEBOOK_SOUND = "Open Notebook";
        private const string CLOSE_NOTEBOOK_SOUND = "Close Notebook";
        private const string GRAB_NOTEBOOK_SOUND = "Grab Notebook";
        private const string TURN_NOTEBOOK_PAGE_SOUND = "Turn Notebook Page";
        
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

        private float _leftBoundX = -5f;
        private float _rightBoundX = 20f;

        private List<NotebookPage> pages;

        private float _initialYPosition;
        private Vector3 dragVector;
        private float _imageBrightness;

        private Coroutine flipCoroutine;

        private bool open;
        private bool _opening;

        private Action _midPointFlip;
        private Action _endFlip;

        protected override void Setup()
        {
            base.Setup();
            _initialYPosition = transform.position.y;
        }

        private void Start()
        {
            _imageBrightness = ColorUtil.GetBrightness(_flipPageBackground.color);
        }

        protected override void Drag(BaseEventData data)
        {
            Vector3 previousPosition = transform.position;
            base.Drag(data);
            Vector3 newPosition = transform.position;

            float x = Mathf.Max(_leftBoundX, Mathf.Min(_rightBoundX, newPosition.x));
            float y = Mathf.Max(_initialYPosition, Mathf.Min(6.33f, newPosition.y));
            transform.position = new Vector3(x, y, previousPosition.z);

            Vector3 currPosition = transform.position;
            dragVector = currPosition - previousPosition;
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

            if (_opening)
            {
                return;
            }

            _opening = true;
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
                AudioManager.Instance.Play3DSound(GRAB_NOTEBOOK_SOUND, 5, 100, transform.position);
                yield return MoveUpCoroutine();
            }
            AudioManager.Instance.Play3DSound(OPEN_NOTEBOOK_SOUND, 5, 100, transform.position);
            yield return StartCoroutine(RotatePageCoroutine((RectTransform)leftPage.transform, PAGE_OPEN_TIME, 179.9f, 0f, 0.5f, open));

            if (EventsManager.OnOpenMapPages != null)
            {
                EventsManager.OnOpenMapPages();
            }
            
            this.open = true;
            _opening = false;
        }

        public void Close(Action close)
        {
            if (flipCoroutine != null) StopCoroutine(flipCoroutine);
            StartCoroutine(CloseCoroutine(close));
        }

        public void Hide(Action close)
        {
            if (!gameObject.activeSelf)
            {
                return;
            }
            if (flipCoroutine != null) StopCoroutine(flipCoroutine);
            StartCoroutine(HideCoroutine(close));
        }

        private IEnumerator HideCoroutine(Action close)
        {
            draggable = false;
            clickable = false;
            if (open)
            {
                yield return CloseCoroutine(close);    
            }

            Transform ownTransform = transform;
            Vector3 position = ownTransform.position;
            yield return TransformUtility.SetPositionYCoroutine(ownTransform, position.y, position.y + 1, 0.5f);
            
            position = ownTransform.position;
            yield return TransformUtility.SetPositionYCoroutine(ownTransform, position.y, position.y - 2, 0.5f);
        }

        private IEnumerator CloseCoroutine(Action close)
        {
            float speed = Mathf.Max(0.4f, (Vector2.Distance(transform.position, new Vector2(_camera.transform.position.x, _initialYPosition)) / 8f));
            StartCoroutine(RotatePageCoroutine((RectTransform)leftPage.transform, PAGE_CLOSE_TIME * speed, 0.01f, 180f, 0.5f, close));
            yield return MoveDownCoroutine(PULL_DOWN_BOOK_TIME * speed);

            for (int i = 0; i < pageMarkerActiveParent.childCount; i++)
            {
                pageMarkerActiveParent.GetChild(i).SetParent(pageMarkerParent);
            }
            AudioManager.Instance.Play3DSound(CLOSE_NOTEBOOK_SOUND, 5, 100, transform.position);
            open = false;
        }

        private IEnumerator MoveUpCoroutine()
        {
            Transform notebookTransform = transform;
            Vector3 position = notebookTransform.position;
            yield return StartCoroutine(TransformUtility.SetPositionCoroutine(notebookTransform, position, new Vector3(position.x, -.5f, position.z), PULL_UP_BOOK_TIME));
        }

        public void StartMoveDownCoroutine()
        {
            StartCoroutine(MoveDownCoroutine(PULL_DOWN_BOOK_TIME * (Vector2.Distance(transform.position, new Vector2(_camera.transform.position.x, _initialYPosition)) / 7f)));
        }

        private IEnumerator MoveDownCoroutine(float t)
        {
            Transform notebookTransform = transform;
            yield return StartCoroutine(TransformUtility.SetPositionCoroutine(notebookTransform, notebookTransform.position, new Vector3(_camera.transform.position.x, _initialYPosition), t));
        }

        public void FlipPageLeft(Action midPointFlip, Action endFlip)
        {
            if (flipCoroutine != null)
            {
                StopCoroutine(flipCoroutine);
            }
            AudioManager.Instance.Play3DSound(TURN_NOTEBOOK_PAGE_SOUND, 5, 100, transform.position + new Vector3(-2.5f, 0, 0));
            flipCoroutine = StartCoroutine(FlipPageLeftCoroutine(midPointFlip, endFlip));
        }

        public void FlipPageRight(Action midPointFlip, Action endFlip)
        {
            if (flipCoroutine != null)
            {
                StopCoroutine(flipCoroutine);
            }
            AudioManager.Instance.Play3DSound(TURN_NOTEBOOK_PAGE_SOUND, 5, 100, transform.position + new Vector3(2.5f, 0, 0));
            flipCoroutine = StartCoroutine(FlipPageRightCoroutine(midPointFlip, endFlip));
        }

        private IEnumerator FlipPageLeftCoroutine(Action midPointFlip, Action endFlip)
        {
            flipPage.SetActive(true);

            yield return StartCoroutine(RotatePageCoroutine((RectTransform)flipPage.transform, PAGE_FLIP_TIME / 2f, 0f, 90f, 0.5f));
            
            midPointFlip();
            
            StartCoroutine(ShadePageCoroutine(_flipPageBackground, PAGE_FLIP_TIME / 2f, 0.3f, _imageBrightness, 2f));
            yield return StartCoroutine(RotatePageCoroutine((RectTransform)flipPage.transform, PAGE_FLIP_TIME / 2f, 90f, 180f, 2f));
            
            endFlip();
            
            flipPage.SetActive(false);
            flipCoroutine = null;
        }

        private IEnumerator FlipPageRightCoroutine(Action midPointFlip, Action endFlip)
        {
            flipPage.SetActive(true);

            StartCoroutine(ShadePageCoroutine(_flipPageBackground, PAGE_FLIP_TIME / 2f, _imageBrightness, 0.3f, 2f));
            yield return StartCoroutine(RotatePageCoroutine((RectTransform)flipPage.transform, PAGE_FLIP_TIME / 2f, 180f, 90f, 0.5f));
            
            midPointFlip();
            
            _flipPageBackground.color = ColorUtil.SetBrightness(_flipPageBackground.color, _imageBrightness);
            yield return StartCoroutine(RotatePageCoroutine((RectTransform)flipPage.transform, PAGE_FLIP_TIME / 2f, 90f, 0f, 2f));
            
            endFlip();
            
            flipPage.SetActive(false);
            flipCoroutine = null;
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
