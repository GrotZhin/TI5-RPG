#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class ProximityBarrierMenu
{
    const string MatPath = "Assets/ProximityBarrier.mat";

    [MenuItem("GameObject/Proximity Barrier/Create Barrier Quad", false, 10)]
    static void CreateBarrier()
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Quad);
        go.name = "BarrierQuad";
        Undo.RegisterCreatedObjectUndo(go, "Create Barrier Quad");
        var sv = SceneView.lastActiveSceneView;
        if (sv != null) go.transform.position = sv.pivot;
        go.transform.localScale = new Vector3(10f, 5f, 1f);
        go.GetComponent<MeshRenderer>().sharedMaterial = GetOrCreateMaterial();
        var barrier = go.AddComponent<ProximityBarrier>();
        barrier.SetupCollider();
        Selection.activeGameObject = go;
    }

    [MenuItem("GameObject/Proximity Barrier/Create Barrier Material", false, 11)]
    static void CreateMaterialOnly()
    {
        Selection.activeObject = GetOrCreateMaterial();
    }

    static Material GetOrCreateMaterial()
    {
        var mat = AssetDatabase.LoadAssetAtPath<Material>(MatPath);
        if (mat != null) return mat;

        var shader = Shader.Find("Universal Render Pipeline/Unlit");
        if (shader == null) shader = Shader.Find("Sprites/Default");

        mat = new Material(shader) { name = "ProximityBarrier" };

        mat.SetFloat("_Surface", 1f);
        mat.SetFloat("_Blend", 0f);
        mat.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetFloat("_ZWrite", 0f);
        mat.SetFloat("_Cull", 0f);
        mat.SetOverrideTag("RenderType", "Transparent");
        mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        mat.SetShaderPassEnabled("ShadowCaster", false);
        mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        mat.SetColor("_BaseColor", new Color(0.4f, 0.8f, 1f, 0.5f));

        AssetDatabase.CreateAsset(mat, MatPath);
        AssetDatabase.SaveAssets();
        return mat;
    }
}
#endif