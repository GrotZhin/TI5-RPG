using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class GameFeel : MonoBehaviour
{
    bool waiting;
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
        if (waiting) return;
        Time.timeScale = 0.0f;
        StartCoroutine(ReturnTime(duration));

    }
    
 
    IEnumerator ReturnTime(float durantion)
    {
        Debug.Log("to no aguardo");
        waiting = true;
        yield return new WaitForSecondsRealtime(durantion);
        Time.timeScale = 1.0f;
        waiting = false;
        Debug.Log("sai do aguardo");
    }
}
