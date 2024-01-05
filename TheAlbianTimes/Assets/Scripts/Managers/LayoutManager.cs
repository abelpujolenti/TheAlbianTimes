using Layout;
using UnityEngine;

namespace Managers
{
    public class LayoutManager : MonoBehaviour
    {

        private static LayoutManager _instance;

        public static LayoutManager Instance => _instance;

        private NewsHeadlinePiecesContainer _newsHeadlinePiecesContainer;

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
    }
}
