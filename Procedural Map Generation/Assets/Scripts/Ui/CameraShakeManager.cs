using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
public class CameraShakeManager : MonoBehaviour
{
    public static CameraShakeManager Shaker;
    
    public CinemachineCamera Cam1;
    public CinemachineCamera Cam2;
    CinemachineBasicMultiChannelPerlin CamShake;
    CinemachineBasicMultiChannelPerlin Cam1Shake;
    CinemachineBasicMultiChannelPerlin Cam2Shake;
    private Coroutine shakeCoroutine;

    public void Start()
    {
        Cam1Shake = Cam1.GetComponent<CinemachineBasicMultiChannelPerlin>();
        Cam2Shake = Cam2.GetComponent<CinemachineBasicMultiChannelPerlin>();

    }
    void Awake()
    {
        
        if (Shaker == null) Shaker = this;
        else Destroy(gameObject);
        
    }
    void Update()
    {
        
    }
    public void ShakePulseCutscene()
    {
        ShakePulse(10,10,4);
    }
    public void ShakePulse(float CamShakeAmp, float CamShakeFreq, float Dur)
    {
        // if (OnOffOptions.Instance.CameraShake)
        // {
        //     if (CamShake != null)
        //     {
        //         if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);    
        //         shakeCoroutine = StartCoroutine(ShakeNature(CamShakeAmp, CamShakeFreq, Dur));
        //     }
        // }
    }

    private IEnumerator ShakeNature(float startAmp, float startFreq, float Dur)
    {
        float elapsedTime = 0;
        CamShake.AmplitudeGain = startAmp;
        CamShake.FrequencyGain = startFreq;

        while (elapsedTime < Dur)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float lerpPercent = elapsedTime / Dur;

            CamShake.AmplitudeGain = Mathf.Lerp(startAmp, 0f, lerpPercent);
            CamShake.FrequencyGain = Mathf.Lerp(startFreq, 0f, lerpPercent);
            yield return null;
        }
        CamShake.AmplitudeGain = 0f;
        CamShake.FrequencyGain = 0f;
        shakeCoroutine = null;
    }
}
