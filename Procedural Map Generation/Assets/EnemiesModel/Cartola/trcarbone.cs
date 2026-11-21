using UnityEngine;

public class trcarbone : MonoBehaviour
{
    public Transform bone2, bone1, bone3;
    public EnemyAttackCheck check;
    public void trocar()
    {
        check.attackPoint = bone2;
    }
    public void voltar()
    {
        check.attackPoint = bone1;
    }
    public void rodar()
    {
        check.attackPoint = bone3;
    }
}
