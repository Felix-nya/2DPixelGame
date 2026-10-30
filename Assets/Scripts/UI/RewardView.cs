using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardView : MonoBehaviour
{
    [Header("Выбор героя на арене")]
    [SerializeField] private GameObject targetPanel;
    [SerializeField] private TMP_Text targetText;
    [SerializeField] private Button cancelButton;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private Transform cardsContainer;
    [SerializeField] private CardView cardPrefab;
    [SerializeField] private Sprite cardCommon;
    [SerializeField] private Sprite cardRare;
    [SerializeField] private Sprite cardLegendary;

    private void Start()
    {
        cancelButton.onClick.AddListener(gameManager.CancelPendingReward);
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
        bool rewardState = gameManager.State == GameState.Reward && gameManager.CurrentRewardPlayer != null;
        bool targeting = rewardState && gameManager.PendingReward != null;

        panel.SetActive(rewardState && !targeting);
        targetPanel.SetActive(targeting);

        if (!rewardState) return;

        if (targeting)
        {
            targetText.text = $"{gameManager.CurrentRewardPlayer.Name}, выберите героя для награды «{gameManager.PendingReward.rewardName}»";
            return;
        }

        ClearCards();
        if (gameManager.PendingReward == null)
        {
            titleText.text = $"{gameManager.CurrentRewardPlayer.Name}, выберите награду";
            foreach (var reward in gameManager.CurrentOptions)
            {
                Reward r = reward;
                CreateCard(r.rewardName, r.description, r.icon, r.rare, () => gameManager.ChooseReward(r));
            }
        }
    }

    private void CreateCard(string title, string description, Sprite icon, CardRares rare, System.Action onClick)
    {
        CardView card = Instantiate(cardPrefab, cardsContainer);
        Sprite cardSprite;
        switch (rare) {
            case CardRares.Common:
                cardSprite = cardCommon;
                break;
            case CardRares.Rare:
                cardSprite = cardRare;
                break;
            case CardRares.Legendary:
                cardSprite = cardLegendary;
                break;
            default:
                throw new ArgumentException("Неизвестная редкость карты");
        }
        card.Setup(title, description, icon, cardSprite, onClick);
    }

    private void ClearCards()
    {
        foreach (Transform child in cardsContainer) Destroy(child.gameObject);
    }
}