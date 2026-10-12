using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private TextMeshPro text;
    [SerializeField] private float lifetime = 0.8f;
    [SerializeField] private float riseSpeed = 1.2f;

    private float age;
    private Color baseColor;

    public void Show(string message, Color color, float sizeMultiplier)
    {
        text.text = message;
        text.fontSize *= sizeMultiplier;
        baseColor = color;
        text.color = color;
    }

    private void FixedUpdate()
    {
        // реальное время, чтобы цифры читались и на ускоренной скорости
        float dt = Time.fixedDeltaTime;
        age += dt;

        transform.position += Vector3.up * (riseSpeed * dt);

        // первую половину жизни текст непрозрачный, затем тает
        float fade = Mathf.Clamp01((age - lifetime * 0.5f) / (lifetime * 0.5f));
        Color c = baseColor;
        c.a = 1f - fade;
        text.color = c;

        if (age >= lifetime)
            Destroy(gameObject);
    }
}