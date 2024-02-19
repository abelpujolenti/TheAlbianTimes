using Managers;
using System.Linq;
using UnityEngine;

namespace Workspace.Layout
{
    public class TESTPublishButton : MonoBehaviour
    {
        [SerializeField] private NewspaperMold newspaperMold;
        private const string CONVEYOR_BELT_SOUND = "Conveyor Belt";
        private AudioSource _audioSourceConveyorBelt;
        private void Start()
        {
            _audioSourceConveyorBelt = gameObject.AddComponent<AudioSource>();
            (AudioSource, string)[] tuples = {
                (_audioSourceConveyorBelt, CONVEYOR_BELT_SOUND)
            };
            SoundManager.Instance.SetMultipleAudioSourcesComponents(tuples);
        }
        public void PressPublishButton()
        {
            //_audioSourceConveyorBelt.Play();
            PublishingManager.Instance.Publish(newspaperMold.GetNewsHeadlines().ToList());
            GameManager.Instance.AddToRound();
        }
    }
}
