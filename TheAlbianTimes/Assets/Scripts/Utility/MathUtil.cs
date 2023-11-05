using UnityEngine;

namespace Utility
{
    public class MathUtil : MonoBehaviour
    {
        public static bool Roll(int chance)
        {
            return Roll((float)chance);
        }
        public static bool Roll(float chance)
        {
            return Random.Range(0f, 100f) < chance;
        }
        
        public static float Map(float value, float originalMin, float originalMax, float newMin, float newMax)
        {
            return newMin + (value - originalMin) * (newMax - newMin) / (originalMax - originalMin);
        }
    }
}
