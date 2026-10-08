using System;
using System.Collections.Generic;
using System.Data.Common;
using NUnit.Framework;
using UnityEngine;

public class PathFind : MonoBehaviour
{
    private GridManager gridManager;
    [SerializeField] Transform seeker, target;

    private void Awake()
    {
        gridManager = GetComponent<GridManager>();
    }
    void Update()
    {
        FindPath(seeker.position, target.position);
    }
    public void FindPath(Vector3 startPos, Vector3 targetPosition)
    {
        Node startNode = gridManager.GetNodeFromWorldPosition(startPos);
        Node targetNode = gridManager.GetNodeFromWorldPosition(targetPosition);

        List<Node> openedList = new List<Node>();
        HashSet<Node> closedList = new HashSet<Node>();

        openedList.Add(startNode);
        while (openedList.Count > 0)
        {
            Node currentNode = openedList[0];
            for (int i = 1; i < openedList.Count; i++)
            {
                if (openedList[i].fCost < currentNode.fCost || openedList[i].fCost == currentNode.fCost && openedList[i].hCost < currentNode.hCost){
                    currentNode = openedList[i];
                    }
            }
            openedList.Remove(currentNode);
            closedList.Add(currentNode);

            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }
            foreach (Node neighbour in gridManager.GetNeighbours(currentNode))
            {
                if (neighbour.isObstacle || closedList.Contains(neighbour))
                    continue;

                int newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);

                if (!openedList.Contains(neighbour) || neighbour.gCost > newCostToNeighbour)
                {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parentNode = currentNode;
                    if (!openedList.Contains(neighbour))
                        openedList.Add(neighbour);
                }
            }

        }
    }
    private void RetracePath(Node startNode, Node endNode)
    {
        Node currentNode = endNode;
        List<Node> path = new List<Node>();

        while(currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parentNode;
        }
        gridManager.path = path;
        
         
   }
    private int GetDistance(Node nodeA, Node nodeB)
    {
        int distanceX = Mathf.Abs(nodeA.posX - nodeB.posX);
        int distanceY = Mathf.Abs(nodeA.posY - nodeB.posY);

        if (distanceX > distanceY)
            return 14 * distanceY + 10 * (distanceX - distanceY);
        return 14 * distanceX + 10 * (distanceY - distanceX);
    }
}
