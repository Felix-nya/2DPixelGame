using System.Collections.Generic;

public class PlayerData
{
    /// <summary>
    /// Количество юнитов у одного игрока, на данный момент ограничение не действует, но в будущем может использоваться.
    /// </summary>
    public const int MaxTeamSize = 100;

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