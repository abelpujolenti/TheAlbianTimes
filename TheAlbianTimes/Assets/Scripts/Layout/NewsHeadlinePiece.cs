using System;
using System.Collections;
using Managers;
using NoMonoBehavior;
using UnityEngine;

namespace Layout
{
    public class NewsHeadlinePiece : MonoBehaviour
    {
        private const float TRANSPARENCY_VALUE = 0.9f;
        private const float FULL_OPACITY = 1;
        private const float SPEED_MOVEMENT = 10;

        [SerializeField] private NewsType _newsType;

        [SerializeField] private NewsHeadlineSubPiece[] _newsHeadlineSubPieces;

        private Cell[] _snappedCells;

        private Vector2[] _subPiecesPositionsRelativeToRoot;

        private Vector2 _containerMinCoordinates;
        private Vector2 _containerMaxCoordinates;

        private Vector3 _offset;
        private Vector2 _initialPosition;

        private int _newsHeadlineId;

        [SerializeField] private bool _transferDrag;

        private void Start()
        {
            _subPiecesPositionsRelativeToRoot = new Vector2[_newsHeadlineSubPieces.Length];

            _offset = new Vector3();

            for (int i = 0; i < _newsHeadlineSubPieces.Length; i++)
            {
                _offset += _newsHeadlineSubPieces[i].transform.position;
                _subPiecesPositionsRelativeToRoot[i] =
                    _newsHeadlineSubPieces[i].transform.position - transform.position;
            }

            _offset /= _newsHeadlineSubPieces.Length;

            _offset = transform.position - _offset;
        }

        public void BeginDrag()
        {
            transform.SetAsLastSibling();

            foreach (NewsHeadlineSubPiece newsHeadlineSubPiece in _newsHeadlineSubPieces)
            {
                newsHeadlineSubPiece.Fade(TRANSPARENCY_VALUE);
            }

            EventsManager.OnDropNewsHeadlinePiece += EndDrag;
            EventsManager.OnCheckDistanceToMouse += DistanceToPosition;

            if (_snappedCells == null)
            {
                return;
            }

            foreach (Cell cell in _snappedCells)
            {
                //Debug.Log("True " + cell.GetCoordinates());
                cell.SetFree(true);
            }
        }

        private void EndDrag(NewsHeadlineSubPiece draggedSubPiece, Vector2 mousePosition)
        {

            if (EventsManager.OnPreparingCells == null ||
                EventsManager.OnSuccessfulSnap == null)
            {
                return;
            }

            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

            foreach (NewsHeadlineSubPiece newsHeadlineSubPiece in _newsHeadlineSubPieces)
            {
                newsHeadlineSubPiece.Fade(FULL_OPACITY);
            }

            EventsManager.OnDropNewsHeadlinePiece -= EndDrag;
            EventsManager.OnCheckDistanceToMouse -= DistanceToPosition;

            if (!gameObject.activeSelf)
            {
                return;
            }

            _snappedCells = EventsManager.OnPreparingCells(draggedSubPiece, mousePosition, _newsHeadlineSubPieces);

            if (_snappedCells == null)
            {
                bool allSubPiecesInside = true;
                foreach (NewsHeadlineSubPiece newsHeadlineSubPiece in _newsHeadlineSubPieces)
                {
                    if (IsCoordinateInsideBounds(newsHeadlineSubPiece.transform.position))
                    {
                        continue;
                    }

                    allSubPiecesInside = false;
                    break;
                }

                if (allSubPiecesInside)
                {
                    _initialPosition = transform.position;
                }

                EventsManager.OnFailSnap(this);
                return;
            }

            transform.position = EventsManager.OnSuccessfulSnap(_snappedCells);
        }

        public void SlideFromOriginToDestination(Vector2 origin)
        {
            StartCoroutine(Slide(origin, _initialPosition));
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
            transform.position = Vector2.Lerp(origin, destination, timer);
            return timer;
        }

        private NewsHeadlineSubPiece[] GetNewsHeadlineNeighborPieces(Vector2 coordinates)
        {
            NewsHeadlineSubPiece[] newsHeadlinePieces = { };

            int index = 0;

            foreach (NewsHeadlineSubPiece newsHeadlinePiece in _newsHeadlineSubPieces)
            {
                if (newsHeadlinePiece.GetCoordinates() == coordinates)
                {
                    continue;
                }

                newsHeadlinePieces[index] = newsHeadlinePiece;
                index++;
            }

            return newsHeadlinePieces;
        }

        public Vector2[] GetSubPiecesPositionsRelativeToRoot()
        {
            return _subPiecesPositionsRelativeToRoot;
        }

        public void SetNewsType(NewsType newsType)
        {
            _newsType = newsType;
        }

        public NewsType GetNewsType()
        {
            return _newsType;
        }

        public void SetContainerLimiters(Vector2 containerMinCoordinates, Vector2 containerMaxCoordinates)
        {
            _containerMinCoordinates = containerMinCoordinates;
            _containerMaxCoordinates = containerMaxCoordinates;
        }

        public void SetInitialPosition(Vector2 newInitialPosition)
        {
            _initialPosition = newInitialPosition;
        }

        public Vector2 GetInitialPosition()
        {
            return _initialPosition;
        }

        public void SetNewsHeadlineId(int newsHeadlineId)
        {
            _newsHeadlineId = newsHeadlineId;
        }

        public int GetNewsHeadlineId()
        {
            return _newsHeadlineId;
        }

        public void SetPieceToSubPieces()
        {
            foreach (NewsHeadlineSubPiece newsHeadlineSubPiece in _newsHeadlineSubPieces)
            {
                newsHeadlineSubPiece.SetNewsHeadlinePiece(this);
            }
        }

        public NewsHeadlineSubPiece[] GetNewsHeadlinesSubPieces()
        {
            return _newsHeadlineSubPieces;
        }

        private bool IsCoordinateInsideBounds(Vector2 coordinate)
        {
            return coordinate.x < _containerMaxCoordinates.x && coordinate.x > _containerMinCoordinates.x &&
                   coordinate.y > _containerMaxCoordinates.y && coordinate.y < _containerMinCoordinates.y;
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

        public void SetSubPieces(NewsHeadlineSubPiece[] subPieces)
        {
            _newsHeadlineSubPieces = subPieces;
        }

        public int GetSubPieceCount()
        {
            return _newsHeadlineSubPieces.Length;
        }
    }
}
