using System.Collections.Generic;
using UnityEngine;
public class MoneyOrbManager : MonoBehaviour
{
    public static MoneyOrbManager Instance { get; private set; }

    [Header("Visual (opcional)")]
    [SerializeField] GameObject orbPrefab;
    [SerializeField] float fallbackScale = 0.25f;

    [Header("Quantidade")]
    [SerializeField] int coinsPerOrb = 5;
    [SerializeField] int maxOrbsPerDrop = 10;
    [SerializeField] int maxActiveOrbs = 150;
    [SerializeField] int prewarm = 40;

    [Header("Explosão")]
    [SerializeField] float burstSpeed = 5f;
    [SerializeField] float burstUp = 6f;
    [SerializeField] float gravity = 25f;
    [SerializeField, Range(0f, 0.9f)] float bounce = 0.45f;
    [SerializeField] LayerMask groundMask = ~0;
    [SerializeField] float groundOffset = 0.15f;

    [Header("Atração ao player")]
    [SerializeField] Vector2 magnetDelay = new Vector2(0.45f, 0.8f);
    [SerializeField] float startSpeed = 6f;
    [SerializeField] float maxSpeed = 35f;
    [SerializeField] float acceleration = 60f;
    [SerializeField] float collectRadius = 0.6f;
    [SerializeField] float targetHeight = 1f;

    [Header("Bônus de estilo")]
    [SerializeField] float styleBonusPerRank = 0.1f;

    [Header("Feedback")]
    [SerializeField] bool showPopup = true;
    [SerializeField] float popupDelay = 0.4f;

    class Orb
    {
        public Transform t;
        public Vector3 vel;
        public float age, magnetAt, speed, groundY;
        public int value;
        public bool magnet;
    }

    readonly List<Orb> active = new List<Orb>(64);
    readonly Stack<Orb> pool = new Stack<Orb>(64);
    Vector3 baseScale;

    int pendingCoins;
    float popupTimer;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        baseScale = orbPrefab != null ? orbPrefab.transform.localScale : Vector3.one * fallbackScale;

        for (int i = 0; i < prewarm; i++) pool.Push(CreateOrb());
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void Spawn(Vector3 position, int totalCoins)
    {
        if (totalCoins <= 0) return;

        if (styleBonusPerRank > 0f && StyleSystem.Instance != null)
        {
            int rank = StyleSystem.Instance.RankIndex;
            if (rank >= 0) totalCoins = Mathf.RoundToInt(totalCoins * (1f + styleBonusPerRank * (rank + 1)));
        }

        int count = Mathf.Clamp(Mathf.CeilToInt(totalCoins / (float)Mathf.Max(1, coinsPerOrb)), 1, maxOrbsPerDrop);
        int each = totalCoins / count;
        int remainder = totalCoins - each * count;
        float groundY = position.y - 1f;
        if (Physics.Raycast(position + Vector3.up * 0.5f, Vector3.down, out RaycastHit hit, 20f,groundMask, QueryTriggerInteraction.Ignore)) groundY = hit.point.y + groundOffset;

        for (int i = 0; i < count; i++)
        {
            int value = each + (i < remainder ? 1 : 0);

            if (active.Count >= maxActiveOrbs)
            {
                GiveCoins(value);
                continue;
            }

            Orb o = pool.Count > 0 ? pool.Pop() : CreateOrb();

            Vector2 dir = Random.insideUnitCircle.normalized * Random.Range(0.4f, 1f) * burstSpeed;
            o.vel = new Vector3(dir.x, Random.Range(0.7f, 1.2f) * burstUp, dir.y);
            o.age = 0f;
            o.magnetAt = Random.Range(magnetDelay.x, magnetDelay.y);
            o.magnet = false;
            o.speed = startSpeed;
            o.groundY = groundY;
            o.value = value;
            o.t.position = position;
            o.t.localScale = Vector3.zero;
            o.t.gameObject.SetActive(true);
            active.Add(o);
        }
    }

    void Update()
    {
        float dt = Time.deltaTime;

        if (pendingCoins > 0)
        {
            popupTimer -= dt;
            if (popupTimer <= 0f)
            {
                if (showPopup && FeedbackPopup.Instance != null)FeedbackPopup.Instance.Show($"+{pendingCoins} moedas", true);
                pendingCoins = 0;
            }
        }

        if (active.Count == 0) return;

        PlayerStats player = PlayerStats.Instance;
        bool hasTarget = player != null && !player.IsDead;
        Vector3 target = hasTarget ? player.transform.position + Vector3.up * targetHeight : Vector3.zero;

        for (int i = active.Count - 1; i >= 0; i--)
        {
            Orb o = active[i];
            Transform t = o.t;
            o.age += dt;

            if (!o.magnet && hasTarget && o.age >= o.magnetAt) o.magnet = true;

            if (o.magnet && hasTarget)
            {
                Vector3 to = target - t.position;
                float sqr = to.sqrMagnitude;

                if (sqr <= collectRadius * collectRadius) { Collect(i); continue; }

                float dist = Mathf.Sqrt(sqr);
                o.speed = Mathf.Min(maxSpeed, o.speed + acceleration * dt);
                Vector3 desired = (to / dist) * o.speed;
                o.vel = Vector3.Lerp(o.vel, desired, 1f - Mathf.Exp(-14f * dt));
                Vector3 step = o.vel * dt;

                if (step.sqrMagnitude >= sqr) { Collect(i); continue; }  
                t.position += step;
            }
            else
            {
                o.vel.y -= gravity * dt;
                Vector3 p = t.position + o.vel * dt;

                if (p.y < o.groundY)
                {
                    p.y = o.groundY;
                    if (o.vel.y < 0f)
                    {
                        o.vel.y = -o.vel.y * bounce;
                        o.vel.x *= 0.7f;
                        o.vel.z *= 0.7f;
                        if (o.vel.y < 1f) o.vel.y = 0f;
                    }
                }
                t.position = p;
            }
            t.localScale = baseScale * Mathf.Min(1f, o.age / 0.12f);
        }
    }

    void Collect(int index)
    {
        Orb o = active[index];

        GiveCoins(o.value);
        pendingCoins += o.value;
        popupTimer = popupDelay;

        o.t.gameObject.SetActive(false);
        pool.Push(o);

        int last = active.Count - 1;
        active[index] = active[last];
        active.RemoveAt(last);
    }

    void GiveCoins(int amount)
    {
        if (PlayerWallet.Instance != null) PlayerWallet.Instance.AddCoins(amount);
    }

    Orb CreateOrb()
    {
        GameObject go;
        if (orbPrefab != null)
        {
            go = Instantiate(orbPrefab, transform);
        }
        else
        {
            go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.transform.SetParent(transform, false);
        }

        foreach (var c in go.GetComponentsInChildren<Collider>(true)) Destroy(c);
        foreach (var rb in go.GetComponentsInChildren<Rigidbody>(true)) Destroy(rb);

        go.SetActive(false);
        return new Orb { t = go.transform };
    }
}