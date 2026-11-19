using UnityEngine;

[CreateAssetMenu(fileName = "FormationLine", menuName = "AI/FormationLine")]
public class FormationLine : Formation
{
    [SerializeField]
    float spacing  = 3f;

    public override Vector3 GetFormation(Agent agent)
    {
        Transform leader = agent.group.leader.transform;
        int index = agent.group.list.IndexOf(agent);
        Vector3 position = leader.position - leader.forward * spacing * (index+1);
        return position;
    }
}
