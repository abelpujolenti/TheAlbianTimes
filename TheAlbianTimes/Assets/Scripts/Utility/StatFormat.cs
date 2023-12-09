using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatFormat : MonoBehaviour
{
    private const string statIncreaseColor = "green";
    private const string statDecreaseColor = "red";
    public static string FormatValueChange(float value)
    {
        return value >= 0 ? "<color=" + statIncreaseColor + ">(+" + value.ToString("p0") + ")</color>" : "<color=" + statDecreaseColor + ">(" + value.ToString("p0") + ")</color>";
    }
}
