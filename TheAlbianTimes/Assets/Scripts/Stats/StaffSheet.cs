using Countries;
using Managers;
using TMPro;
using UnityEngine;

namespace Stats
{
    public class StaffSheet : MonoBehaviour
    {
        private const string CLICK_BUTTON_SOUND = "Click Button";
        private const string PRESS_LIGHTSWITCH_SOUND = "Press Lightswitch";
        
        [SerializeField] TextMeshProUGUI staff;
        [SerializeField] TextMeshProUGUI cost;
        float staffCost = PlayerDataManager.baseStaffCost;
        float censorshipAverage = 0f;
        int staffChange = 0;

        private void Start()
        {
            if (GameManager.Instance.GetRound() == 0)
            {
                gameObject.SetActive(false);
                return;
            }
            UpdateStaffCost();
            UpdateText();
        }

        public void HireStaff()
        {
            AudioManager.Instance.Play2DSound(CLICK_BUTTON_SOUND);
            staffChange++;
            UpdateText();
        }
        public void FireStaff()
        {
            AudioManager.Instance.Play2DSound(CLICK_BUTTON_SOUND);
            staffChange--;
            UpdateText();
        }
        public void ApplyStaffChange()
        {
            AudioManager.Instance.Play3DSound(PRESS_LIGHTSWITCH_SOUND, 5, 100, transform.position);
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
            string text = "<color=red>Cost: " + (staffCost * (GameManager.Instance.gameState.playerData.staff + staffChange)).ToString("F2") + " / day</color>";
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
}
