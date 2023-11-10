using Editorial;
using Managers.ScriptableObjects;
using UnityEngine;

namespace Managers
{
    public class EditorialManager : MonoBehaviour
    {

        private static EditorialManager _instance;

        public static EditorialManager Instance => _instance;

        [SerializeField] private EditorialManagerData _editorialManagerData;

        private GameObject _newsFolderCanvas;
        private GameObject _biasContainerCanvas;
        private GameObject _newsHeadlineContainer;

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

        private void Start()
        {
            _newsHeadlineContainer = GameObject.Find("NewsHeadlineContainer");
        }

        public void DeactivateBiasCanvas()
        {
            _biasContainerCanvas.gameObject.SetActive(false);
        }

        public void ActivateBiasCanvas()
        {
            _biasContainerCanvas.gameObject.SetActive(true);
        }

        public void SetNewsFolderCanvas(GameObject newsFolderCanvasGameObject)
        {
            _newsFolderCanvas = newsFolderCanvasGameObject;
        }

        public void SetBiasContainerCanvas(GameObject biasContainerCanvasGameObject)
        {
            _biasContainerCanvas = biasContainerCanvasGameObject;
        }

        public void SendNewsHeadlineToLayoutManager(GameObject newsHeadline, int newsHeadlineId) 
        {
            GameManager.Instance.SendNewsHeadlineToLayoutManager(newsHeadline, newsHeadlineId);
            newsHeadline.transform.SetParent(_newsHeadlineContainer.transform, false);
        }

        public void SendNewsHeadlineToNewsFolderCanvas(int newsHeadlineId)
        {
            GameObject newsHeadline = LookForDesiredNewsHeadline(newsHeadlineId);
            newsHeadline.transform.SetParent(_newsFolderCanvas.transform, false);
            EventsManager.OnAddNewsHeadlineToFolder(newsHeadline);
        }

        private GameObject LookForDesiredNewsHeadline(int newsHeadlineId)
        {
            GameObject newsHeadline = null;
            
            foreach (Transform childTransform in _newsHeadlineContainer.transform)
            {
                newsHeadline = childTransform.gameObject;
                
                if (childTransform.gameObject.GetInstanceID() == newsHeadlineId)
                {
                    break;
                }
            }

            return newsHeadline;
        }
    }
}
