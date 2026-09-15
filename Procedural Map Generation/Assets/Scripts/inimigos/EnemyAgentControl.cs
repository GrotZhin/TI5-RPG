using System;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class EnemyAgentControl
{
    public EnemyAgent self;
    public float maxSpeed = 3;
    public float radius = 4.8f;

    [Range(0,1)]
    public float separate = 0.9f, seek = 0.6f, align = 0.03f, avoid = 1f, cohesion = 0.05f;

    public LayerMask obstacle;

    public EnemyAgentControl (EnemyAgent agent) {  self = agent; obstacle = LayerMask.GetMask("Obstacle"); }

    public Vector3 Move(int fleeMod = 1, int seekMod = 1)
    {
        Vector3 dir = self.transform.forward;
        if (self.player != null)
            dir = (self.player.transform.position - self.transform.position) * seek * seekMod;

        //dir.y = menosagent.transform.position.y;

        if (dir.sqrMagnitude > 0.000001f)
        {
            dir.Normalize();
            dir.y = 0;
        }
        Debug.DrawRay(self.transform.position, Separate(), Color.red);
        Debug.DrawRay(self.transform.position, Align(), Color.green);
        Debug.DrawRay(self.transform.position, Cohesion(), Color.yellow);
        Debug.DrawRay(self.transform.position, Avoid(), Color.blue);

        Vector3 result = dir + 
                         separate * fleeMod * Separate() + 
                         align * seekMod * Align() + 
                         cohesion * Cohesion() + 
                         avoid * Avoid();

        result = Vector3.ClampMagnitude(result, maxSpeed);
        Debug.DrawRay(self.transform.position, result, Color.black);
       return result;
    }


    public Vector3 Separate()
    {
        Vector3 separation = Vector3.zero;
        int count = 0;

        foreach (EnemyAgent other in self.GetNeighbours())
        {
            Vector3 direction = self.transform.position - other.transform.position;
            direction.y = 0f;

            Vector3 dot = other.transform.position - self.transform.position;

            float distance = direction.magnitude;
            if (distance < 3f && Vector3.Dot(self.transform.forward, dot.normalized)>0)
            {
                // Quanto mais próximo, maior a repulsão
                separation += direction.normalized / distance;
                count++;
            }
        }
            if (count > 0)
        {
            separation /= count;
        }
        //Debug.Log(menosagent.passavizinho().Count + "  " + separation);
        return separation;
    }


    Vector3 Align()
    {
        Vector3 align = self.cc.velocity;
        int count = 1;
        if (self.GetNeighbours().Count == 0) return Vector3.zero;
        foreach (EnemyAgent e in self.GetNeighbours())
        {
            var angle = Vector3.Angle(self.transform.forward, e.transform.position - self.transform.position);
            if (angle < 135)
            {
                align += e.cc.velocity;
                count++;
            }
        }
        if (count == 0) return Vector3.zero;
        align /= count;

        return align.normalized * maxSpeed;
    }

    Vector3 Cohesion()
    {
        Vector3 center = self.transform.position;
        int count = 1;
        foreach (EnemyAgent e in self.GetNeighbours())
        {
            var angle = Vector3.Angle(self.transform.forward, e.transform.position - self.transform.position);
            if (angle < 135)
            {
                center += e.transform.position;
                count++;
            }
        }
        center /= count;
        Vector3 result = center - self.transform.position;
        //if (result.magnitude < 0.5) result = Vector3.zero;
        return result;
    }

    Vector3 Avoid()
    {
        RaycastHit hit;
        Vector3 result = Vector3.zero;
        Vector3 origem = self.transform.position + self.transform.up;
        if (Physics.Raycast(origem, self.transform.forward, out hit, radius, obstacle)) 
        {
            Debug.DrawRay(hit.point, hit.normal, Color.hotPink);
            result = ((hit.normal + self.cc.velocity.normalized) * 0.5f) * maxSpeed;
        }
        Debug.DrawRay(origem, self.transform.forward * radius, Color.pink);
        return result;
    }
}
