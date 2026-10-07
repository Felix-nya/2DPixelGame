using TMPro;
using UnityEngine;

public class HeroSelectView : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private Transform cardsContainer;
    [SerializeField] private CardView cardPrefab;

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
        bool show = gameManager.State == GameState.HeroSelect && gameManager.CurrentPickPlayer != null;
        panel.SetActive(show);
        if (!show) return;

        foreach (Transform child in cardsContainer)
            Destroy(child.gameObject);

        titleText.text = $"{gameManager.CurrentPickPlayer.Name}, выберите героя";
        bool isLeft = gameManager.CurrentPickPlayer == gameManager.Player1;

        foreach (var hero in gameManager.StartingHeroes)
        {
            HeroData h = hero;   // копия для замыкания
            Stats s = h.baseStats;
            string ability = h.ability != null ? h.ability.abilityName : "нет";
            string info = $"Здоровье: {s.maxHealth:0}\nУрон: {s.damage:0}\nБроня: {s.armor:0}\nСпособность: {ability}";

            Sprite sprite = isLeft ? h.spriteLeftTeam : h.spriteRightTeam;
            if (sprite == null) sprite = h.spriteLeftTeam;

            CardView card = Instantiate(cardPrefab, cardsContainer);
            card.Setup(h.heroName, info, sprite, () => gameManager.PickStartHero(h));
        }
    }
}