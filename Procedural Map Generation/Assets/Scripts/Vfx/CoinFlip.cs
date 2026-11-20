using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;
public class CoinFlip : MonoBehaviour
{
    public float Rot;
    void Start()
    {
    gameObject.transform.DORotate(new Vector3(360,360,360),Rot,RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1,LoopType.Incremental).SetUpdate(true);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
