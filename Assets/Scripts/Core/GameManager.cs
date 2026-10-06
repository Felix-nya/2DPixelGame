using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private HeroData startHeroPlayer1;   // временно, до экрана выбора героя
    [SerializeField] private HeroData startHeroPlayer2;
    [SerializeField] private int startingLives = 3;
    [SerializeField] private List<Reward> rewardPool = new List<Reward>();
    [SerializeField] private int rewardChoices = 3;

    public PlayerData Player1 { get; private set; }
    public PlayerData Player2 { get; private set; }
    public GameState State { get; private set; }
    public int Round { get; private set; }
    public PlayerData CurrentRewardPlayer => currentRewardPlayer;
    public IReadOnlyList<Reward> CurrentOptions => currentOptions;
    public Reward PendingReward => pendingReward;

    public event System.Action Changed;

    private string winnerText = "";

    private readonly Queue<PlayerData> rewardQueue = new Queue<PlayerData>();
    private PlayerData loser;
    private PlayerData currentRewardPlayer;
    private List<Reward> currentOptions = new List<Reward>();
    private Reward pendingReward;   // награда, для которой ждём выбор героя

    private void Start()
    {
        Player1 = new PlayerData("Игрок 1", startingLives);
        Player2 = new PlayerData("Игрок 2", startingLives);
        Player1.AddHero(startHeroPlayer1);
        Player2.AddHero(startHeroPlayer2);

        battleManager.BattleEnded += OnBattleEnded;
        StartBattle();
    }

    private void StartBattle()
    {
        Round++;
        State = GameState.Battle;
        currentRewardPlayer = null;
        battleManager.StartBattle(Player1.Team, Player2.Team);
        NotifyChanged();
    }

    private void OnBattleEnded(BattleResult result)
    {
        // проигравший бой теряет жизнь, при ничьей никто
        loser = null;
        if (result == BattleResult.LeftWon) { Player2.LoseLife(); loser = Player2; }
        else if (result == BattleResult.RightWon) { Player1.LoseLife(); loser = Player1; }

        if (!Player1.IsAlive || !Player2.IsAlive)
        {
            State = GameState.GameOver;
            winnerText = Player1.IsAlive ? $"Победил {Player1.Name}!" : $"Победил {Player2.Name}!";
            Debug.Log(winnerText);
            NotifyChanged();
            return;
        }

        StartRewardPhase();
    }

    // ---------- Награды ----------

    private void StartRewardPhase()
    {
        State = GameState.Reward;
        rewardQueue.Clear();
        rewardQueue.Enqueue(Player1);
        rewardQueue.Enqueue(Player2);
        NextRewardPlayer();
    }

    private void NextRewardPlayer()
    {
        if (rewardQueue.Count == 0)
        {
            StartBattle();
            return;
        }

        currentRewardPlayer = rewardQueue.Dequeue();
        pendingReward = null;

        // проигравший бой получает на один вариант больше
        int count = rewardChoices + (currentRewardPlayer == loser ? 1 : 0);
        currentOptions = RewardGenerator.Generate(rewardPool, currentRewardPlayer, count);
        NotifyChanged();

        if (currentOptions.Count == 0)
            NextRewardPlayer();   // предлагать нечего, переходим к следующему
    }

    public void ChooseReward(Reward reward)
    {
        if (!reward.NeedsHeroTarget)
        {
            reward.Apply(currentRewardPlayer, null);
            NextRewardPlayer();
        }
        else if (currentRewardPlayer.Team.Count == 1)
        {
            reward.Apply(currentRewardPlayer, currentRewardPlayer.Team[0]);
            NextRewardPlayer();
        }
        else
        {
            pendingReward = reward;   // ждём, пока игрок выберет героя
            NotifyChanged();
        }
    }

    public void ChooseTarget(HeroRuntimeData hero)
    {
        pendingReward.Apply(currentRewardPlayer, hero);
        NextRewardPlayer();
    }

    // ---------- Временный интерфейс (настоящий сделаем на этапе 6) ----------

    private void OnGUI()
    {
        GUI.skin.label.fontSize = 18;
        GUI.skin.button.fontSize = 18;

        if (State == GameState.GameOver)
        {
            GUI.Label(new Rect(10, 110, 500, 30), winnerText);
            if (GUI.Button(new Rect(10, 150, 200, 40), "Играть снова"))
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void NotifyChanged()
    {
        Changed?.Invoke();
    }
}