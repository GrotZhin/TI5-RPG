using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Folhas caindo e flutuando ao vento. Toda a animação roda no shader (GPU),
/// o script só desenha uma vez com instancing, então o custo na CPU é mínimo.
/// A caixa é centrada neste objeto.
/// </summary>
public class LeafSystem : MonoBehaviour
{
    struct Leaf { public Matrix4x4 objectToWorld; }

    [Header("Material")]
    [Tooltip("Material usando o shader Custom/LeavesBOTW")]
    public Material sourceMaterial;

    [Header("Caixa (centrada neste objeto)")]
    public Vector3 boxSize = new Vector3(30, 12, 30);

    [Header("Quantidade")]
    [Range(1, 5000)] public int leafCount = 300;
    public int seed = 4321;

    [Header("Tamanho")]
    [Min(0.01f)] public float leafSize = 0.18f;
    [Range(0, 1)] public float sizeVariation = 0.5f;

    [Header("Movimento")]
    public float fallSpeed = 0.8f;
    public float sway = 0.6f;
    public float spinSpeed = 1.5f;
    [Tooltip("Inclinação máxima da folha, em radianos")]
    public float tilt = 0.8f;
    public Vector2 windDirection = new Vector2(1, 0.3f);
    public float windSpeed = 1.2f;

    Material mat;
    Mesh mesh;
    Leaf[] leaves;

    void OnEnable()
    {
        if (sourceMaterial == null)
        {
            Debug.LogError("LeafSystem: atribua o material (Custom/LeavesBOTW).", this);
            enabled = false;
            return;
        }
        mat = new Material(sourceMaterial) { enableInstancing = true };
        mesh = BuildLeafMesh();
        Generate();
    }

    void OnDisable()
    {
        if (mat != null) Destroy(mat);
        if (mesh != null) Destroy(mesh);
        leaves = null;
    }

    [ContextMenu("Regenerar folhas")]
    public void Generate()
    {
        var rnd = new System.Random(seed);
        leaves = new Leaf[leafCount];
        for (int i = 0; i < leafCount; i++)
        {
            // Posição normalizada (0..1) dentro da caixa; o shader converte para metros
            var u = new Vector3((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble());
            leaves[i].objectToWorld = Matrix4x4.TRS(u, Quaternion.identity, Vector3.one);
        }
    }

    void Update()
    {
        if (mat == null) return;
        if (leaves == null || leaves.Length != leafCount) Generate();

        Vector3 center = transform.position;
        Vector3 min = center - boxSize * 0.5f;

        mat.SetVector("_BoxMin", min);
        mat.SetVector("_BoxSize", boxSize);
        mat.SetFloat("_Size", leafSize);
        mat.SetFloat("_SizeVariation", sizeVariation);
        mat.SetFloat("_FallSpeed", fallSpeed);
        mat.SetFloat("_Sway", sway);
        mat.SetFloat("_SpinSpeed", spinSpeed);
        mat.SetFloat("_Tilt", tilt);
        mat.SetVector("_WindDir", new Vector4(windDirection.x, windDirection.y, 0, 0));
        mat.SetFloat("_WindSpeed", windSpeed);

        var rp = new RenderParams(mat)
        {
            shadowCastingMode = ShadowCastingMode.Off,
            receiveShadows = false,
            layer = gameObject.layer,
            worldBounds = new Bounds(center, boxSize + Vector3.one * (sway * 2f + leafSize * 2f))
        };

        Graphics.RenderMeshInstanced(rp, mesh, 0, leaves, leafCount);
    }

    // Quad de 1x1 deitado no plano XZ; o comprimento da folha vai no eixo Z (uv.y)
    static Mesh BuildLeafMesh()
    {
        var m = new Mesh { name = "LeafQuad" };
        m.vertices = new[]
        {
            new Vector3(-0.5f, 0, -0.5f),
            new Vector3( 0.5f, 0, -0.5f),
            new Vector3(-0.5f, 0,  0.5f),
            new Vector3( 0.5f, 0,  0.5f)
        };
        m.uv = new[]
        {
            new Vector2(0, 0), new Vector2(1, 0),
            new Vector2(0, 1), new Vector2(1, 1)
        };
        m.triangles = new[] { 0, 2, 1, 1, 2, 3 };
        m.bounds = new Bounds(Vector3.zero, Vector3.one);
        return m;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.6f, 0.2f, 0.6f);
        Gizmos.DrawWireCube(transform.position, boxSize);
    }
}
