using UnityEngine;
using UnityEngine.Animations.Rigging;
public class FaceAdvanced : MonoBehaviour
{
    public Transform body;
    public MultiAimConstraint aim;
    public MultiAimConstraint aim2;
    public float spd;
    public GameObject TargetPos;
    private Transform InterestTransform;
    private Vector3 OgPos;
    bool islooking=false;
    void Start()
    {

        OgPos = TargetPos.transform.localPosition;
    }

    void Update()
    {
        if (InterestTransform != null)
    {
        TargetPos.transform.position = Vector3.Lerp(TargetPos.transform.position, InterestTransform.position, spd * Time.deltaTime);
    }
    else
    {
        TargetPos.transform.localPosition = Vector3.Lerp(TargetPos.transform.localPosition, OgPos, spd * Time.deltaTime);
    }
    float targetWeight = 0f;
    float targetWeight2 = 0f;

    if (islooking)
    {
        float difference = TargetPos.transform.position.z - body.position.z;
        targetWeight = (difference < 1) ? 0f : 1f;
        targetWeight2 = (difference < 1) ? 0f : 0.45f;
    }
    aim.weight = Mathf.MoveTowards(aim.weight, targetWeight, spd * Time.deltaTime);
    aim2.weight = Mathf.MoveTowards(aim2.weight, targetWeight2, spd * Time.deltaTime);

    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interest"))
        {
            InterestTransform = other.transform;
        }
        islooking=true;
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interest") && InterestTransform == other.transform)
        {
            InterestTransform = null;
        }
        islooking=false;
    }
}
