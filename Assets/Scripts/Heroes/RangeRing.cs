using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RangeRing : MonoBehaviour
{
    [SerializeField, Min(8)] private int segments = 64;
    [SerializeField] private float lineWidth = 0.05f;
    [SerializeField] private Color color = new Color(1f, 1f, 1f, 0.9f);
    [SerializeField] private int sortingOrder = 30;

    private LineRenderer line;

    private LineRenderer Line
    {
        get
        {
            if (line == null)
            {
                line = GetComponent<LineRenderer>();
                line.loop = true;
                line.useWorldSpace = false;
                line.positionCount = segments;
                line.widthMultiplier = lineWidth;
                line.startColor = color;
                line.endColor = color;
                line.sortingOrder = sortingOrder;
                line.material = new Material(Shader.Find("Sprites/Default"));
                line.enabled = false;
            }
            return line;
        }
    }

    public void Show(float radius)
    {
        for (int i = 0; i < segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;
            Line.SetPosition(i, new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f));
        }
        Line.enabled = true;
    }

    public void Hide()
    {
        Line.enabled = false;
    }
}