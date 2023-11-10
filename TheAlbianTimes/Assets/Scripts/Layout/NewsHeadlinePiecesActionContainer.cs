using Managers;
using UnityEngine;

namespace Layout
{
    public class NewsHeadlinePiecesActionContainer : MonoBehaviour
    {
        [SerializeField] private GameObject _newsHeadlinePieceSendAction;
        
        private void OnEnable()
        {
            EventsManager.OnDragNewsHeadlinePiece += ActivateContainer;
        }
        
        private void OnDisable()
        {
            EventsManager.OnDragNewsHeadlinePiece -= ActivateContainer;
        }

        private void ActivateContainer()
        {
            _newsHeadlinePieceSendAction.SetActive(true);
        }
    }
}
