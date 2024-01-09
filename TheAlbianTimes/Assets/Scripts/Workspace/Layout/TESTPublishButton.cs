using Managers;
using UnityEngine;

namespace Workspace.Layout
{
    public class TESTPublishButton : MonoBehaviour
    {
        public void PressPublishButton()
        {
            PublishingManager.Instance.Publish();
            GameManager.Instance.AddToRound();
        }
    }
}
