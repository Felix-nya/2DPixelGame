using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroInfoPopup : MonoBehaviour
{
    [SerializeField] private GameObject targetPanel;
    [SerializeField] private Canvas canvas;
    [SerializeField] private RectTransform window;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text statsText;
    [SerializeField] private TMP_Text itemsText;
    [SerializeField] private Button giveButton;
    [SerializeField] private TMP_Text giveButtonText;
    [SerializeField] private Button closeButton;
    [SerializeField] private float heightOffset = 1f;

    private Action onGive;

    private void Awake()
    {
        window.pivot = new Vector2(0.5f, 0f);
        giveButton.onClick.AddListener(() => onGive?.Invoke());
        closeButton.onClick.AddListener(Hide);
        Hide();
    }

    public void Show(Hero hero, string giveLabel, bool canGive, Action onGive)
    {
        targetPanel.SetActive(false);
        this.onGive = onGive;

        HeroRuntimeData runtime = hero.Runtime;
        Stats s = runtime.Stats;

        titleText.text = runtime.Data.heroName;
        statsText.text = $"Здоровье: {s.maxHealth:0}\nУрон: {s.damage:0}\nБроня: {s.armor:0}\n" +
                         $"Скорость атаки: {s.attackSpeed:0.0#}\nДальность: {s.range:0.0}\n" +
                         $"Крит: {s.critChance:0}% / {s.critDamage:0}%";
        itemsText.text = "Предметы: " + runtime.ItemsText;

        giveButtonText.text = canGive ? giveLabel : "Нельзя дать";
        giveButton.interactable = canGive;

        window.gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(window);
        Reposition(hero);
    }

    public void Hide()
    {
        targetPanel.SetActive(true);
        onGive = null;
        window.gameObject.SetActive(false);
    }

    private void Reposition(Hero hero)
    {
        Camera cam = Camera.main;
        Vector3 screen = cam.WorldToScreenPoint(hero.transform.position + Vector3.up * heightOffset);

        Vector2 size = window.rect.size * canvas.scaleFactor;
        float x = Mathf.Clamp(screen.x, size.x * 0.5f, Screen.width - size.x * 0.5f);
        float y = Mathf.Clamp(screen.y, 0f, Mathf.Max(0f, Screen.height - size.y));

        window.position = new Vector3(x, y, 0f);
    }
}