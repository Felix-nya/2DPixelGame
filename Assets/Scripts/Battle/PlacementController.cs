using UnityEngine;

public class PlacementController : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private PlacementManager placement;

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
        ShowForbiddenAreas(hero, true);
    }

    private void OnDrag(Hero hero, Vector2 pointerWorld)
    {
        if (dragged != hero) return;

        Vector2 target = pointerWorld + dragOffset;
        hero.transform.position = new Vector3(target.x, target.y, hero.transform.position.z);
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

        ShowForbiddenAreas(hero, false);
        dragged = null;
    }

    private void ShowForbiddenAreas(Hero moving, bool visible)
    {
        Team enemy = moving.Team == Team.Left ? Team.Right : Team.Left;
        placement.GetZone(enemy).ShowForbidden(visible);

        foreach (var other in battleManager.Heroes)
        {
            if (other == moving || other.Team != moving.Team) continue;

            if (visible)
                other.ShowForbiddenZone(other.Runtime.Data.PlacementRadius + moving.Runtime.Data.PlacementRadius);
            else
                other.HideForbiddenZone();
        }
    }
}