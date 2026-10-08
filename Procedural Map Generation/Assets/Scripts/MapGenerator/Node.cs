using UnityEngine;

public class Node
{
    public Vector3 worldPosition;
    public bool isObstacle;

    public int gCost, hCost;
    public int posX, posY;
    public int sizeX, sizeY;
    public Node parentNode;


    public Node(Vector3 worldPosition, bool isObstacle, int posX, int posY)
    {
        this.worldPosition = worldPosition;
        this.isObstacle = isObstacle;
        this.posX = posX;
        this.posY = posY;
    }

    public int fCost
    {
        get{return gCost + hCost;}
    }
}
