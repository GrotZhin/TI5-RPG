using UnityEngine;

public class TriggerDetector : MonoBehaviour
{
    public enum TriggerType { Start, End }
    public TriggerType Type;
    
    public TimerManager TimManager;

    void OnTriggerEnter(Collider other)
    {
        // Verifica se quem pisou tem a tag "Player"
        if (other.CompareTag("Player"))
        {
            if (Type == TriggerType.Start)
            {
                TimManager.TimStart(); // Começa o timer
            }
            else if (Type == TriggerType.End) // Corrigido aqui!
            {
                TimManager.TimStop(); // Para o timer (Corrigido aqui!)
            }
        }
    }
}