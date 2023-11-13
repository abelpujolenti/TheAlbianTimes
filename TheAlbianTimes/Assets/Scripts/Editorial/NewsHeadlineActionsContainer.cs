
using Managers;
using UnityEngine;

namespace Editorial
{
    public class NewsHeadlineActionsContainer : MonoBehaviour
    {
        [SerializeField] private ChangeNewsHeadlineAction _newsHeadlineChangeAction;
        
        private void OnEnable()
        {
            EventsManager.OnPrepareNewsHeadlineActions += ActivateContainers;
        }
        
        private void OnDisable()
        {
            EventsManager.OnPrepareNewsHeadlineActions -= ActivateContainers;
        }

        private void ActivateContainers(NewsHeadline newsHeadline)
        {
            if (newsHeadline.GetChosenBiasIndex() == newsHeadline.GetSelectedBiasIndex())
            {
                return;
            }
            _newsHeadlineChangeAction.gameObject.SetActive(true);
        }
    }
}
