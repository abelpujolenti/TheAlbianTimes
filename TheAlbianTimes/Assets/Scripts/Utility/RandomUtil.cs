using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomUtil : MonoBehaviour
{
    public static bool Roll(int chance)
    {
        return Roll((float)chance);
    }
    public static bool Roll(float chance)
    {
        return Random.Range(0f, 100f) < chance;
    }
}
