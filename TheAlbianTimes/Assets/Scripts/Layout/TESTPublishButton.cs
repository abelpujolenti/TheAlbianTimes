using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TESTPublishButton : MonoBehaviour
{
    public void PressPublishButton()
    {
        PublishingManager.Instance.Publish();
        GameManager.Instance.AddToRound();
    }
}
