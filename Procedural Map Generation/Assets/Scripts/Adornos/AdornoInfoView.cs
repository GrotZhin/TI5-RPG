using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AdornoInfoView : MonoBehaviour
{
    [Header("Buff / Debuff")]
    public TMP_Text buffText;
    public TMP_Text debuffText;
    public string buffPrefix = "buff: ";
    public string debuffPrefix = "debuff: ";

    [Header("Textos (opcionais)")]
    public TMP_Text nameText;
    public TMP_Text tierText;
    public TMP_Text categoryText;
    public TMP_Text primaryText;
    public TMP_Text secondaryText;
    public TMP_Text descriptionText;
    public TMP_Text ownedText;
    public TMP_Text summaryText;

    [Header("Imagem (opcional)")]
    public Image iconImage;

    [Header("Cores automáticas (opcionais)")]
    public Graphic[] tierTinted;
    public Graphic[] categoryTinted;
    public Color goodColor = new Color(0.49f, 0.99f, 0.49f);
    public Color badColor = new Color(1f, 0.42f, 0.42f);

    [Header("Comportamento")]
    public bool faceCamera = true;

    [Header("Extensão ")]
    public UnityEvent<AdornoData> onSetup;

    public void Setup(AdornoData data, int owned)
    {
        if (data == null) return;

        Color tierColor = data.tier.ToColor();
        Color catColor = data.category.ToColor();
        string buff = "", debuff = "";
        AddEffect(data.primary, ref buff, ref debuff);
        if (data.secondary.value != 0f) AddEffect(data.secondary, ref buff, ref debuff);
        Set(buffText, buff, goodColor);
        Set(debuffText, debuff, badColor);
        Set(nameText, data.itemName, tierColor);
        Set(tierText, data.tier.Label(), tierColor);
        Set(categoryText, data.category.Label(), catColor);
        Set(descriptionText, data.description, null);
        Set(ownedText, owned > 0 ? $"Você tem x{owned}" : "", null);
        if (summaryText != null) summaryText.text = data.Summary();
        Set(primaryText, data.primary.Describe(), data.primary.IsBeneficial ? goodColor : badColor);
        bool hasSecondary = data.secondary.value != 0f;
        Set(secondaryText, hasSecondary ? data.secondary.Describe() : "",
            data.secondary.IsBeneficial ? goodColor : badColor);

        if (iconImage != null)
        {
            iconImage.sprite = data.icon;
            iconImage.enabled = data.icon != null;
        }

        Tint(tierTinted, tierColor);
        Tint(categoryTinted, catColor);

        onSetup?.Invoke(data);
    }

    void LateUpdate()
    {
        if (!faceCamera) return;
        Camera cam = Camera.main;
        if (cam != null) transform.rotation = cam.transform.rotation;
    }

    void AddEffect(StatModifier m, ref string buff, ref string debuff)
    {
        if (m.IsBeneficial)  buff += (buff.Length > 0 ? "\n" : "") + buffPrefix + m.Describe();
        else debuff += (debuff.Length > 0 ? "\n" : "") + debuffPrefix + m.Describe();
    }

    static void Set(TMP_Text t, string value, Color? color)
    {
        if (t == null) return;
        t.text = value;
        if (color.HasValue) t.color = color.Value;
        t.gameObject.SetActive(!string.IsNullOrEmpty(value));
    }

    static void Tint(Graphic[] list, Color c)
    {
        if (list == null) return;
        foreach (var g in list)
        {
            if (g == null) continue;
            c.a = g.color.a;
            g.color = c;
        }
    }
}