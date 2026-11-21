using UnityEngine;

public class PlayerCutModelTp : MonoBehaviour
{
    public Transform PlayablePlayer;
    public Transform CutPlayer;

    public void CutTakePlace()
    {
        // 1. Copia a posição exata
        CutPlayer.position = PlayablePlayer.position;
        
        // 2. Copia a rotação (para o boneco não olhar para o lado errado)
        CutPlayer.rotation = PlayablePlayer.rotation;

        // 3. Troca quem está visível/ativo na cena
        PlayablePlayer.gameObject.SetActive(false);
        CutPlayer.gameObject.SetActive(true);

        Debug.Log("Mudou");
    }
}