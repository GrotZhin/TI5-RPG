using UnityEngine;

public class RotationX : MonoBehaviour
{
    public float velocidade = 50f;

    void Update()
    {
        transform.Rotate(velocidade * Time.deltaTime, 0f, 0f);
    }
}