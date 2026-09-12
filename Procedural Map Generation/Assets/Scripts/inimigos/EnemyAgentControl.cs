using UnityEngine;

public class EnemyAgentControl
{
    public EnemyAgent self;
    public float maxSpeed = 5;
    public float radius;

    public float flee = 0.4f, seek = 0.6f, avoid;

    public EnemyAgentControl (EnemyAgent agent) {  self = agent; }


    public Vector3 Move(int fleeMod = 1, int seekMod = 1)
    {
        Vector3 dir = Vector3.zero;
        if (self.player != null)
            dir = self.player.transform.position - self.transform.position;

        //dir.y = menosagent.transform.position.y;

        if (dir.sqrMagnitude > 0.000001f)
        {
            dir.Normalize();
            dir.y = 0;
        }
        Debug.DrawRay(self.transform.position, dir, Color.blue);
        Debug.DrawRay(self.transform.position, Follow(), Color.green);
        Debug.DrawRay(self.transform.position, Separate(), Color.red);
        Debug.DrawRay(self.transform.position, Center(), Color.yellow);

        Vector3 result = dir + flee * fleeMod * Separate() + seek * seekMod * Follow() + Center();
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
            if (distance < 1.5f && Vector3.Dot(self.transform.forward, dot.normalized)>0)
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


    Vector3 Follow()
    {
        Vector3 follow = self.cc.velocity;
        if (self.GetNeighbours().Count == 0) return self.transform.forward * maxSpeed;
        foreach (EnemyAgent e in self.GetNeighbours())
        {
            follow += e.cc.velocity;
        }
        follow = follow / (self.GetNeighbours().Count + 1);

        return follow.normalized * maxSpeed;
    }

    Vector3 Center()
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

    public Vector3 PesoMover(Vector3 mover, float dirWeight,  Vector3 separar, float separationWeight)
    {
        Vector3 resultado =
            mover * dirWeight +
            separar * separationWeight;

        if (resultado.sqrMagnitude > 0f)
            resultado.Normalize();

        return resultado;
    }
}
