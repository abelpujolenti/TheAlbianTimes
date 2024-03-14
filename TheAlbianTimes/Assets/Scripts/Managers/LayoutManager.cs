using UnityEngine;
using Workspace.Layout;

namespace Managers
{
    public class LayoutManager : MonoBehaviour
    {
        private static LayoutManager _instance;

        public static LayoutManager Instance => _instance;

        private NewsHeadlinePiecesContainer _newsHeadlinePiecesContainer;

        private bool _isMoldInPlace = true;
        private bool _isMoldDraggable;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void SetNewsHeadlinePiecesContainer(NewsHeadlinePiecesContainer newsHeadlinePiecesContainer)
        {
            _newsHeadlinePiecesContainer = newsHeadlinePiecesContainer;
        }

        public NewsHeadlinePiecesContainer GetNewsHeadlinePiecesContainer()
        {
            return _newsHeadlinePiecesContainer;
        }

        public void SetIsMoldInPlace(bool isMoldInPlace)
        {
            _isMoldInPlace = isMoldInPlace;
        }

        public bool IsMoldInPlace()
        {
            return _isMoldInPlace;
        }

        public void SetIsMoldDraggable(bool isMoldDraggable)
        {
            _isMoldDraggable = isMoldDraggable;
        }

        public bool IsMoldDraggable()
        {
            return _isMoldDraggable;
        }
    }
}
