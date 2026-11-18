using DG.Tweening;
using TMPro;
using UnityEngine;

public class MoneyHUD : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] string format = "{0}";

    [Header("Animação")]
    [SerializeField] float countSpeed = 10f;
    [SerializeField] float gainPunch = 0.12f;

    float shown;
    int target;
    int lastDrawn = int.MinValue;
    bool subscribed;

    void Awake()
    {
        if (text == null) text = GetComponentInChildren<TMP_Text>(true);
    }

    void OnEnable()
    {
        TrySubscribe();
        SnapToWallet();
    }

    void Start()
    {
        TrySubscribe();
        SnapToWallet();
    }

    void OnDisable()
    {
        if (subscribed && PlayerWallet.Instance != null) PlayerWallet.Instance.OnCoinsChanged -= OnCoinsChanged;
        subscribed = false;
    }

    void TrySubscribe()
    {
        if (subscribed || PlayerWallet.Instance == null) return;
        PlayerWallet.Instance.OnCoinsChanged += OnCoinsChanged;
        subscribed = true;
    }

    void SnapToWallet()
    {
        if (PlayerWallet.Instance == null) return;
        target = PlayerWallet.Instance.coins;
        shown = target;
        Draw(target);
    }

    void OnCoinsChanged(int coins)
    {
        bool gained = coins > target;
        target = coins;

        if (gained && gainPunch > 0f && isActiveAndEnabled)
        {
            transform.DOKill(true);
            transform.DOPunchScale(Vector3.one * gainPunch, 0.2f, 6, 0.6f).SetUpdate(true);
        }
    }

    void Update()
    {
        if (Mathf.Approximately(shown, target)) return;
        shown = Mathf.Lerp(shown, target, 1f - Mathf.Exp(-countSpeed * Time.unscaledDeltaTime));
        if (Mathf.Abs(shown - target) < 0.5f) shown = target;

        Draw(Mathf.RoundToInt(shown));
    }

    void Draw(int value)
    {
        if (text == null || value == lastDrawn) return;
        lastDrawn = value;
        text.text = string.Format(format, value);
    }
}