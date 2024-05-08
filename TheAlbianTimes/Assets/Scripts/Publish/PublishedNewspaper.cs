using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utility;

namespace Publish
{
    public class PublishedNewspaper : InteractableRectTransform
    {
        [SerializeField] private Image[] layouts;
        private Dictionary<Vector3, NewsData> sortedArticles;
        private Vector3 startPosition;
        private int newspaperLayoutIndex = 0;
        private float leanThreshold = 0f;
        private float sendThreshold = 1.8f;
        private Coroutine hintCoroutine;

        private void Start()
        {
            startPosition = transform.position;
            
            GetSortedArticles();
            StartCoroutine(Reveal());
            hintCoroutine = StartCoroutine(HintCoroutine());
        }

        private IEnumerator Reveal()
        {
            transform.position += new Vector3(0f, -20f, 0f);
            ChooseLayout();
            SetArticles();
            yield return TransformUtility.SetPositionCoroutine(transform, transform.position, transform.position + new Vector3(0f, 20f, 0f), 1f);
        }

        private IEnumerator HintCoroutine()
        {
            yield return new WaitForSeconds(2f);
            while (this != null)
            {
                Vector3 offset = new Vector3(0f, .7f, 1.7f);
                StartCoroutine(SetRotationCoroutine(30f, .3f));
                yield return TransformUtility.SetPositionCoroutine(transform, transform.position, transform.position + offset, .3f);
                StartCoroutine(SetRotationCoroutine(0f, .18f));
                yield return TransformUtility.SetPositionCoroutine(transform, transform.position, transform.position - offset, .18f);
                yield return new WaitForSeconds(7f);
            }
        }

        protected override void BeginDrag(BaseEventData data)
        {
            PointerEventData pointerData = (PointerEventData)data;

            Vector3 mousePosition;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRect, pointerData.position, _camera, out mousePosition);

            _vectorOffset = gameObjectToDrag.transform.position - mousePosition;

            held = true;
        }

        protected override void Drag(BaseEventData data)
        {
            if (hintCoroutine != null) StopCoroutine(hintCoroutine);
            base.Drag(data);
            float xRotation = Mathf.Min(60f, Mathf.Max(0f, (transform.position.y - leanThreshold) * 20f));
            transform.rotation = Quaternion.Euler(new Vector3(xRotation, transform.rotation.y, transform.rotation.z));
            float z = startPosition.z + Mathf.Max(0f, 2.8f * (transform.position.y - leanThreshold));
            transform.position = new Vector3(transform.position.x, transform.position.y, z);
        }

        protected override void EndDrag(BaseEventData data)
        {
            base.EndDrag(data);
            if (transform.position.y > sendThreshold)
            {
                draggable = false;
                StartCoroutine(FlyOffCoroutine(3f, .3f));
            }
            else
            {
                StartCoroutine(SnapCoroutine(.3f));
            }
        }

        private IEnumerator SnapCoroutine(float t)
        {
            draggable = false;
            StartCoroutine(TransformUtility.SetPositionCoroutine(transform, transform.position, new Vector3(startPosition.x, leanThreshold, startPosition.z), t));
            yield return SetRotationCoroutine(0f, t);
        }

        private void ChooseLayout()
        {
            Vector3 avgPos = Vector3.zero;
            foreach (var article in sortedArticles)
            {
                avgPos += article.Key;
            }
            avgPos /= sortedArticles.Count;

            if (avgPos.x > 0)
            {
                if (avgPos.y > 0)
                {
                    newspaperLayoutIndex = 0;
                }
                else
                {
                    newspaperLayoutIndex = 2;
                }
            }
            else if (sortedArticles.Count < 6)
            {
                newspaperLayoutIndex = 1;
            }
            else
            {
                newspaperLayoutIndex = 2;
            }

            layouts[newspaperLayoutIndex].gameObject.SetActive(true);
        }

        private void SetArticles()
        {
            var texts = layouts[newspaperLayoutIndex].transform.GetComponentsInChildren<TextMeshProUGUI>();
            for (int i = 0; i < texts.Length; i++)
            {
                if (i < sortedArticles.Count)
                {
                    var article = sortedArticles.Values.ToArray()[i];
                    texts[i].text = article.biases[article.currentBias].headline;
                }
                else
                {
                    texts[i].text = "";
                }
            }
        }

        private void GetSortedArticles()
        {
            sortedArticles = PublishingManager.Instance.currentPublishedArticles
                .OrderBy(k => -k.Key.y)
                .ThenBy(k => k.Key.x)
                .ToDictionary(k => k.Key, k => k.Value);
        }

        private IEnumerator FlyOffCoroutine(float moveTime, float rotateTime)
        {
            StartCoroutine(SetRotationCoroutine(80f, rotateTime));
            yield return TransformUtility.SetPositionCoroutine(transform, transform.position, transform.position + new Vector3(0f, 100f, 3000f), moveTime);
            Finish();
        }

        private new IEnumerator SetRotationCoroutine(float xRotation, float t)
        {
            float elapsedT = 0f;
            Vector3 startRotation = transform.rotation.eulerAngles;
            while (elapsedT <= t)
            {
                float x = Mathf.LerpAngle(startRotation.x, xRotation, elapsedT / t);
                transform.rotation = Quaternion.Euler(new Vector3(x, transform.rotation.y, transform.rotation.z));
                yield return new WaitForFixedUpdate();
                elapsedT += Time.fixedDeltaTime;
            }
            transform.rotation = Quaternion.Euler(new Vector3(xRotation, transform.rotation.y, transform.rotation.z));

        }

        private void Finish()
        {
            CountryEvent currEvent = CountryEventManager.Instance.PopFirstEvent();
            if (currEvent != null)
            {
                Debug.Log(currEvent.id);
                currEvent.Run();
                CountryEventManager.Instance.triggeredEvents.Add(currEvent);
            }
            GameManager.Instance.LoadScene(ScenesName.STATS_SCENE);
        }
    }
}
