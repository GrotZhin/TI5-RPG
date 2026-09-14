using UnityEngine;
using TMPro;

public class TimerManager : MonoBehaviour
{
    public float Reset;
    public TMP_Text TimTxt; // Certifique-se de arrastar o objeto "Text (TMP)" aqui no Inspector!
    float CurrentTemp;
    bool AtiveCrom;
    bool ResetCrom;
    private float cronometroReset = 0f;

    void Start()
    {
        // Força a UI a mostrar 00:00:00 assim que o jogo começa
        Ui(); 
    }

    void Update()
    {
        if (AtiveCrom)
        {
            CurrentTemp += Time.deltaTime;
            Ui();
        }
        if (ResetCrom)
        {
            cronometroReset += Time.deltaTime;
            
            if (cronometroReset >= Reset)
            {
                TimReset();
            }
        }
    }

    public void TimStart()
    {
        if (!AtiveCrom && !ResetCrom)
        {
            AtiveCrom = true;
            Debug.Log("Timer Iniciado!");
        }
    }

    public void TimStop()
    {
        if (AtiveCrom)
        {
            AtiveCrom = false;
            ResetCrom = true;
            cronometroReset = 0f;
            Debug.Log($"Timer Parado! Tempo final: {CurrentTemp:F2}s");
        }
    }

    private void TimReset()
    {
        CurrentTemp = 0f;
        AtiveCrom = false;
        ResetCrom = false;
        cronometroReset = 0f;
        Ui();
        Debug.Log("Timer Resetado");
    }

    private void Ui()
    {
        if (TimTxt != null)
        {
            int minutos = Mathf.FloorToInt(CurrentTemp / 60F);
            int segundos = Mathf.FloorToInt(CurrentTemp % 60F);
            int milesimos = Mathf.FloorToInt((CurrentTemp * 100F) % 100F);

            // Atualiza o componente TextMeshPro text corretamente
            TimTxt.text = string.Format("{0:00}:{1:00}:{2:00}", minutos, segundos, milesimos);
        }
    }
}