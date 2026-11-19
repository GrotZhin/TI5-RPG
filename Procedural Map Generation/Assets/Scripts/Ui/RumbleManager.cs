using System.Collections;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class RumbleManager : MonoBehaviour
{
    public static RumbleManager Rumble;
    Gamepad Control;

    void Awake()
    {
        if(Rumble == null)
        {
            Rumble = this;
        }
    }
    public void RumblePulse(float LowFreq,float HighFreq,float Dur)
    {
        // if (OnOffOptions.Instance.Rumble)
        // {
        //     Control = Gamepad.current;
        //     if(Control != null)
        //     {
        //         Control.SetMotorSpeeds(LowFreq,HighFreq);
        //         StartCoroutine(RumbleStopper(Dur,Control));
        //     }
        // }
    }
    private IEnumerator RumbleStopper(float Dur,Gamepad Control)
    {
        float elapsedTime = 0;
        while(elapsedTime < Dur)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Control.SetMotorSpeeds(0,0);
    }
}
