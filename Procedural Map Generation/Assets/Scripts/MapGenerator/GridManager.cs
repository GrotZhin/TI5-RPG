using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private LayerMask ObstacleMask;
    [SerializeField] private Vector2 gridWorldSize;
    [SerializeField] private float nodeRadius;

    private float nodeDiamenter;
    private int gridSizeX;
    private int gridSizeY;

    Node[,] grid;
    public List<Node> path;
    void Start()
    {
         nodeDiamenter = nodeRadius * 2;
         gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiamenter);
         gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiamenter);

        //CreateGrid();
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
        if (grid != null)
        {
            Node playerNode = GetNodeFromWorldPosition(playerTransform.position);
            foreach (Node node in grid)
            {
                Gizmos.color = node.isObstacle ? Color.red : Color.white;
                if (node == playerNode) Gizmos.color = Color.purple;
                if (path != null && path.Contains(node)) Gizmos.color = Color.black;
                Gizmos.DrawCube(node.worldPosition, nodeDiamenter * Vector3.one);
            }
        }
    }
    public void InstantiateGrid()
    {
        nodeDiamenter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiamenter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiamenter);
        CreateGrid();
    }
    private void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldPosition, gridBottomLeft;
        bool isObstacle;
        gridBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                worldPosition = gridBottomLeft + Vector3.right *
                (x * nodeDiamenter + nodeRadius) + Vector3.forward * (y * nodeDiamenter + nodeRadius);
                isObstacle = Physics.CheckSphere(worldPosition, nodeRadius, ObstacleMask);
                grid[x, y] = new Node(worldPosition, isObstacle, x, y);
            }
        }
    }
    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int newX = node.posX + x;
                int newY = node.posY + y;

                if (newX >= 0 && newX < gridSizeX && newY >= 0 && newY < gridSizeY)
                {
                    neighbours.Add(grid[newX, newY]);
                }
            }
        }

        return neighbours;
    }
    public Node GetNodeFromWorldPosition(Vector3 worldPosition)
    {
        Vector3 localPos = worldPosition - transform.position;
        float percentX = (localPos.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (localPos.z + gridWorldSize.y / 2) / gridWorldSize.y;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }
    public Vector3 GetVector3FromNodePosition(Node node)
    {
        Vector3 pos =new Vector3(node.posX, 0, node.posY);
        Vector3 localPos = pos - transform.position;  
        return localPos;

    }

}
