public readonly struct DamagePacket
{
    public readonly float Amount;
    public readonly bool IsCrit;
    public readonly DamageSource Source;

    public DamagePacket(float amount, bool isCrit, DamageSource source)
    {
        Amount = amount;
        IsCrit = isCrit;
        Source = source;
    }
}