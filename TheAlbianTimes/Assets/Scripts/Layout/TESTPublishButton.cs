using Managers;
using UnityEngine;

public class TESTPublishButton : MonoBehaviour
{
    public void PressPublishButton()
    {
        PublishingManager.Instance.Publish();
        GameManager.Instance.AddToRound();
    }
}
