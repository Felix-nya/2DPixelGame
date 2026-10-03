using System.Collections.Generic;

public class PlayerData
{
    public const int MaxTeamSize = 4;

    public string Name { get; }
    public int Lives { get; private set; }
    public List<HeroRuntimeData> Team { get; } = new List<HeroRuntimeData>();

    public bool IsAlive => Lives > 0;
    public bool TeamIsFull => Team.Count >= MaxTeamSize;

    public PlayerData(string name, int lives)
    {
        Name = name;
        Lives = lives;
    }

    public void LoseLife()
    {
        Lives--;
    }

    public void AddHero(HeroData data)
    {
        if (!TeamIsFull)
            Team.Add(new HeroRuntimeData(data));
    }
}