using System.Collections.Generic;
using UnityEngine;
using Workspace.Editorial;

namespace Managers
{
    public class EditorialManager : MonoBehaviour
    {

        private static EditorialManager _instance;

        public static EditorialManager Instance => _instance;

        private NewsFolder _newsFolder;
        
        private GameObject _biasContainerCanvas;

        private List<int> _linkIds = new List<int>();

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

        public void TurnOnBiasContainer()
        {
            if (_biasContainerCanvas.activeSelf)
            {
                return;
            }
            _biasContainerCanvas.SetActive(true);
        }

        public void TurnOffBiasContainer()
        {
            _biasContainerCanvas.SetActive(false);
        }

        public void AddLinkId(int linkId)
        {
            _linkIds.Add(linkId);
        }

        public void SubtractLinkId(int linkId)
        {
            _linkIds.Remove(linkId);
        }

        public int[] GetLinksIds()
        {
            return _linkIds.ToArray();
        }

        public void SetTotalNewsToLoad(int totalNewsToLoad)
        {
            _newsFolder.SetTotalNewsToPosition(totalNewsToLoad);
        }
    }
}
