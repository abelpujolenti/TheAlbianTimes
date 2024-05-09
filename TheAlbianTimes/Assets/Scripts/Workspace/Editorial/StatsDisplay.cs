using System;
using System.Collections;
using Managers;
using TMPro;
using UnityEngine;

namespace Workspace.Editorial
{
    public class StatsDisplay : MonoBehaviour
    {
        private const float TIME_TO_FADE = 1.5f;
        private const float DISTANCE_TO_TRAVEL = 0.75f;
        
        private static readonly string[] units = { "", "k", "M", "B", "T" };

        [SerializeField] private TextMeshProUGUI moneyText;
        [SerializeField] private TextMeshProUGUI _moneyReceivedText;
        [SerializeField] private TextMeshProUGUI staffText;
        [SerializeField] private TextMeshProUGUI reputationText;

        private Coroutine _fadeCoroutine;

        void Start()
        {
            GameManager.Instance.SetStatsDisplay(this);

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

        public void ReceiveMoney(float money)
        {
            _moneyReceivedText.gameObject.transform.position = moneyText.gameObject.transform.position;

            _moneyReceivedText.text = "+" + FormatThreeDigitFloat(money);

            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
            }
            
            _fadeCoroutine = StartCoroutine(FadeOut());
        }

        private IEnumerator FadeOut()
        {
            float time = 0;

            Transform textTransform = _moneyReceivedText.gameObject.transform;
            Color color = Color.green;
            Vector3 initialPosition = textTransform.position;
            float initialPositionY = initialPosition.y;
            float positionY;

            _moneyReceivedText.color = color;
            
            while (time < TIME_TO_FADE)
            {
                time += Time.deltaTime;
                
                color.a = Mathf.Lerp(1, 0, time / TIME_TO_FADE);
                positionY = Mathf.Lerp(initialPositionY, initialPositionY - DISTANCE_TO_TRAVEL, Mathf.Sqrt(time / TIME_TO_FADE));

                textTransform.position = new Vector3(initialPosition.x, positionY, initialPosition.z);
                _moneyReceivedText.color = color;
                yield return null;
            }

            color.a = 0;
            _moneyReceivedText.color = color;
        }
    }
}
