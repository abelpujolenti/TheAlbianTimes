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

        [SerializeField] private NewsType _newsType;

        [SerializeField] private NewsHeadlineSubPiece[] _newsHeadlineSubPieces;

        private Cell[] _snappedCells;

        private Vector2[] _subPiecesPositionsRelativeToRoot;

        private Vector2 _containerMinCoordinates;
        private Vector2 _containerMaxCoordinates;
        private Vector2 _initialPosition;

        private bool _rotation;
        private bool _transferDrag;

        private void Start()
        {
            _subPiecesPositionsRelativeToRoot = new Vector2[_newsHeadlineSubPieces.Length];
        }

        public void BeginDrag()
        {
            transform.SetAsLastSibling();

            SoundManager.Instance.GrabPieceSound();

            EventsManager.OnCheckDistanceToMouse += DistanceToPosition;

            foreach (NewsHeadlineSubPiece newsHeadlineSubPiece in _newsHeadlineSubPieces)
            {
                newsHeadlineSubPiece.Fade(TRANSPARENCY_VALUE);
            }

            if (_snappedCells == null)
            {
                return;
            }

            foreach (Cell cell in _snappedCells)
            {
                cell.SetFree(true);
            }
        }

        public void EndDrag(NewsHeadlineSubPiece draggedSubPiece, Vector2 mousePosition)
        {
            EventsManager.OnCheckDistanceToMouse -= DistanceToPosition;
            
            foreach (NewsHeadlineSubPiece newsHeadlineSubPiece in _newsHeadlineSubPieces)
            {
                newsHeadlineSubPiece.Fade(FULL_OPACITY);
            }

            if (!gameObject.activeSelf)
            {
                return;
            }

            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            _snappedCells = EventsManager.OnPreparingCells(draggedSubPiece, mousePosition, _newsHeadlineSubPieces);

            if (_snappedCells == null)
            {
                _rotation = !EventsManager.OnFailSnap(this);
                Debug.Log("Snapped " + _rotation);

                SoundManager.Instance.DropPieceSound();
                return;
            }

            transform.position = EventsManager.OnSuccessfulSnap(_snappedCells);
            _rotation = false;
            Debug.Log("Not Snapped");

            SoundManager.Instance.SnapPieceSound();
        }

        public Vector2[] GetSubPiecesPositionsRelativeToRoot()
        {
            return _subPiecesPositionsRelativeToRoot;
        }

        public NewsHeadlineSubPiece[] GetNewsHeadlinesSubPieces()
        {
            return _newsHeadlineSubPieces;
        }
        public void SetTransferDrag(bool transferDrag)
        {
            _transferDrag = transferDrag;
        }

        public bool GetTransferDrag()
        {
            return _transferDrag;
        }

        public bool CanRotate()
        {
            return _rotation;
        }

        private Vector2 DistanceToPosition(Vector2 position)
        {
            return (Vector2)transform.position - position;
        }

        public void SetSubPieces(NewsHeadlineSubPiece[] subPieces)
        {
            _newsHeadlineSubPieces = subPieces;
        }
    }
}
