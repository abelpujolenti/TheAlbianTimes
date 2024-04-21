using System;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Characters
{
    [Serializable]
    public class CharacterData
    {
        public Character.Id characterId;
        public Dictionary<string, float> values = new Dictionary<string, float>();
        public CharacterData() { }
        public CharacterData(CharacterData d)
        {
            characterId = d.characterId;
            foreach (KeyValuePair<string, float> v in d.values)
            {
                values.Add(v.Key, v.Value);
            }
        }
    }

    public class Character : MonoBehaviour
    {
        #region Names
        public enum Id
        {
            TEST,
            AMOUNT
        }
        public static readonly string[] names = { "test" };
        #endregion

        #region Stats

        [SerializeField] protected HashSet<string> eventIds;
        #endregion

        #region Properties
        public CharacterData data = new CharacterData();
        private CharacterData previousData = new CharacterData();
        #endregion

        private void Awake()
        {
            SetDefaultValues();
        }

        private void SetDefaultValues()
        {
        }

        public string DisplayValues()
        {
            string d = "";
            d += "<b>" + GetName() + ":</b>\n";
            foreach (KeyValuePair<string, float> value in data.values)
            {
                if (value.Value <= 1f && value.Value >= 0f)
                {
                    d += value.Key + ": " + value.Value.ToString("p0") + " " + StatFormat.FormatValueChange(GetValueChange(value.Key)) + "  ";
                }
                else
                {

                }
            }
            Debug.Log(d);
            return d;
        }

        public void SaveRoundData()
        {
            previousData = new CharacterData(data);
        }

        public float GetValueChange(string key)
        {
            if (!data.values.ContainsKey(key) || !previousData.values.ContainsKey(key)) return 0f;
            return data.values[key] - previousData.values[key];
        }

        #region Getters/Setters
        public void AddToValue(string key, float value)
        {
            if (!data.values.ContainsKey(key))
            {
                SetValue(key, value);
            }
            else
            {
                SetValue(key, value + data.values[key]);
            }
        }
        public void SetValue(string key, float value)
        {
            data.values[key] = value;
        }

        public Id GetId()
        {
            return data.characterId;
        }
        public string GetName()
        {
            return names[(int)GetId()];
        }

        #endregion
    }
}