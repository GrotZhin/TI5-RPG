using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AdornoLootTable", menuName = "Loja/Adorno Loot Table")]
public class AdornoLootTable : ScriptableObject
{
    [Serializable] public struct TierWeight { public AdornoTier tier; [Min(0f)] public float weight; }
    [Serializable] public struct CategoryWeight { public AdornoCategory category; [Min(0f)] public float weight; }

    public TierWeight[] tierWeights =
    {
        new TierWeight { tier = AdornoTier.Comum,weight = 50f },new TierWeight { tier = AdornoTier.Incomum,  weight = 28f },new TierWeight { tier = AdornoTier.Raro, weight = 14f },new TierWeight { tier = AdornoTier.Epico, weight = 6f }, new TierWeight { tier = AdornoTier.Lendario, weight = 1.8f }, new TierWeight { tier = AdornoTier.Ultra, weight = 0.2f },
    };
    public CategoryWeight[] categoryWeights =
    {
        new CategoryWeight { category = AdornoCategory.Vermelho, weight = 1f },new CategoryWeight { category = AdornoCategory.Amarelo,  weight = 1f }, new CategoryWeight { category = AdornoCategory.Azul,     weight = 1f },
    };

    public AdornoData[] pool;
    public AdornoData Roll()
    {
        if (pool == null || pool.Length == 0) return null;
        var tiers = new List<KeyValuePair<AdornoTier, float>>();
        foreach (var w in tierWeights) if (w.weight > 0f && Exists(w.tier, null)) tiers.Add(new KeyValuePair<AdornoTier, float>(w.tier, w.weight));
        if (tiers.Count == 0) return null;
        AdornoTier tier = Pick(tiers);
        var cats = new List<KeyValuePair<AdornoCategory, float>>();
        foreach (var w in categoryWeights) if (w.weight > 0f && Exists(tier, w.category)) cats.Add(new KeyValuePair<AdornoCategory, float>(w.category, w.weight));
        if (cats.Count == 0) return null;
        AdornoCategory cat = Pick(cats);

        var options = new List<AdornoData>();
        foreach (var a in pool) if (a != null && a.tier == tier && a.category == cat) options.Add(a);
        return options[UnityEngine.Random.Range(0, options.Count)];
    }

    bool Exists(AdornoTier tier, AdornoCategory? cat)
    {
        foreach (var a in pool) if (a != null && a.tier == tier && (cat == null || a.category == cat.Value)) return true;
        return false;
    }

    static T Pick<T>(List<KeyValuePair<T, float>> list)
    {
        float total = 0f;
        foreach (var kv in list) total += kv.Value;
        float roll = UnityEngine.Random.value * total;
        foreach (var kv in list)
        {
            if (roll < kv.Value) return kv.Key;
            roll -= kv.Value;
        }
        return list[list.Count - 1].Key;
    }
}