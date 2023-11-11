using System.Collections.Generic;
using Editorial;
using Managers;
using UnityEngine;

namespace Layout
{
    public class NewsHeadlinePiecesContainer : MonoBehaviour
    {
        private const int APPEAR_Y_POSITION = 10;
        
        [SerializeField] private RectTransform _rectTransform;

        [SerializeField] private GameObject TESTPiece;

        private readonly Vector3[] _corners = new Vector3[4];

        private List<GameObject> _instancedNewsHeadlinePieceNotInSight = new ();
        
        private Vector2 _containerMinCoordinates;
        private Vector2 _containerMaxCoordinates;

        private void OnEnable()
        {
            EventsManager.OnAddNewsHeadlinePieceToLayout += AddNewsHeadlinePiece;
            EventsManager.OnSendNewsHeadlinePieceToEditorial += AddNewsHeadlinePieceToNotInSightList;
        }

        private void OnDisable()
        {
            EventsManager.OnAddNewsHeadlinePieceToLayout -= AddNewsHeadlinePiece;
            EventsManager.OnSendNewsHeadlinePieceToEditorial -= AddNewsHeadlinePieceToNotInSightList;
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

        private void AddNewsHeadlinePiece(GameObject newsHeadline, int newsHeadlineId)
        {
            GameObject auxNewsHeadlinePiece;
            GameObject newsHeadlinePiece = null;

            NewsHeadlinePiece newsHeadlinePieceComponent = null;

            for (int i = 0; i < _instancedNewsHeadlinePieceNotInSight.Count; i++)
            {
                auxNewsHeadlinePiece = _instancedNewsHeadlinePieceNotInSight[i];

                if (auxNewsHeadlinePiece.GetComponent<NewsHeadlinePiece>().GetNewsHeadlineId() != newsHeadlineId)
                {
                    continue;
                }
                newsHeadlinePiece = auxNewsHeadlinePiece;
                newsHeadlinePieceComponent = newsHeadlinePiece.GetComponent<NewsHeadlinePiece>();
                newsHeadlinePiece.SetActive(true);
                newsHeadlinePiece.transform.SetParent(gameObject.transform);
                _instancedNewsHeadlinePieceNotInSight.RemoveAt(i);
                break;
            }

            if (newsHeadlinePiece == null)
            {
                GameObject newsHeadlinePieceGameObject = Instantiate(TESTPiece, transform);

                newsHeadlinePieceComponent = newsHeadlinePieceGameObject.GetComponent<NewsHeadlinePiece>();
                
                newsHeadlinePieceComponent.SetNewsType(newsHeadline.GetComponent<NewsHeadline>().GetNewsType());
                
                newsHeadlinePieceComponent.SetContainerLimiters(_containerMinCoordinates, _containerMaxCoordinates);
            
                newsHeadlinePieceComponent.SetNewsHeadlineId(newsHeadlineId);
            }

            Vector2 position = PositionNewsHeadlinePiece(newsHeadlinePieceComponent);
            
            newsHeadlinePieceComponent.SetInitialPosition(position);

            Vector2 origin = position + new Vector2(0, APPEAR_Y_POSITION);

            newsHeadlinePieceComponent.SlideFromOriginToDestination(origin);
        }

        private void AddNewsHeadlinePieceToNotInSightList(GameObject newsHeadlinePiece)
        {
            newsHeadlinePiece.SetActive(false);
            _instancedNewsHeadlinePieceNotInSight.Add(newsHeadlinePiece);
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
