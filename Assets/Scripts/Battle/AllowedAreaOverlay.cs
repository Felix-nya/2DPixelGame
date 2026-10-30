using UnityEngine;

public class AllowedAreaOverlay : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Sprite squareSprite;
    [SerializeField] private Color color = new Color(1f, 0.2f, 0.2f, 0.3f);
    [SerializeField] private int sortingOrder = 1;

    private readonly SpriteRenderer[] strips = new SpriteRenderer[4];

    private void Awake()
    {
        if (cam == null) cam = Camera.main;

        for (int i = 0; i < strips.Length; i++)
        {
            var go = new GameObject("Strip" + i);
            go.transform.SetParent(transform, false);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = squareSprite;
            sr.color = color;
            sr.sortingOrder = sortingOrder;
            strips[i] = sr;
        }
        Hide();
    }

    public void Show(Bounds allowed)
    {
        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;
        Vector3 c = cam.transform.position;

        float left = c.x - halfWidth - 1f;
        float right = c.x + halfWidth + 1f;
        float bottom = c.y - halfHeight - 1f;
        float top = c.y + halfHeight + 1f;

        float x0 = Mathf.Clamp(allowed.min.x, left, right);
        float x1 = Mathf.Clamp(allowed.max.x, left, right);
        float y0 = Mathf.Clamp(allowed.min.y, bottom, top);
        float y1 = Mathf.Clamp(allowed.max.y, bottom, top);

        SetStrip(0, left, bottom, x0, top);
        SetStrip(1, x1, bottom, right, top);
        SetStrip(2, x0, bottom, x1, y0);
        SetStrip(3, x0, y1, x1, top);
    }

    public void Hide()
    {
        foreach (var sr in strips)
        {
            if (sr != null) sr.gameObject.SetActive(false);
        }
    }

    private void SetStrip(int index, float x0, float y0, float x1, float y1)
    {
        SpriteRenderer sr = strips[index];
        float width = x1 - x0;
        float height = y1 - y0;

        sr.gameObject.SetActive(width > 0.001f && height > 0.001f);
        sr.transform.position = new Vector3((x0 + x1) / 2f, (y0 + y1) / 2f, 0f);
        sr.transform.localScale = new Vector3(width, height, 1f);
    }
}