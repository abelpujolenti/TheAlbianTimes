using System;
using System.Collections;
using Managers;
using NoMonoBehavior;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utility;
using Random = UnityEngine.Random;

namespace Editorial
{
    public class NewsHeadline : InteractableRectTransform
    {
        private const float CHANGE_CONTENT_Y_COORDINATE = 1000;
        private const float SEND_X_COORDINATE = 1000;
        private const float SPEED_MOVEMENT = 10;
        private const float Y_DISTANCE_TO_MOVE_ON_HOVER = 10f;
        private const float SECONDS_AWAITING_TO_RETURN_TO_FOLDER = 3;

        private Coroutine _moveCoroutine;

        [SerializeField]private NewsType _newsType;
        
        private NewsFolder _newsFolder;

        [SerializeField] private TextMeshProUGUI _textMeshPro; 
        
        private String[] _shortBiasDescription;
        private String[] _biasContent;

        private Vector2 _destination;
        private Vector2 _origin;
        
        private int _folderOrderIndex;
        private int _chosenBiasIndex;
        private int _selectedBiasIndex;
        
        private bool _inFront;

        void Start()
        {
            _textMeshPro.text = _biasContent[0];

            gameObject.GetComponent<Image>().color =
                new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        }

        protected override void BeginDrag(BaseEventData data)
        {
            if (!draggable)
            {
                return;
            }
            
            base.BeginDrag(data);

            EventsManager.OnDragNewsHeadline(this);
            EditorialManager.Instance.DeactivateBiasCanvas();
        }

        protected override void EndDrag(BaseEventData data)
        {
            if (!draggable)
            {
                return;
            }
            
            base.EndDrag(data);

            PointerEventData pointerData = (PointerEventData) data;
            
            EditorialManager.Instance.ActivateBiasCanvas();
            EventsManager.OnDropNewsHeadline(this, pointerData.position);
        }

        protected override void PointerEnter(BaseEventData data)
        {
            if (!hoverable)
            {
                return;
            }

            _destination = _origin + new Vector2(0, Y_DISTANCE_TO_MOVE_ON_HOVER);
            
            SlideInFolder(_origin, _destination);
        }

        public void SlideInFolder(Vector2 origin, Vector2 destination)
        {
            if (_moveCoroutine != null)
            {
                StopCoroutine(_moveCoroutine);
            }
            _moveCoroutine = StartCoroutine(Slide(origin, destination));
        }

        protected override void PointerExit(BaseEventData data)
        {
            if (!hoverable)
            {
                return;
            }
            
            SlideInFolder(transform.localPosition, _origin);
        }

        protected override void PointerClick(BaseEventData data)
        {
            if (!clickable)
            {
                return;
            }
            
            _newsFolder.ReorderNewsHeadline(_folderOrderIndex, _chosenBiasIndex, _shortBiasDescription);
            
            if (_moveCoroutine != null)
            {
                StopCoroutine(_moveCoroutine);
            }

            transform.localPosition = _origin;
        }

        public void SetFolderOrderIndex(int newFolderOrderIndex)
        {
            _folderOrderIndex = newFolderOrderIndex;
        }

        public void SetInFront(bool isInFront)
        {
            ModifyFlags(isInFront);
            if (_inFront)
            {
                EventsManager.OnChangeNewsHeadlineContent += ChangeContent;
                EventsManager.OnChangeSelectedBiasIndex += SetSelectedBiasIndex;
                return;
            }

            EventsManager.OnChangeNewsHeadlineContent -= ChangeContent;
            EventsManager.OnChangeSelectedBiasIndex -= SetSelectedBiasIndex;
        }

        public void SetOrigin(Vector2 newOrigin)
        {
            _origin = newOrigin;
        }

        private float MoveToDestination(Vector2 origin, Vector2 destination, float timer)
        {
            timer += Time.deltaTime * SPEED_MOVEMENT;
            transform.localPosition = Vector3.Lerp(origin, destination, timer);
            
            return timer;
        }

        public void SendToLayout()
        {
            StartCoroutine(SendNewsHeadlineToLayout());
        }

        private IEnumerator SendNewsHeadlineToLayout()
        {
            EventsManager.OnChangeNewsHeadlineContent -= ChangeContent;
            
            float timer = 0f;

            Vector2 destination = new Vector2(SEND_X_COORDINATE, 0);

            while (timer < 1)
            {
                timer = MoveToDestination(_origin, _origin + destination, timer);
                yield return null;
            }
            
            EditorialManager.Instance.SendNewsHeadlineToLayoutManager(gameObject, gameObject.GetInstanceID());
            
            gameObject.SetActive(false);
        }

        private IEnumerator Slide(Vector2 origin, Vector2 destination)
        {
            float timer = 0;

            while (timer < 1)
            {
                timer = MoveToDestination(origin, destination, timer);
                yield return null;
            }
        }

        private void ChangeContent(int newChosenBiasIndex)
        {
            _chosenBiasIndex = newChosenBiasIndex;
            _textMeshPro.text = _biasContent[_chosenBiasIndex];
            SetInFront(false);
            EventsManager.OnChangeFolderOrderIndexWhenGoingToFolder += GiveDestinationToReturnToFolder;
            _newsFolder.ProcedureWhenSendNewsHeadlineToRewrite();
            Vector2 destination = new Vector2(0, CHANGE_CONTENT_Y_COORDINATE);
            StartCoroutine(SendToChangeContent(destination));
            _origin = destination;
        }
        
        private IEnumerator SendToChangeContent(Vector2 destination)
        {
            float timer = 0;

            while (timer < 1)
            {
                timer = MoveToDestination(_origin, destination, timer);
                yield return null;
            }
            
            yield return new WaitForSeconds(SECONDS_AWAITING_TO_RETURN_TO_FOLDER);

            ReturnToFolder(_origin);
        }

        public void AddToFolder()
        {
            EventsManager.OnChangeFolderOrderIndexWhenGoingToFolder += GiveDestinationToReturnToFolder;
            ReturnToFolder(new Vector2(0, CHANGE_CONTENT_Y_COORDINATE));
        }

        private void ReturnToFolder(Vector2 origin)
        {
            int countOfTotalNewsHeadline = _newsFolder.GetNewsHeadlinesLength();

            if (countOfTotalNewsHeadline > 1)
            {
                countOfTotalNewsHeadline--;
            }
            
            _destination = new Vector2(0, _newsFolder.GiveNewFolderYCoordinate(_folderOrderIndex, countOfTotalNewsHeadline));
            
            StartCoroutine(SendToFolderAgain(origin));
        }

        private IEnumerator SendToFolderAgain(Vector2 origin)
        {
            transform.SetAsFirstSibling();

            float timer = 0;

            while (timer < 1)
            {
                timer = MoveToDestination(origin, _destination, timer);
                yield return null;
            }

            EventsManager.OnChangeFolderOrderIndexWhenGoingToFolder -= GiveDestinationToReturnToFolder; 

            _origin = _destination;
            
            _newsFolder.SubtractOneToSentNewsHeadline();
        }

        public void DropOnFolder()
        {
            StartCoroutine(Slide(transform.localPosition, _origin));
        }

        private void GiveDestinationToReturnToFolder()
        {
            int countOfTotalNewsHeadline = _newsFolder.GetNewsHeadlinesLength();

            if (countOfTotalNewsHeadline > 1)
            {
                countOfTotalNewsHeadline--;
            }
            
            _destination = new Vector2(0, _newsFolder.GiveNewFolderYCoordinate(_folderOrderIndex, countOfTotalNewsHeadline));
        }

        public void SetNewsFolder(NewsFolder newsFolder)
        {
            _newsFolder = newsFolder;
        }

        private void SetSelectedBiasIndex(int newSelectedBiasIndex)
        {
            _selectedBiasIndex = newSelectedBiasIndex;
        }

        public int GetSelectedBiasIndex()
        {
            return _selectedBiasIndex;
        }

        public int GetChosenBiasIndex()
        {
            return _chosenBiasIndex;
        }

        private void ModifyFlags(bool value)
        {
            _inFront = value;
            draggable = value;
            clickable = !value;
            hoverable = !value;
        }

        public void SetShortBiasDescription(String[] shortBiasDescription)
        {
            _shortBiasDescription = shortBiasDescription;
        }

        public String[] GetShortBiasDescription()
        {
            return _shortBiasDescription;
        }

        public void SetBiasContent(String[] biasContent)
        {
            _biasContent = biasContent;
        }

        public void SetNewsType(NewsType newsType)
        {
            _newsType = newsType;
        }

        public NewsType GetNewsType()
        {
            return _newsType;
        }
    }
}
