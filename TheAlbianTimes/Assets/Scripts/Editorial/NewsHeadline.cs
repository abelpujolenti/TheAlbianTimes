using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using Utility;

namespace Editorial
{
    public class NewsHeadline : MovableRectTransform
    {
        private const float SPEED_MOVEMENT = 5;
        
        [SerializeField] private NewsFolder _newsFolder;

        [SerializeField] private int _folderOrderIndex;

        [SerializeField] private float _yDistanceToMoveOnHover;

        private Vector2 _destination;
        private Vector2 _origin;

        private Coroutine _moveCoroutine;

        private int _siblingsCount;
        
        private bool _inFront;
        // Start is called before the first frame update
        new void Start()
        {
            base.Start();
            
            _origin = transform.localPosition;

            int _siblingIndex = transform.GetSiblingIndex();
            
            _siblingsCount = transform.parent.childCount;
            
            _folderOrderIndex = (_siblingsCount - 1) - transform.GetSiblingIndex();

            _inFront = _siblingIndex == _siblingsCount - 1;

        }

        protected override void PointerEnter(BaseEventData data)
        {
            if (_inFront)
            {
                return;
            }

            _destination = transform.localPosition + new Vector3(0, _yDistanceToMoveOnHover, 0);

            if (_moveCoroutine != null)
            {
                StopCoroutine(_moveCoroutine);
                _moveCoroutine = StartCoroutine(MoveOnHover(_origin, _destination));
                return;
            }
            _moveCoroutine = StartCoroutine(MoveOnHover(_origin, _destination));
        }

        protected override void PointerExit(BaseEventData data)
        {
            if (_inFront)
            {
                return;
            }

            if (_moveCoroutine != null)
            {
                StopCoroutine(_moveCoroutine);
                _moveCoroutine = StartCoroutine(MoveOnHover(transform.localPosition, _origin));
                return;
            }
            _moveCoroutine = StartCoroutine(MoveOnHover(transform.localPosition, _origin));
        }

        protected override void PointerClick(BaseEventData data)
        {
            if (_inFront)
            {
                return;
            }
            
            _newsFolder.SwitchInFrontNewsHeadline(_folderOrderIndex);
            _inFront = true;
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

        private IEnumerator MoveOnHover(Vector2 origin, Vector2 destination)
        {
            float timer = 0;

            while (timer < 1)
            {
                timer += Time.deltaTime * SPEED_MOVEMENT;
                transform.localPosition = Vector3.Lerp(origin, destination, timer);
                yield return null;
            }
        }
    }
}
