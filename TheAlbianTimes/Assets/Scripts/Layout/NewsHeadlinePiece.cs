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
        
        private Coroutine _setRotationCoroutine;

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

                SoundManager.Instance.DropPieceSound();
                return;
            }
            
            SlideToRotation(0f, 0.1f);
            transform.position = EventsManager.OnSuccessfulSnap(_snappedCells);
            _rotation = false;

            SoundManager.Instance.SnapPieceSound();
        }
        

        private void SlideToRotation(float rotation, float t)
        {
            if (_setRotationCoroutine != null)
            {
                StopCoroutine(_setRotationCoroutine);
            }
            _setRotationCoroutine = StartCoroutine(SetRotationCoroutine(rotation, t));
        }
        private IEnumerator SetRotationCoroutine(float zRotation, float t)
        {
            float elapsedT = 0f;
            Vector3 startRotation = transform.rotation.eulerAngles;
            while (elapsedT <= t)
            {
                float z = Mathf.LerpAngle(startRotation.z, zRotation, elapsedT / t);
                transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, z));
                yield return new WaitForFixedUpdate();
                elapsedT += Time.fixedDeltaTime;
            }
            transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, zRotation));
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
