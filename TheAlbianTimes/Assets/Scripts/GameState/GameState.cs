using System.Collections.Generic;
using System.Linq;
using Characters;
using Countries;

public class GameStatePlayerData
{
    public float money = 1000f;
    public int staff = 10;
    public float reputation = .5f;
}

public class GameState
{
    public int currRound;
    public GameStatePlayerData playerData;
    public Country[] countries;
    public Character[] characters;
    public HashSet<string> viewedArticles;
    public HashSet<string> viewedDialogue;
    public GameState Clone()
    {
        GameState ret = new GameState();
        ret.currRound = currRound;
        ret.playerData = new GameStatePlayerData();
        ret.playerData.money = playerData.money;
        ret.playerData.staff = playerData.staff;
        ret.playerData.reputation = playerData.reputation;
        ret.countries = countries.ToArray();
        ret.characters = characters.ToArray();
        ret.viewedArticles = viewedArticles.ToHashSet();
        ret.viewedDialogue = viewedDialogue.ToHashSet();
        return ret;
    }
}
