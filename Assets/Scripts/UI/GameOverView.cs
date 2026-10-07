using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverView : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text winnerText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;

    private void Start()
    {
        gameManager.Changed += Refresh;
        restartButton.onClick.AddListener(Restart);
        menuButton.onClick.AddListener(() => SceneManager.LoadScene(SceneNames.MainMenu));
        Refresh();
    }

    private void OnDestroy()
    {
        if (gameManager != null)
            gameManager.Changed -= Refresh;
    }

    private void Refresh()
    {
        bool show = gameManager.State == GameState.GameOver && gameManager.Winner != null;
        panel.SetActive(show);

        if (show)
            winnerText.text = $"Победил {gameManager.Winner.Name}!";
    }

    private void Restart()
    {
        SceneManager.LoadScene(SceneNames.Game);
    }
}