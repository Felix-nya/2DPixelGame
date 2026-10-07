using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleSpeedView : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Button speedButton;
    [SerializeField] private TMP_Text speedLabel;
    [SerializeField] private float[] speeds = { 1f, 2f };

    private int speedIndex;

    private void Start()
    {
        gameManager.Changed += Refresh;
        speedButton.onClick.AddListener(NextSpeed);
        Refresh();
    }

    private void OnDestroy()
    {
        if (gameManager != null)
            gameManager.Changed -= Refresh;

        Time.timeScale = 1f;   // timeScale сохраняется между сценами, возвращаем в норму
    }

    private void NextSpeed()
    {
        speedIndex = (speedIndex + 1) % speeds.Length;
        Refresh();
    }

    private void Refresh()
    {
        bool inBattle = gameManager.State == GameState.Battle;
        speedButton.gameObject.SetActive(inBattle);

        // ускорение действует только в бою, в остальных состояниях время обычное
        Time.timeScale = inBattle ? speeds[speedIndex] : 1f;
        speedLabel.text = $"Скорость: x{speeds[speedIndex]:0.#}";
    }
}