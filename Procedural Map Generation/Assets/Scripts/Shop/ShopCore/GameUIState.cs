using System;
using UnityEngine;

public static class GameUIState
{
    static bool open;

    public static event Action<bool> OnChanged;

    public static bool IsUIOpen
    {
        get => open;
        set
        {
            if (open == value) return;
            open = value;
            OnChanged?.Invoke(value);
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetOnPlay()
    {
        open = false;
        OnChanged = null;
    }
}