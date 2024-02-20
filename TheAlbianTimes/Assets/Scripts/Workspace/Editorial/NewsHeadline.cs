using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Managers;
using NoMonoBehavior;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utility;
using Workspace.Layout;
using Workspace.Layout.Pieces;
using Random = UnityEngine.Random;

namespace Workspace.Editorial
{
    public class NewsHeadline : ThrowableInteractableRectTransform
    {
        private const String DROP_PAPER_ON_TABLE_SOUND = "Drop Paper On Table";
        private const String DROP_PAPER_SOUND_IN_FOLDER = "Drop Paper In Folder";
        private const String GRAB_PAPER_SOUND = "Grab Paper";
        private const String THUD_SOUND = "Thud";
        private const String SUBMIT_PAPER_SOUND = "Submit Paper";
        
        private const float CHANGE_CONTENT_Y_COORDINATE = 1000;
        private const float SPEED_MOVEMENT = 15;
        private const float TIME_TO_SLIDE = 2f;
        private const float Y_DISTANCE_TO_MOVE_ON_HOVER = 10f;
        private const float SECONDS_AWAITING_TO_RETURN_TO_FOLDER = 1.5f;
        private const float PAPER_BRIGHTNESS = .9f;
        private const float PAPER_BRIGHTNESS_BASE_DECREASE = .14f;

        private readonly float _midPoint = MIN_X_POSITION_CAMERA + (MAX_X_POSITION_CAMERA - MIN_X_POSITION_CAMERA) / 2;

        [SerializeField] private NewsHeadlinePiece _newsHeadlinePieceToTransferDrag;

        [SerializeField] private TextMeshProUGUI _headlineText;
        [SerializeField] private TextMeshProUGUI _contentText;
        [SerializeField] private TextMeshProUGUI _articleTagText;
        
        [SerializeField] private GameObject _biasMarker;
        [SerializeField] private GameObject _extraLabel;
        
        [SerializeField] private Transform _markerInkPrefab;

        [SerializeField] private NewsType _newsType;

        private Coroutine _moveCoroutine;
        private Coroutine _spawnBiasMarkCoroutine;

        private GameObject _gameObjectToTransferDrag;

        private NewsFolder _newsFolder;
        
        private Image _background;

        private NewsData _data;

        private NewsConsequenceData[] _newsConsequencesData;

        private string[] _biasesNames;
        private string[] _headlinesText;
        private string[] _biasesDescription;
        private string[] _biasesContents;

        private string _imagePath;

        private Vector2 _destination;
        private Vector2 _origin;

        private int _linkId;
        private int _totalBiasesToActivate;
        [SerializeField]private int _folderOrderIndex;
        [SerializeField]private int _chosenBiasIndex;
        [SerializeField]private int _selectedBiasIndex;
        
        private bool _inFront;
        private bool _transferDrag;
        private bool _sendToChange;
        private bool _modified;
        private bool _onFolder = true;

        private AudioSource _audioSourceDropPaperOnTable;
        private AudioSource _audioSourceDropPaperInFolder;
        private AudioSource _audioSourceGrabPaper;
        private AudioSource _audioSourceThud;
        private AudioSource _audioSourceSubmitPaper;

        void Start()
        {
            _audioSourceDropPaperOnTable = gameObject.AddComponent<AudioSource>();
            _audioSourceDropPaperInFolder = gameObject.AddComponent<AudioSource>();
            _audioSourceGrabPaper = gameObject.AddComponent<AudioSource>();
            _audioSourceThud = gameObject.AddComponent<AudioSource>();
            _audioSourceSubmitPaper = gameObject.AddComponent<AudioSource>();
            (AudioSource, String)[] tuples =
            {
                (_audioSourceDropPaperOnTable, DROP_PAPER_ON_TABLE_SOUND),
                (_audioSourceDropPaperInFolder, DROP_PAPER_SOUND_IN_FOLDER),
                (_audioSourceGrabPaper, GRAB_PAPER_SOUND),
                (_audioSourceThud, THUD_SOUND),
                (_audioSourceSubmitPaper, SUBMIT_PAPER_SOUND)
            };
            SoundManager.Instance.SetMultipleAudioSourcesComponents(tuples);
            
            UpdateText(_headlinesText[0], _biasesContents[0]);
            _articleTagText.text = PieceData.newsTypeName[(int)_newsType];

            Color color = PieceData.newsTypeColors[(int)_newsType];
            _articleTagText.GetComponentInParent<Image>().color = ColorUtil.SetSaturationMultiplicative(color, 0.5f);
            _background = gameObject.GetComponent<Image>();
            _background.color = ColorUtil.SetSaturationMultiplicative(color, 0.15f);
            UpdateShading(transform.parent.childCount - 1 - transform.GetSiblingIndex());
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

            if (!_newsHeadlinePieceToTransferDrag.GetTransferDrag())
            {
                EditorialManager.Instance.TurnOffBiasContainer();    
            }
            
            SlideToRotation(0f, 0.1f);
            
            _newsFolder.SetDragging(true);

            _audioSourceGrabPaper.Play();
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
            _rotate = false;
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
            StateOnDropOutOfFolder();
        }

        protected override void EndDrag(BaseEventData data)
        {
            if (_newsHeadlinePieceToTransferDrag.GetTransferDrag())
            {
                _rotate = true;
                _newsHeadlinePieceToTransferDrag.SetTransferDrag(false);
            }

            if (!draggable)
            {
                return;
            }
            
            _newsFolder.SetDragging(false);
            _newsFolder.CheckCurrentNewsHeadlinesSent();
            
            EventsManager.OnCrossMidPointWhileScrolling -= GetGameObjectToTransferDrag;
            EventsManager.OnCheckDistanceToMouse -= DistanceToPosition;
            EventsManager.OnStartEndDrag(false);    
            
            PointerEventData pointerData = (PointerEventData) data;
            if (EventsManager.OnDropNewsHeadline == null)
            {
                DropNewsHeadline(pointerData.position);
                base.EndDrag(data);
                return;
            }
            EventsManager.OnDropNewsHeadline(this, pointerData.position);

            _audioSourceSubmitPaper.Play();
            base.EndDrag(data);
        }

        public void DropNewsHeadline(Vector2 position)
        {
            if (_onFolder)
            {
                if (_newsFolder.IsCoordinateInsideBounds(position))
                {
                    StartCoroutine(Slide(transform.localPosition, _origin));
                    _audioSourceDropPaperInFolder.Play();
                }
                else
                {
                    DropOutFolder();   
                    _audioSourceDropPaperOnTable.Play();
                }
            }
            else if (_newsFolder.IsCoordinateInsideBounds(position))
            {
                DropOnFolder(false);
            }
        }

        private void DropOnFolder(bool allAtOnce)
        {
            if (!gameObject.activeSelf)
            {
                return;
            }
            _audioSourceDropPaperInFolder.Play();
            EventsManager.OnPressPanicButton -= DropOnFolder;
            _newsFolder.AddNewsHeadlineComponent(this, allAtOnce);
        }

        private void DropOutFolder()
        {
            EventsManager.OnPressPanicButton += DropOnFolder;
            _onFolder = false;
            StateOnDropOutOfFolder();
            _newsFolder.DropNewsHeadlineOutOfFolder(false);
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

        protected override void PointerExit(BaseEventData data)
        {
            if (!hoverable)
            {
                return;
            }
            SlideInFolder(transform.localPosition, _origin);
        }

        public void SlideInFolder(Vector2 origin, Vector2 destination)
        {
            if (_moveCoroutine != null)
            {
                StopCoroutine(_moveCoroutine);
            }
            
            _moveCoroutine = StartCoroutine(Slide(origin, destination));
        }

        protected override void PointerClick(BaseEventData data)
        {
            if (!clickable)
            {
                return;
            }
            
            _newsFolder.ReorderNewsHeadline(_folderOrderIndex, _selectedBiasIndex, _biasesNames, _biasesDescription, _totalBiasesToActivate);
            
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
                return;
            }
            UnsubscribeEvents();
        }

        private void UnsubscribeEvents()
        {
            EventsManager.OnChangeNewsHeadlineContent -= ChangeContent;
            EventsManager.OnChangeSelectedBiasIndex -= SetSelectedBiasIndex;
        }

        public void UpdateShading(int index)
        {
            _background.color = ColorUtil.SetBrightness(_background.color, Mathf.Max(.2f, PAPER_BRIGHTNESS - index * PAPER_BRIGHTNESS_BASE_DECREASE));
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
            _rotate = false;
            float timer = 0;

            while (timer < TIME_TO_SLIDE)
            {
                timer = MoveToDestination(origin, destination, timer);
                yield return null;
            }
            _rotate = true;
        }

        private void ChangeContent()
        {
            _chosenBiasIndex = _selectedBiasIndex;
            _data.currentBias = _selectedBiasIndex;
            UpdateText(_headlinesText[_chosenBiasIndex], _biasesContents[_chosenBiasIndex]);
            
            _newsFolder.DropNewsHeadlineOutOfFolder(true);

            ClearBiasMarks();

            Vector2 destination = new Vector2(0, CHANGE_CONTENT_Y_COORDINATE);
            
            StartCoroutine(SendToChangeContent(destination));
            
            _origin = destination;
        }

        private void UpdateText(string headline, string text)
        {
            foreach (string s in Country.names)
            {
                string color = "#701f1f";
                headline = Regex.Replace(headline, "\\b" + s + "\\b", "<color=" + color + ">" + s + "</color>");
                headline = Regex.Replace(headline, "\\b" + s.ToLower() + "\\b", "<color=" + color + ">" + s.ToLower() + "</color>");
                text = Regex.Replace(text, "\\b" + s + "\\b", "<font=courbd SDF><color=" + color + ">" + s + " <font=cour SDF></color>");
                text = Regex.Replace(text, "\\b" + s.ToLower() + "\\b", "<font=courbd SDF><color=" + color + ">" + s.ToLower() + " <font=cour SDF></color>");
            }

            Dictionary<string, Color> keyWords = new Dictionary<string, Color>();
            keyWords.Add("Moon", new Color(0.3f, 0.3f, 0.1f));

            foreach (KeyValuePair<string, Color> v in keyWords)
            {
                string s = v.Key;
                string color = "#" + v.Value.ToHexString();
                headline = Regex.Replace(headline, "\\b" + s + "\\b", "<color=" + color + ">" + s + "</color>");
                headline = Regex.Replace(headline, "\\b" + s.ToLower() + "\\b", "<color=" + color + ">" + s.ToLower() + "</color>");
                text = Regex.Replace(text, "\\b" + s + "\\b", "<font=courbd SDF><color=" + color + ">" + s + " <font=cour SDF></color>");
                text = Regex.Replace(text, "\\b" + s.ToLower() + "\\b", "<font=courbd SDF><color=" + color + ">" + s.ToLower() + " <font=cour SDF></color>");
            }

            _headlineText.text = headline;
            _contentText.text = text;
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

            PrepareToAddToFolder();
        }

        public void PrepareToAddToFolder()
        {
            _inFront = false;
            draggable = false;
            hoverable = false;
            clickable = false;
            
            transform.rotation = Quaternion.identity;
            
            int countOfTotalNewsHeadline = _newsFolder.GetNewsHeadlinesLength();

            if (countOfTotalNewsHeadline > 1)
            {
                countOfTotalNewsHeadline--;
            }
            
            _destination = new Vector2(0, _newsFolder.GiveNewFolderYCoordinate(_folderOrderIndex, countOfTotalNewsHeadline));
            
            EventsManager.OnChangeFolderOrderIndexWhenGoingToFolder += GiveDestinationToReturnToFolder;
            
            StartCoroutine(SendToFolderAgain());
        }

        private IEnumerator SendToFolderAgain()
        {
            transform.SetAsFirstSibling();

            float timer = 0;

            Vector2 origin = transform.localPosition;

            if (_modified)
            {
                _audioSourceDropPaperInFolder.Play();
                _audioSourceThud.Play();    
            }

            while (timer < TIME_TO_SLIDE)
            {
                timer = MoveToDestination(origin, _destination, timer);
                yield return null;
            }

            EventsManager.OnChangeFolderOrderIndexWhenGoingToFolder -= GiveDestinationToReturnToFolder; 

            _origin = _destination;

            _newsFolder.ReturnNewsHeadline(this, _folderOrderIndex, _onFolder);
            
            _rotate = true;
            _onFolder = true;
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

        public void SpawnBiasMark(int biasIndex, Vector3 position)
        {
            if (_biasMarker == null) return;

            float positionOffset = Random.Range(1f, 2.7f);
            position += new Vector3(positionOffset, 0f, 0f);

            Quaternion rotation = Quaternion.Euler(0f, 0f, Random.Range(-6f, 6f));
            Image markerInk = Instantiate(_markerInkPrefab, position, rotation, _biasMarker.transform).GetComponent<Image>();
            markerInk.color = PieceData.biasColors[biasIndex];

            float widthMultiplier = Random.Range(1.2f, 3f);
            markerInk.transform.localScale =  new Vector3(markerInk.transform.localScale.x * widthMultiplier, markerInk.transform.localScale.y, markerInk.transform.localScale.z);
            float fillTime = widthMultiplier * .09f;

            if (_spawnBiasMarkCoroutine != null) StopCoroutine(_spawnBiasMarkCoroutine);
            float startT = positionOffset * .1f;
            _spawnBiasMarkCoroutine = StartCoroutine(SpawnBiasMarkCoroutine(markerInk, fillTime, startT));
        }

        private IEnumerator SpawnBiasMarkCoroutine(Image markerInkImage, float t, float startT)
        {
            yield return new WaitForSeconds(startT);
            float elapsedT = 0f;
            while (elapsedT <= t)
            {
                markerInkImage.fillAmount = elapsedT / t;
                yield return new WaitForFixedUpdate();
                elapsedT += Time.fixedDeltaTime;
            }
            markerInkImage.fillAmount = 1f;
        }

        public void ClearBiasMarks()
        {
            if (_biasMarker == null) return;
            if (_spawnBiasMarkCoroutine != null) StopCoroutine(_spawnBiasMarkCoroutine);
            for (int i = 0; i < _biasMarker.transform.childCount; i++)
            {
                Destroy(_biasMarker.transform.GetChild(i).gameObject);
            }
        }

        public void SetNewsFolder(NewsFolder newsFolder)
        {
            _newsFolder = newsFolder;
        }

        public void SetLinkId(int linkId)
        {
            _linkId = linkId;
            _totalBiasesToActivate = _biasesNames.Length;
            if (_linkId == 0)
            {
                return;
            }

            _totalBiasesToActivate--;
            EventsManager.OnLinkFouthBiasWithNewsHeadline += LinkFourthBias;
        }

        private void LinkFourthBias(int linkId)
        {
            if (_linkId != linkId)
            {
                return;
            }

            _extraLabel.SetActive(true);
            _totalBiasesToActivate++;
            EventsManager.OnLinkFouthBiasWithNewsHeadline -= LinkFourthBias;
        }

        public int GetTotalBiasesToActivate()
        {
            return _totalBiasesToActivate;
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

        public void SetImagePath(String imagePath)
        {
            _imagePath = imagePath;
        }

        public void SetNewsConsequencesData(NewsConsequenceData[] newsConsequencesData)
        {
            _newsConsequencesData = newsConsequencesData;
        }

        public void SetBiasNames(string[] biasesNames)
        {
            _biasesNames = biasesNames;
        }

        public string[] GetBiasesNames()
        {
            return _biasesNames;
        }

        public void SetBiasesDescription(string[] biasesDescription)
        {
            _biasesDescription = biasesDescription;
        }

        public string[] GetBiasesDescription()
        {
            return _biasesDescription;
        }

        public void SetHeadlinesText(string[] headlinesText)
        {
            _headlinesText = headlinesText;
        }

        public void SetBiasContent(string[] biasesContents)
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

        public bool WasOnFolder()
        {
            return _onFolder;
        }
        
        private Vector2 DistanceToPosition(Vector2 position)
        {
            return (Vector2)transform.position - position;
        }
    }
}
