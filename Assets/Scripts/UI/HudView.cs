using TMPro;
using UnityEngine;

public class HudView : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private TMP_Text roundText;
    [SerializeField] private TMP_Text player1Text;
    [SerializeField] private TMP_Text player2Text;

    private void Start()
    {
        gameManager.Changed += Refresh;
        Refresh();
    }

    private void OnDestroy()
    {
        if (gameManager != null)
            gameManager.Changed -= Refresh;
    }

    private void Refresh()
    {
        if (gameManager.Player1 == null) return;   // игра ещё не инициализирована

        roundText.text = $"Раунд {gameManager.Round}";
        player1Text.text = FormatPlayer(gameManager.Player1);
        player2Text.text = FormatPlayer(gameManager.Player2);
    }

    private static string FormatPlayer(PlayerData player)
    {
        return $"{player.Name}\nЖизни: {player.Lives}   Героев: {player.Team.Count}";
    }
}