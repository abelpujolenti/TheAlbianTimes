using System.Collections.Generic;

public class GameStatePlayerData
{
    public float money = 1000f;
    public int staff = 10;
    public float reputation = .5f;
}

public class GameState
{
    public GameStatePlayerData playerData;
    public Country[] countries;
    public Character[] characters;
    public HashSet<string> viewedArticles;
    public HashSet<string> viewedDialogue;
}
