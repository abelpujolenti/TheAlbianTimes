using Managers;
using UnityEngine;

namespace CameraController
{
    public class CameraScrollContainer : MonoBehaviour
    {
        [SerializeField] private CameraSideScroll[] _cameraSideScrolls;
        
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
            foreach (CameraSideScroll cameraSideScroll in _cameraSideScrolls)
            {
                if (cameraSideScroll.IsExceed())
                {
                    continue;
                }
                cameraSideScroll.gameObject.SetActive(active);
            }
        }

        private void EnableExceedCameraSideScrolls()
        {
            foreach (CameraSideScroll cameraSideScroll in _cameraSideScrolls)
            {
                cameraSideScroll.SetExceed(false);
            }
            
            ModifyActiveCameraSideScrolls(true);

            EventsManager.OnExceedCameraLimitsWhileDragging -= EnableExceedCameraSideScrolls;
        }

        public void SubscribeOnExceedEvent() 
        {
            EventsManager.OnExceedCameraLimitsWhileDragging += EnableExceedCameraSideScrolls;
        }
    }
}
