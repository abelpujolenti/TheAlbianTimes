using UnityEngine;

namespace Workspace.Notebook
{
    public class NotebookPage : MonoBehaviour
    {
        [SerializeField] private bool _righPage;

        private GameObject _currentPage;

        public void ChangeContent(GameObject pagePrefab)
        {
            Destroy(_currentPage);

            if (pagePrefab == null)
            {
                return;
            }
            
            Transform pageTransform = pagePrefab.transform;
            pageTransform.SetParent(transform);
            pageTransform.localPosition = new Vector3(0, 0, 0);
            pageTransform.localScale = new Vector3(1, 1, 1);
            _currentPage = pagePrefab;
        }
    }
}
