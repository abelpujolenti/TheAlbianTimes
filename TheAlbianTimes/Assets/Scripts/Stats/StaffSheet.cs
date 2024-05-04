using Countries;
using Managers;
using TMPro;
using UnityEngine;

public class StaffSheet : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI staff;
    [SerializeField] TextMeshProUGUI cost;
    float staffCost = PlayerDataManager.baseStaffCost;
    float censorshipAverage = 0f;
    int staffChange = 0;

    private void Start()
    {
        UpdateStaffCost();
        UpdateText();
    }

    public void HireStaff()
    {
        staffChange++;
        UpdateText();
    }
    public void FireStaff()
    {
        staffChange--;
        UpdateText();
    }
    public void ApplyStaffChange()
    {
        UpdateStaff(staffChange);
        staffChange = 0;
    }
    private void UpdateText()
    {
        UpdateStaffCount();
        UpdateCost();
    }
    private void UpdateStaffCount()
    {
        int amount = GameManager.Instance.gameState.playerData.staff + staffChange;
        staff.text = "Staff: " + amount;
    }
    private void UpdateCost()
    {
        string text = "<color=red>Cost: " + (staffCost * GameManager.Instance.gameState.playerData.staff + staffChange).ToString("F1") + " / day</color>";
        cost.text = text;
    }
    public float UpdateStaffCost()
    {
        float censorshipCost = UpdateAverageCensorship() * PlayerDataManager.censorshipMarkup;
        float reputationGain = GameManager.Instance.gameState.playerData.reputation * PlayerDataManager.reputationDiscount;
        staffCost = PlayerDataManager.baseStaffCost + censorshipCost - reputationGain;
        return staffCost;
    }
    public float UpdateAverageCensorship()
    {
        censorshipAverage = 0f;
        foreach (Country country in GameManager.Instance.gameState.countries)
        {
            censorshipAverage += country.GetCensorship();
        }
        censorshipAverage /= (float)Country.Id.AMOUNT;
        return censorshipAverage;
    }
    public void UpdateStaff(int amount)
    {
        int staffCount = GameManager.Instance.gameState.playerData.staff += amount;
        if (staffCount <= 0)
        {
            Debug.Log("no bitches");
        }
    }
}
