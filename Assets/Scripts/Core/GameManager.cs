using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private PlacementManager placement;
    [SerializeField] private List<HeroData> availableHeroes = new List<HeroData>();
    [SerializeField] private int startingLives = 3;
    [SerializeField] private float battleEndDelay = 1.5f;
    [SerializeField] private List<Reward> rewardPool = new List<Reward>();
    [SerializeField] private int rewardChoices = 3;
    [SerializeField] private RarityWeights rarityWeights = new();

    public PlayerData Player1 { get; private set; }
    public PlayerData Player2 { get; private set; }
    public GameState State { get; private set; }
    public int Round { get; private set; }
    public PlayerData Winner { get; private set; }

    public IReadOnlyList<HeroData> StartingHeroes => availableHeroes;
    public PlayerData CurrentPickPlayer => currentPickPlayer;

    public PlayerData CurrentPlacementPlayer => currentPlacementPlayer;

    public PlayerData CurrentRewardPlayer => currentRewardPlayer;
    public IReadOnlyList<Reward> CurrentOptions => currentOptions;
    public Reward PendingReward => pendingReward;

    public event System.Action Changed;

    private readonly Queue<PlayerData> rewardQueue = new Queue<PlayerData>();
    private PlayerData loser;
    private PlayerData currentPickPlayer;
    private PlayerData currentPlacementPlayer;
    private PlayerData currentRewardPlayer;
    private List<Reward> currentOptions = new List<Reward>();
    private Reward pendingReward;

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

    public Team GetTeam(PlayerData player) => player == Player1 ? Team.Left : Team.Right;

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
            StartPlacementPhase();
        }
    }

    private void StartPlacementPhase()
    {
        State = GameState.Placement;
        currentRewardPlayer = null;
        pendingReward = null;

        RefreshArena();

        currentPlacementPlayer = Player1;
        NotifyChanged();
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

    private void RefreshArena()
    {
        placement.AutoPlace(Player1, Team.Left);
        placement.AutoPlace(Player2, Team.Right);
        battleManager.SpawnTeams(Player1.Team, Player2.Team);
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

        NotifyChanged();
        StartCoroutine(FinishBattleRoutine());
    }

    private IEnumerator FinishBattleRoutine()
    {
        yield return new WaitForSecondsRealtime(battleEndDelay);

        if (!Player1.IsAlive || !Player2.IsAlive)
        {
            Winner = Player1.IsAlive ? Player1 : Player2;
            State = GameState.GameOver;
            Debug.Log($"Победил {Winner.Name}!");
            NotifyChanged();
            yield break;
        }

        StartRewardPhase();
    }

    private void StartRewardPhase()
    {
        State = GameState.Reward;
        rewardQueue.Clear();
        rewardQueue.Enqueue(Player1);
        rewardQueue.Enqueue(Player2);

        RefreshArena();
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

        if (currentOptions.Count == 0)
            NextRewardPlayer();
    }

    public void ChooseReward(Reward reward)
    {
        if (reward.NeedsHeroTarget)
        {
            pendingReward = reward;
            NotifyChanged();
            return;
        }

        ApplyReward(reward, null);
    }

    public void CancelPendingReward()
    {
        if (pendingReward == null) return;

        pendingReward = null;
        NotifyChanged();
    }

    public bool CanGive(HeroRuntimeData hero)
    {
        return pendingReward != null
            && currentRewardPlayer != null
            && currentRewardPlayer.Team.Contains(hero)
            && pendingReward.CanTarget(hero);
    }

    public void ChooseTarget(HeroRuntimeData hero)
    {
        if (!CanGive(hero)) return;

        ApplyReward(pendingReward, hero);
    }

    private void ApplyReward(Reward reward, HeroRuntimeData target)
    {
        pendingReward = null;
        reward.Apply(currentRewardPlayer, target);

        RefreshArena();
        NextRewardPlayer();
    }

    // Проверка распределения редкостей: правый клик по компоненту, пункт Test rarity distribution
    [ContextMenu("Test rarity distribution")]
    private void TestRarityDistribution()
    {
        var player = new PlayerData("test", 3);
        player.AddHero(availableHeroes[0]);

        int[] counts = new int[3];
        const int runs = 10000;
        for (int i = 0; i < runs; i++)
        {
            foreach (var reward in RewardGenerator.Generate(rewardPool, player, 1, rarityWeights))
                counts[(int)reward.rare]++;
        }

        Debug.Log($"Обычные {counts[0] * 100f / runs:0.0}%, редкие {counts[1] * 100f / runs:0.0}%, легендарные {counts[2] * 100f / runs:0.0}%");
    }
}