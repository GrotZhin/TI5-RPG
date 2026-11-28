using UnityEngine;

[CreateAssetMenu(fileName = "SkillData", menuName = "Scriptable Objects/SkillData")]
public class SkillData : ScriptableObject
{
    [SerializeField]
    private float force;
    [SerializeField]
    private float range;
    [SerializeField]
    private float hitBox;
    [SerializeField]
    private bool left;
    [SerializeField]
    private bool both;
    [SerializeField]
    private bool arrow;
    [SerializeField]
    private float hitStop = 0.05f;
    [SerializeField]
    private float stylePoints = 10f;

    public float StylePoints => stylePoints;
    public float HitBox => hitBox;
    public float Force => force;
    public float Range => range;
    public float HitStop => hitStop;
    public bool Left => left;
    public bool Both => both;
    public bool Arrow => arrow;


}
