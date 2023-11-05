using System.Collections;
using Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utility;
using Random = UnityEngine.Random;

namespace Editorial
{
    public class NewsHeadline : MovableRectTransform
    {
        private const float SPEED_MOVEMENT = 10;

        private const float Y_DISTANCE_TO_MOVE_ON_HOVER = 10f;

        private Coroutine _moveCoroutine;
        
        private NewsFolder _newsFolder;

        private Bias[] _bias;

        private Vector2 _destination;
        private Vector2 _origin;
        
        [SerializeField]private int _folderOrderIndex;
        private int _siblingsCount;
        [SerializeField]private int _chosenBiasIndex;
        
        [SerializeField]private bool _inFront;
        
        new void Start()
        {
            base.Start();

            gameObject.GetComponent<Image>().color =
                new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            
            _origin = transform.localPosition;

            int _siblingIndex = transform.GetSiblingIndex();
            
            _siblingsCount = transform.parent.childCount;
            
            _folderOrderIndex = (_siblingsCount - 1) - transform.GetSiblingIndex();

            if (_siblingIndex == _siblingsCount - 1)
            {
                _inFront = true;
                //ActionsManager.OnSendBias +=
            }
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
                _moveCoroutine = StartCoroutine(Slide(_origin, _destination));
                return;
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
                _moveCoroutine = StartCoroutine(Slide(transform.localPosition, _origin));
                return;
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

            _inFront = true;
            _newsFolder.SwitchInFrontNewsHeadline(_folderOrderIndex);
            Debug.Log(transform.localPosition);
            ActionsManager.OnChangeNewsHeadlineContent += ChangeContent;
            ActionsManager.OnChangeFrontNewsHeadline(_chosenBiasIndex);
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

        public int GetFolderOrderIndex()
        {
            return _folderOrderIndex;
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
            _inFront = false;
            ModifyFlags(false);
            _chosenBiasIndex = newChosenBiasIndex;
            ActionsManager.OnChangeNewsHeadlineContent -= ChangeContent;
            Vector2 destination = _newsFolder.SendNewsHeadlineToRewriteContent();
            StartCoroutine(SendToChangeContent(_origin, destination));
            _origin = destination;
        }

        private void ReturnToFolder()
        {
            Vector2 destination = new Vector2(0, _newsFolder.GetFolderMaxYCoordinate());
            StartCoroutine(SendToFolderAgain(_origin, destination));
        }

        private IEnumerator SendToFolderAgain(Vector2 origin, Vector2 destination)
        {
            transform.SetAsFirstSibling();
            
            float timer = 0;
            
            while (timer < 1)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            timer = 0;

            while (timer < 1)
            {
                timer = MoveToDestination(origin, destination, timer);
                yield return null;
            }

            _origin = destination;

            ModifyFlags(true);

            ActionsManager.OnAddNewsHeadlineToFolder(this);
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
    }
}
