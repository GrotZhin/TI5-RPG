using Unity.VisualScripting;
using UnityEngine;

public class OnOffOptions : MonoBehaviour
{
    public  bool Rumble;
    public  bool CameraShake;
    public static OnOffOptions Instance;

    void Awake() {
        if(Instance == null)
        {
            Instance = this;
        }
    }
    public void RumbleToggle()
    {
        Rumble = !Rumble;
    }
    public void CameraShakeToggle()
    {
        CameraShake = !CameraShake;
    }
}
