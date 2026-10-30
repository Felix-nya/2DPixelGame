using UnityEngine;

public class ZoneMarker : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private SpriteRenderer Renderer
    {
        get
        {
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            return spriteRenderer;
        }
    }

    public void Show(float halfSize, Color color)
    {
        transform.localScale = new Vector3(halfSize * 2f, halfSize * 2f, 1f);
        Renderer.color = color;
        gameObject.SetActive(true);
    }

    public void SetColor(Color color)
    {
        Renderer.color = color;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}