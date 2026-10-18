using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Loja/Item Database")]
public class ItemDatabase : ScriptableObject
{
    public ItemData[] items;

    private Dictionary<string, ItemData> lookup;

    public static string GetId(ItemData item) => item != null ? item.name : null;

    public ItemData Get(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        if (lookup == null) Build();
        lookup.TryGetValue(id, out ItemData item);
        return item;
    }

    void Build()
    {
        lookup = new Dictionary<string, ItemData>();
        if (items == null) return;

        foreach (var item in items)
        {
            if (item == null) continue;
            if (lookup.ContainsKey(item.name))
            {
                Debug.LogWarning($"[ItemDatabase] ID duplicado: '{item.name}'. Renomeie um dos assets.");
                continue;
            }
            lookup.Add(item.name, item);
        }
    }

    void OnValidate()
    {
        lookup = null;
    }

#if UNITY_EDITOR
    [ContextMenu("Coletar todos os ItemData do projeto")]
    void CollectAll()
    {
        items = AssetDatabase.FindAssets("t:ItemData").Select(g => AssetDatabase.LoadAssetAtPath<ItemData>(AssetDatabase.GUIDToAssetPath(g))).Where(i => i != null).ToArray();
        lookup = null;
        EditorUtility.SetDirty(this);
        Debug.Log($"[ItemDatabase] {items.Length} itens coletados.");
    }
#endif
}
