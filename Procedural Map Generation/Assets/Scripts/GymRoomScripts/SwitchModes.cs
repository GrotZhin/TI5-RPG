using UnityEngine;

public class SwitchModes : MonoBehaviour
{
    public enum SwitchType { SCISSORS, NEEDLE}
    public SwitchType type;
    public static bool scissorsMode, needleMode;
    public RuntimeAnimatorController[] controllerList; 
    public GameObject[]scissors;
    public GameObject[]needle;
    public Animator taylaani;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (scissorsMode)
            {
                scissorsMode = false;
                
                scissors[0].SetActive(false);
                scissors[1].SetActive(false);
                scissors[2].SetActive(false);
                taylaani.runtimeAnimatorController = controllerList[0];
                
                if(type == SwitchType.NEEDLE)
                {
                    needleMode = true;
                    needle[0].SetActive(true);
                    needle[1].SetActive(true);
                    needle[2].SetActive(true);
                    needle[3].SetActive(true);
                    taylaani.runtimeAnimatorController = controllerList[2];
                }
            }
            else if (needleMode)
            {
                needleMode = false;

                needle[0].SetActive(false);
                needle[1].SetActive(false);
                needle[2].SetActive(false);
                needle[3].SetActive(false);
                taylaani.runtimeAnimatorController = controllerList[0];
                if (type == SwitchType.SCISSORS)
                {

                    scissorsMode = true;
                    scissors[0].SetActive(true);
                    scissors[1].SetActive(true);
                    scissors[2].SetActive(true);
                    taylaani.runtimeAnimatorController = controllerList[1];
                }
            }
            else
            {
                switch (type)
                {
                    case SwitchType.SCISSORS:
                        scissorsMode = true;
                        scissors[0].SetActive(true);
                        scissors[1].SetActive(true);
                        scissors[2].SetActive(true);
                        taylaani.runtimeAnimatorController = controllerList[1];
                        break;

                    case SwitchType.NEEDLE:
                        needleMode = true;
                        needle[0].SetActive(true);
                        needle[1].SetActive(true);
                        needle[2].SetActive(true);
                        needle[3].SetActive(true);
                        taylaani.runtimeAnimatorController = controllerList[2];
                        break;
                }
            }
        }
    }
}
