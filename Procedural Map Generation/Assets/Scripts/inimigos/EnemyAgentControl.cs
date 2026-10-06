using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;
using Random = UnityEngine.Random;

[Serializable]
public class EnemyAgentControl
{
    public EnemyAgent self;
    public float radius = 4.8f;
    public float vida = 0f;
    [Range(0,1)]
    public float separate = 0.9f, seek = 0.6f, align = 0.03f, avoid = 1f, cohesion = 0.05f;
    Vector3 dir;
    public LayerMask obstacle;

    public EnemyAgentControl (EnemyAgent agent) {  self = agent; obstacle = LayerMask.GetMask("Obstacle"); }

    public Vector3 Move(int fleeMod = 1, Vector3? dire = null)
    {
        if (self.player != null)
        {
            dir = self.player.transform.position - self.transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.000001f)
            {
                dir.Normalize();
                dir *= seek;
            }
        }
        else
        {
            dir = self.transform.forward;

        }
        
        Vector3 result =
        dir * fleeMod +
        separate * Separate() +
        align * Align() +
        cohesion * Cohesion() +
        avoid * Avoid();
        Debug.DrawRay(self.transform.position, result, Color.black);
        Debug.DrawRay(self.transform.position, Separate(), Color.red);
        Debug.DrawRay(self.transform.position, Align(), Color.green);
        Debug.DrawRay(self.transform.position, Cohesion(), Color.yellow);
        Debug.DrawRay(self.transform.position, Avoid(), Color.blue);
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
            float distanceSqr = direction.sqrMagnitude;
            if (distanceSqr < 0.0001f)
                continue;

            float distance = Mathf.Sqrt(distanceSqr);
            // Distância máxima em que queremos separar
            if (distance < 5f)
            {
                Vector3 toOther = other.transform.position - self.transform.position;
                toOther.y = 0f;
                // Só separa quem está à frente
                if (Vector3.Dot(self.transform.forward, toOther.normalized) > 0f)
                {
                    // Quanto mais perto, maior a força
                    float strength = 1f - (distance / 3f);

                    separation += direction.normalized * strength;
                    count++;
                }
            }
        }
        if (count > 0)
            separation /= count;

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

        return align.normalized;
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
            result = ((hit.normal + self.cc.velocity.normalized) * 0.5f);
        }
        Debug.DrawRay(origem, self.transform.forward * radius, Color.pink);
        return result;
    }
    public void dano(int dano)
    {
        vida -= dano;
    }
}
