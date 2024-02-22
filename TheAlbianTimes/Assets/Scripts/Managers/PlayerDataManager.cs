using Countries;
using Managers;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    private static PlayerDataManager _instance;
    public static PlayerDataManager Instance => _instance;

    [SerializeField] const float incomeMultiplier = 0.5f;
    [SerializeField] const float censorshipMarkup = 10f;
    [SerializeField] const float reputationDiscount = 6f;
    [SerializeField] const float baseStaffCost = 20f;
    [SerializeField] const float firingCost = 10f;
    [SerializeField] const float hiringCost = 20f;
    public float staffCost = baseStaffCost;
    public float censorshipAverage = 0f;
    public int staffChange = 0;

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
    public void HireStaff()
    {
        staffChange++;
    }
    public void FireStaff()
    {
        staffChange--;
    }
    public void ApplyStaffChange()
    {
        UpdateStaff(staffChange);
        float cost = CalculateCostOfStaffChange();
        UpdateMoney(cost);
        staffChange = 0;
    }
    public float CalculateCostOfStaffChange()
    {
        return staffChange < 0 ? staffChange * firingCost : -staffChange * hiringCost;
    }
    public void UpdateStaff(int amount)
    {
        int staffCount = GameManager.Instance.gameState.playerData.staff += amount;
        if (staffCount <= 0)
        {
            Debug.Log("no bitches");
        }
    }
    public void UpdateMoney(float amount)
    {
        float money = GameManager.Instance.gameState.playerData.money += amount;
        if (money <= 0)
        {
            Debug.Log("no cash");
        }
    }
    public void UpdateReputation(float amount)
    {
        GameManager.Instance.gameState.playerData.reputation = amount;
    }
    public float CalculateRevenue()
    {
        float income = 0;
        foreach (Country country in GameManager.Instance.gameState.countries)
        {
            float addIncome = incomeMultiplier * (1f - country.GetCensorship()) * country.GetPurchasingPower() * country.GetPopulation() * country.GetReputation();
            income += addIncome;
            Debug.Log(country.name + " Income: " + addIncome);
        }
        return income;
    }
    public float CalculateCosts()
    {
        return UpdateStaffCost() * GameManager.Instance.gameState.playerData.staff;
    }
    public float UpdateStaffCost()
    {
        float censorshipCost = UpdateAverageCensorship() * censorshipMarkup;
        float reputationGain = GameManager.Instance.gameState.playerData.reputation * reputationDiscount;
        staffCost = baseStaffCost + censorshipCost - reputationGain;
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
    public float CalculateGlobalReputation()
    {
        float globalReputation = 0f;
        float populationSqrt = 0f;
        foreach (Country country in GameManager.Instance.gameState.countries)
        {
            float p = Mathf.Sqrt(country.GetPopulation());
            populationSqrt += p;
            globalReputation += country.GetReputation() * p;
        }
        globalReputation /= populationSqrt;
        return globalReputation;
    }
}
