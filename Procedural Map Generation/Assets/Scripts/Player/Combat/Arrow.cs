using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Arrow : MonoBehaviour
{
    float speed = 48f;
    float lifeSpawn;
    SkillData skill;
    public LayerMask layer;

    public readonly HashSet<Agent> alreadyHit = new HashSet<Agent>();


    public void Init(SkillData skill)
    {
        this.skill = skill;
        lifeSpawn = skill.Range/speed;
        Destroy(gameObject, lifeSpawn + 0.1f);
        transform.DOScale(Vector3.zero, lifeSpawn).SetEase(Ease.InQuart);
    }

    // Update is called once per frame
    void Update()
    {
        CheckCollision(transform, skill);
        transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
    }
    void CheckCollision(Transform point, SkillData data)
    {
        Collider[] colliders = Physics.OverlapBox(point.position, new Vector3(data.HitBox, data.HitBox, data.HitBox), Quaternion.identity, layer);

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].TryGetComponent<Agent>(out Agent enemy))
            {
                if (!alreadyHit.Add(enemy)) continue;
                if (enemy.stats.Life > 0)
                {
                    GameFeel.gameFeel.Stop(data.HitStop);
                    GameFeel.gameFeel.CamaraShake();
                    if (StyleSystem.Instance != null) StyleSystem.Instance.RegisterHit(data);
                    enemy.ChangeState(new ReceiveDamage(enemy, data.Force, enemy.renderer));
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        if (skill == null) return;
        if (skill.Arrow)
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(skill.HitBox, skill.HitBox, skill.HitBox));
        }

    }

}
