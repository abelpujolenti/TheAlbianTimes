using AbstractClasses;
using Managers;
using UnityEngine;

namespace Editorial
{
    public class NewsHeadlineActionsContainer : MonoBehaviour
    {
        [SerializeField] private NewsHeadlineAction _newsHeadlineSendAction;
        [SerializeField] private NewsHeadlineAction _newsHeadlineChangeAction;
        
        private void OnEnable()
        {
            EventsManager.OnDragNewsHeadline += ActivateContainers;
        }
        
        private void OnDisable()
        {
            EventsManager.OnDragNewsHeadline -= ActivateContainers;
        }

        private void ActivateContainers(NewsHeadline newsHeadline)
        {
            if (newsHeadline.GetChosenBiasIndex() == newsHeadline.GetSelectedBiasIndex())
            {
                _newsHeadlineSendAction.gameObject.SetActive(true);
                return;
            }
            _newsHeadlineChangeAction.gameObject.SetActive(true);
        }
    }
}
