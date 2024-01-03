using Managers;
using UnityEngine;

namespace Layout
{
    public class NewsHeadlinePiecesContainer : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;

        private readonly Vector3[] _corners = new Vector3[4];
        
        private Vector2 _containerMinCoordinates;
        private Vector2 _containerMaxCoordinates;

        private void OnEnable()
        {
            EventsManager.OnFailSnap += AddNewsHeadlinePiece; 
        }

        private void OnDisable()
        {
            EventsManager.OnFailSnap -= AddNewsHeadlinePiece; 
        }

        private void Start()
        {
            _rectTransform.GetWorldCorners(_corners);
            
            SetContainerLimiters();
        }

        private void SetContainerLimiters()
        {
            _containerMinCoordinates.x = _corners[0].x;
            _containerMinCoordinates.y = _corners[1].y;
            _containerMaxCoordinates.x = _corners[2].x;
            _containerMaxCoordinates.y = _corners[3].y;
        }

        private bool AddNewsHeadlinePiece(NewsHeadlinePiece newsHeadlinePieceComponent)
        {
            return TakeSubPiecesPositionByMouse(newsHeadlinePieceComponent);
        }

        private bool TakeSubPiecesPositionByMouse(NewsHeadlinePiece newsHeadlinePieceComponent)
        {
            foreach (Vector2 subPiecePosition in newsHeadlinePieceComponent.GetSubPiecesPositionsRelativeToRoot())
            {
                if (IsCoordinateInsideBounds(subPiecePosition + (Vector2)newsHeadlinePieceComponent.gameObject.transform.position))
                {
                    continue;
                }
                return false;
            }

            return true;
        }

        private bool IsCoordinateInsideBounds(Vector2 coordinate)
        {
            return coordinate.x < _containerMaxCoordinates.x && coordinate.x > _containerMinCoordinates.x &&
                   coordinate.y > _containerMaxCoordinates.y && coordinate.y < _containerMinCoordinates.y;
        }
    }
}
