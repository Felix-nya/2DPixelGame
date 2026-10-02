using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private HeroData startHeroPlayer1;   // временно, до экрана выбора героя
    [SerializeField] private HeroData startHeroPlayer2;
    [SerializeField] private int startingLives = 3;
    [SerializeField] private float pauseAfterBattle = 2f;

    public PlayerData Player1 { get; private set; }
    public PlayerData Player2 { get; private set; }
    public GameState State { get; private set; }
    public int Round { get; private set; }

    private string winnerText = "";

    private void Start()
    {
        Player1 = new PlayerData("Игрок 1", startingLives);
        Player2 = new PlayerData("Игрок 2", startingLives);
        Player1.Team.Add(startHeroPlayer1);
        Player2.Team.Add(startHeroPlayer2);

        battleManager.BattleEnded += OnBattleEnded;
        StartBattle();
    }

    private void StartBattle()
    {
        Round++;
        State = GameState.Battle;
        battleManager.StartBattle(Player1.Team, Player2.Team);
    }

    private void OnBattleEnded(BattleResult result)
    {
        // проигравший бой теряет жизнь, при ничьей никто
        if (result == BattleResult.LeftWon) Player2.LoseLife();
        else if (result == BattleResult.RightWon) Player1.LoseLife();

        if (!Player1.IsAlive || !Player2.IsAlive)
        {
            State = GameState.GameOver;
            winnerText = Player1.IsAlive ? $"Победил {Player1.Name}!" : $"Победил {Player2.Name}!";
            Debug.Log(winnerText);
            return;
        }

        State = GameState.Reward;
        StartCoroutine(RewardPhase());
    }

    private IEnumerator RewardPhase()
    {
        // TODO этап 5: выбор наград. Пока просто пауза.
        yield return new WaitForSeconds(pauseAfterBattle);
        StartBattle();
    }

    // Временный интерфейс, настоящий сделаем на этапе 6
    private void OnGUI()
    {
        GUI.skin.label.fontSize = 18;
        GUI.skin.button.fontSize = 18;

        GUI.Label(new Rect(10, 10, 500, 30), $"Раунд {Round}   Состояние: {State}");
        GUI.Label(new Rect(10, 40, 500, 30), $"{Player1.Name}: жизни {Player1.Lives}");
        GUI.Label(new Rect(10, 70, 500, 30), $"{Player2.Name}: жизни {Player2.Lives}");

        if (State == GameState.GameOver)
        {
            GUI.Label(new Rect(10, 110, 500, 30), winnerText);
            if (GUI.Button(new Rect(10, 150, 200, 40), "Играть снова"))
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}