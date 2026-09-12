using UnityEngine;

[CreateAssetMenu(fileName = "inimigoscriar", menuName = "inimigo/criar")]
public class inimigoscriar : ScriptableObject
{
    public MeshRenderer render;
    public EnemyAgent agente;
}
