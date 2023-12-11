using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueSystem : MonoBehaviour
{
    public NoMonoBehavior.DialogueContainer container = new NoMonoBehavior.DialogueContainer();

    public static DialogueSystem instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            DestroyImmediate(gameObject);

    }
}
