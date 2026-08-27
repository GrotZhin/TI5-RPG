using UnityEngine;

public abstract class AbstractMapGenerator : MonoBehaviour
{
  [SerializeField]
  protected Vector3Int startPosition = Vector3Int.zero;
  [SerializeField]
  protected PrefabVisualizer prefabVisualizer = null;

  public void GenerateMap()
    {
        RunProceduralGeneration();
    }

    protected abstract void RunProceduralGeneration();


}
