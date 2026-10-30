using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    [SerializeField] private PlacementZone leftZone;
    [SerializeField] private PlacementZone rightZone;
    [SerializeField, Min(0f)] private float edgeMargin = 0.3f;
    [SerializeField, Min(0.1f)] private float autoPlaceStep = 0.5f;

    public PlacementZone GetZone(Team team) => team == Team.Left ? leftZone : rightZone;

    public Bounds GetAllowedBounds(Team team)
    {
        Bounds b = GetZone(team).Bounds;
        b.Expand(new Vector3(-edgeMargin * 2f, -edgeMargin * 2f, 0f));
        return b;
    }

    public bool IsValidPosition(Team team, PlayerData player, HeroRuntimeData hero, Vector2 position)
    {
        if (!GetZone(team).Contains(position, edgeMargin)) return false;

        foreach (var other in player.Team)
        {
            if (other == hero || !other.IsPlaced) continue;

            float minDistance = hero.Data.PlacementRadius + other.Data.PlacementRadius;
            Vector2 delta = position - other.Position;
            if (Mathf.Abs(delta.x) < minDistance && Mathf.Abs(delta.y) < minDistance) return false;
        }
        return true;
    }

    public void AutoPlace(PlayerData player, Team team)
    {
        foreach (var hero in player.Team)
        {
            if (hero.IsPlaced) continue;

            hero.Position = FindFreePosition(team, player, hero);
            hero.IsPlaced = true;
        }
    }

    private Vector2 FindFreePosition(Team team, PlayerData player, HeroRuntimeData hero)
    {
        Bounds b = GetZone(team).Bounds;
        float step = autoPlaceStep;

        float frontX = team == Team.Left ? b.max.x - edgeMargin : b.min.x + edgeMargin;
        float direction = team == Team.Left ? -1f : 1f;
        int columns = Mathf.CeilToInt(b.size.x / step);
        int halfRows = Mathf.CeilToInt(b.extents.y / step);

        for (int c = 0; c <= columns; c++)
        {
            float x = frontX + direction * c * step;
            for (int r = 0; r <= halfRows; r++)
            {
                for (int sign = 1; sign >= -1; sign -= 2)
                {
                    if (r == 0 && sign == -1) continue;

                    var candidate = new Vector2(x, b.center.y + sign * r * step);
                    if (IsValidPosition(team, player, hero, candidate)) return candidate;
                }
            }
        }

        Debug.LogWarning("В зоне нет свободного места для героя");
        return b.center;
    }
}