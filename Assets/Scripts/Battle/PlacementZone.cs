using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PlacementZone : MonoBehaviour
{
    [SerializeField] private SpriteRenderer forbiddenOverlay;

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

    private void Awake()
    {
        FitOverlay();
        ShowForbidden(false);
    }

    public bool Contains(Vector2 point, float margin)
    {
        Bounds b = Box.bounds;
        return point.x >= b.min.x + margin && point.x <= b.max.x - margin
            && point.y >= b.min.y + margin && point.y <= b.max.y - margin;
    }

    public void ShowForbidden(bool visible)
    {
        if (forbiddenOverlay != null)
            forbiddenOverlay.gameObject.SetActive(visible);
    }

    private void FitOverlay()
    {
        if (forbiddenOverlay == null) return;

        Bounds b = Box.bounds;
        Transform t = forbiddenOverlay.transform;
        t.position = new Vector3(b.center.x, b.center.y, t.position.z);
        t.localScale = new Vector3(b.size.x, b.size.y, 1f);
    }
}