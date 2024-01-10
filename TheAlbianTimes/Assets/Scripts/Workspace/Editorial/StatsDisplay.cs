using System;
using System.Collections.Generic;
using Managers;
using TMPro;
using UnityEngine;

namespace Workspace.Editorial
{
    public class StatsDisplay : MonoBehaviour
    {
        private static readonly string[] units = { "", "k", "M", "B", "T" };

        TextMeshProUGUI moneyText;
        TextMeshProUGUI staffText;
        TextMeshProUGUI reputationText;
        void Start()
        {
            GameManager.Instance.SetStatsDisplay(this);
            moneyText = transform.Find("money").GetComponent<TextMeshProUGUI>();
            staffText = transform.Find("staff").GetComponent<TextMeshProUGUI>();
            reputationText = transform.Find("reputation").GetComponent<TextMeshProUGUI>();

            if (GameManager.Instance.gameState == null) return;
            moneyText.text = FormatThreeDigitFloat(GameManager.Instance.gameState.playerData.money);
            staffText.text = GameManager.Instance.gameState.playerData.staff.ToString("D3");
            reputationText.text = GameManager.Instance.gameState.playerData.reputation.ToString("P0");
        }

        public string FormatThreeDigitFloat(float value)
        {
            int digits = 0;
            int v = (int)value;
            while (v > 0)
            {
                digits++;
                v /= 10;
            }

            int wholeDigits = (digits - 1) % 3;
            int decimalDigits = 2 - wholeDigits;
            int hiddenDigits = Mathf.Max(0, digits - wholeDigits - 1);
            int t = (int)Math.Pow(10, hiddenDigits);
            int d = (int)Math.Pow(10, decimalDigits);
            int wholePart = (int)value / t;
            float decimalPart = ((int)((value / (float)t) * (float)d) % d) / (float)d;
            return ((float)wholePart + decimalPart) + units[(digits - 1) / 3];
        }

        public void UpdateMoney(float money)
        {
            moneyText.text = FormatThreeDigitFloat(money);
        }
    }
}
