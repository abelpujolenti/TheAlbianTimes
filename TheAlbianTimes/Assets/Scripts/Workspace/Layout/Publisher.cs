using System.Linq;
using Managers;
using UnityEngine;
using UnityEngine.Serialization;

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
        }
        public void Publish()
        {
            //_audioSourceConveyorBelt.Play();
            PublishingManager.Instance.Publish(_newspaperMold.GetNewsHeadlines().ToList());
            GameManager.Instance.AddToRound();
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
