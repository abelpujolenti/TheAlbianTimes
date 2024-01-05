using System;

[Serializable]
public class DialogueData
{
    public DialogueLine[] lines;
}

[Serializable]
public class DialogueLine
{
    public DialoguePart[] parts;
    public DialogueOption[] options;
}

[Serializable]
public class DialoguePart
{
    public string speaker;
    public string text;
}

[Serializable]
public class DialogueOption
{
    public int id;
    public string text;
    public int followupLines;
    public CountryEventCondition[] countryConditions;
    public CharacterEventCondition[] characterConditions;
}
