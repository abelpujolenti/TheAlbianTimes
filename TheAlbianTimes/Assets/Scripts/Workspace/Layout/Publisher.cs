using System.Collections;
using System.Linq;
using Managers;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace Workspace.Layout
{
    public class Publisher : MonoBehaviour
    {
        private const string CONVEYOR_BELT_SOUND = "Conveyor Belt";
        private const string DROP_MOLD = "Drop Mold";
        private const string PRINT_NEWSPAPER = "Print Newspaper";

        private const float DELAY_BEFORE_PLAYING_AUDIO = 1.7f;
        
        [SerializeField] private NewspaperMold _newspaperMold;
        
        [SerializeField] private RectTransform _rectTransform;
        
        private Vector3[] _corners = new Vector3[4];
    
        private Vector2 _containerMinCoordinates;
        private Vector2 _containerMaxCoordinates;

        private Camera _camera;

        private int _conveyorBeltAudioId;

        [SerializeField] Image[] beams;

        [SerializeField] float beamSpeed = 30f;
        [SerializeField] float beamSpacing = 100f;
        private double beamScrollT = 0d;

        private bool _isScrolling = false;
        
        private void Start()
        {
            _rectTransform.GetWorldCorners(_corners);
            _camera = Camera.main;
            SetContainerLimiters();

            for (int i = 0; i < beams.Length; i++)
            {
                float x = ((i * beamSpacing) * beamSpeed) % 70 + 10f;
                beams[i].rectTransform.anchoredPosition = new Vector2(x, 0f);
            }
        }

        public void Publish()
        {
            PublishingManager.Instance.Publish(_newspaperMold.GetNewsHeadlines().ToList());

            StartCoroutine(EndRoundCoroutine(3.2f));
        }

        private IEnumerator EndRoundCoroutine(float t)
        {
            AudioManager.Instance.ChangeAudioSnapshot(AudioSnapshots.TRANSITION, t);

            StartCoroutine(TransformUtility.SetRotationCoroutine(_newspaperMold.transform, 90f, 0.3f));
            StartCoroutine(TransformUtility.SetPositionCoroutine(_newspaperMold.transform, _newspaperMold.transform.position, transform.position + new Vector3(-3f, 1f, 0f), 0.3f));
            yield return TransformUtility.SetScaleCoroutine(_newspaperMold.transform, new Vector3(.45f, .45f, .45f), 0.25f);
            AudioManager.Instance.Play3DSound(DROP_MOLD, 5, 100, transform.position);
            yield return new WaitForSeconds(.1f);

            StartCoroutine(StartPrintSound());
            
            StartCoroutine(TransformUtility.SetPositionCoroutine(_newspaperMold.transform, _newspaperMold.transform.position, transform.position + new Vector3(30f, 0f, 0f), 4f));
            yield return TransformUtility.SetPositionCoroutine(Camera.main.transform, Camera.main.transform.position, Camera.main.transform.position + new Vector3(40f, 0f, 0f), t);

            //GameManager.Instance.sceneLoader.SetScene("PublishScene");
        }

        private void SetContainerLimiters()
        {
            _containerMinCoordinates.x = _corners[0].x;
            _containerMinCoordinates.y = _corners[1].y;
            _containerMaxCoordinates.x = _corners[2].x;
            _containerMaxCoordinates.y = _corners[3].y;
        }

        public void IsCoordinateInsideBounds(Vector2 coordinate)
        {
            Vector3 screenWorldMousePosition = _camera.ScreenToWorldPoint(coordinate);

            if (screenWorldMousePosition.x < _containerMaxCoordinates.x && screenWorldMousePosition.x > _containerMinCoordinates.x &&
                screenWorldMousePosition.y > _containerMaxCoordinates.y && screenWorldMousePosition.y < _containerMinCoordinates.y)
            {
                Publish();
            }
        }

        private IEnumerator StartConveyorBelt()
        {
            _conveyorBeltAudioId = AudioManager.Instance.Play2DLoopSound(CONVEYOR_BELT_SOUND);

            while (_isScrolling)
            {
                for (int i = 0; i < beams.Length; i++)
                {
                    float x = (float)(((beamScrollT + i * beamSpacing) * beamSpeed) % 70) +  10f;
                    beams[i].rectTransform.anchoredPosition = new Vector2(x, 0f);
                }
                beamScrollT += Time.deltaTime;
                yield return null;
            }
            
            AudioManager.Instance.StopLoopingAudio(_conveyorBeltAudioId);
        }

        private IEnumerator StartPrintSound()
        {
            float time = 0;

            while (time <= DELAY_BEFORE_PLAYING_AUDIO)
            {
                time += Time.deltaTime;
                yield return null;
            }
            AudioManager.Instance.Play2DSound(PRINT_NEWSPAPER);

            time = 0;
            
            while (time <= 1.6f)
            {
                time += Time.deltaTime;
                yield return null;
            }

            time = 0;
            AudioManager.Instance.StopLoopingAudio(_conveyorBeltAudioId);

            while (time <= 1.6f)
            {
                time += Time.deltaTime;
                yield return null;
            }
            GameManager.Instance.sceneLoader.SetScene("PublishScene");
        }

        public void SetIsScrolling(bool isScrolling)
        {
            _isScrolling = isScrolling;

            if (!_isScrolling)
            {
                return;
            }
            
            StartCoroutine(StartConveyorBelt());
        }

        public bool IsScrolling()
        {
            return _isScrolling;
        }
    }
}
