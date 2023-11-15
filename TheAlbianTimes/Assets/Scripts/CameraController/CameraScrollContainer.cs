using Managers;
using UnityEngine;

namespace CameraController
{
    public class CameraScrollContainer : MonoBehaviour
    {
        [SerializeField] private GameObject[] _cameraSideScrolls;
        
        private void OnEnable()
        {
            EventsManager.OnStartEndDrag += ModifyActiveCameraSideScrolls;
        }

        private void OnDisable()
        {
            EventsManager.OnStartEndDrag -= ModifyActiveCameraSideScrolls;
        }

        private void ModifyActiveCameraSideScrolls(bool active)
        {
            for (int i = 0; i < _cameraSideScrolls.Length; i++)
            {
                _cameraSideScrolls[i].SetActive(active);
            }

            EventsManager.OnExceedCameraLimitsWhileDragging -= ModifyActiveCameraSideScrolls;
        }

        public void SubscribeOnExceedEvent() 
        {
            EventsManager.OnExceedCameraLimitsWhileDragging += ModifyActiveCameraSideScrolls;
        }
    }
}
