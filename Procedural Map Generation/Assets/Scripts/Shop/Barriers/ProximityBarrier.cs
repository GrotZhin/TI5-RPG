using UnityEngine;
[DisallowMultipleComponent]
[RequireComponent(typeof(Renderer))]
public class ProximityBarrier : MonoBehaviour
{
    [Header("Alvo")]
    public Transform target;

    [Header("Aparição")]
    public float revealDistance = 6f;
    public float fullDistance = 1.5f;
    [Range(0f, 1f)] public float maxAlpha = 0.6f;
    public float fadeSpeed = 3f;
    public Color color = new Color(0.4f, 0.8f, 1f, 1f);

    [Header("Seguir altura (Y)")]
    public bool followTargetY = true;
    public float yOffset = 1.5f;
    public float ySmoothTime = 0.1f;

    [Header("Colisão")]
    public float thickness = 0.5f;

    private Renderer rend;
    private BoxCollider box;
    private MaterialPropertyBlock mpb;
    private float currentAlpha = -1f;
    private float yVelocity;

    static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    static readonly int ColorId = Shader.PropertyToID("_Color");

    void Reset()
    {
        SetupCollider();
    }

    void Awake()
    {
        rend = GetComponent<Renderer>();
        rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        rend.receiveShadows = false;

        mpb = new MaterialPropertyBlock();
        SetupCollider();
        SetAlpha(0f);
    }

    public void SetupCollider()
    {
        var mc = GetComponent<MeshCollider>();
        if (mc != null)
        {
            if (Application.isPlaying) Destroy(mc);
            else DestroyImmediate(mc);
        }

        box = GetComponent<BoxCollider>();
        if (box == null) box = gameObject.AddComponent<BoxCollider>();

        box.isTrigger = false;
        float zScale = Mathf.Max(0.0001f, Mathf.Abs(transform.lossyScale.z));
        box.size = new Vector3(1f, 1f, thickness / zScale);
        box.center = Vector3.zero;
    }

    void Update()
    {
        if (target == null)
        {
            if (PlayerStats.Instance != null) target = PlayerStats.Instance.transform;
            else { SetAlpha(0f); return; }
        }

        FollowY();

        Vector3 p = target.position;
        float dist = Vector3.Distance(box.ClosestPoint(p), p);

        float t = Mathf.InverseLerp(revealDistance, fullDistance, dist); // 0 = longe, 1 = perto
        float goal = t * maxAlpha;

        SetAlpha(Mathf.MoveTowards(Mathf.Max(currentAlpha, 0f), goal, fadeSpeed * Time.deltaTime));
    }

    void FollowY()
    {
        if (!followTargetY) return;

        Vector3 pos = transform.position;
        float goalY = target.position.y + yOffset;
        pos.y = ySmoothTime <= 0f ? goalY : Mathf.SmoothDamp(pos.y, goalY, ref yVelocity, ySmoothTime);
        transform.position = pos;
    }

    void SetAlpha(float a)
    {
        if (Mathf.Approximately(a, currentAlpha)) return;
        currentAlpha = a;

        // Não desenha nada quando está invisível. A colisão continua ativa.
        rend.enabled = a > 0.001f;
        if (!rend.enabled) return;
        Color c = color;
        c.a = a;
        rend.GetPropertyBlock(mpb);
        mpb.SetColor(BaseColorId, c); // URP
        mpb.SetColor(ColorId, c);     // fallback (Built-in)
        rend.SetPropertyBlock(mpb);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.4f, 0.8f, 1f, 0.8f);
        Gizmos.matrix = Matrix4x4.identity;
        Vector3 c = transform.position;
        Gizmos.DrawWireSphere(c, 0.2f);
    }
}