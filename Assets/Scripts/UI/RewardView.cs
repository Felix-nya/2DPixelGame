using TMPro;
using UnityEngine;

public class RewardView : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject panel;          // панель с карточками (дочерний объект)
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
        bool show = gameManager.State == GameState.Reward && gameManager.CurrentRewardPlayer != null;
        panel.SetActive(show);
        if (!show) return;

        ClearCards();

        if (gameManager.PendingReward == null)
        {
            titleText.text = $"{gameManager.CurrentRewardPlayer.Name}, выберите награду";

            foreach (var reward in gameManager.CurrentOptions)
            {
                Reward r = reward;   // копия для замыкания
                CreateCard(r.rewardName, r.description, r.icon, () => gameManager.ChooseReward(r));
            }
        }
        else
        {
            titleText.text = $"{gameManager.PendingReward.rewardName}: кому дать?";

            foreach (var hero in gameManager.GetValidTargets(gameManager.PendingReward))
            {
                HeroRuntimeData h = hero;
                string info = $"Здоровье: {h.Stats.maxHealth:0}\nУрон: {h.Stats.damage:0}\nПредметы: {h.ItemsText}";
                CreateCard(h.Data.heroName, info, HeroSprite(h), () => gameManager.ChooseTarget(h));
            }
        }
    }

    private void CreateCard(string title, string description, Sprite icon, System.Action onClick)
    {
        CardView card = Instantiate(cardPrefab, cardsContainer);
        card.Setup(title, description, icon, onClick);
    }

    private void ClearCards()
    {
        foreach (Transform child in cardsContainer)
            Destroy(child.gameObject);
    }

    private Sprite HeroSprite(HeroRuntimeData hero)
    {
        bool isLeft = gameManager.CurrentRewardPlayer == gameManager.Player1;
        Sprite sprite = isLeft ? hero.Data.spriteLeftTeam : hero.Data.spriteRightTeam;
        return sprite != null ? sprite : hero.Data.spriteLeftTeam;
    }
}