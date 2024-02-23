using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] GameObject notebook;
    [SerializeField] GameObject mail;
    void Start()
    {
        if (GameManager.Instance.GetRound() > 0) return;

        notebook.SetActive(false);
        mail.SetActive(false);
    }
}