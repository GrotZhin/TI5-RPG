using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class StyleRankUI : MonoBehaviour
{
    static readonly string[] RankObjectNames = { "DRank", "CRank", "BRank", "ARank", "SRank", "SSRank" };

    [Header("Shader RankFill")]
    [SerializeField] float materialFillEmpty = 0f;
    [SerializeField] float materialFillFull = 1.25f;
    [SerializeField] string materialFillProperty = "_Fill";

    [Header("Animação")]
    [SerializeField] float fillSmoothing = 14f;
    [SerializeField] float rankUpPunch = 0.25f;

    class RankView
    {
        public GameObject root;
        public Image[] fills;
        public Material[] materials;
    }

    readonly RankView[] views = new RankView[StyleSystem.RankCount];
    int currentRank = -1;
    float targetFill;
    float shownFill;
    bool subscribed;

    void Awake()
    {
        for (int i = 0; i < views.Length; i++)
        {
            Transform t = transform.Find(RankObjectNames[i]);
            if (t == null)
            {
                Debug.LogWarning($"[StyleRankUI] Filho '{RankObjectNames[i]}' não encontrado em {name}.");
                continue;
            }

            var v = new RankView { root = t.gameObject };

            var list = new System.Collections.Generic.List<Image>();
            foreach (var img in t.GetComponentsInChildren<Image>(true)) if (img.gameObject.name.StartsWith("Fill")) list.Add(img);
            v.fills = list.ToArray();
            v.materials = new Material[v.fills.Length];

            for (int f = 0; f < v.fills.Length; f++)
            {
                var img = v.fills[f];
                if (img.type != Image.Type.Filled && img.material != null && img.material.HasProperty(materialFillProperty))
                {
                    v.materials[f] = new Material(img.material);
                    img.material = v.materials[f];
                }
            }
            views[i] = v;
        }

        HideAll();
    }

    void OnEnable() => TrySubscribe();

    void Start()
    {
        TrySubscribe();
        if (StyleSystem.Instance != null) Apply(StyleSystem.Instance.RankIndex, StyleSystem.Instance.Fill01, snap: true);
    }

    void OnDisable()
    {
        if (subscribed && StyleSystem.Instance != null)
        {
            StyleSystem.Instance.OnStyleChanged -= OnStyleChanged;
            StyleSystem.Instance.OnRankChanged -= OnRankChanged;
        }
        subscribed = false;
    }

    void TrySubscribe()
    {
        if (subscribed || StyleSystem.Instance == null) return;
        StyleSystem.Instance.OnStyleChanged += OnStyleChanged;
        StyleSystem.Instance.OnRankChanged += OnRankChanged;
        subscribed = true;
    }

    void Update()
    {
        if (!subscribed) TrySubscribe();
        if (currentRank < 0) return;
        shownFill = Mathf.Lerp(shownFill, targetFill, 1f - Mathf.Exp(-fillSmoothing * Time.unscaledDeltaTime));
        WriteFill(views[currentRank], shownFill);
    }

    void OnStyleChanged(int rank, float fill)
    {
        targetFill = fill;
        if (rank != currentRank) return;
    }

    void OnRankChanged(int oldRank, int newRank)
    {
        Apply(newRank, StyleSystem.Instance.Fill01, snap: false);

        if (newRank > oldRank && newRank >= 0 && views[newRank] != null && rankUpPunch > 0f)
        {
            var tr = views[newRank].root.transform;
            tr.DOKill(true);
            tr.DOPunchScale(Vector3.one * rankUpPunch, 0.3f, 8, 0.6f).SetUpdate(true);
        }
    }

    void Apply(int rank, float fill, bool snap)
    {
        currentRank = rank;
        targetFill = fill;

        for (int i = 0; i < views.Length; i++)
            if (views[i] != null) views[i].root.SetActive(i == rank);

        if (rank >= 0 && views[rank] != null)
        {
            shownFill = fill;
            WriteFill(views[rank], shownFill);
        }
    }

    void HideAll()
    {
        for (int i = 0; i < views.Length; i++) if (views[i] != null) views[i].root.SetActive(false);
    }

    void WriteFill(RankView v, float fill01)
    {
        if (v == null) return;
        for (int i = 0; i < v.fills.Length; i++)
        {
            var img = v.fills[i];
            if (img.type == Image.Type.Filled) img.fillAmount = fill01;
            else if (v.materials[i] != null) v.materials[i].SetFloat(materialFillProperty, Mathf.Lerp(materialFillEmpty, materialFillFull, fill01));
        }
    }
}