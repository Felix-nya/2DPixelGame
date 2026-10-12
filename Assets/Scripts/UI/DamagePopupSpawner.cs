using UnityEngine;

public class DamagePopupSpawner : MonoBehaviour
{
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private FloatingText prefab;
    [SerializeField] private Color damageColor = Color.white;
    [SerializeField] private Color critColor = new Color(1f, 0.8f, 0.1f);
    [SerializeField] private Color healColor = new Color(0.3f, 1f, 0.3f);
    [SerializeField] private float heightOffset = 0.9f;

    private void Start()
    {
        battleManager.HeroDamaged += OnDamaged;
        battleManager.HeroHealed += OnHealed;
    }

    private void OnDestroy()
    {
        if (battleManager == null) return;
        battleManager.HeroDamaged -= OnDamaged;
        battleManager.HeroHealed -= OnHealed;
    }

    private void OnDamaged(Hero hero, float amount, bool isCrit)
    {
        int value = Mathf.RoundToInt(amount);
        if (value <= 0) return;

        string message = isCrit ? $"-{value}!" : $"-{value}";
        Spawn(hero, message, isCrit ? critColor : damageColor, isCrit ? 1.5f : 1f);
    }

    private void OnHealed(Hero hero, float amount)
    {
        int value = Mathf.RoundToInt(amount);
        if (value <= 0) return;

        Spawn(hero, $"+{value}", healColor, 1f);
    }

    private void Spawn(Hero hero, string message, Color color, float sizeMultiplier)
    {
        // небольшой случайный сдвиг по горизонтали, чтобы цифры не слипались
        Vector3 offset = new Vector3(Random.Range(-0.25f, 0.25f), heightOffset, 0f);

        FloatingText popup = Instantiate(prefab, hero.transform.position + offset, Quaternion.identity);
        popup.Show(message, color, sizeMultiplier);
    }
}