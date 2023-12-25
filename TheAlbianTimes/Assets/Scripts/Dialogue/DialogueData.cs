using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueData
{
    public DialogueLine[] lines;
}

[System.Serializable]
public class DialogueLine
{
    public DialoguePart[] parts;
    public DialogueOption[] options;
}

[System.Serializable]
public class DialoguePart
{
    public string speaker;
    public string text;
}

[System.Serializable]
public class DialogueOption
{
    public int id;
    public string text;
    public int followupLines;
    public CountryEventCondition[] conditions;
}
