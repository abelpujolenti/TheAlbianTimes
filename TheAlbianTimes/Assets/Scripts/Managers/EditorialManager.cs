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

        private GameObject _biasContainerCanvas;

        private NewsFolder _newsFolder;

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

        public void SetNewsFolder(NewsFolder newsFolder)
        {
            _newsFolder = newsFolder;
        }

        public void SetBiasContainerCanvas(GameObject biasContainerCanvasGameObject)
        {
            _biasContainerCanvas = biasContainerCanvasGameObject;
        }
    }
}
