using Countries;
using Managers;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class BalanceSheet : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] TextMeshProUGUI amounts;

    void Start()
    {
        SetValues();
    }

    void SetValues ()
    {
        text.text = "";
        amounts.text = "";

        float revenue = PlayerDataManager.Instance.CalculateRevenue();
        AddLine("Revenue:", revenue, 22f);

        var values = PlayerDataManager.Instance.CalculateRevenueComponents();
        for (int i = 0; i < values.Length; i++)
        {
            AddLine(Country.names[i] + ":", values[i], 15f);
        }

        float cost = PlayerDataManager.Instance.CalculateCosts();
        AddLine("Savings:", GameManager.Instance.prevGameState.playerData.money , 22f);
        AddLine("Costs:", -cost, 22f);
        text.text += "\n";
        amounts.text += "\n";
        AddLine("Total:", GameManager.Instance.gameState.playerData.money, 25f);
    }

    void AddLine(string name, float value, float size)
    {
        text.text += "<size=" + size + ">" + GetColor(value) + name + "\n";
        amounts.text += "<size=" + size + ">" + GetColor(value) + value.ToString("F1") + "$" + "\n";
    }

    string GetColor(float value)
    {
        Color color = value < 0 ? Color.red : Color.white;
        return "<color=#" + color.ToHexString() + ">";
    }
}
