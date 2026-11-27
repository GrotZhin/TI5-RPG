using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GrassSystem : MonoBehaviour
{
    public enum SpawnMode { LayerObjects, Box }

    struct Blade { public Matrix4x4 objectToWorld; }

    class Chunk
    {
        public Bounds bounds;
        public Blade[] blades;
    }

    class Bucket
    {
        public List<Blade> blades = new List<Blade>();
        public Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        public Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
    }

    [Header("Material")]
    [Tooltip("Material usando o shader Custom/Grass")]
    public Material sourceMaterial;

    [Header("Onde nasce a grama")]
    public SpawnMode mode = SpawnMode.LayerObjects;
    [Tooltip("LayerObjects: layers dos objetos que recebem grama. Box: layers do chão para o raycast.")]
    public LayerMask groundMask = ~0;
    [Range(0, 90)] public float maxSlope = 40f;
    [Range(0, 1)] public float alignToNormal = 0.3f;
    [Tooltip("Limite")]
    public int maxBlades = 300000;

    [Header("Bloqueio")]
    [Tooltip("Layers dos objetos que NÃO podem ter grama")]
    public LayerMask blockerMask = 0;
    [Tooltip("Raio da checagem de colisão")]
    public float blockerRadius = 0.12f;
    public bool skipCovered = true;
    public float coverHeight = 40f;

    [Header("Modo Box)")]
    public Vector2 areaSize = new Vector2(60, 60);
    public float rayHeight = 50f;

    [Header("Densidade")]
    [Tooltip("Gramas")]
    public float density = 18f;
    public float chunkSize = 10f;
    public int seed = 1234;
    public float bladeWidth = 0.09f;
    [Range(2, 6)] public int bladeSegments = 3;

    [Header("Altura")]
    [Min(0)] public float grassHeight = 0.3f;
    [Range(0, 1)] public float heightVariation = 0.4f;

    [Header("Distância / LOD")]
    public float drawDistance = 40f;
    public float lodStartDistance = 15f;
    [Range(0.05f, 1)] public float farDensity = 0.3f;

    [Header("Vento")]
    public float windStrength = 0.25f;
    public float windSpeed = 1.2f;
    public float windScale = 0.15f;
    public Vector2 windDirection = new Vector2(1, 0.4f);

    [Header("Interação")]
    public Transform player;
    public float playerRadius = 1.5f;

    static readonly int PlayerPosId = Shader.PropertyToID("_GrassPlayerPos");

    readonly List<Chunk> chunks = new List<Chunk>();
    readonly Dictionary<Vector2Int, Bucket> buckets = new Dictionary<Vector2Int, Bucket>();
    readonly Dictionary<Vector3Int, bool> coverCache = new Dictionary<Vector3Int, bool>();
    Material mat;
    Mesh mesh;
    Plane[] planes = new Plane[6];
    bool warnedCam;
    int totalBlades;

    void OnEnable()
    {
        if (sourceMaterial == null)
        {
            Debug.LogError("Sem material", this);
            enabled = false;
            return;
        }

        mat = new Material(sourceMaterial) { enableInstancing = true };
        mesh = BuildBladeMesh(bladeSegments, bladeWidth);
        var sw = System.Diagnostics.Stopwatch.StartNew();
        Generate();
        sw.Stop();

        if (totalBlades == 0)
            Debug.LogWarning("0 gramas", this);
        else
            Debug.Log(" " + totalBlades + " gramas geradas em " + chunks.Count + " chunk(s), " + sw.Elapsed.TotalSeconds.ToString("F1") + " s.", this);

        Shader.SetGlobalVector(PlayerPosId, new Vector4(0, -9999, 0, 0.001f));
    }

    void OnDisable()
    {
        if (mat != null) Destroy(mat);
        if (mesh != null) Destroy(mesh);
        chunks.Clear();
    }

    [ContextMenu("Regenerar grama")]
    public void Generate()
    {
        chunks.Clear();
        buckets.Clear();
        coverCache.Clear();
        totalBlades = 0;
        var rnd = new System.Random(seed);

        if (mode == SpawnMode.LayerObjects) ScatterOnLayerMeshes(rnd);
        else ScatterInBox(rnd);

        foreach (var kv in buckets)
        {
            Bucket b = kv.Value;
            if (b.blades.Count == 0) continue;
            var bounds = new Bounds();
            bounds.SetMinMax(b.min, b.max);
            chunks.Add(new Chunk { bounds = bounds, blades = b.blades.ToArray() });
        }

        buckets.Clear();
    }

    void ScatterOnLayerMeshes(System.Random rnd)
    {
        MeshFilter[] filters = FindObjectsByType<MeshFilter>(FindObjectsSortMode.None);

        foreach (MeshFilter mf in filters)
        {
            if (!mf.gameObject.activeInHierarchy) continue;
            if ((groundMask.value & (1 << mf.gameObject.layer)) == 0) continue;

            Mesh m = mf.sharedMesh;
            if (m == null) continue;
            if (!m.isReadable)
            {
                Debug.LogWarning("a malha '" + m.name + "' (objeto '" + mf.name +
                    "') não tem Read/Write ligado", mf);
                continue;
            }

            Vector3[] verts = m.vertices;
            int[] tris = m.triangles;
            Matrix4x4 l2w = mf.transform.localToWorldMatrix;

            var wv = new Vector3[verts.Length];
            for (int i = 0; i < verts.Length; i++) wv[i] = l2w.MultiplyPoint3x4(verts[i]);

            for (int t = 0; t < tris.Length; t += 3)
            {
                Vector3 a = wv[tris[t]], b = wv[tris[t + 1]], c = wv[tris[t + 2]];
                Vector3 cross = Vector3.Cross(b - a, c - a);
                float area = cross.magnitude * 0.5f;
                if (area < 1e-6f) continue;

                Vector3 n = cross.normalized;
                if (Vector3.Angle(n, Vector3.up) > maxSlope) continue;

                float expected = area * density;
                int count = (int)expected;
                if (rnd.NextDouble() < expected - count) count++;

                for (int k = 0; k < count; k++)
                {
                    if (totalBlades >= maxBlades)
                    {

                        return;
                    }

                    float r1 = Mathf.Sqrt((float)rnd.NextDouble());
                    float r2 = (float)rnd.NextDouble();
                    Vector3 p = (1 - r1) * a + r1 * (1 - r2) * b + r1 * r2 * c;
                    AddBlade(p, n, rnd);
                }
            }
        }
    }

    void ScatterInBox(System.Random rnd)
    {
        Vector3 center = transform.position;
        Vector3 min = new Vector3(center.x - areaSize.x * 0.5f, 0, center.z - areaSize.y * 0.5f);
        int total = Mathf.RoundToInt(density * areaSize.x * areaSize.y);

        for (int i = 0; i < total; i++)
        {
            if (totalBlades >= maxBlades) break;

            float px = min.x + (float)rnd.NextDouble() * areaSize.x;
            float pz = min.z + (float)rnd.NextDouble() * areaSize.y;
            Vector3 start = new Vector3(px, center.y + rayHeight, pz);

            if (!Physics.Raycast(start, Vector3.down, out RaycastHit hit, rayHeight * 2f,
                    groundMask, QueryTriggerInteraction.Ignore)) continue;
            if (Vector3.Angle(hit.normal, Vector3.up) > maxSlope) continue;

            AddBlade(hit.point, hit.normal, rnd);
        }
    }

    bool IsBlocked(Vector3 p)
    {
        if (blockerMask.value == 0) return false;

        if (Physics.CheckSphere(p + Vector3.up * (blockerRadius + 0.1f), blockerRadius,
                blockerMask, QueryTriggerInteraction.Collide))
            return true;

        if (skipCovered)
        {
            var key = new Vector3Int(Mathf.FloorToInt(p.x * 2f), Mathf.FloorToInt(p.y * 2f), Mathf.FloorToInt(p.z * 2f));
            if (!coverCache.TryGetValue(key, out bool covered))
            {
                covered = Physics.Raycast(p + Vector3.up * coverHeight, Vector3.down, coverHeight - 0.2f,
                    blockerMask, QueryTriggerInteraction.Collide);
                coverCache[key] = covered;
            }

            if (covered) return true;
        }

        return false;
    }

    void AddBlade(Vector3 pos, Vector3 normal, System.Random rnd)
    {
        if (IsBlocked(pos)) return;

        Quaternion align = Quaternion.FromToRotation(Vector3.up,
            Vector3.Slerp(Vector3.up, normal, alignToNormal));
        Quaternion rot = align * Quaternion.Euler(0, (float)rnd.NextDouble() * 360f, 0);
        float w = Mathf.Lerp(0.8f, 1.3f, (float)rnd.NextDouble());

        var key = new Vector2Int(Mathf.FloorToInt(pos.x / chunkSize), Mathf.FloorToInt(pos.z / chunkSize));
        if (!buckets.TryGetValue(key, out Bucket bucket))
        {
            bucket = new Bucket();
            buckets[key] = bucket;
        }

        bucket.blades.Add(new Blade { objectToWorld = Matrix4x4.TRS(pos, rot, new Vector3(w, 1, w)) });
        bucket.min = Vector3.Min(bucket.min, pos);
        bucket.max = Vector3.Max(bucket.max, pos);
        totalBlades++;
    }

    void Update()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            if (!warnedCam)
            {
                warnedCam = true;
            }

            return;
        }

        if (mat == null) return;

        mat.SetFloat("_Height", grassHeight);
        mat.SetFloat("_HeightVariation", heightVariation);
        mat.SetFloat("_WindStrength", windStrength);
        mat.SetFloat("_WindSpeed", windSpeed);
        mat.SetFloat("_WindScale", windScale);
        mat.SetVector("_WindDir", new Vector4(windDirection.x, windDirection.y, 0, 0));
        mat.SetFloat("_FadeStart", drawDistance * 0.75f);
        mat.SetFloat("_FadeEnd", drawDistance);

        if (player != null)
        {
            Vector3 p = player.position;
            Shader.SetGlobalVector(PlayerPosId, new Vector4(p.x, p.y, p.z, playerRadius));
        }

        GeometryUtility.CalculateFrustumPlanes(cam, planes);
        Vector3 camPos = cam.transform.position;

        var rp = new RenderParams(mat)
        {
            shadowCastingMode = ShadowCastingMode.Off,
            receiveShadows = true,
            layer = gameObject.layer
        };

        float heightPad = grassHeight * 1.5f + 0.5f;

        for (int i = 0; i < chunks.Count; i++)
        {
            Chunk c = chunks[i];

            float dist = Mathf.Sqrt(c.bounds.SqrDistance(camPos));
            if (dist > drawDistance) continue;

            Bounds b = c.bounds;
            b.Expand(new Vector3(0, heightPad, 0));
            if (!GeometryUtility.TestPlanesAABB(planes, b)) continue;

            float t = Mathf.InverseLerp(lodStartDistance, drawDistance, dist);
            int count = Mathf.CeilToInt(c.blades.Length * Mathf.Lerp(1f, farDensity, t));

            rp.worldBounds = b;
            Graphics.RenderMeshInstanced(rp, mesh, 0, c.blades, count);
        }
    }

    static Mesh BuildBladeMesh(int segments, float width)
    {
        var verts = new List<Vector3>();
        var uvs = new List<Vector2>();
        var tris = new List<int>();

        for (int i = 0; i < segments; i++)
        {
            float t = i / (float)segments;
            float w = Mathf.Lerp(0.5f, 0.12f, t) * width;
            float z = t * t * 0.12f;
            verts.Add(new Vector3(-w, t, z));
            verts.Add(new Vector3(w, t, z));
            uvs.Add(new Vector2(0, t));
            uvs.Add(new Vector2(1, t));
        }

        verts.Add(new Vector3(0, 1, 0.14f));
        uvs.Add(new Vector2(0.5f, 1));

        for (int i = 0; i < segments - 1; i++)
        {
            int a = i * 2, b = a + 1, c = a + 2, d = a + 3;
            tris.AddRange(new[] { a, c, b, b, c, d });
        }

        int last = (segments - 1) * 2;
        tris.AddRange(new[] { last, segments * 2, last + 1 });

        var m = new Mesh { name = "GrassBlade" };
        m.SetVertices(verts);
        m.SetUVs(0, uvs);
        m.SetTriangles(tris, 0);
        m.bounds = new Bounds(new Vector3(0, 0.5f, 0), new Vector3(1, 1.5f, 1));
        return m;
    }

    void OnDrawGizmosSelected()
    {
        if (mode != SpawnMode.Box) return;
        Gizmos.color = new Color(0.3f, 1f, 0.3f, 0.6f);
        Gizmos.DrawWireCube(transform.position, new Vector3(areaSize.x, 0.2f, areaSize.y));
    }
}