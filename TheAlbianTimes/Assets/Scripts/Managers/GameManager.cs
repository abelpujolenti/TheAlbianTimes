using Editorial;
using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;

        public static GameManager Instance => _instance;

        private LayoutManager _layoutManager;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void SetLayoutManager(LayoutManager layoutManager)
        {
            _layoutManager.CopyComponent(layoutManager);
        }

        public void SendNewsHeadlineToEditorialManager(GameObject newsHeadline)
        {
            EditorialManager.Instance.SendNewsHeadlineToNewsFolderCanvas(newsHeadline);
        }

        public void SendNewsHeadlineToLayoutManager(NewsHeadline newsHeadline)
        {
            
        }
    }
}
