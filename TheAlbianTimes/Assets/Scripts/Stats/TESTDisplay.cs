using Countries;
using Managers;
using TMPro;
using UnityEngine;

public class TESTDisplay : MonoBehaviour
{
    TextMeshProUGUI countriesText;
    TextMeshProUGUI playerText;

    void Start()
    {
    }

    public void HireStaff()
    {
        PlayerDataManager.Instance.HireStaff();
        UpdateText();
    }
    public void FireStaff()
    {
        PlayerDataManager.Instance.FireStaff();
        UpdateText();
    }
    public void Continue()
    {
        PlayerDataManager.Instance.ApplyStaffChange();
        GameManager.Instance.sceneLoader.SetScene("DialogueScene");
    }
    public void UpdateText()
    {
        countriesText = transform.Find("countries").Find("text").GetComponent<TextMeshProUGUI>();
        playerText = transform.Find("player").Find("text").GetComponent<TextMeshProUGUI>();

        string ct = "";
        foreach (Country c in GameManager.Instance.gameState.countries)
        {
            ct += c.DisplayValues() + "\n";
        }
        countriesText.text = ct;

        string pt = "";
        int staffChange = PlayerDataManager.Instance.staffChange;
        float staffChangeCost = PlayerDataManager.Instance.CalculateCostOfStaffChange();
        string staffChangeStr = staffChange > 0 ? "<color=green>(+" + staffChange + ")</color>" : "<color=red>(" + staffChange + ")</color>";
        string staffChangeCostStr = staffChangeCost > 0 ? "<color=green>(+" + staffChangeCost + ")</color>" : "<color=red>(" + staffChangeCost + ")</color>";
        pt += "Money: " + GameManager.Instance.gameState.playerData.money + staffChangeCostStr + "\n";
        pt += "Staff: " + GameManager.Instance.gameState.playerData.staff + staffChangeStr + "\n";
        pt += "Reputation: " + GameManager.Instance.gameState.playerData.reputation.ToString("p") + "\n";
        playerText.text = pt;

        // TODO: Change in stats should definetely be saved somewhere
    }
}
