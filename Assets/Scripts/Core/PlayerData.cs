using System.Collections.Generic;

public class PlayerData
{
    public string Name { get; }
    public int Lives { get; private set; }
    public List<HeroData> Team { get; } = new List<HeroData>();
    public bool IsAlive => Lives > 0;

    public PlayerData(string name, int lives)
    {
        Name = name;
        Lives = lives;
    }

    public void LoseLife()
    {
        Lives--;
    }
}