using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using Utility;
using Workspace.Mail.Content;
using Random = UnityEngine.Random;

namespace Workspace.Mail
{
    public class MailContainer : InteractableRectTransform
    {
        private const String OPEN_DRAWER_SOUND = "Open Drawer";
        private const String CLOSE_DRAWER_SOUND = "Close Drawer";
        
        [SerializeField] private float maxX = -9.33f;
        [SerializeField] private float minX = -13.1f;
        [SerializeField] private float isOpenThreshold = -11.5f;
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
            MailManager.Instance.SetEnvelopesContainer(_envelopesContainerRectTransform);
            
            _envelopes = new List<GameObject>();
            _envelopesContent = new List<GameObject>();
            
            _envelopesContainerRectTransform.GetWorldCorners(_corners);

            for (int i = 0; i < _corners.Length; i++)
            {
                Instantiate(new GameObject(), _corners[i], new Quaternion());
            }
            
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

            Vector3 newPos = canvas.transform.TransformPoint(mousePosition) + _vectorOffset;
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
                return;
            }
            OpenContainer();
        }

        private void OpenContainer()
        {
            _moveContainerCoroutine = StartCoroutine(MoveContainerEnum(gameObjectToDrag.transform.position.x, maxX, openTime));
            AudioManager.Instance.Play3DSound(OPEN_DRAWER_SOUND, 10, 100, transform.position);
        }

        private void CloseContainer()
        {
            _moveContainerCoroutine = StartCoroutine(MoveContainerEnum(gameObjectToDrag.transform.position.x, minX, closeTime));
            AudioManager.Instance.Play3DSound(CLOSE_DRAWER_SOUND, 10, 100, transform.position);
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

        private bool IsOpen()
        {
            return gameObjectToDrag.transform.position.x > isOpenThreshold;
        }
        
        private Vector2 PositionInsideContainer(Vector2 size)
        {
            Vector2 position;
            
            position.x = Random.Range(_containerMinCoordinates.x + size.x, _containerMaxCoordinates.x - size.x);
            position.y = Random.Range(_containerMinCoordinates.y + size.y, _containerMaxCoordinates.y - size.y);
            
            return position;
        }

        private void ReceiveEnvelope(GameObject envelope)
        {
            _envelopes.Add(envelope);

            RectTransform envelopRectTransform = envelope.GetComponent<RectTransform>();

            Vector3[] envelopeCorners = new Vector3[4]; 

            envelopRectTransform.GetWorldCorners(envelopeCorners);

            Vector2 size = new Vector2(Math.Abs(envelopeCorners[0].x - envelopeCorners[2].x), Math.Abs(envelopeCorners[1].y - envelopeCorners[3].y));
            
            envelope.transform.position = PositionInsideContainer(size);
            
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

        private bool IsInsideCoordinates(Vector2 point)
        {
            return point.x > _containerMinCoordinates.x && point.x < _containerMaxCoordinates.x &&
                   point.y > _containerMinCoordinates.y && point.y < _containerMaxCoordinates.y;
        }

        private void SetEnvelopeContentProperties(GameObject envelopeContent)
        {
            envelopeContent.transform.SetParent(_envelopesContainerRectTransform);

            RectTransform envelopeContentRectTransform = envelopeContent.GetComponent<RectTransform>();

            Vector3[] envelopeCorners = new Vector3[4]; 

            envelopeContentRectTransform.GetWorldCorners(envelopeCorners);

            Vector2 size = new Vector2(Math.Abs(envelopeCorners[0].x - envelopeCorners[2].x), Math.Abs(envelopeCorners[1].y - envelopeCorners[3].y));
            
            envelopeContent.transform.position = PositionInsideContainer(size);
            
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
