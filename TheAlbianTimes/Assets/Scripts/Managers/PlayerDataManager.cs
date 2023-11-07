using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class PlayerDataManager : MonoBehaviour
{
    private static PlayerDataManager _instance;
    public static PlayerDataManager Instance => _instance;

    [SerializeField] const float incomeMultiplier = 0.5f;
    [SerializeField] const float censorshipMarkup = 10f;
    [SerializeField] const float baseStaffCost = 20f;
    [SerializeField] const float firingCost = 10f;
    [SerializeField] const float hiringCost = 20f;
    public float staffCost = baseStaffCost;
    public float censorshipAverage = 0f;

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
        UpdateMoney(-hiringCost);
        UpdateStaff(1);
    }
    public void FireStaff()
    {
        UpdateMoney(-firingCost);
        UpdateStaff(-1);
    }
    public void UpdateMoney(float amount)
    {
        float money = GameManager.Instance.gameState.playerData.money += amount;
        if (money <= 0)
        {
            Debug.Log("no cash");
        }
    }
    public void UpdateStaff(int amount)
    {
        int staffCount = GameManager.Instance.gameState.playerData.staff += amount;
        if (staffCount <= 0)
        {
            Debug.Log("no bitches");
        }
    }
    public float CalculateIncome()
    {
        float income = 0;
        foreach (Country country in GameManager.Instance.gameState.countries)
        {
            income += incomeMultiplier * (1f - country.GetCensorship()) * country.GetPurchasingPower() * country.GetPopulation() * country.GetReputation();
        }
        return income;
    }
    public float CalculateCosts()
    {
        return UpdateStaffCost() * GameManager.Instance.gameState.playerData.staff;
    }
    public float UpdateStaffCost()
    {
        staffCost = baseStaffCost + UpdateAverageCensorship() * censorshipMarkup;
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
}
