using System;
using UnityEngine;

public class PlayerCutModelTp : MonoBehaviour
{
    public Transform PlayablePlayer;
    public Transform CutPlayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void CutTakePlace()
    {
        CutPlayer.position = PlayablePlayer.position;
    }
}
