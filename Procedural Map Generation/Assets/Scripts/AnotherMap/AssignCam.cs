using Unity.Cinemachine;
using UnityEngine;

public class AssignCam : MonoBehaviour
{
    public CinemachineCamera cinemachine;
    public static AssignCam atribute;
    public Transform trackingTarget;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if(atribute == null) atribute = this;
    }

    // Update is called once per frame
    void Start()
    {
        if(trackingTarget != null) cinemachine.Follow = trackingTarget;
    }
}
