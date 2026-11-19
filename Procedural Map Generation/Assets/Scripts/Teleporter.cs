
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public CharacterController player;
    public Transform pontoInicial;

    void Update()
    {
        if (player.transform.position.y < -40f)
        {
            player.enabled = false;
            player.transform.position = pontoInicial.position;
            player.enabled = true;
        }
    }
}
