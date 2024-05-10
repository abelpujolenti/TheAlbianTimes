using Dialogue;
using Managers;
using UnityEngine;

public class DialogueConsequenceManager : MonoBehaviour
{
    private static DialogueConsequenceManager _instance;
    public static DialogueConsequenceManager Instance => _instance;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void ApplyDialogueConsequences(DialogueConsequence[] consequences)
    {
        foreach (DialogueConsequence consequence in consequences)
        {
            ApplyDialogueConsequencesToCountry(consequence);
        }
    }
    private void ApplyDialogueConsequencesToCountry(DialogueConsequence consequence)
    {
        GameManager.Instance.gameState.characters[(int)consequence.entityId].AddToValue(consequence.key, consequence.value);
    }

}
