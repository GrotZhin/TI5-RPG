using UnityEngine;

public class CorredorPortal : MonoBehaviour
{
    [SerializeField] Renderer[] paredesInternas;
    [SerializeField] BoxCollider area;
    [SerializeField] Camera cam;

    const int Equal = 3;
    const int Always = 8;

    Material[] mats;
    bool dentro;

    void Start()
    {
        if (area == null) area = GetComponent<BoxCollider>();
        if (cam == null) cam = Camera.main;

        mats = new Material[paredesInternas.Length];
        for (int i = 0; i < paredesInternas.Length; i++)
            mats[i] = paredesInternas[i].material;

        Definir(false);
    }

    void LateUpdate()
    {
        bool agora = CameraDentro();
        if (agora != dentro) Definir(agora);
    }

    bool CameraDentro()
    {
        Vector3 p = transform.InverseTransformPoint(cam.transform.position) - area.center;
        Vector3 meio = area.size * 0.5f;
        return Mathf.Abs(p.x) <= meio.x && Mathf.Abs(p.y) <= meio.y && Mathf.Abs(p.z) <= meio.z;
    }

    void Definir(bool valor)
    {
        dentro = valor;
        foreach (var m in mats)
            m.SetFloat("_StencilComp", valor ? Always : Equal);
    }
}