using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class GameFeel : MonoBehaviour
{
    bool waitingHitStop;
    bool waitingFlashEffect;
    public static GameFeel gameFeel;
    [SerializeField]
    private float durantion = 1f;

    void Awake()
    {
        if (gameFeel == null)
        {
            gameFeel = this;
        }
    }

    public void Stop(float duration)
    {
        if (waitingHitStop) return;
        Time.timeScale = 0.0f;
        StartCoroutine(ReturnTime(duration));

    }

    IEnumerator ReturnTime(float durantion)
    {
        waitingHitStop = true;
        yield return new WaitForSecondsRealtime(durantion);
        Time.timeScale = 1.0f;
        waitingHitStop = false;
    }

    public void FlashEffect(float duration, SkinnedMeshRenderer mat)
    {

        if (waitingFlashEffect) return;
        
        mat.material.EnableKeyword("_EMISSION");
        Color actualColor = mat.material.GetColor("_EmissionColor");
        mat.material.SetColor("_Emissive_Color", Color.white * 3f);

        StartCoroutine(ReturnColor(duration, mat, actualColor));
    }
    IEnumerator ReturnColor(float duration, SkinnedMeshRenderer mat, Color color)
    {
        waitingFlashEffect = true;
        yield return new WaitForSecondsRealtime(duration);
        mat.material.SetColor("_Emissive_Color", color);
        waitingFlashEffect = false;
    }

}
