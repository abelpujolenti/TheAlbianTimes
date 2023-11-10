using Layout;
using Managers;
using UnityEngine;

namespace AbstractClasses
{
    public abstract class NewsHeadlinePieceAction : NewsAction
    {
        private void OnEnable()
        {
            EventsManager.OnFailSnap += Action;
        }

        private void OnDisable()
        {
            EventsManager.OnFailSnap -= Action;
        }

        protected abstract void Action(NewsHeadlinePiece newsHeadlinePiece, Vector2 mousePosition);
    }
}
