public class StatModifier
{
    public StatType Stat { get; }
    public float Flat { get; }
    public float Percent { get; }
    public float Remaining { get; set; }
    public StatModifier(StatType stat, float flat, float percent, float duration)
    {
        Stat = stat;
        Flat = flat;
        Percent = percent;
        Remaining = duration;
    }
}