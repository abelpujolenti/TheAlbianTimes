using System;
using System.IO;
using System.Linq;
using Characters;
using Countries;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class SaveFile
{
    public CountryData[] countryData;
    public CharacterData[] characterData;
    public GameStatePlayerData playerData;
    public string[] viewedArticles;
    public string[] publishedArticles;

    public SaveFile()
    {
        countryData = new CountryData[0];
        characterData = new CharacterData[0];
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
        characterData = new CharacterData[gameState.characters.Length];
        for (int i = 0; i < gameState.characters.Length; i++)
        {
            if (gameState.characters[i] == null) continue;
            characterData[i] = gameState.characters[i].data;
        }
        viewedArticles = gameState.viewedArticles.ToArray();
        publishedArticles = gameState.viewedDialogue.ToArray();
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
