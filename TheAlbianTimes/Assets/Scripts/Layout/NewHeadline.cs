using System;
using UnityEngine;

namespace Layout
{
    public class NewHeadline : MonoBehaviour
    {
        [SerializeField] private NewsType _newsType;
        
        [SerializeField] private NewsHeadlinePiece[] _newsHeadlinePieces;

        private Cell[] _snappedCells;

        private Vector3 _offset;
        
        void Start()
        {
            switch (_newsType)
            {
                case NewsType.MILITARY:
                    
                    break;
                
                case NewsType.CULTURAL:
                    
                    break;
                
                case NewsType.ECONOMIC:
                    
                    break;
                
                case NewsType.SPORTS:
                    
                    break;
            }

            _offset = new Vector3();

            foreach (NewsHeadlinePiece piece in _newsHeadlinePieces)
            {
                _offset += piece.transform.position;
            }

            _offset /= _newsHeadlinePieces.Length;

            _offset = transform.position - _offset;

        }

        public void BeginDrag()
        {
            ActionsManager.OnReleaseNewsHeadline += TryToSnap;

            if (_snappedCells == null)
            {
                return;
            }

            for (int i = 0; i < _snappedCells.Length; i++)
            {
                _snappedCells[i].SetFree(true);
            }
        }

        private void TryToSnap(NewsHeadlinePiece draggedPiece, Vector2 mousePosition)
        {
            if (ActionsManager.OnPreparingCells == null || 
                ActionsManager.OnSuccessFul == null)
            {
                return;
            }

            _snappedCells = ActionsManager.OnPreparingCells(draggedPiece, mousePosition, _newsHeadlinePieces);

            if (_snappedCells == null)
            {
                return;
            }

            for (int i = 0; i < _newsHeadlinePieces.Length; i++)
            {
                if (!_snappedCells[i].IsFree())
                {
                    return;
                }
            }

            transform.position = ActionsManager.OnSuccessFul(_snappedCells, transform.position) + _offset;
            
            ActionsManager.OnReleaseNewsHeadline -= TryToSnap;
        }

        private NewsHeadlinePiece[] GetNewsHeadlineNeighborPieces(Vector2 coordinates)
        {
            NewsHeadlinePiece[] newsHeadlinePieces = { };

            int index = 0; 

            foreach (NewsHeadlinePiece newsHeadlinePiece in _newsHeadlinePieces)
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
