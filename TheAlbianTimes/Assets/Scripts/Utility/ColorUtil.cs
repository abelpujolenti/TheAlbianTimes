using UnityEngine;

public class ColorUtil : MonoBehaviour
{
    public static Color SetSaturation(Color c, float saturation)
    {
        float h, s, v;
        Color.RGBToHSV(c, out h, out s, out v);
        s = saturation;
        return Color.HSVToRGB(h, s, v);
    }
    public static Color SetSaturationMultiplicative(Color c, float saturation)
    {
        float h, s, v;
        Color.RGBToHSV(c, out h, out s, out v);
        s *= saturation;
        return Color.HSVToRGB(h, s, v);
    }
    public static Color SetBrightness(Color c, float brightness)
    {
        float h, s, v;
        Color.RGBToHSV(c, out h, out s, out v);
        v = brightness;
        return Color.HSVToRGB(h, s, v);
    }
    public static Color SetBrightnessMultiplicative(Color c, float brightness)
    {
        float h, s, v;
        Color.RGBToHSV(c, out h, out s, out v);
        v *= brightness;
        return Color.HSVToRGB(h, s, v);
    }
    public static float GetBrightness(Color c)
    {
        float h, s, v;
        Color.RGBToHSV(c, out h, out s, out v);
        return v;
    }
    public static float GetSaturation(Color c)
    {
        float h, s, v;
        Color.RGBToHSV(c, out h, out s, out v);
        return s;
    }
}
