using UnityEngine;

[CreateAssetMenu(fileName = "ArmorBuffAbility", menuName = "Game/Abilities/Armor Buff")]
public class ArmorBuffAbility : Ability
{
    public float armorBonus = 5f;
    public float duration = 4f;

    public override bool TryUse(Hero caster, BattleManager battle)
    {
        caster.AddArmorBuff(armorBonus, duration);
        return true;
    }
}