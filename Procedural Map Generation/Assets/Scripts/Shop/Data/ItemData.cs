using UnityEngine;
public enum ItemType
{
    Generico,Ataque,Vida,Escudo,Estamina
}

[CreateAssetMenu(fileName = "NovoItem", menuName = "Loja/Item")]
public class ItemData : ScriptableObject
{
    public string itemName = "Item";
    [TextArea] public string description = "";
    public Sprite icon;
    public int price = 10;
    [Header("Efeito no jogador")]
    public ItemType type = ItemType.Generico;
    public float statValue = 0f;
}