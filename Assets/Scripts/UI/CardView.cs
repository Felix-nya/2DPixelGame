using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CardView : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Image cardImage;
    [SerializeField] private Button button;

    public void Setup(string title, string description, Sprite icon, Sprite card, Action onClick)
    {
        cardImage = GetComponent<Image>();
        cardImage.sprite = card;

        titleText.text = title;
        descriptionText.text = description;

        iconImage.sprite = icon;
        iconImage.gameObject.SetActive(icon != null);

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick());
    }
}