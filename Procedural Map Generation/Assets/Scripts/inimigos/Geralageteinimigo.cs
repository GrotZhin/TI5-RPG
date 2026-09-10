using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Geralageteinimigo : MonoBehaviour
{
    public static Geralageteinimigo Geralinimigo;
    List<inimigoagente> agenteini = new List<inimigoagente>();

    void Awake()
    {
        if (Geralinimigo == null)
        {
            Geralinimigo = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Addageteinimigo(inimigoagente obs)
    {
       agenteini.Add(obs);
    }

    public Vector3 Mover(Vector3 dirtmp, inimigoagente menosagente)
    {
        Vector3 target = dirtmp + menosagente.player.transform.position;

        Vector3 dir = target - menosagente.transform.position;
        //dir.y = menosagente.transform.position.y;

        if (dir.sqrMagnitude > 0f)
        {
            dir.Normalize();
            dir.y = 0;
        }
       return dir;
    }


    public Vector3 Separar(inimigoagente menosagente, ref int i)
    {
        Vector3 separation = Vector3.zero;
        int count = 0;
        /*foreach (inimigoagente other in agenteini)
        {
            if (other != menosagente)
            {
                Vector3 offset = other.transform.position - menosagente.transform.position;
                offset.y = 0f;

                float distance = offset.magnitude;
                if (distance < 1.5f && offset.normalized.z >=0)
                {
                    // Quanto mais próximo, maior a repulsão
                    separation += offset.normalized / distance;
                    count++;
                }
            }
        }*/
        foreach (inimigoagente other in menosagente.passavizinho())
        {
            Vector3 offset = menosagente.transform.position - other.transform.position;
            offset.y = 0f;
            Vector3 dot = other.transform.position - menosagente.transform.position;

            float distance = offset.magnitude;
            if (distance < 1.5f && Vector3.Dot(menosagente.transform.forward, dot.normalized)>0)
            {
                // Quanto mais próximo, maior a repulsão
                separation += offset.normalized / distance;
                count++;
            }
        }
            if (count > 0)
        {
            separation /= count;
            i= count;
        }
        //Debug.Log(menosagente.passavizinho().Count + "  " + separation);
        return separation;
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
