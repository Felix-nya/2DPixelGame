public class HeroRuntimeData
{
    public HeroData Data { get; }
    public Stats Stats { get; set; }   // база + все полученные баффы

    public HeroRuntimeData(HeroData data)
    {
        Data = data;
        Stats = data.baseStats;
    }
}