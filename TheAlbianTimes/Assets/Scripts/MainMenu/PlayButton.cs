using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayButton : MonoBehaviour
{
    public void OnClick()
    {
        GameManager.Instance.sceneLoader.SetScene("WorkspaceScene");
    }
}
