using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;


public class Formation : ScriptableObject
{
    public float radius;

    public Vector3 GetPosition(Agent agent)
    {
        if(!agent.isLeader) return GetFormation(agent);
        Vector3 randDir = Random.insideUnitSphere * radius;
        randDir.y = 0;
        Vector3 randPoint = agent.transform.position + randDir;
        NavMeshHit hit;
        Vector3 finalPoint = agent.transform.position;
        if(NavMesh.SamplePosition(randPoint, out hit, 2f, 1))
        {
            finalPoint = hit.position;
        }
        return finalPoint;
    }
    //protected Vector3 FixPosition(Vector3 position, Vector3)
    public virtual Vector3 GetFormation(Agent agent)
    {
        return agent.transform.position;
    }
    
}
