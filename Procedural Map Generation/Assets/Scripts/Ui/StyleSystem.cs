using System;
using System.Collections.Generic;
using UnityEngine;
public class StyleSystem : MonoBehaviour
{
    public static StyleSystem Instance { get; private set; }

    public const int RankCount = 6;
    public static readonly string[] RankNames = { "D", "C", "B", "A", "S", "SS" };

    [Header("Escala")]
    [SerializeField] float rankSize = 100f;

    [Header("Decaimento por rank")]
    [SerializeField] float[] decayPerSecond = { 4f, 6f, 9f, 13f, 18f, 24f };
    [SerializeField] float decayDelay = 1.2f;

    [Header("Ganho de pontos")]
    [SerializeField, Range(0f, 0.3f)] float gainFalloffPerRank = 0.08f;
    [SerializeField] float airHitMultiplier = 1.25f;
    [SerializeField] float freshMoveMultiplier = 1.25f;

    [Header("Penalidade de repetição")]
    [SerializeField] int historySize = 8;
    [SerializeField, Range(0f, 0.5f)] float repeatPenalty = 0.25f;
    [SerializeField, Range(0f, 1f)] float minRepeatMultiplier = 0.1f;
    [SerializeField] float sameSwingWindow = 0.1f;

    [Header("Movimento (pulo / dash)")]
    [SerializeField] float jumpPoints = 3f;
    [SerializeField] float dashPoints = 4f;
    [SerializeField] float combatWindow = 4f;
    [SerializeField] float movementCooldown = 0.6f;

    [Header("Esquiva perfeita")]
    [SerializeField] float perfectDodgePoints = 25f;
    [SerializeField] float perfectDodgeCooldown = 1f;
    [SerializeField] float dodgeWindow = 0.4f;

    [Header("Dano sofrido")]
    [SerializeField] float hitPenaltyRanks = 1.5f;
    [SerializeField] float hitLockout = 1.0f;

    [Header("Referências")]
    [SerializeField] CharacterController playerController;

    float points;
    float lastGainTime = -999f;
    float lastCombatTime = -999f;
    float lastMovementTime = -999f;
    float lastDodgeTime = -999f;
    float lastDashTime = -999f;
    float lockedUntil = -999f;

    readonly Queue<string> history = new Queue<string>();
    string lastSwingId;
    float lastSwingTime = -999f;
    float lastSwingMultiplier = 1f;

    int lastRank = -1;

    public event Action<int, float> OnStyleChanged;
    public event Action<int, int> OnRankChanged;

    public float Points => points;
    public float MaxPoints => RankCount * rankSize - 0.01f;
    public int RankIndex => points <= 0.01f ? -1 : Mathf.Min((int)(points / rankSize), RankCount - 1);
    public float Fill01 => RankIndex < 0 ? 0f : Mathf.Clamp01((points - RankIndex * rankSize) / rankSize);
    public string RankName => RankIndex < 0 ? "-" : RankNames[RankIndex];
    public bool RecentlyDashed => Time.time - lastDashTime <= dodgeWindow;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        if (playerController == null)
        {
            var move = FindFirstObjectByType<PlayerMove>();
            if (move != null) playerController = move.GetComponent<CharacterController>();
        }

        TaylaAnimationBehavior.Dash += OnDashStarted;
        TaylaAnimationBehavior.Jump += OnJumpStarted;
        if (PlayerStats.Instance != null) PlayerStats.Instance.OnDied += ResetStyle;

        Notify(true);
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
        TaylaAnimationBehavior.Dash -= OnDashStarted;
        TaylaAnimationBehavior.Jump -= OnJumpStarted;
        if (PlayerStats.Instance != null)
        PlayerStats.Instance.OnDied -= ResetStyle;
    }

    void Update()
    {
        if (points <= 0f) return;
        if (Time.time < lockedUntil) return;
        if (Time.time - lastGainTime < decayDelay) return;

        int r = Mathf.Max(RankIndex, 0);
        float decay = decayPerSecond[Mathf.Min(r, decayPerSecond.Length - 1)];
        points = Mathf.Max(0f, points - decay * Time.deltaTime);
        Notify(false);
    }
    public void RegisterHit(SkillData skill)
    {
        if (skill == null) return;
        lastCombatTime = Time.time;
        float mult;

        if (skill.name == lastSwingId && Time.time - lastSwingTime <= sameSwingWindow)
        {
            mult = lastSwingMultiplier;
        }
        else
        {
            mult = GetRepeatMultiplier(skill.name);
            PushHistory(skill.name);
            lastSwingId = skill.name;
            lastSwingMultiplier = mult;
        }
        lastSwingTime = Time.time;

        if (playerController != null && !playerController.isGrounded) mult *= airHitMultiplier;

        AddPoints(skill.StylePoints * mult);
    }

    public void OnPlayerHit(float damage)
    {
        lastCombatTime = Time.time;
        points = Mathf.Max(0f, points - hitPenaltyRanks * rankSize);
        history.Clear();
        lastSwingId = null;
        lockedUntil = Time.time + hitLockout;
        Notify(false);
    }

    public void RegisterPerfectDodge()
    {
        if (!RecentlyDashed) return;
        if (Time.time - lastDodgeTime < perfectDodgeCooldown) return;
        lastDodgeTime = Time.time;
        lastCombatTime = Time.time;
        AddPoints(perfectDodgePoints);
    }

    public void ResetStyle()
    {
        points = 0f;
        history.Clear();
        lastSwingId = null;
        Notify(false);
    }

    void OnDashStarted()
    {
        lastDashTime = Time.time;
        GiveMovementPoints("Dash", dashPoints);
    }

    void OnJumpStarted(bool isDouble)
    {
        GiveMovementPoints(isDouble ? "DoubleJump" : "Jump", jumpPoints);
    }

    void GiveMovementPoints(string id, float basePoints)
    {
        if (Time.time - lastCombatTime > combatWindow) return;
        if (Time.time - lastMovementTime < movementCooldown) return;
        lastMovementTime = Time.time;

        float mult = GetRepeatMultiplier(id);
        PushHistory(id);
        AddPoints(basePoints * mult);
    }

    float GetRepeatMultiplier(string id)
    {
        int count = 0;
        foreach (var h in history) if (h == id) count++;

        if (count == 0) return freshMoveMultiplier;
        return Mathf.Max(minRepeatMultiplier, 1f - repeatPenalty * count);
    }

    void PushHistory(string id)
    {
        history.Enqueue(id);
        while (history.Count > historySize) history.Dequeue();
    }

    void AddPoints(float amount)
    {
        if (amount <= 0f) return;
        if (Time.time < lockedUntil) return;

        int r = Mathf.Max(RankIndex, 0);
        amount *= 1f - gainFalloffPerRank * r;

        points = Mathf.Min(points + amount, MaxPoints);
        lastGainTime = Time.time;
        Notify(false);
    }

    void Notify(bool force)
    {
        int rank = RankIndex;
        if (rank != lastRank)
        {
            OnRankChanged?.Invoke(lastRank, rank);
            lastRank = rank;
        }
        OnStyleChanged?.Invoke(rank, Fill01);
    }
}