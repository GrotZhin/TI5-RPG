using UnityEngine;
using UnityEditor.ShaderGraph;
public class RoomWall : MonoBehaviour
{
    private GameObject player;
    private Material mat;

    void Start()
    {
        // Encontra o jogador pela Tag no início
        player = GameObject.FindGameObjectWithTag("Player");
        
        // Pega o material do MeshRenderer anexado a este GameObject
        mat = GetComponent<MeshRenderer>().material;
    }

    void Update()
    {
        // Garante que o player foi encontrado para evitar erros no console
        if (player != null && mat != null)
        {
            // Atualiza a propriedade _Pos do Shader a cada frame com a posição atual do Player
            mat.SetVector("_Pos", player.transform.position);
        }
    }
}
