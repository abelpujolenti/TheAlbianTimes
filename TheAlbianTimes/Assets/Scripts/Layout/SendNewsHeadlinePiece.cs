using AbstractClasses;
using Managers;
using UnityEngine;

namespace Layout
{
    public class SendNewsHeadlinePiece : NewsHeadlinePieceAction
    {
        protected override void Action(NewsHeadlinePiece newsHeadlinePiece, Vector2 mousePosition)
        {
            if (!IsCoordinateInsideBounds(mousePosition))
            {
                newsHeadlinePiece.SlideFromOriginToDestination(newsHeadlinePiece.transform.position);
                gameObject.SetActive(false);
                return;
            }

            EventsManager.OnSendNewsHeadlinePiece();
            
            gameObject.SetActive(false);
        }
    }
}
