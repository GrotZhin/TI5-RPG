using UnityEngine;

public class PlayerCutModelTp : MonoBehaviour
{
    public Transform PlayablePlayer;
    public Transform CutPlayer;
    public Vector3 positionOffset;
    public void CutTakePlace()
    {
        CutPlayer.position = PlayablePlayer.position + PlayablePlayer.TransformDirection(positionOffset);
        
        CutPlayer.rotation = PlayablePlayer.rotation;

        PlayablePlayer.gameObject.SetActive(false);
        CutPlayer.gameObject.SetActive(true);

        Debug.Log("Mudou");
    }
}