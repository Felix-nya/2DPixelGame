using UnityEngine;

public class PlacementController : MonoBehaviour
{
    private static readonly Color BlockedColor = new Color(1f, 0.2f, 0.2f, 0.35f);
    private static readonly Color FreeColor = new Color(0.3f, 1f, 0.3f, 0.35f);

    [SerializeField] private GameManager gameManager;
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private PlacementManager placement;
    [SerializeField] private AllowedAreaOverlay overlay;

    private Hero dragged;
    private Vector2 dragOffset;

    private void Start()
    {
        battleManager.HeroSpawned += OnHeroSpawned;
    }

    private void OnDestroy()
    {
        if (battleManager != null)
            battleManager.HeroSpawned -= OnHeroSpawned;
    }

    private void OnHeroSpawned(Hero hero)
    {
        var pointer = hero.GetComponent<HeroPointer>();
        if (pointer == null) return;

        pointer.BeginDrag += OnBeginDrag;
        pointer.Drag += OnDrag;
        pointer.EndDrag += OnEndDrag;
    }

    private bool CanDrag(Hero hero)
    {
        PlayerData placer = gameManager.CurrentPlacementPlayer;
        return gameManager.State == GameState.Placement
            && placer != null
            && hero.Team == gameManager.GetTeam(placer);
    }

    private void OnBeginDrag(Hero hero, Vector2 pointerWorld)
    {
        if (!CanDrag(hero)) return;

        dragged = hero;
        dragOffset = (Vector2)hero.transform.position - pointerWorld;
        ShowHints(hero);
        UpdateValidity(hero);
    }

    private void OnDrag(Hero hero, Vector2 pointerWorld)
    {
        if (dragged != hero) return;

        Vector2 target = pointerWorld + dragOffset;
        hero.transform.position = new Vector3(target.x, target.y, hero.transform.position.z);
        UpdateValidity(hero);
    }

    private void OnEndDrag(Hero hero, Vector2 pointerWorld)
    {
        if (dragged != hero) return;

        PlayerData placer = gameManager.CurrentPlacementPlayer;
        Vector2 position = hero.transform.position;

        if (placement.IsValidPosition(hero.Team, placer, hero.Runtime, position))
        {
            hero.Runtime.Position = position;
            hero.Runtime.IsPlaced = true;
        }
        else
        {
            Vector2 old = hero.Runtime.Position;
            hero.transform.position = new Vector3(old.x, old.y, hero.transform.position.z);
        }

        HideHints(hero);
        dragged = null;
    }

    private void ShowHints(Hero moving)
    {
        overlay.Show(placement.GetAllowedBounds(moving.Team));

        foreach (var other in battleManager.Heroes)
        {
            if (other == moving || other.Team != moving.Team) continue;
            other.ShowZone(other.Runtime.Data.PlacementRadius, BlockedColor);
        }

        moving.ShowZone(moving.Runtime.Data.PlacementRadius, FreeColor);
        moving.ShowRange(moving.Stats.range);
    }

    private void UpdateValidity(Hero moving)
    {
        PlayerData placer = gameManager.CurrentPlacementPlayer;
        bool valid = placement.IsValidPosition(moving.Team, placer, moving.Runtime, moving.transform.position);
        moving.SetZoneColor(valid ? FreeColor : BlockedColor);
    }

    private void HideHints(Hero moving)
    {
        overlay.Hide();
        moving.HideRange();

        foreach (var hero in battleManager.Heroes)
        {
            if (hero.Team == moving.Team) hero.HideZone();
        }
    }
}