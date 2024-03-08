using System.Collections;
using System.Linq;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Workspace.Layout
{
    public class Publisher : MonoBehaviour
    {
        private const string CONVEYOR_BELT_SOUND = "Conveyor Belt";
        
        [SerializeField] private NewspaperMold _newspaperMold;
        
        [SerializeField] private RectTransform _rectTransform;
        
        private Vector3[] _corners = new Vector3[4];
    
        private Vector2 _containerMinCoordinates;
        private Vector2 _containerMaxCoordinates;

        private Camera _camera;
        
        private AudioSource _audioSourceConveyorBelt;

        [SerializeField] Image[] beams;

        [SerializeField] float beamSpeed = 30f;
        [SerializeField] float beamSpacing = 100f;
        private double beamScrollT = 0d;

        public bool scrolling = false;
        
        private void Start()
        {
            _rectTransform.GetWorldCorners(_corners);
            _camera = Camera.main;
            SetContainerLimiters();
            
            _audioSourceConveyorBelt = gameObject.AddComponent<AudioSource>();
            (AudioSource, string)[] tuples = {
                (_audioSourceConveyorBelt, CONVEYOR_BELT_SOUND)
            };
            
            SoundManager.Instance.SetMultipleAudioSourcesComponents(tuples);

            for (int i = 0; i < beams.Length; i++)
            {
                float x = ((i * beamSpacing) * beamSpeed) % 70 + 10f;
                beams[i].rectTransform.anchoredPosition = new Vector2(x, 0f);
            }
        }

        private void Update()
        {
            if (scrolling)
            {
                for (int i = 0; i < beams.Length; i++)
                {
                    float x = (float)(((beamScrollT + i * beamSpacing) * beamSpeed) % 70) +  10f;
                    beams[i].rectTransform.anchoredPosition = new Vector2(x, 0f);
                }
                beamScrollT += Time.deltaTime;
            }   
        }

        public void Publish()
        {
            //_audioSourceConveyorBelt.Play();
            PublishingManager.Instance.Publish(_newspaperMold.GetNewsHeadlines().ToList());
            GameManager.Instance.AddToRound();

            StartCoroutine(EndRoundCoroutine(3.5f));
        }

        private IEnumerator EndRoundCoroutine(float t)
        {
            ((RectTransform)_newspaperMold.transform).pivot = new Vector2(.5f, .5f);
            StartCoroutine(TransformUtility.SetRotationCoroutine(_newspaperMold.transform, 90f, 0.3f));
            StartCoroutine(TransformUtility.SetPositionCoroutine(_newspaperMold.transform, _newspaperMold.transform.position, transform.position + new Vector3(-3f, 0f, 0f), 0.3f));
            yield return TransformUtility.SetScaleCoroutine(_newspaperMold.transform, new Vector3(.35f, .35f, .35f), 0.25f);
            yield return new WaitForSeconds(.1f);

            StartCoroutine(TransformUtility.SetPositionCoroutine(_newspaperMold.transform, _newspaperMold.transform.position, transform.position + new Vector3(30f, 0f, 0f), 4f));
            yield return TransformUtility.SetPositionCoroutine(Camera.main.transform, Camera.main.transform.position, Camera.main.transform.position + new Vector3(40f, 0f, 0f), t);

            GameManager.Instance.sceneLoader.SetScene("PublishScene");
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
    }
}
