using Managers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TESTDialogueManager : MonoBehaviour
{
    string text1 = "We are aware of the article your newspaper published. This is not a threat. But that might have to change... Do not publish information regarding the internal affairs of ";
    string text2 = ".";
    string text;
    TextMeshProUGUI tmpText;
    TextMeshProUGUI tmpTitle;
    Image bg;
    Button button;

    void Start()
    {
        tmpText = transform.Find("text").GetComponent<TextMeshProUGUI>();
        tmpTitle = transform.Find("title").GetComponent<TextMeshProUGUI>();
        bg = GetComponent<Image>();
        button = transform.Find("continueButton").GetComponent<Button>();
        button.gameObject.SetActive(false);
        StartCoroutine(TestDialogue());
    }

    private IEnumerator TestDialogue()
    {
        bg.color = new Color(bg.color.r, bg.color.g, bg.color.b, 0f);
        tmpTitle.color = new Color(tmpTitle.color.r, tmpTitle.color.g, tmpTitle.color.b, 0f);

        yield return new WaitForFixedUpdate();

        Country madCountry = GameManager.Instance.gameState.countries[0];
        foreach (Country c in GameManager.Instance.gameState.countries)
        {
            if (madCountry.GetReputation() > c.GetReputation())
            {
                madCountry = c;
            }
        }
        text = text1 + Country.names[(int)madCountry.GetId()] + text2;

        SoundManager.Instance.PhoneSound();
        yield return new WaitForSeconds(2.5f);

        for (int i = 0; i < 255; i++)
        {
            bg.color = new Color(bg.color.r, bg.color.g, bg.color.b, i / 255f);
            tmpTitle.color = new Color(tmpTitle.color.r, tmpTitle.color.g, tmpTitle.color.b, i / 255f);
            yield return new WaitForSeconds(.02f);
        }

        string runningText;

        for (int i = 0; i < text.Length; i++)
        {
            runningText = "<color=#FFFFFFFF>" + text.Substring(0, i + 1) + "<color=#FFFFFF00>" + text.Substring(i + 1, text.Length - i - 1);
            tmpText.text = runningText;
            SoundManager.Instance.TypeSound();
            yield return new WaitForSeconds(.08f);
            if (i == 75) yield return new WaitForSeconds(1.2f);
        }
        button.gameObject.SetActive(true);
    }

    public void ContinueToNextRound()
    {
        GameManager.Instance.sceneLoader.SetScene("WorkspaceScene");
    }

}
