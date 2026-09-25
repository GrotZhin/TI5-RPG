using UnityEngine;

public class SwitchModes : MonoBehaviour
{
   
   public bool scissorsMode;
   public GameObject[]scissors;
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
            taylaani.SetLayerWeight(1,0);
        }
        else
        {
            scissorsMode = true;
            scissors[0].SetActive(true);
            scissors[1].SetActive(true);
            scissors[2].SetActive(true);
            taylaani.SetLayerWeight(1,1);
        }
        }
    }
}
