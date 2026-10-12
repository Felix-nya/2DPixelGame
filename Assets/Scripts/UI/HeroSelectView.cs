using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class HeroSelectView : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private Transform cardsContainer;
    [SerializeField] private CardView cardPrefab;
    [SerializeField, Min(1)] private int heroChoices = 3;

    private readonly List<HeroData> options = new();
    private PlayerData optionsOwner;

    private void Start()
    {
        gameManager.Changed += Refresh;
        Refresh();
    }

    private void OnDestroy()
    {
        if (gameManager != null)
            gameManager.Changed -= Refresh;
    }

    private void Refresh()
    {
        PlayerData picker = gameManager.CurrentPickPlayer;
        bool show = gameManager.State == GameState.HeroSelect && picker != null;
        panel.SetActive(show);
        if (!show)
        {
            optionsOwner = null;
            return;
        }
        if (picker != optionsOwner)
        {
            RollOptions();
            optionsOwner = picker;
        }
        titleText.text = $"{picker.Name}, выберите героя";
        bool isLeft = picker == gameManager.Player1;
        foreach (Transform child in cardsContainer) Destroy(child.gameObject);
        foreach (var hero in options) CreateHeroCard(hero, isLeft);
    }

    private void RollOptions()
    {
        var pool = new List<HeroData>(gameManager.StartingHeroes);
        options.Clear();
        int count = Mathf.Min(heroChoices, pool.Count);
        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, pool.Count);
            options.Add(pool[index]);
            pool.RemoveAt(index);
        }
    }

    private void CreateHeroCard(HeroData hero, bool isLeft)
    {
        Stats s = hero.baseStats;
        string ability = hero.ability != null ? hero.ability.abilityName : "нет";
        string info = $"Здоровье: {s.maxHealth:0}\nУрон: {s.damage:0}\nБроня: {s.armor:0}\nКрит: {s.critChance:0}% / {s.critDamage:0}%\nСпособность: {ability}";
        Sprite sprite = isLeft ? hero.spriteLeftTeam : hero.spriteRightTeam;
        if (sprite == null) sprite = hero.spriteLeftTeam;
        CardView card = Instantiate(cardPrefab, cardsContainer);
        card.Setup(hero.heroName, info, sprite, () => gameManager.PickStartHero(hero));
    }
}