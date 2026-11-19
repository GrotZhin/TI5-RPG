using UnityEngine;
using Unity.Cinemachine;

public class SetCameraTarget : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cam;

    void Start()
    {
        GameObject alvo = GameObject.Find("Player");
        if (alvo != null) cam.Follow = alvo.transform;
        else Debug.LogWarning("Player não encontrado!");
    }
}