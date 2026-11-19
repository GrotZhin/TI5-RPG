using UnityEngine;
using UnityEngine.SceneManagement;
public class GymRoomTp : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public string Name;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(Name);
        }
    }
}
