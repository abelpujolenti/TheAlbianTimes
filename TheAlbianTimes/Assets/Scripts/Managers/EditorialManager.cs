using System.Collections.Generic;
using UnityEngine;
using Workspace.Editorial;

namespace Managers
{
    public class EditorialManager : MonoBehaviour
    {

        private static EditorialManager _instance;

        public static EditorialManager Instance => _instance;
        
        [SerializeField] private GameObject _biasContainerGameObject;
        [SerializeField] private BiasContainer _biasContainer;

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

        public void TurnOnBiasContainer()
        {
            if (_biasContainerGameObject.activeSelf)
            {
                return;
            }
            _biasContainerGameObject.SetActive(true);
        }

        public void TurnOffBiasContainer()
        {
            _biasContainerGameObject.SetActive(false);
            if (EventsManager.OnClickBias == null)
            {
                return;
            }

            EventsManager.OnClickBias();
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
    }
}
