using System;

[Serializable]
public class ThreatCountryEvent : CountryEvent
{
    public string title;
    public string text;

    public override void Run()
    {
        base.Run();

    }
}
