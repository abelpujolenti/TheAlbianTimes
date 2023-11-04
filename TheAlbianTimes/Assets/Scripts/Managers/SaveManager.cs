using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class SaveFile
{
    public CountryData[] countryData;
    public GameStatePlayerData playerData;
    public SaveFile()
    {
        countryData = new CountryData[0];
        playerData = new GameStatePlayerData();
    }
    public SaveFile(GameState gameState)
    {
        playerData = gameState.playerData;
        countryData = new CountryData[gameState.countries.Length];
        for (int i = 0; i < gameState.countries.Length; i++)
        {
            if (gameState.countries[i] == null) continue;
            countryData[i] = gameState.countries[i].data;
        }
    }
}

public class SaveManager
{
    public SaveFile save;
    string path = Application.streamingAssetsPath + "/Json/Save/save.json";
    public void SaveToJson(GameState gameState)
    {
        File.WriteAllText(path, JsonConvert.SerializeObject(new SaveFile(gameState)));
    }
    public void LoadFromJson()
    {
        save = JsonConvert.DeserializeObject<SaveFile>(File.ReadAllText(path));
    }
    public bool SaveFileExists()
    {
        return File.Exists(path);
    }
}
