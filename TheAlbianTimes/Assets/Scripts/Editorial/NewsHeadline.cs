using System;
using System.Collections;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utility;
using Random = UnityEngine.Random;

namespace Editorial
{
    public class NewsHeadline : MovableRectTransform
    {
        private const float CHANGE_CONTENT_Y_COODINATE = 1000;
        private const float SPEED_MOVEMENT = 10;
        private const float Y_DISTANCE_TO_MOVE_ON_HOVER = 10f;
        private const float SECONDS_AWAITING_TO_RETURN_TO_FOLDER = 3;

        private Coroutine _moveCoroutine;
        
        private NewsFolder _newsFolder;

        [SerializeField] private TextMeshProUGUI _textMeshPro; 
        
        private String[] _shortBiasDescription;
        private String[] _biasContent;

        private Vector2 _destination;
        private Vector2 _origin;
        
        [SerializeField]private int _folderOrderIndex;
        [SerializeField]private int _chosenBiasIndex;
        
        [SerializeField]private bool _inFront;
        
        new void Start()
        {
            base.Start();

            _textMeshPro.text = _biasContent[0];

            gameObject.GetComponent<Image>().color =
                new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        }

        protected override void PointerEnter(BaseEventData data)
        {
            if (!hoverable)
            {
                return;
            }
            
            if (_inFront)
            {
                return;
            }

            _destination = _origin + new Vector2(0, Y_DISTANCE_TO_MOVE_ON_HOVER);

            if (_moveCoroutine != null)
            {
                StopCoroutine(_moveCoroutine);
            }
            _moveCoroutine = StartCoroutine(Slide(_origin, _destination));
        }

        protected override void PointerExit(BaseEventData data)
        {
            if (!hoverable)
            {
                return;
            }
            
            if (_inFront)
            {
                return;
            }

            if (_moveCoroutine != null)
            {
                StopCoroutine(_moveCoroutine);
            }
            _moveCoroutine = StartCoroutine(Slide(transform.localPosition, _origin));
        }

        protected override void PointerClick(BaseEventData data)
        {
            if (!clickable)
            {
                return;
            }
            
            if (_inFront)
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
            _inFront = isInFront;
        }

        public void SetOrigin(Vector2 newOrigin)
        {
            _origin = newOrigin;
            transform.localPosition = _origin;
        }

        public Vector2 GetOrigin()
        {
            return _origin;
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

        private float MoveToDestination(Vector2 origin, Vector2 destination, float timer)
        {
            timer += Time.deltaTime * SPEED_MOVEMENT;
            transform.localPosition = Vector3.Lerp(origin, destination, timer);
            
            return timer;
        }

        private IEnumerator SendToChangeContent(Vector2 origin, Vector2 destination)
        {
            float timer = 0;

            while (timer < 1)
            {
                timer = MoveToDestination(origin, destination, timer);
                yield return null;
            }

            ReturnToFolder();
        }

        public void ChangeContent(int newChosenBiasIndex)
        {
            ModifyFlags(false);
            _chosenBiasIndex = newChosenBiasIndex;
            _textMeshPro.text = _biasContent[_chosenBiasIndex];
            ActionsManager.OnChangeNewsHeadlineContent -= ChangeContent;
            ActionsManager.OnChangeFolderOrderIndexWhenGoingToFolder += GiveDestinationToReturnToFolder;
            _newsFolder.ProcedureWhenSendNewsHeadlineToRewrite();
            Vector2 destination = new Vector2(0, CHANGE_CONTENT_Y_COODINATE);
            StartCoroutine(SendToChangeContent(_origin, destination));
            _origin = destination;
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

        private void ReturnToFolder()
        {
            int countOfTotalNewsHeadline = _newsFolder.GetNewsHeadlinesLength();

            if (countOfTotalNewsHeadline > 1)
            {
                countOfTotalNewsHeadline--;
            }
            
            _destination = new Vector2(0, _newsFolder.GiveNewFolderYCoordinate(_folderOrderIndex, countOfTotalNewsHeadline));
            
            StartCoroutine(SendToFolderAgain(_origin));
        }

        private IEnumerator SendToFolderAgain(Vector2 origin)
        {
            transform.SetAsFirstSibling();
            
            yield return new WaitForSeconds(SECONDS_AWAITING_TO_RETURN_TO_FOLDER);

            float timer = 0;

            while (timer < 1)
            {
                timer = MoveToDestination(origin, _destination, timer);
                yield return null;
            }

            ActionsManager.OnChangeFolderOrderIndexWhenGoingToFolder -= GiveDestinationToReturnToFolder; 

            _origin = _destination;

            ModifyFlags(true);
            
            _newsFolder.SubtractOneToSentNewsHeadline();
        }

        public void SetNewsFolder(NewsFolder newsFolder)
        {
            _newsFolder = newsFolder;
        }

        public int GetChosenBiasIndex()
        {
            return _chosenBiasIndex;
        }

        private void ModifyFlags(bool value)
        {
            clickable = value;
            hoverable = value;
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
    }
}
