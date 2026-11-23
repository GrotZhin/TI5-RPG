using UnityEngine;
[CreateAssetMenu(fileName = "NovoAdorno", menuName = "Loja/Adorno")]
public class AdornoData : ItemData
{
    [Header("Adorno")]
    public AdornoCategory category = AdornoCategory.Vermelho;
    public AdornoTier tier = AdornoTier.Comum;

    [Header("Efeitos")]
    public StatModifier primary;
    public StatModifier secondary;

    public string Summary(int stacks = 1)
    {
        string main = primary.IsBeneficial ? "7CFC7C" : "FF6B6B";
        string sub = secondary.IsBeneficial ? "7CFC7C" : "FF6B6B";
        return $"<color=#{tier.ToHex()}>{itemName}</color>  <size=70%>({tier.Label()})</size>\n" + $"<size=70%><color=#{category.ToHex()}>{category.Label()}</color></size>\n" + $"<color=#{main}>{primary.Describe(stacks)}</color>\n" + $"<color=#{sub}>{secondary.Describe(stacks)}</color>";
    }

    void OnValidate()
    {
        type = ItemType.Generico;
        statValue = 0f;

        AdornoCategory? cat = AdornoStatInfo.CategoryOf(primary.stat);
        if (cat == null || cat.Value != category) Debug.LogWarning($"[Adorno] '{name}': o efeito principal ({primary.stat}) não é da categoria {category}.", this);
        if (!primary.IsBeneficial) Debug.LogWarning($"[Adorno] '{name}': o efeito principal deveria ser uma vantagem.", this);
    }
}