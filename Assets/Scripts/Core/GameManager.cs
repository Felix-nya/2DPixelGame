using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private List<HeroData> availableHeroes = new();
    [SerializeField] private int startingLives = 3;
    [SerializeField] private List<Reward> rewardPool = new();
    [SerializeField] private int rewardChoices = 3;
    [SerializeField] private RarityWeights rarityWeights = new();
    [SerializeField] private PlacementManager placement;

    public PlayerData Player1 { get; private set; }
    public PlayerData Player2 { get; private set; }
    public GameState State { get; private set; }
    public int Round { get; private set; }
    public PlayerData Winner { get; private set; }
    public PlayerData CurrentPlacementPlayer => currentPlacementPlayer;

    public Team GetTeam(PlayerData player) => player == Player1 ? Team.Left : Team.Right;

    private PlayerData currentPlacementPlayer;

    public IReadOnlyList<HeroData> StartingHeroes => availableHeroes;
    public PlayerData CurrentPickPlayer => currentPickPlayer;

    public PlayerData CurrentRewardPlayer => currentRewardPlayer;
    public IReadOnlyList<Reward> CurrentOptions => currentOptions;
    public Reward PendingReward => pendingReward;

    public event Action Changed;

    private readonly Queue<PlayerData> rewardQueue = new();
    private PlayerData loser;
    private PlayerData currentPickPlayer;
    private PlayerData currentRewardPlayer;
    private List<Reward> currentOptions = new();
    private Reward pendingReward;

    private void Start()
    {
        Player1 = new PlayerData("Игрок 1", startingLives);
        Player2 = new PlayerData("Игрок 2", startingLives);
        battleManager.BattleEnded += OnBattleEnded;
        StartHeroSelect();
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
            StartPlacementPhase();
        }
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
            reward.Apply(currentRewardPlayer, targets[0]);
            NextRewardPlayer();
        }
        else if (targets.Count > 1)
        {
            pendingReward = reward;
            NotifyChanged();
        }
    }

    public void ChooseTarget(HeroRuntimeData hero)
    {
        pendingReward.Apply(currentRewardPlayer, hero);
        NextRewardPlayer();
    }

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

    public void ConfirmPlacement()
    {
        if (State != GameState.Placement) return;

        if (currentPlacementPlayer == Player1)
        {
            currentPlacementPlayer = Player2;
            NotifyChanged();
        }
        else
        {
            currentPlacementPlayer = null;
            StartBattle();
        }
    }

    private void StartPlacementPhase()
    {
        State = GameState.Placement;
        currentRewardPlayer = null;
        placement.AutoPlace(Player1, Team.Left);
        placement.AutoPlace(Player2, Team.Right);
        battleManager.SpawnTeams(Player1.Team, Player2.Team);

        currentPlacementPlayer = Player1;
        NotifyChanged();
    }

    private void StartBattle()
    {
        Round++;
        State = GameState.Battle;
        battleManager.BeginBattle();
        NotifyChanged();
    }

    private void OnBattleEnded(BattleResult result)
    {
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
            StartPlacementPhase();
            return;
        }
        currentRewardPlayer = rewardQueue.Dequeue();
        pendingReward = null;
        int count = rewardChoices + (currentRewardPlayer == loser ? 1 : 0);
        currentOptions = RewardGenerator.Generate(rewardPool, currentRewardPlayer, count, rarityWeights);
        NotifyChanged();
        if (currentOptions.Count == 0) NextRewardPlayer();
    }

    private void NotifyChanged()
    {
        Changed?.Invoke();
    }

    private void StartHeroSelect()
    {
        State = GameState.HeroSelect;
        currentPickPlayer = Player1;
        NotifyChanged();
    }
}