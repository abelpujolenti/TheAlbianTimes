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

        public void SendNewsHeadlineToGameManager(GameObject newsHeadline) 
        {
            newsHeadline.transform.SetParent(_newsHeadlineContainer.transform, false);
        }

        public void SendNewsHeadlineToNewsFolderCanvas(GameObject newsHeadline)
        {
            newsHeadline.transform.SetParent(_newsFolderCanvas.transform, false);
            ActionsManager.OnAddNewsHeadlineToFolder(newsHeadline);
        }
    }
}
