using UnityEngine;

namespace Utility
{
    public class StatFormat : MonoBehaviour
    {
        private const string statIncreaseColor = "green";
        private const string statNeutralColor = "#3e3e3e";
        private const string statDecreaseColor = "red";
        public static string FormatValueChange(float value)
        {
            if (value == 0)
            {
                return "<color=" + statNeutralColor + ">(+" + value.ToString("p0") + ")</color>";
            }

            if (value > 0)
            {
                return "<color=" + statIncreaseColor + ">(+" + value.ToString("p0") + ")</color>";
            }
            
            return "<color=" + statDecreaseColor + ">(" + value.ToString("p0") + ")</color>";
        }
    }
}
