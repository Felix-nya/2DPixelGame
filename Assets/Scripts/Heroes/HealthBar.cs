using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Transform background;
    [SerializeField] private Transform fill;
    [SerializeField] private float width = 0.9f;
    [SerializeField] private float height = 0.12f;

    private SpriteRenderer fillRenderer;

    private void Awake()
    {
        fillRenderer = fill.GetComponent<SpriteRenderer>();
        background.localScale = new Vector3(width, height, 1f);
        SetRatio(1f);
    }

    public void SetRatio(float ratio)
    {
        ratio = Mathf.Clamp01(ratio);

        // заливка сжимается к левому краю
        fill.localScale = new Vector3(width * ratio, height, 1f);
        fill.localPosition = new Vector3(-width / 2f + width * ratio / 2f, 0f, 0f);
    }

    public void SetColor(Color color)
    {
        fillRenderer.color = color;
    }
}