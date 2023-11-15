using System;
using System.Collections;
using Layout;
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
        private const float SPEED_MOVEMENT = 10;
        private const float Y_DISTANCE_TO_MOVE_ON_HOVER = 10f;
        private const float SECONDS_AWAITING_TO_RETURN_TO_FOLDER = 3;
        
        private readonly float _midPoint = MIN_X_POSITION_CAMERA + (MAX_X_POSITION_CAMERA - MIN_X_POSITION_CAMERA) / 2;

        private Coroutine _moveCoroutine;

        private GameObject _gameObjectToTransferDrag;

        [SerializeField]private NewsHeadlinePiece _newsHeadlinePieceToTransferDrag;

        [SerializeField] private TextMeshProUGUI _headlineText;
        [SerializeField] private TextMeshProUGUI _contentText;
        
        private NewsFolder _newsFolder;

        [SerializeField]private NewsType _newsType;

        private String _imagePath;

        private NewsConsequenceData[] _newsConsequencesData;

        private String[] _biasesNames;
        private String[] _headlinesText;
        private String[] _biasesDescription;
        private String[] _biasesContents;

        private Vector2 _destination;
        private Vector2 _origin;
        
        [SerializeField]private int _folderOrderIndex;
        [SerializeField]private int _chosenBiasIndex;
        [SerializeField]private int _selectedBiasIndex;
        
        [SerializeField]private bool _inFront;
        [SerializeField]private bool _transferDrag;

        void Start()
        {
            _headlineText.text = _headlinesText[0];
            _contentText.text = _biasesContents[0];

            gameObject.GetComponent<Image>().color =
                new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        }

        public void SimulateBeginDrag(BaseEventData data)
        {
            BeginDrag(data);
        }

        protected override void BeginDrag(BaseEventData data)
        {
            if (!draggable)
            {
                return;
            }
            
            base.BeginDrag(data);
            EventsManager.OnCrossMidPointWhileScrolling += GetGameObjectToTransferDrag;
            EventsManager.OnCheckDistanceToMouse += DistanceToPosition;
            EventsManager.OnPrepareNewsHeadlineActions(this);
            _newsFolder.TurnOff();
            _newsFolder.SetDragging(true);
        }

        protected override void Drag(BaseEventData data)
        {
            if (!draggable || _transferDrag)
            {
                return;
            }
            
            PointerEventData pointerData = (PointerEventData)data;

            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(pointerData.position);
            
            if (mousePosition.x > _midPoint)
            {
                CrossMidPoint(pointerData, mousePosition);
                
                return;
            }
            
            base.Drag(data);
        }

        private void CrossMidPoint(PointerEventData pointerData, Vector2 mousePosition)
        {
            if (_chosenBiasIndex != _selectedBiasIndex)
            {
                return;
            }

            if (!_newsHeadlinePieceToTransferDrag.GetTransferDrag())
            {
                _transferDrag = true;
            }
            else
            {
                _newsHeadlinePieceToTransferDrag.SetTransferDrag(false);
            }
            EndDrag(pointerData);

            Transform parentTransform = _gameObjectToTransferDrag.transform.parent;
            parentTransform.position = mousePosition;
            parentTransform.gameObject.SetActive(true);
            _gameObjectToTransferDrag.GetComponent<NewsHeadlineSubPiece>().SimulateBeginDrag(pointerData);
                
            pointerData.pointerDrag = _gameObjectToTransferDrag;
                
                
            gameObject.SetActive(false);
        }

        public void SimulateEndDrag(BaseEventData data)
        {
            StateOnSendToLayout();
            _newsFolder.SendNewsHeadlineToLayout();
            EndDrag(data);
        }

        protected override void EndDrag(BaseEventData data)
        {
            if (_newsHeadlinePieceToTransferDrag.GetTransferDrag())
            {
                EventsManager.OnReturnNewsHeadlineToFolder(this);
            }
            
            if (!draggable)
            {
                return;
            }
            
            base.EndDrag(data);
            
            EventsManager.OnCrossMidPointWhileScrolling -= GetGameObjectToTransferDrag;
            EventsManager.OnCheckDistanceToMouse -= DistanceToPosition;
            
            if (!_transferDrag)
            {
                EventsManager.OnStartEndDrag(false);    
            }

            _newsFolder.SetDragging(false);
            
            if (EventsManager.OnDropNewsHeadline == null)
            {
                if (!_transferDrag && !_newsHeadlinePieceToTransferDrag.GetTransferDrag())
                {
                    DropOnFolder();    
                }
                _newsHeadlinePieceToTransferDrag.SetTransferDrag(false);
                return;
            }
            
            PointerEventData pointerData = (PointerEventData) data;
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
            
            _newsFolder.ReorderNewsHeadline(_folderOrderIndex, _chosenBiasIndex, _biasesNames, _biasesDescription);
            
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

        private void StateOnSendToLayout()
        {
            _inFront = false;
            UnsubscribeEvents();
        }

        private void ModifyFlags(bool value)
        {
            _inFront = value;
            draggable = value;
            clickable = !value;
            hoverable = !value;
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
            UnsubscribeEvents();
        }

        private void UnsubscribeEvents()
        {
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
            _headlineText.text = _headlinesText[_chosenBiasIndex];
            _contentText.text = _biasesContents[_chosenBiasIndex];
            
            EventsManager.OnChangeFolderOrderIndexWhenGoingToFolder += GiveDestinationToReturnToFolder;
            
            _newsFolder.ProcedureOnChangeBias();
            
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

            ReturnToFolder();
        }

        public void AddToFolder()
        {
            EventsManager.OnChangeFolderOrderIndexWhenGoingToFolder += GiveDestinationToReturnToFolder;
            ReturnToFolder();
        }

        private void ReturnToFolder()
        {
            int countOfTotalNewsHeadline = _newsFolder.GetNewsHeadlinesLength();

            if (countOfTotalNewsHeadline > 1)
            {
                countOfTotalNewsHeadline--;
            }
            
            _destination = new Vector2(0, _newsFolder.GiveNewFolderYCoordinate(_folderOrderIndex, countOfTotalNewsHeadline));
            
            StartCoroutine(SendToFolderAgain());
        }

        private IEnumerator SendToFolderAgain()
        {
            transform.SetAsFirstSibling();

            float timer = 0;

            Vector2 origin = transform.localPosition;

            while (timer < 1)
            {
                timer = MoveToDestination(origin, _destination, timer);
                yield return null;
            }

            EventsManager.OnChangeFolderOrderIndexWhenGoingToFolder -= GiveDestinationToReturnToFolder; 

            _origin = _destination;

            if (!_inFront)
            {
                SetInFront(_folderOrderIndex == 0);    
            }

            _newsFolder.SubtractOneToSentNewsHeadline();
        }

        public void DropOnFolder()
        {
            //TODO BUG ON RETURN NEWSHEADLINEPIECE TO LAYOUT 
            _newsFolder.TurnOn();    
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

        public void SetNewsType(NewsType newsType)
        {
            _newsType = newsType;
        }

        public NewsType GetNewsType()
        {
            return _newsType;
        }

        public void SetImagePath(String imagePath)
        {
            _imagePath = imagePath;
        }

        public void SetNewsConsequencesData(NewsConsequenceData[] newsConsequencesData)
        {
            _newsConsequencesData = newsConsequencesData;
        }

        public void SetBiasNames(String[] biasesNames)
        {
            _biasesNames = biasesNames;
        }

        public String[] GetBiasesNames()
        {
            return _biasesNames;
        }

        public void SetBiasesDescription(String[] biasesDescription)
        {
            _biasesDescription = biasesDescription;
        }

        public String[] GetBiasesDescription()
        {
            return _biasesDescription;
        }

        public void SetHeadlinesText(String[] headlinesText)
        {
            _headlinesText = headlinesText;
        }

        public void SetBiasContent(String[] biasesContents)
        {
            _biasesContents = biasesContents;
        }
        
        public void SetGameObjectToTransferDrag(GameObject gameObjectToTransferDrag)
        {
            _gameObjectToTransferDrag = gameObjectToTransferDrag;
        }

        public void SetNewsHeadlineSubPieceToTransferDrag(NewsHeadlinePiece newsHeadlinePieceToTransferDrag)
        {
            _newsHeadlinePieceToTransferDrag = newsHeadlinePieceToTransferDrag;
        }

        private GameObject GetGameObjectToTransferDrag(PointerEventData pointerData)
        {
            Drag(pointerData);
            
            //TODO BUG ON TRANSFER WITH SCROLL
            
            return !_transferDrag ? gameObject : _newsHeadlinePieceToTransferDrag.gameObject;
        }

        public void SetTransferDrag(bool transferDrag)
        {
            _transferDrag = transferDrag;
        }

        public bool GetTransferDrag()
        {
            return _transferDrag;
        }
        
        private Vector2 DistanceToPosition(Vector2 position)
        {
            return (Vector2)transform.position - position;
        }
    }
}
