using System;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class EnemyAgentControl
{
    public EnemyAgent self;
    public float maxSpeed = 5;
    public float radius = 4.8f;

    [Range(0,1)]
    public float separate = 0.4f, seek = 0.6f, align = 0.6f, avoid = 1, cohesion = 1;

    public EnemyAgentControl (EnemyAgent agent) {  self = agent; }


    public Vector3 Move(int fleeMod = 1, int seekMod = 1)
    {
        Vector3 dir = self.transform.forward * maxSpeed;
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
        if (self.GetNeighbours().Count == 0) return Vector3.zero;
        foreach (EnemyAgent e in self.GetNeighbours())
        {
            align += e.cc.velocity;
        }
        align = align / (self.GetNeighbours().Count + 1);

        return align.normalized * maxSpeed;
    }

    Vector3 Cohesion()
    {
        Vector3 center = self.transform.position;
        foreach (EnemyAgent e in self.GetNeighbours())
        {
            center += e.transform.position;
        }
        center /= (self.GetNeighbours().Count + 1);
        Vector3 result = center - self.transform.position;
        if (result.magnitude < 0.5) result = Vector3.zero;
        return result;
    }

    Vector3 Avoid()
    {
        Vector3 result = Vector3.zero;
        /*
         -------------------------------------

          Implementar Avoid de obstáculos aqui

        ----------------------------------------
        */
        return result;
    }
}
