using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    #region Stats
    [SerializeField] private Button continueButton;
    [SerializeField] private RawImage background;
    [SerializeField] private Image character;

    private string[] words = new string[4]
    {
        "hey ho, hello, i'm here",
        "im a pen",
        "use me to mark positive stuff",
        "oh yeah and to make sure i speak correctly i really wanted to express my gratitude to Albia for letting me write this paper its been amazing"
    };

    private int currentWords = 0;

    DialogueSystem ds;
    TextArchitect architect;

    #endregion

    void Start()
    {
        ds = DialogueSystem.instance;
        architect = new TextArchitect(ds.container.dialogueText);
        architect.Build(words[currentWords]);

        /*ds.container.nameText = transform.Find("Name").GetComponentInChildren<TextMeshProUGUI>();
        ds.container.dialogueText = transform.Find("Dialogue").GetComponentInChildren<TextMeshProUGUI>();
        
        continueButton = transform.Find("ContinueButton").GetComponentInChildren<Button>();
        background = transform.Find("Background").GetComponentInChildren<Image>();
        character = transform.Find("Pen").GetComponentInChildren<Image>();*/
    }

    public void ContinueText()
    {
        currentWords++;
        architect.Build(words[currentWords]);
    }
}
