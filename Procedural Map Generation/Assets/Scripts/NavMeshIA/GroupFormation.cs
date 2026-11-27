using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupFormation : MonoBehaviour
{
    public List<Agent> list = new List<Agent>();
    public Agent leader;
    public Formation formation;

   public void SetLeader()
    {
        Debug.Log(list.Count);
        var leader = list[Random.Range(0,list.Count)];
        leader.isLeader = true;
        this.leader = leader;
    }

}
