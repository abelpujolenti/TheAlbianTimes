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

namespace Editorial
{
    public class NewsHeadline : ThrowableInteractableRectTransform
    {
        private const float CHANGE_CONTENT_Y_COORDINATE = 1000;
        private const float SPEED_MOVEMENT = 15;
        private const float TIME_TO_SLIDE = 2f;
        private const float Y_DISTANCE_TO_MOVE_ON_HOVER = 10f;
        private const float SECONDS_AWAITING_TO_RETURN_TO_FOLDER = 1.5f;

        private readonly float _midPoint = MIN_X_POSITION_CAMERA + (MAX_X_POSITION_CAMERA - MIN_X_POSITION_CAMERA) / 2;

        private Coroutine _moveCoroutine;

        private GameObject _gameObjectToTransferDrag;

        [SerializeField] private NewsHeadlinePiece _newsHeadlinePieceToTransferDrag;

        [SerializeField] private TextMeshProUGUI _headlineText;
        [SerializeField] private TextMeshProUGUI _contentText;
        [SerializeField] private TextMeshProUGUI _articleTagText;
        [SerializeField] private GameObject _biasMarker;

        private NewsFolder _newsFolder;

        [SerializeField] private NewsType _newsType;

        private String _imagePath;

        private NewsData _data;

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
        
        private bool _inFront;
        private bool _transferDrag;
        private bool _sendToLayout;
        private bool _modified;
        private bool _onFolder = true;

        void Start()
        {
            _headlineText.text = _headlinesText[0];
            _contentText.text = _biasesContents[0];
            _articleTagText.text = PieceData.newsTypeName[(int)_newsType];

            Color color = PieceData.newsTypeColors[(int)_newsType];
            _articleTagText.GetComponentInParent<Image>().color = ColorUtil.SetSaturationMultiplicative(color, 0.5f);
            gameObject.GetComponent<Image>().color = ColorUtil.SetSaturationMultiplicative(color, 0.15f);
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

            if (!_newsHeadlinePieceToTransferDrag.GetTransferDrag())
            {
                EditorialManager.Instance.TurnOffBiasContainer();    
            }
            
            SlideToRotation(0f, 0f);
            
            _newsFolder.SetDragging(true);

            SoundManager.Instance.GrabPaperSound();
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
                gameObjectToDrag.transform.position = new Vector2(_midPoint, mousePosition.y + _vectorOffset.y);
                return;
            }
            
            _transferDrag = true;
            _newsHeadlinePieceToTransferDrag.SetTransferDrag(false);
            
            gameObject.SetActive(false);
            
            _newsFolder.SetDragging(false);
            
            EventsManager.OnCrossMidPointWhileScrolling -= GetGameObjectToTransferDrag;
            EventsManager.OnCheckDistanceToMouse -= DistanceToPosition;

            Transform parentTransform = _gameObjectToTransferDrag.transform.parent;
            parentTransform.position = mousePosition;
            parentTransform.gameObject.SetActive(true);
            _gameObjectToTransferDrag.GetComponent<NewsHeadlineSubPiece>().SimulateBeginDrag(pointerData);
                
            pointerData.pointerDrag = _gameObjectToTransferDrag;
        }

        public void SimulateEndDrag(BaseEventData data)
        {
            EndDrag(data);
            
            if (!_sendToLayout)
            {
                StateOnDropOutOfFolder();
            
                _newsFolder.SendNewsHeadlineToLayout();
            }

            if (!_newsFolder.IsFolderEmpty())
            {
                EditorialManager.Instance.TurnOnBiasContainer();
            }
        }

        protected override void EndDrag(BaseEventData data)
        {
            if (_newsHeadlinePieceToTransferDrag.GetTransferDrag())
            {
                _newsHeadlinePieceToTransferDrag.SetTransferDrag(false);
                
                if (_sendToLayout)
                {
                    _newsFolder.AddNewsHeadlineComponent(this, false);
                    _sendToLayout = false;
                }
            }
            
            ////
            if (!draggable)
            {
                return;
            }
            ////

            if (_onFolder)
            {
                DropOutFolder();
            }            
            else
            {
                DropOnFolder();
            }
            
            base.EndDrag(data);
            
            _newsFolder.SetDragging(false);
            _newsFolder.CheckCurrentNewsHeadlinesSent();
            
            EventsManager.OnCrossMidPointWhileScrolling -= GetGameObjectToTransferDrag;
            EventsManager.OnCheckDistanceToMouse -= DistanceToPosition;
            
            if (!gameObject.activeSelf)
            {
                return;
            }
            
            EventsManager.OnStartEndDrag(false);    
            
            if (EventsManager.OnDropNewsHeadline == null)
            {
                SoundManager.Instance.DropPaperSound();
                return;
            }
            
            PointerEventData pointerData = (PointerEventData) data;
            EventsManager.OnDropNewsHeadline(this, pointerData.position);

            SoundManager.Instance.SubmitPaperSound();
        }

        private void DropOutFolder()
        {
            //EventsManager.OnPressPanicButton +=
            _onFolder = false;
            _newsFolder.DropNewsHeadlineOutOfFolder(false);
            StateOnDropOutOfFolder();
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
            
            _newsFolder.ReorderNewsHeadline(_folderOrderIndex, _selectedBiasIndex, _biasesNames, _biasesDescription);
            
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

        private void StateOnDropOutOfFolder()
        {
            _inFront = false;
            draggable = true;
            clickable = false;
            hoverable = false;
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
                EventsManager.OnChangeSelectedBiasIndex += EnableBiasMarks;
                return;
            }
            UnsubscribeEvents();
        }

        private void UnsubscribeEvents()
        {
            EventsManager.OnChangeNewsHeadlineContent -= ChangeContent;
            EventsManager.OnChangeSelectedBiasIndex -= SetSelectedBiasIndex;
            EventsManager.OnChangeSelectedBiasIndex -= EnableBiasMarks;
        }

        public void SetOrigin(Vector2 newOrigin)
        {
            _origin = newOrigin;
        }

        private float MoveToDestination(Vector2 origin, Vector2 destination, float timer)
        {
            timer += Time.deltaTime * SPEED_MOVEMENT;
            transform.localPosition = Vector3.Lerp(origin, destination, timer / TIME_TO_SLIDE);
            
            return timer;
        }

        private IEnumerator Slide(Vector2 origin, Vector2 destination)
        {
            float timer = 0;

            while (timer < TIME_TO_SLIDE)
            {
                timer = MoveToDestination(origin, destination, timer);
                yield return null;
            }
        }

        private void ChangeContent()
        {
            _chosenBiasIndex = _selectedBiasIndex;
            _headlineText.text = _headlinesText[_chosenBiasIndex];
            _contentText.text = _biasesContents[_chosenBiasIndex];

            EventsManager.OnChangeFolderOrderIndexWhenGoingToFolder += GiveDestinationToReturnToFolder;
            
            _newsFolder.AddNewsHeadlinesSent();
            _newsFolder.DropNewsHeadlineOutOfFolder(true);

            DisableBiasMarks();
            
            Vector2 destination = new Vector2(0, CHANGE_CONTENT_Y_COORDINATE);
            
            StartCoroutine(SendToChangeContent(destination));
            
            _origin = destination;
        }
        
        private IEnumerator SendToChangeContent(Vector2 destination)
        {
            float timer = 0;

            _modified = true;

            while (timer < TIME_TO_SLIDE)
            {
                timer = MoveToDestination(_origin, destination, timer);
                yield return null;
            }

            yield return new WaitForSeconds(SECONDS_AWAITING_TO_RETURN_TO_FOLDER);

            ReturnToFolder();
        }

        public void AddToFolder()
        {
            SlideToRotation(0f, 0f);
            EventsManager.OnChangeFolderOrderIndexWhenGoingToFolder += GiveDestinationToReturnToFolder;
            ReturnToFolder();
        }

        private void ReturnToFolder()
        {
            _inFront = false;
            draggable = false;
            hoverable = false;
            clickable = false;
            
            SlideToRotation(0f, 0f);
            
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

            if (_modified)
            {
                SoundManager.Instance.ReturnPaperSound();    
            }

            while (timer < TIME_TO_SLIDE)
            {
                timer = MoveToDestination(origin, _destination, timer);
                yield return null;
            }

            EventsManager.OnChangeFolderOrderIndexWhenGoingToFolder -= GiveDestinationToReturnToFolder; 

            _origin = _destination;

            _newsFolder.ReturnNewsHeadline(this, _folderOrderIndex, !_onFolder);
            
            _rotate = true;
            _onFolder = true;
        }

        private void DropOnFolder()
        {
            _rotate = false;
            _newsFolder.AddNewsHeadlineComponent(this, false);
            EditorialManager.Instance.TurnOnBiasContainer();    
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

        private void EnableBiasMarks(int biasIndex)
        {
            if (_biasMarker == null) return;

            if (biasIndex == _chosenBiasIndex)
            {
                DisableBiasMarks();
                return;
            }

            _biasMarker.SetActive(true);
            foreach(Image image in _biasMarker.GetComponentsInChildren<Image>())
            {
                image.color = PieceData.biasColors[biasIndex];
            }
        }
        private void DisableBiasMarks()
        {
            if (_biasMarker == null)
            {
                return;
            }
            _biasMarker.SetActive(false);
        }

        public void SetNewsFolder(NewsFolder newsFolder)
        {
            _newsFolder = newsFolder;
        }

        public NewsFolder GetNewsFolder()
        {
            return _newsFolder;
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

        public void SetNewsData(NewsData data)
        {
            _data = data;
        }
        public NewsData GetNewsData()
        {
            return _data;
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

        public void SetSendToLayout(bool send)
        {
            _sendToLayout = send;
        }
        
        private Vector2 DistanceToPosition(Vector2 position)
        {
            return (Vector2)transform.position - position;
        }
    }
}
