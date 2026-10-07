using TMPro;
using UnityEngine;

public class BattleTimerView : MonoBehaviour
{
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private TMP_Text timerText;

    private int shownSeconds = -1;

    private void Update()
    {
        bool active = battleManager.IsActive;
        timerText.gameObject.SetActive(active);

        if (!active)
        {
            shownSeconds = -1;
            return;
        }

        int seconds = Mathf.CeilToInt(battleManager.TimeLeft);
        if (seconds == shownSeconds) return;   // текст меняем только когда изменилась секунда

        shownSeconds = seconds;
        timerText.text = $"Время: {seconds} с";
    }
}