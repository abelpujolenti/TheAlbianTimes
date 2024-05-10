using System;
using Events;
using Managers;

namespace Dialogue
{
    [Serializable]
    public class DialogueData
    {
        public string name;

        public int firstRound;
        public float duration;
        public float priority;
        public string audioName;
        public CountryEventCondition[] countryConditions;
        public CharacterEventCondition[] characterConditions;

        public DialogueLine[] lines;

        public bool ConditionsFulfilled(int round)
        {
            if (GameManager.Instance.gameState.viewedDialogue.Contains(name) || round < firstRound || round >= firstRound + duration) return false;
            if (countryConditions == null && characterConditions == null) return true;
            bool ret = true;
            foreach (CountryEventCondition condition in countryConditions)
            {
                if (!condition.IsFulfilled())
                {
                    ret = false;
                }
            }
            foreach (CharacterEventCondition condition in characterConditions)
            {
                if (!condition.IsFulfilled())
                {
                    ret = false;
                }
            }
            return ret;
        }
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
        public string mood;
        public int followupLines;
        public CountryEventCondition[] countryConditions;
        public CharacterEventCondition[] characterConditions;
    }
}