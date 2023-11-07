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

        [SerializeField] private GameObject _biasContainerCanvas;
        [SerializeField] private GameObject _newsHeadlineContainer;

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

        public void DeactivateBiasCanvas()
        {
            _biasContainerCanvas.gameObject.SetActive(false);
        }

        public void ActivateBiasCanvas()
        {
            _biasContainerCanvas.gameObject.SetActive(true);
        }

        public void SendNewsHeadlineToGameManager(NewsHeadline newsHeadline) 
        {
            newsHeadline.transform.parent = _newsHeadlineContainer.transform;
        }

        public void SaveBiasContainerCanvas()
        {
            _editorialManagerData.biasContainerCanvas = _biasContainerCanvas;
        }

    }
}
