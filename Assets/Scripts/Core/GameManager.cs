using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private List<HeroData> availableHeroes = new List<HeroData>();
    [SerializeField] private int startingLives = 3;
    [SerializeField] private List<Reward> rewardPool = new List<Reward>();
    [SerializeField] private int rewardChoices = 3;

    public PlayerData Player1 { get; private set; }
    public PlayerData Player2 { get; private set; }
    public GameState State { get; private set; }
    public int Round { get; private set; }
    public PlayerData Winner { get; private set; }

    // для экрана выбора героя
    public IReadOnlyList<HeroData> StartingHeroes => availableHeroes;
    public PlayerData CurrentPickPlayer => currentPickPlayer;

    // для экрана выбора награды
    public PlayerData CurrentRewardPlayer => currentRewardPlayer;
    public IReadOnlyList<Reward> CurrentOptions => currentOptions;
    public Reward PendingReward => pendingReward;

    public event System.Action Changed;

    private readonly Queue<PlayerData> rewardQueue = new Queue<PlayerData>();
    private PlayerData loser;
    private PlayerData currentPickPlayer;
    private PlayerData currentRewardPlayer;
    private List<Reward> currentOptions = new List<Reward>();
    private Reward pendingReward;   // награда, для которой ждём выбор героя

    private void Start()
    {
        Player1 = new PlayerData("Игрок 1", startingLives);
        Player2 = new PlayerData("Игрок 2", startingLives);

        battleManager.BattleEnded += OnBattleEnded;
        StartHeroSelect();
    }

    private void NotifyChanged()
    {
        Changed?.Invoke();
    }

    // ---------- Выбор стартового героя ----------

    private void StartHeroSelect()
    {
        State = GameState.HeroSelect;
        currentPickPlayer = Player1;
        NotifyChanged();
    }

    public void PickStartHero(HeroData hero)
    {
        currentPickPlayer.AddHero(hero);

        if (currentPickPlayer == Player1)
        {
            currentPickPlayer = Player2;
            NotifyChanged();
        }
        else
        {
            currentPickPlayer = null;
            StartBattle();
        }
    }

    // ---------- Бой ----------

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
            Winner = Player1.IsAlive ? Player1 : Player2;
            Debug.Log($"Победил {Winner.Name}!");
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
            return;
        }

        var targets = GetValidTargets(reward);

        if (targets.Count == 1)
        {
            // подходит один герой, выбирать не из чего
            reward.Apply(currentRewardPlayer, targets[0]);
            NextRewardPlayer();
        }
        else if (targets.Count > 1)
        {
            pendingReward = reward;   // ждём, пока игрок выберет героя
            NotifyChanged();
        }
        // если подходящих героев нет, награда не попала бы в предложения (см. IsAvailable)
    }

    public void ChooseTarget(HeroRuntimeData hero)
    {
        pendingReward.Apply(currentRewardPlayer, hero);
        NextRewardPlayer();
    }

    // герои текущего игрока, которым можно дать эту награду
    public List<HeroRuntimeData> GetValidTargets(Reward reward)
    {
        var result = new List<HeroRuntimeData>();
        foreach (var hero in currentRewardPlayer.Team)
        {
            if (reward.CanTarget(hero))
                result.Add(hero);
        }
        return result;
    }
}