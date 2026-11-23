using UnityEngine;

public class AdornoPickup : MonoBehaviour, IInteractable
{
    public AdornoData adorno;

    [Header("Visual")]
    public Renderer tintRenderer;
    public Renderer auraRenderer;
    public string[] auraColorProperties = { "_BaseColor", "_Color", "_TintColor" };
    public float auraIntensity = 1f;

    [Header("Luz (opcional)")]
    public bool addGlowLight = false;
    public float glowRange = 3f;

    [Header("Coleta")]
    public bool pickupOnTouch = true;
    public float pickupDelay = 0.5f;
    public float triggerRadius = 0.8f;

    [Header("Animação")]
    public bool spinAndBob = true;
    public float spinSpeed = 70f;
    public float bobHeight = 0.12f;
    public float bobSpeed = 2f;

    [Header("Visualizado")]
    public AdornoInfoView infoPrefab;
    public Vector3 infoOffset = new Vector3(0.9f, 0.2f, 0f);
    public float infoShowRange = 4f;

    Vector3 startPos;
    float spawnTime;
    bool collected;
    AdornoInfoView infoInstance;


    void Start()
    {
        startPos = transform.position;
        spawnTime = Time.time;

        Collider col = GetComponentInChildren<Collider>();
        if (col == null)
        {
            var sc = gameObject.AddComponent<SphereCollider>();
            sc.radius = triggerRadius;
            col = sc;
        }
        if (pickupOnTouch) col.isTrigger = true;

        if (pickupOnTouch && GetComponent<Rigidbody>() == null)
        {
            var rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        if (adorno != null)
        {
            ApplyVisuals();
            CreateInfo();
        }
    }

    void Update()
    {
        if (!spinAndBob) return;
        transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f, Space.World);
        transform.position = startPos + Vector3.up * (Mathf.Sin(Time.time * bobSpeed) * bobHeight);
    }

    Vector3 InfoWorldPos()
    {
        Camera cam = Camera.main;
        if (cam == null) return startPos + infoOffset;

        Transform c = cam.transform;
        return startPos + c.right * infoOffset.x + Vector3.up * infoOffset.y + c.forward * infoOffset.z;
    }

    void LateUpdate()
    {
        if (infoInstance == null) return;
        infoInstance.transform.position = InfoWorldPos();

        bool show = !GameUIState.IsUIOpen;
        if (show && infoShowRange > 0f && PlayerStats.Instance != null)
        {
            float sqr = (PlayerStats.Instance.transform.position - startPos).sqrMagnitude;
            show = sqr <= infoShowRange * infoShowRange;
        }
        if (infoInstance.gameObject.activeSelf != show) infoInstance.gameObject.SetActive(show);
    }

    void OnDestroy()
    {
        if (infoInstance != null) Destroy(infoInstance.gameObject);
    }

    public void Setup(AdornoData data)
    {
        adorno = data;
        ApplyVisuals();
        CreateInfo();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!pickupOnTouch) return;
        TryCollect(other);
    }
    void OnTriggerStay(Collider other)
    {
        if (!pickupOnTouch) return;
        TryCollect(other);
    }

    void TryCollect(Collider other)
    {
        if (collected || adorno == null) return;
        if (Time.time - spawnTime < pickupDelay) return;
        if (other.GetComponentInParent<PlayerStats>() == null) return; 
        collected = true;
        Give(adorno);
        Destroy(gameObject);
    }

    public void Interact(GameObject player)
    {
        if (adorno == null || collected) return;
        collected = true;
        Give(adorno);
        Destroy(gameObject);
    }

    public string GetInteractionPrompt()
    {
        if (pickupOnTouch) return "";
        return adorno == null ? "" : $"Pressione E para pegar <color=#{adorno.tier.ToHex()}>{adorno.itemName}</color>";
    }


    void ApplyVisuals()
    {
        if (adorno == null) return;

        Color tierColor = adorno.tier.ToColor();
        float k = adorno.tier.GlowIntensity();

        if (tintRenderer == null) tintRenderer = GetComponentInChildren<Renderer>();
        if (auraRenderer == null)
        {
            foreach (var r in GetComponentsInChildren<Renderer>()) if (r != tintRenderer) { auraRenderer = r; break; }
        }

        if (tintRenderer != null)
        {
            var mpb = new MaterialPropertyBlock();
            tintRenderer.GetPropertyBlock(mpb);
            Color baseColor = adorno.category.ToColor();
            mpb.SetColor("_BaseColor", baseColor);
            mpb.SetColor("_Color", baseColor);
            tintRenderer.SetPropertyBlock(mpb);
        }

        if (auraRenderer != null && auraRenderer != tintRenderer)
            ApplyAura(tierColor);

        if (addGlowLight)
        {
            var l = GetComponentInChildren<Light>();
            if (l == null)
            {
                var go = new GameObject("GlowLight");
                go.transform.SetParent(transform, false);
                l = go.AddComponent<Light>();
                l.type = LightType.Point;
            }
            l.color = tierColor;
            l.intensity = k;
            l.range = glowRange;
        }
    }

    void ApplyAura(Color tierColor)
    {
        var mat = auraRenderer.sharedMaterial;
        if (mat == null) return;

        var mpb = new MaterialPropertyBlock();
        auraRenderer.GetPropertyBlock(mpb);

        foreach (var prop in auraColorProperties)
        {
            if (!mat.HasProperty(prop)) continue;
            Color c = tierColor * auraIntensity;
            c.a = mat.GetColor(prop).a;
            mpb.SetColor(prop, c);
        }
        auraRenderer.SetPropertyBlock(mpb);
    }


    void CreateInfo()
    {
        if (infoPrefab == null || infoInstance != null || adorno == null) return;

        infoInstance = Instantiate(infoPrefab, transform.position + infoOffset, Quaternion.identity);

        int owned = 0;
        if (InventoryManager.Instance != null)
        {
            var slot = InventoryManager.Instance.slots.Find(s => s.item == adorno);
            if (slot != null) owned = slot.quantity;
        }
        infoInstance.Setup(adorno, owned);
    }

    public static void Give(AdornoData adorno)
    {
        if (InventoryManager.Instance == null)
        {
            Debug.LogError("[Adorno] Sem InventoryManager na cena: o adorno não foi entregue.");
            return;
        }

        InventoryManager.Instance.AddItem(adorno, 1);

        var slot = InventoryManager.Instance.slots.Find(s => s.item == adorno);
        int stacks = slot != null ? slot.quantity : 1;
        Debug.Log($"[Adorno] Recebeu {adorno.itemName} ({adorno.tier}, {adorno.category}) - agora x{stacks}");

        ShowPopup(adorno, stacks);
    }

    static void ShowPopup(AdornoData adorno, int stacks)
    {
        FeedbackPopup fp = FeedbackPopup.Instance;

        if (fp == null)
        {
            Debug.LogWarning("[Adorno] Não existe FeedbackPopup ativo na cena (FeedbackPopup.Instance é null). O popup não pode aparecer.");
            return;
        }
        if (fp.popupPanel == null || fp.popupText == null)
        {
            Debug.LogWarning("[Adorno] O FeedbackPopup está sem 'Popup Panel' ou 'Popup Text' preenchidos.", fp);
            return;
        }
        if (!fp.gameObject.activeInHierarchy)
        {
            Debug.LogWarning("[Adorno] O objeto do FeedbackPopup está DESATIVADO (provavelmente o script está no próprio Panel, " + "que é desligado no Awake). Mova o script para um GameObject separado, fora do Panel.", fp);
            return;
        }

        fp.Show(adorno.Summary() + (stacks > 1 ? $"\n(x{stacks})" : ""), true);
    }
}