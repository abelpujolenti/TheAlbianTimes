using Managers;
using NoMonoBehavior;
using UnityEngine;

namespace Layout
{
    public class NewHeadlinePiece : MonoBehaviour
    {
        private const float TRANSPARENCY_VALUE = 0.9f;
        private const float FULL_OPACITY = 1;

        [SerializeField] private NewsTypeData _newsType;
        
        [SerializeField] private NewsHeadlineSubPiece[] _newsHeadlinePieces;

        private Cell[] _snappedCells;

        private Vector2[] _piecesCoordinates;

        private Vector3 _offset;
        private Vector3 _lastPosition;
        
        void Start()
        {
            
            /*_piecesCoordinates = LayoutManager.Instance.GetPiecesCoordinates(_newsType.type);

            foreach (var piece in _piecesCoordinates) { 
            
                Debug.Log(piece);
            }*/

            _offset = new Vector3();

            foreach (NewsHeadlineSubPiece piece in _newsHeadlinePieces)
            {
                _offset += piece.transform.position;
            }

            _offset /= _newsHeadlinePieces.Length;

            _offset = transform.position - _offset;

        }

        public void BeginDrag()
        {
            ActionsManager.OnReleaseNewsHeadline += EndDrag;
            
            transform.SetAsLastSibling();

            for (int i = 0; i < _newsHeadlinePieces.Length; i++)
            {
                ActionsManager.OnDragNewsHeadline += _newsHeadlinePieces[i].Fade;
            }

            ActionsManager.OnDragNewsHeadline(TRANSPARENCY_VALUE);

            if (_snappedCells == null)
            {
                return;
            }

            for (int i = 0; i < _snappedCells.Length; i++)
            {
                _snappedCells[i].SetFree(true);
            }
        }

        private void EndDrag(NewsHeadlineSubPiece draggedSubPiece, Vector2 mousePosition)
        {
            if (ActionsManager.OnPreparingCells == null || 
                ActionsManager.OnSuccessFulSnap == null ||
                ActionsManager.OnDragNewsHeadline == null)
            {
                return;
            }

            ActionsManager.OnDragNewsHeadline(FULL_OPACITY);
            
            ActionsManager.OnReleaseNewsHeadline -= EndDrag;

            for (int i = 0; i < _newsHeadlinePieces.Length; i++)
            {
                ActionsManager.OnDragNewsHeadline -= _newsHeadlinePieces[i].Fade;
            }
            

            _snappedCells = ActionsManager.OnPreparingCells(draggedSubPiece, mousePosition, _newsHeadlinePieces);

            if (_snappedCells == null)
            {
                return;
            }

            transform.position = ActionsManager.OnSuccessFulSnap(_snappedCells, transform.position) + _offset;
        }

        private NewsHeadlineSubPiece[] GetNewsHeadlineNeighborPieces(Vector2 coordinates)
        {
            NewsHeadlineSubPiece[] newsHeadlinePieces = { };

            int index = 0; 

            foreach (NewsHeadlineSubPiece newsHeadlinePiece in _newsHeadlinePieces)
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
    }
}
