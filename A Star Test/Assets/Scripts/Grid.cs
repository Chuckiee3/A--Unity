using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public LayerMask obstacleMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    public Transform player;
    public List<Node> path;
    public bool gridCompleted = false;
    private Node[,] grid;

    private float nodeDiameter;
    private int gridSizeX;
    private int gridSizeY;

    private void Start()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    private void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - gridWorldSize.x / 2 *  Vector3.right - gridWorldSize.y / 2 * Vector3.forward;
        
        for (var i = 0; i < gridSizeX; i++)
        {
            var xVal = (i * nodeDiameter + nodeRadius);
            for (var j = 0; j < gridSizeY; j++)
            {
                var yVal = (j * nodeDiameter + nodeRadius);
                Vector3 worldPoint = worldBottomLeft + Vector3.right * xVal + Vector3.forward * (yVal);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, obstacleMask));
                grid[i, j] = new Node(walkable, worldPoint ,i ,j );
            }
        }

        gridCompleted = true;
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = Mathf.Clamp((worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x,0,1);
        float percentY = Mathf.Clamp((worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y,0,1);
        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];

    }

    public List<Node> GetNeighbours(Node node)
    {
        var neighbours = new List<Node>();
        for (var i = -1; i <= 1 ; i++)
        {
            for (var j = -1; j <= 1 ; j++)
            {
                if (i == 0 && j == 0) continue;
                int checkX = node.gridX + i;
                int checkY = node.gridY + j;
                if(checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) neighbours.Add(grid[checkX,checkY]);
            }
        }

        return neighbours;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
        if (grid != null)
        {
            Node playerNode = NodeFromWorldPoint(player.position);
            foreach (Node node in grid)
            {
                Gizmos.color = (node.walkable) ? Color.white : Color.black;
                if (playerNode == node)
                {
                    Gizmos.color = Color.blue;
                }
                if (path != null)
                {
                    if (path.Contains(node))
                    {
                        Gizmos.color = Color.magenta;
                    }
                }
                Gizmos.DrawCube(node.worldPosition, Vector3.one * (nodeDiameter-.1f));
            
            }

        }

      
    }
}
