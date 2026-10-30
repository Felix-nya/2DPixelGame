using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PlacementZone : MonoBehaviour
{
    private BoxCollider2D box;

    private BoxCollider2D Box
    {
        get
        {
            if (box == null) box = GetComponent<BoxCollider2D>();
            return box;
        }
    }

    public Bounds Bounds => Box.bounds;
    public bool Contains(Vector2 point, float margin)
    {
        Bounds b = Box.bounds;
        return point.x >= b.min.x + margin && point.x <= b.max.x - margin
            && point.y >= b.min.y + margin && point.y <= b.max.y - margin;
    }
}