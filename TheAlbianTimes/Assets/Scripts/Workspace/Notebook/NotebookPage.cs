using System;
using UnityEngine;

namespace Workspace.Notebook
{
    public class NotebookPage : MonoBehaviour
    {
        [SerializeField] private bool _leftPage;
        
        private GameObject _currentPage;

        public void ChangeContent(GameObject pagePrefab)
        {
            if (_currentPage != null)
            {
                _currentPage.SetActive(false);
            }
            
            if (pagePrefab == null)
            {
                return;
            }
            
            pagePrefab.SetActive(true);
            Transform pageTransform = pagePrefab.transform;
            pageTransform.SetParent(transform);
            pageTransform.localPosition = new Vector3(0, 0, 0);
            pageTransform.localScale = new Vector3(1, 1, 1);
            pageTransform.localRotation = new Quaternion(0, 0, 0, 1);
            _currentPage = pagePrefab;
        }

        public void UnattachPageContent()
        {
            _currentPage = null;
        }

        public GameObject GetCurrentPage()
        {
            return _currentPage;
        }
    }
}
