
using UnityEngine;

public class LampFail : MonoBehaviour
{
    public Light luz;

    void Start()
    {
        InvokeRepeating("Falhar", 3f, 1f);
    }

    void Falhar()
    {
        if (Random.value < 0.15f)
        {
            luz.enabled = false;
            Invoke("Acender", Random.Range(0.05f, 0.2f));
        }
    }

    void Acender()
    {
        luz.enabled = true;
    }
}
