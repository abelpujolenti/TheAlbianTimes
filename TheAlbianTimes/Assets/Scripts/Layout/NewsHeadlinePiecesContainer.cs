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

        private void AddNewsHeadlinePiece(NewsHeadlinePiece newsHeadlinePieceComponent)
        {
            bool mousePositionValid = TakePositionByMouse(newsHeadlinePieceComponent);

            if (!mousePositionValid)
            {
                if (newsHeadlinePieceComponent.GetInitialPosition() == new Vector2())
                {
                    newsHeadlinePieceComponent.SetInitialPosition(PositionNewsHeadlinePiece(newsHeadlinePieceComponent));    
                }
                newsHeadlinePieceComponent.SlideFromOriginToDestination(newsHeadlinePieceComponent.transform.position);
            }
            else
            {
                newsHeadlinePieceComponent.SetInitialPosition(newsHeadlinePieceComponent.transform.position);
            }
        }

        private bool TakePositionByMouse(NewsHeadlinePiece newsHeadlinePieceComponent)
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

        private Vector2 PositionNewsHeadlinePiece(NewsHeadlinePiece newsHeadlinePieceComponent)
        {
            Vector2 position;

            bool allSubPiecesInside;

            do
            {
                allSubPiecesInside = true;
                
                position.x = Random.Range(_containerMinCoordinates.x, _containerMaxCoordinates.x);
                position.y = Random.Range(_containerMinCoordinates.y, _containerMaxCoordinates.y);

                foreach (Vector2 subPiecePosition in newsHeadlinePieceComponent.GetSubPiecesPositionsRelativeToRoot())
                {
                    if (IsCoordinateInsideBounds(subPiecePosition + position))
                    {
                        continue;
                    }
                    allSubPiecesInside = false;
                    break;
                }

            } while (!allSubPiecesInside);

            return position;
        }

        private bool IsCoordinateInsideBounds(Vector2 coordinate)
        {
            return coordinate.x < _containerMaxCoordinates.x && coordinate.x > _containerMinCoordinates.x &&
                   coordinate.y > _containerMaxCoordinates.y && coordinate.y < _containerMinCoordinates.y;
        }
    }
}
