using UnityEngine;

public class RotateSkybox : MonoBehaviour
{
    public float RotateSpeed = 1.2f;

    private void Update()
    {
        RenderSettings.skybox.SetFloat ("_Rotation" , Time.time * RotateSpeed);
    }
}
