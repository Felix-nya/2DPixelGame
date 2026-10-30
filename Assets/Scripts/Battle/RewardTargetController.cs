using UnityEngine;

public class RewardTargetController : MonoBehaviour
{
    private static readonly Color ValidColor = new Color(0.3f, 1f, 0.3f, 0.35f);
    private static readonly Color HoverColor = new Color(1f, 0.9f, 0.2f, 0.55f);
    private static readonly Color Dimmed = new Color(0.55f, 0.55f, 0.55f, 1f);

    [SerializeField] private GameManager gameManager;
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private HeroInfoPopup popup;

    private bool IsTargeting =>
        gameManager.State == GameState.Reward
        && gameManager.PendingReward != null
        && gameManager.CurrentRewardPlayer != null;

    private void Start()
    {
        battleManager.HeroSpawned += OnHeroSpawned;
        gameManager.Changed += Refresh;
    }

    private void OnDestroy()
    {
        if (battleManager != null) battleManager.HeroSpawned -= OnHeroSpawned;
        if (gameManager != null) gameManager.Changed -= Refresh;
    }

    private void OnHeroSpawned(Hero hero)
    {
        var pointer = hero.GetComponent<HeroPointer>();
        if (pointer != null)
        {
            pointer.Entered += OnEntered;
            pointer.Exited += OnExited;
            pointer.Clicked += OnClicked;
        }
        ApplyVisuals(hero);
    }
    private void Refresh()
    {
        popup.ClosePopup();
        foreach (var hero in battleManager.Heroes)
            ApplyVisuals(hero);
    }

    private bool IsCandidate(Hero hero)
    {
        return IsTargeting && hero.Team == gameManager.GetTeam(gameManager.CurrentRewardPlayer);
    }

    private void ApplyVisuals(Hero hero)
    {
        hero.HideZone();
        hero.SetHighlight(Color.white);

        if (!IsTargeting) return;

        if (IsCandidate(hero) && gameManager.CanGive(hero.Runtime))
            hero.ShowZone(hero.Runtime.Data.PlacementRadius, ValidColor);
        else
            hero.SetHighlight(Dimmed);
    }

    private void OnEntered(Hero hero)
    {
        if (!IsCandidate(hero) || !gameManager.CanGive(hero.Runtime)) return;
        hero.ShowZone(hero.Runtime.Data.PlacementRadius, HoverColor);
    }

    private void OnExited(Hero hero)
    {
        if (IsCandidate(hero)) ApplyVisuals(hero);
    }

    private void OnClicked(Hero hero)
    {
        if (!IsCandidate(hero)) return;

        Reward reward = gameManager.PendingReward;
        bool canGive = gameManager.CanGive(hero.Runtime);

        popup.Show(hero, $"Дать: {reward.rewardName}", canGive, () => gameManager.ChooseTarget(hero.Runtime));
    }
}