using System.Collections;
using System.Collections.Generic;
using Mail.Content;
using Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using Utility;

namespace Mail
{
    public class MailContainer : InteractableRectTransform
    {
        
        [SerializeField] private float maxX = -10f;
        [SerializeField] private float minX = -14.7f;
        [SerializeField] private float isOpenThreshold = -13.5f;
        [SerializeField] private float openTime = 1f;
        [SerializeField] private float closeTime = .8f;
        private Coroutine _moveContainerCoroutine;
        
        [SerializeField] private RectTransform _envelopesContainerRectTransform;

        [SerializeField]private List<GameObject> _envelopes;
        [SerializeField]private List<GameObject> _envelopesContent;

        private readonly Vector3[] _corners = new Vector3[4];
        
        private Vector2 _containerMinCoordinates;
        private Vector2 _containerMaxCoordinates;

        private void OnEnable()
        {
            EventsManager.OnAddEnvelope += ReceiveEnvelope;
            EventsManager.OnAddEnvelopeContentToList += ReceiveEnvelopeContent;
            EventsManager.OnAddEnvelopeContent += OpenEnvelope;
        }

        private void OnDisable()
        {
            EventsManager.OnAddEnvelope -= ReceiveEnvelope;
            EventsManager.OnAddEnvelopeContentToList -= ReceiveEnvelopeContent;
            EventsManager.OnAddEnvelopeContent -= OpenEnvelope;
        }

        private void Start()
        {
            _envelopes = new List<GameObject>();
            _envelopesContent = new List<GameObject>();
            
            _envelopesContainerRectTransform.GetWorldCorners(_corners);
            
            SetContainerLimiters();

            GameObject[] envelopes = MailManager.Instance.LoadEnvelopesFromJson();
    
            if (envelopes != null)
            {
                foreach (GameObject envelope in envelopes)
                {
                    ReceiveEnvelope(envelope);
                    ReceiveEnvelopeContent(envelope.GetComponent<Envelope>().GetEnvelopeGameObject(), false);
                }
            }
    
            GameObject[] envelopesContent = MailManager.Instance.LoadEnvelopesContentFromJson();
    
            if (envelopesContent.Length == 0)
            {
                return;
            }
    
            foreach (GameObject envelopeContent in envelopesContent)
            {
                ReceiveEnvelopeContent(envelopeContent, true);
            }
        }
        
        private void SetContainerLimiters()
        {
            _containerMinCoordinates.x = _corners[0].x;
            _containerMinCoordinates.y = _corners[1].y;
            _containerMaxCoordinates.x = _corners[2].x;
            _containerMaxCoordinates.y = _corners[3].y;
        }

        protected override void Drag(BaseEventData data)
        {
            if (!held && !draggable) return;

            if (_moveContainerCoroutine != null)
            {
                StopCoroutine(_moveContainerCoroutine);
            }

            Vector2 mousePosition = GetMousePositionOnCanvas(data);

            Vector2 newPos = (Vector2)canvas.transform.TransformPoint(mousePosition) + _vectorOffset;
            newPos.y = gameObjectToDrag.transform.position.y;
            newPos.x = Mathf.Min(maxX, Mathf.Max(minX, newPos.x));
            gameObjectToDrag.transform.position = newPos;
        }

        protected override void PointerUp(BaseEventData data)
        {
            if (held) return;
            base.PointerClick(data);
            if (_moveContainerCoroutine != null)
            {
                StopCoroutine(_moveContainerCoroutine);
            }
            if (IsOpen())
            {
                CloseContainer();
            }
            else
            {
                OpenContainer();
            }
        }

        private void OpenContainer()
        {
            _moveContainerCoroutine = StartCoroutine(MoveContainerEnum(gameObjectToDrag.transform.position.x, maxX, openTime));
        }

        private void CloseContainer()
        {
            _moveContainerCoroutine = StartCoroutine(MoveContainerEnum(gameObjectToDrag.transform.position.x, minX, closeTime));
        }

        private IEnumerator MoveContainerEnum(float start, float end, float t)
        {
            float elapsedT = 0f;
            while (elapsedT <= t)
            {
                Vector3 newPos = gameObjectToDrag.transform.position;
                newPos.x = Mathf.SmoothStep(start, end, Mathf.Pow(Mathf.Min(1f, elapsedT / t), 2));
                gameObjectToDrag.transform.position = newPos;
                yield return new WaitForFixedUpdate();
                elapsedT += Time.fixedDeltaTime;
            }
            Vector3 endPos = gameObjectToDrag.transform.position;
            endPos.x = end;
            gameObjectToDrag.transform.position = endPos;
        }

        public bool IsOpen()
        {
            return gameObjectToDrag.transform.position.x > isOpenThreshold;
        }
        
        private Vector2 PositionInsideContainer()
        {
            Vector2 position;

            position.x = Random.Range(_containerMinCoordinates.x, _containerMaxCoordinates.x);
            position.y = Random.Range(_containerMinCoordinates.y, _containerMaxCoordinates.y);

            return position;
        }

        private void ReceiveEnvelope(GameObject envelope)
        {
            _envelopes.Add(envelope);
            envelope.transform.position = PositionInsideContainer();
            envelope.GetComponent<Envelope>().SetCanvas(_envelopesContainerRectTransform.gameObject.GetComponent<Canvas>());
        }

        private void ReceiveEnvelopeContent(GameObject envelopeContent, bool alone)
        {
            _envelopesContent.Add(envelopeContent);
            
            if (!alone)
            {
                return;
            }
            
            SetEnvelopeContentProperties(envelopeContent);
        }

        private void SetEnvelopeContentProperties(GameObject envelopeContent)
        {
            envelopeContent.transform.SetParent(_envelopesContainerRectTransform);
            envelopeContent.transform.position = PositionInsideContainer();
            envelopeContent.GetComponent<EnvelopeContent>().SetCanvas(_envelopesContainerRectTransform.gameObject.GetComponent<Canvas>());
        }

        private void OpenEnvelope(GameObject envelope, GameObject envelopeContent)
        {
            SetEnvelopeContentProperties(envelopeContent);
            envelopeContent.SetActive(true);
            envelopeContent.transform.localScale = new Vector3(1, 1, 1);

            _envelopes.Remove(envelope);
        }

        private void OnDestroy()
        {
            MailManager.Instance.SaveEnvelopesToJson(_envelopes.ToArray(), _envelopesContent.ToArray());
        }
    }
}
