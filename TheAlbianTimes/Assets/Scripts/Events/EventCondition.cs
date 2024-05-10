using System;
using Managers;

namespace Events
{
    [Serializable]
    public abstract class EventCondition
    {
        public enum Predicate { EQUAL, LESS, GREATER, LESSEQUAL, GREATEREQUAL }

        public int entityId;
        public string field;
        public float value;
        public Predicate predicate = Predicate.EQUAL;

        public abstract bool IsFulfilled();
        protected bool CheckPredicate(float v)
        {
            return
                predicate == Predicate.EQUAL && v == value ||
                predicate == Predicate.LESS && v < value ||
                predicate == Predicate.GREATER && v > value ||
                predicate == Predicate.LESSEQUAL && v <= value ||
                predicate == Predicate.GREATEREQUAL && v >= value;
        }
    }

    [Serializable]
    public class CharacterEventCondition : EventCondition
    {
        public override bool IsFulfilled()
        {
            if (!GameManager.Instance.gameState.characters[entityId].data.values.ContainsKey(field)) return false;
            return CheckPredicate(GameManager.Instance.gameState.characters[entityId].data.values[field]);
        }
    }

    [Serializable]
    public class CountryEventCondition : EventCondition
    {
        public override bool IsFulfilled()
        {
            if (!GameManager.Instance.gameState.countries[entityId].data.values.ContainsKey(field)) return false;
            return CheckPredicate(GameManager.Instance.gameState.countries[entityId].data.values[field]);
        }
    }
}