using UnityEngine;

[CreateAssetMenu(fileName = "SkillTree", menuName = "Scriptable Objects/SkillTree")]
public class SkillTree : ScriptableObject
{
    public SkillData firstLightHit;
    public SkillData secondLightHit;
    public SkillData thirdLightHit;
    public SkillData HeavyCut;
}
