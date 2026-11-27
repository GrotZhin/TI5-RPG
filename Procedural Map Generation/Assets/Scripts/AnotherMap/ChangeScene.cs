using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    [SerializeField] string sceneName;
    [SerializeField] Animator animator;
    bool animationStarted = false;

    // Update is called once per frame
    void Update()
    {
        var state =  animator.GetCurrentAnimatorStateInfo(0).IsName("ElevatorArmature|RoomExit");
        var timer =  animator.GetCurrentAnimatorStateInfo(0);
        if(state && timer.normalizedTime >=1f)
            LoadScene();
    }
    void LoadScene()
    {
        SceneManager.LoadScene(sceneName);
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            animator.Play("ElevatorArmature|RoomExit");
            animationStarted = true;
        }

    }
}
