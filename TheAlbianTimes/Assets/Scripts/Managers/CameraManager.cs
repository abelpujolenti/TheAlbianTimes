using System.Collections;
using UnityEngine;

namespace Managers
{
    public enum CameraLocation
    {
        EDITORIAL,
        CENTER,
        LAYOUT
    }

    public class CameraManager : MonoBehaviour
    {
        private static CameraManager _instance;

        public static CameraManager Instance => _instance;

        public const float MIN_X_POSITION_CAMERA = 0;
        public const float MAX_X_POSITION_CAMERA = 17.77f;
        
        private Transform camTransform;
        
        private Coroutine moveCameraCoroutine;

        private CameraLocation _cameraLocation;

        private float _initialMouseXPosition;
        
        private bool _scrolling;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                camTransform = Camera.main.transform;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void PanToLayout(float scrollTime)
        {
            if (_scrolling)
            {
                return;
            }

            _scrolling = true;
            moveCameraCoroutine = StartCoroutine(
                MoveCameraEnum(camTransform.position.x, MAX_X_POSITION_CAMERA, scrollTime));
            _cameraLocation = CameraLocation.LAYOUT;
        }
        
        public void PanToEditorial(float scrollTime)
        {
            if (_scrolling)
            {
                return;
            }

            _scrolling = true;
            moveCameraCoroutine = StartCoroutine(
                MoveCameraEnum(camTransform.position.x, MIN_X_POSITION_CAMERA, scrollTime));
            _cameraLocation = CameraLocation.EDITORIAL;
        }

        private IEnumerator MoveCameraEnum(float start, float end, float scrollTime)
        {
            if (moveCameraCoroutine != null)
            {
                StopCoroutine(moveCameraCoroutine);
            }

            float elapsedT = 0f;
            while (elapsedT <= scrollTime)
            {
                Vector3 newPos = camTransform.position;
                newPos.x = Mathf.SmoothStep(start, end, Mathf.Pow(Mathf.Min(1f, (elapsedT / scrollTime)), 2));
                camTransform.position = newPos;
                yield return new WaitForFixedUpdate();
                elapsedT += Time.fixedDeltaTime;
            }

            Vector3 endPos = camTransform.position;
            endPos.x = end;
            camTransform.position = endPos;
            _scrolling = false;
        }

        public CameraLocation GetCameraLocation()
        {
            return _cameraLocation;
        }

        public bool IsScrolling()
        {
            return _scrolling;
        }

        public void SetCameraOnCenter()
        {
            _cameraLocation = CameraLocation.CENTER;
        }
    }
}