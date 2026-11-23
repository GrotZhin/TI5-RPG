using UnityEngine;

public enum AdornoCategory
{
    Vermelho,Amarelo, Azul       
}

public enum AdornoTier
{
    Comum, Incomum,Raro,Epico,Lendario,Ultra      
}

public enum AdornoStat
{
    // --- ermelho ---
    Ataque, VelocidadeAtaque, ChanceCritico,DanoCritico,      

    // --- amarelo---
    EscudoMax, ChanceEsquiva,DanoRecebido,VidaMax,

    // --- azul ---
    DanoProjetil, VelocidadeProjetil, ProjeteisExtras, RecargaProjetil,    

    // --- Utilidade (só aparece como efeito secundário) ---
    Velocidade,GanhoMoedas         
}

public static class AdornoStatInfo
{
    public static bool IsFlat(AdornoStat s) => s == AdornoStat.EscudoMax || s == AdornoStat.VidaMax  || s == AdornoStat.ProjeteisExtras;

    public static bool LowerIsBetter(AdornoStat s) => s == AdornoStat.DanoRecebido || s == AdornoStat.RecargaProjetil;


    public static AdornoCategory? CategoryOf(AdornoStat s)
    {
        switch (s)
        {
            case AdornoStat.Ataque:
            case AdornoStat.VelocidadeAtaque:
            case AdornoStat.ChanceCritico:
            case AdornoStat.DanoCritico:
                return AdornoCategory.Vermelho;

            case AdornoStat.EscudoMax:
            case AdornoStat.ChanceEsquiva:
            case AdornoStat.DanoRecebido:
            case AdornoStat.VidaMax:
                return AdornoCategory.Amarelo;

            case AdornoStat.DanoProjetil:
            case AdornoStat.VelocidadeProjetil:
            case AdornoStat.ProjeteisExtras:
            case AdornoStat.RecargaProjetil:
                return AdornoCategory.Azul;

            default:
                return null;
        }
    }

    public static string Label(AdornoStat s)
    {
        switch (s)
        {
            case AdornoStat.Ataque: return "Dano";
            case AdornoStat.VelocidadeAtaque: return "Vel. de ataque";
            case AdornoStat.ChanceCritico: return "Chance de crítico";
            case AdornoStat.DanoCritico: return "Dano crítico";
            case AdornoStat.EscudoMax: return "Escudo máx.";
            case AdornoStat.ChanceEsquiva: return "Chance de esquiva";
            case AdornoStat.DanoRecebido: return "Dano recebido";
            case AdornoStat.VidaMax: return "Vida máx.";
            case AdornoStat.DanoProjetil: return "Dano de projétil";
            case AdornoStat.VelocidadeProjetil: return "Vel. de projétil";
            case AdornoStat.ProjeteisExtras: return "Projéteis extras";
            case AdornoStat.RecargaProjetil: return "Recarga de projétil";
            case AdornoStat.Velocidade: return "Vel. de movimento";
            case AdornoStat.GanhoMoedas: return "Ganho de moedas";
            default: return s.ToString();
        }
    }
}

[System.Serializable]
public struct StatModifier
{
    public AdornoStat stat;

    [Tooltip("Valor POR STACK. Fixos (Vida, Escudo, Estamina, Projéteis extras) = número (ex: 20). " + "Os outros = fração (0.1 = +10%). Negativo reduz.")]
    public float value;

    public bool IsBeneficial =>
        AdornoStatInfo.LowerIsBetter(stat) ? value <= 0f : value >= 0f;

    public string Describe(int stacks = 1)
    {
        float v = value * stacks;
        string sign = v >= 0f ? "+" : "";
        string label = AdornoStatInfo.Label(stat);
        return AdornoStatInfo.IsFlat(stat)? $"{sign}{v:0.#} {label}" : $"{sign}{v * 100f:0.#}% {label}";
    }
}

public static class AdornoColors
{
    public static Color ToColor(this AdornoCategory c)
    {
        switch (c)
        {
            case AdornoCategory.Vermelho: return new Color(0.9f, 0.22f, 0.27f);
            case AdornoCategory.Amarelo: return new Color(1f, 0.84f, 0.2f);
            default: return new Color(0.23f, 0.53f, 1f);
        }
    }

    public static string Label(this AdornoCategory c)
    {
        switch (c)
        {
            case AdornoCategory.Vermelho: return "Ataque";
            case AdornoCategory.Amarelo: return "Defesa";
            default: return "Projétil";
        }
    }

    public static Color ToColor(this AdornoTier t)
    {
        switch (t)
        {
            case AdornoTier.Comum: return new Color(0.72f, 0.72f, 0.72f);    
            case AdornoTier.Incomum: return new Color(0.3f, 0.85f, 0.35f);   
            case AdornoTier.Raro: return new Color(0.25f, 0.55f, 1f);        
            case AdornoTier.Epico: return new Color(0.65f, 0.3f, 0.95f);    
            case AdornoTier.Lendario: return new Color(1f, 0.6f, 0.1f);
            default: return new Color(1f, 0.12f, 0.12f);                      
        }
    }

    public static float GlowIntensity(this AdornoTier t)
    {
        switch (t)
        {
            case AdornoTier.Comum: return 0.4f;
            case AdornoTier.Incomum: return 0.9f;
            case AdornoTier.Raro: return 1.5f;
            case AdornoTier.Epico: return 2.2f;
            case AdornoTier.Lendario: return 3f;
            default: return 4f;
        }
    }

    public static string Label(this AdornoTier t)
    {
        switch (t)
        {
            case AdornoTier.Epico: return "Épico";
            case AdornoTier.Lendario: return "Lendário";
            default: return t.ToString();
        }
    }

    public static string ToHex(this AdornoCategory c) => ColorUtility.ToHtmlStringRGB(c.ToColor());
    public static string ToHex(this AdornoTier t) => ColorUtility.ToHtmlStringRGB(t.ToColor());
}