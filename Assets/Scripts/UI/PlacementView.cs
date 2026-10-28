using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlacementView : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private Button readyButton;

    private void Start()
    {
        gameManager.Changed += Refresh;
        readyButton.onClick.AddListener(gameManager.ConfirmPlacement);
        Refresh();
    }

    private void OnDestroy()
    {
        if (gameManager != null)
            gameManager.Changed -= Refresh;
    }

    private void Refresh()
    {
        PlayerData placer = gameManager.CurrentPlacementPlayer;
        bool show = gameManager.State == GameState.Placement && placer != null;
        panel.SetActive(show);

        if (show)
            titleText.text = $"{placer.Name}, расставьте героев";
    }
}