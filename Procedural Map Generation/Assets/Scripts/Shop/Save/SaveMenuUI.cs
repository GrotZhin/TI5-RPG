using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class SaveMenuUI : MonoBehaviour
{
    [Tooltip("Tecla para abrir/fechar o menu de save.")]
    public Key menuKey = Key.Escape;

    class SlotRow
    {
        public TMP_Text info;
        public Button save, load, delete;
        public TMP_Text saveLabel, deleteLabel;
    }

    readonly List<SlotRow> rows = new List<SlotRow>();
    GameObject root;
    TMP_Text statusText;
    bool isOpen;

    int pendingSlot = -1;
    string pendingAction = "";

    static readonly Color PanelColor = new Color(0.11f, 0.12f, 0.16f, 0.98f);
    static readonly Color RowColor = new Color(0.19f, 0.21f, 0.27f, 1f);
    static readonly Color SaveColor = new Color(0.20f, 0.60f, 0.35f, 1f);
    static readonly Color LoadColor = new Color(0.20f, 0.45f, 0.80f, 1f);
    static readonly Color DeleteColor = new Color(0.75f, 0.25f, 0.25f, 1f);
    static readonly Color CloseColor = new Color(0.35f, 0.37f, 0.43f, 1f);

    void Start()
    {
        if (SaveManager.Instance == null)
        {
            Debug.LogError("[SaveMenuUI] Não há SaveManager na cena.");
            enabled = false;
            return;
        }

        EnsureEventSystem();
        BuildUI();
        root.SetActive(false);
        SaveManager.Instance.OnSlotsChanged += Refresh;
    }

    void OnDestroy()
    {
        if (SaveManager.Instance != null) SaveManager.Instance.OnSlotsChanged -= Refresh;
    }

    void Update()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null || !kb[menuKey].wasPressedThisFrame) return;

        if (!isOpen && GameUIState.IsUIOpen) return;
        SetOpen(!isOpen);
    }

    void SetOpen(bool open)
    {
        isOpen = open;
        root.SetActive(open);
        GameUIState.IsUIOpen = open;
        pendingSlot = -1;
        if (open)
        {
            statusText.text = "";
            Refresh();
        }
    }

    void OnSaveClicked(int slot)
    {
        bool occupied = SaveManager.Instance.SlotFileExists(slot);
        if (occupied && !(pendingSlot == slot && pendingAction == "save"))
        {
            pendingSlot = slot;
            pendingAction = "save";
            Refresh();
            return;
        }

        pendingSlot = -1;
        bool ok = SaveManager.Instance.SaveToSlot(slot);
        statusText.text = ok ? $"Jogo salvo no Slot {slot + 1}!" : "Erro ao salvar.";
        statusText.color = ok ? Color.green : Color.red;
        Refresh();
    }

    void OnLoadClicked(int slot)
    {
        pendingSlot = -1;
        bool ok = SaveManager.Instance.LoadFromSlot(slot);
        if (ok)
        {
            SetOpen(false);
            if (FeedbackPopup.Instance != null) FeedbackPopup.Instance.Show($"Slot {slot + 1} carregado!", true);
        }
        else
        {
            statusText.text = "Erro ao carregar.";
            statusText.color = Color.red;
            Refresh();
        }
    }

    void OnDeleteClicked(int slot)
    {
        if (!(pendingSlot == slot && pendingAction == "delete"))
        {
            pendingSlot = slot;
            pendingAction = "delete";
            Refresh();
            return;
        }

        pendingSlot = -1;
        SaveManager.Instance.DeleteSlot(slot);
        statusText.text = $"Slot {slot + 1} apagado.";
        statusText.color = Color.white;
        Refresh();
    }

    void Refresh()
    {
        if (!isOpen) return;

        for (int i = 0; i < rows.Count; i++)
        {
            SlotRow row = rows[i];
            SaveData data = SaveManager.Instance.PeekSlot(i);
            bool fileExists = SaveManager.Instance.SlotFileExists(i);

            if (data != null)
            {
                var ts = System.TimeSpan.FromSeconds(data.playTimeSeconds);
                string time = $"{(int)ts.TotalHours:00}:{ts.Minutes:00}:{ts.Seconds:00}";
                row.info.text =
                    $"<b>{data.saveName}</b>\n" +
                    $"{data.SavedAtLocal:dd/MM/yyyy}  às  {data.SavedAtLocal:HH:mm:ss}\n" +
                    $"<size=80%><color=#B5BAC7>Checkpoint: {data.checkpointId}  •  {data.coins} moedas  •  Tempo: {time}</color></size>";
            }
            else if (fileExists)
            {
                row.info.text = $"<b>Slot {i + 1}</b>\n<color=#E07070>Arquivo corrompido (pode sobrescrever)</color>";
            }
            else
            {
                row.info.text = $"<b>Slot {i + 1}</b>\n<color=#8A8F9D>Vazio</color>";
            }

            row.load.interactable = data != null;
            row.delete.interactable = fileExists;

            row.saveLabel.text = (pendingSlot == i && pendingAction == "save") ? "Confirmar?" : (fileExists ? "Sobrescrever" : "Salvar");
            row.deleteLabel.text = (pendingSlot == i && pendingAction == "delete") ? "Confirmar?" : "Apagar";
        }
    }

    void EnsureEventSystem()
    {
        if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() != null) return;
        new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem), typeof(InputSystemUIInputModule));
    }

    void BuildUI()
    {
        GameObject canvasGO = new GameObject("SaveMenuCanvas",
            typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasGO.transform.SetParent(transform, false);

        Canvas canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 200;

        CanvasScaler scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        Image dim = NewImage("Dim", canvasGO.transform, new Color(0f, 0f, 0f, 0.65f));
        Stretch(dim.rectTransform);
        root = dim.gameObject;

        Image panel = NewImage("Panel", root.transform, PanelColor);
        RectTransform prt = panel.rectTransform;
        prt.anchorMin = prt.anchorMax = prt.pivot = new Vector2(0.5f, 0.5f);
        prt.sizeDelta = new Vector2(1000f, 640f);

        VerticalLayoutGroup vlg = panel.gameObject.AddComponent<VerticalLayoutGroup>();
        vlg.padding = new RectOffset(28, 28, 24, 24);
        vlg.spacing = 14f;
        vlg.childAlignment = TextAnchor.UpperCenter;
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;

        TMP_Text title = NewText("Title", panel.transform, "SALVAR / CARREGAR", 40f, TextAlignmentOptions.Center, Color.white);
        title.fontStyle = FontStyles.Bold;
        SetHeight(title.gameObject, 56f);

        int count = SaveManager.Instance.slotCount;
        for (int i = 0; i < count; i++) rows.Add(BuildRow(panel.transform, i));

        statusText = NewText("Status", panel.transform, "", 26f, TextAlignmentOptions.Center, Color.white);
        SetHeight(statusText.gameObject, 40f);

        Image closeRow = NewImage("CloseRow", panel.transform, Color.clear);
        SetHeight(closeRow.gameObject, 56f);
        HorizontalLayoutGroup hlg = closeRow.gameObject.AddComponent<HorizontalLayoutGroup>();
        hlg.childAlignment = TextAnchor.MiddleCenter;
        hlg.childControlWidth = true;
        hlg.childControlHeight = true;
        hlg.childForceExpandWidth = false;
        hlg.childForceExpandHeight = true;

        Button close = NewButton("Close", closeRow.transform, "Fechar (ESC)", CloseColor, 260f, out _);
        close.onClick.AddListener(() => SetOpen(false));
    }

    SlotRow BuildRow(Transform parent, int slot)
    {
        Image rowBg = NewImage($"Slot{slot + 1}", parent, RowColor);
        SetHeight(rowBg.gameObject, 120f);

        HorizontalLayoutGroup hlg = rowBg.gameObject.AddComponent<HorizontalLayoutGroup>();
        hlg.padding = new RectOffset(18, 14, 10, 10);
        hlg.spacing = 10f;
        hlg.childAlignment = TextAnchor.MiddleLeft;
        hlg.childControlWidth = true;
        hlg.childControlHeight = true;
        hlg.childForceExpandWidth = false;
        hlg.childForceExpandHeight = true;

        SlotRow row = new SlotRow();

        row.info = NewText("Info", rowBg.transform, "", 28f, TextAlignmentOptions.MidlineLeft, Color.white);
        row.info.overflowMode = TextOverflowModes.Ellipsis;
        LayoutElement le = row.info.gameObject.AddComponent<LayoutElement>();
        le.flexibleWidth = 1f;

        int captured = slot;
        row.save = NewButton("Save", rowBg.transform, "Salvar", SaveColor, 170f, out row.saveLabel);
        row.save.onClick.AddListener(() => OnSaveClicked(captured));

        row.load = NewButton("Load", rowBg.transform, "Carregar", LoadColor, 150f, out _);
        row.load.onClick.AddListener(() => OnLoadClicked(captured));

        row.delete = NewButton("Delete", rowBg.transform, "Apagar", DeleteColor, 130f, out row.deleteLabel);
        row.delete.onClick.AddListener(() => OnDeleteClicked(captured));

        return row;
    }

    static RectTransform NewRect(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return go.GetComponent<RectTransform>();
    }

    static Image NewImage(string name, Transform parent, Color color)
    {
        RectTransform rt = NewRect(name, parent);
        Image img = rt.gameObject.AddComponent<Image>();
        img.color = color;
        return img;
    }

    static TMP_Text NewText(string name, Transform parent, string text, float size, TextAlignmentOptions align, Color color)
    {
        RectTransform rt = NewRect(name, parent);
        TextMeshProUGUI t = rt.gameObject.AddComponent<TextMeshProUGUI>();
        t.text = text;
        t.fontSize = size;
        t.alignment = align;
        t.color = color;
        t.raycastTarget = false;
        return t;
    }

    static Button NewButton(string name, Transform parent, string label, Color bg, float width, out TMP_Text labelText)
    {
        Image img = NewImage(name, parent, bg);
        Button btn = img.gameObject.AddComponent<Button>();
        btn.targetGraphic = img;

        LayoutElement le = img.gameObject.AddComponent<LayoutElement>();
        le.preferredWidth = width;

        labelText = NewText("Label", img.transform, label, 24f, TextAlignmentOptions.Center, Color.white);
        Stretch(labelText.rectTransform);
        return btn;
    }

    static void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    static void SetHeight(GameObject go, float height)
    {
        LayoutElement le = go.GetComponent<LayoutElement>();
        if (le == null) le = go.AddComponent<LayoutElement>();
        le.preferredHeight = height;
    }
}