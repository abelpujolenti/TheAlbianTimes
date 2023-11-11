using System.Collections.Generic;
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

        public void DeactivateBiasCanvas()
        {
            _biasContainerCanvas.gameObject.SetActive(false);
        }

        public void ActivateBiasCanvas()
        {
            _biasContainerCanvas.gameObject.SetActive(true);
        }

        public void SetNewsFolder(NewsFolder newsFolder)
        {
            _newsFolder = newsFolder;
        }

        public void SetBiasContainerCanvas(GameObject biasContainerCanvasGameObject)
        {
            _biasContainerCanvas = biasContainerCanvasGameObject;
        }

        public void SendNewsHeadlineToLayoutManager(GameObject newsHeadline, int newsHeadlineId) 
        {
            GameManager.Instance.SendNewsHeadlineToLayoutManager(newsHeadline, newsHeadlineId);
        }

        public void SendNewsHeadlineToNewsFolderCanvas(int newsHeadlineId)
        {
            GameObject newsHeadline = LookForDesiredNewsHeadline(newsHeadlineId);
            EventsManager.OnAddNewsHeadlineToFolder(newsHeadline);
        }

        private GameObject LookForDesiredNewsHeadline(int newsHeadlineId)
        {
            List<GameObject> instancedNewsHeadlinesNotInSight = _newsFolder.GetInstancedNewsHeadlineNotInSightList();
            
            GameObject newsHeadline = null;

            for (int i = 0; i < instancedNewsHeadlinesNotInSight.Count; i++)
            {
                newsHeadline = instancedNewsHeadlinesNotInSight[i];

                if (newsHeadline.GetInstanceID() != newsHeadlineId)
                {
                    continue;
                }
                newsHeadline.SetActive(true);
                instancedNewsHeadlinesNotInSight.RemoveAt(i);
                break;
            }

            return newsHeadline;
        }
    }
}
