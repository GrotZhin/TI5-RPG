using System;
using UnityEngine;

public class PlayerWallet : MonoBehaviour
{
    public static PlayerWallet Instance { get; private set; }

    [Tooltip("Quantidade inicial de moedas do jogador")]
    public int coins = 100;

    public event Action<int> OnCoinsChanged;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        OnCoinsChanged?.Invoke(coins);
    }

    public bool HasEnough(int amount) => coins >= amount;

    public bool TrySpend(int amount)
    {
        if (!HasEnough(amount)) return false;
        coins -= amount;
        OnCoinsChanged?.Invoke(coins);
        return true;
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        OnCoinsChanged?.Invoke(coins);
    }
    public void SetCoins(int amount)
    {
        coins = Mathf.Max(0, amount);
        OnCoinsChanged?.Invoke(coins);
    }
}
