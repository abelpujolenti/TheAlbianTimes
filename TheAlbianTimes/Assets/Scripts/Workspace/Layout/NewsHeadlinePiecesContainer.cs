using Managers;
using UnityEngine;

namespace Workspace.Layout
{
    public class NewsHeadlinePiecesContainer : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;

        private readonly Vector3[] _corners = new Vector3[4];
        
        private Vector2 _containerMinCoordinates;
        private Vector2 _containerMaxCoordinates;

        private void Start()
        {
            LayoutManager.Instance.SetNewsHeadlinePiecesContainer(this);
            
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

        public void PositionPieceOnRandomPosition(NewsHeadlinePiece newsHeadlinePiece)
        {
            newsHeadlinePiece.SetInitialPosition(PositionNewsHeadlinePiece(newsHeadlinePiece));

            newsHeadlinePiece.SlideFromOriginToDestination();
        }

        public bool IsValidPiecePosition(NewsHeadlinePiece newsHeadlinePiece)
        {
            return CheckSubPiecesPositionsByGivenPosition(newsHeadlinePiece, newsHeadlinePiece.gameObject.transform.position);
        }

        private bool CheckSubPiecesPositionsByGivenPosition(NewsHeadlinePiece newsHeadlinePiece, Vector2 position)
        {
            foreach (Vector2 subPiecePosition in newsHeadlinePiece.GetSubPiecesPositionsRelativeToRoot())
            {
                if (IsCoordinateInsideBounds(subPiecePosition + position))
                {
                    continue;
                }
                return false;
            }

            return true;
        }

        private Vector2 PositionNewsHeadlinePiece(NewsHeadlinePiece newsHeadlinePiece)
        {
            Vector2 position;
            
            do
            {
                position.x = Random.Range(_containerMinCoordinates.x, _containerMaxCoordinates.x);
                position.y = Random.Range(_containerMinCoordinates.y, _containerMaxCoordinates.y);

            } while (!CheckSubPiecesPositionsByGivenPosition(newsHeadlinePiece, position));

            return position;
        }

        private bool IsCoordinateInsideBounds(Vector2 coordinate)
        {
            return coordinate.x < _containerMaxCoordinates.x && coordinate.x > _containerMinCoordinates.x &&
                   coordinate.y > _containerMaxCoordinates.y && coordinate.y < _containerMinCoordinates.y;
        }
    }
}
