using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    public Transform seeker;
    public Transform destination;
    private bool pathFound;
    private Grid grid;

    private void Awake()
    {
        grid = GetComponent<Grid>();
        pathFound = false;
    }

    private void Update()
    {
        if (grid.gridCompleted && !pathFound)
        {
            FindPath(seeker.position, destination.position);
        }
    }

    public void FindPath(Vector3 startPos, Vector3 destinationPos)
    {
        var startNode = grid.NodeFromWorldPoint(startPos);
        var destinationNode = grid.NodeFromWorldPoint(destinationPos);
        
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count != 0)
        {
            Node currentNode = openSet[0];
            for (var i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FCost < currentNode.FCost)
                {
                    currentNode = openSet[i];
                }else if (openSet[i].FCost == currentNode.FCost)
                {
                    if (openSet[i].hCost < currentNode.hCost)
                    {
                        currentNode = openSet[i];
                    }
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);
                if (currentNode == destinationNode)
                {
                    RetracePath(startNode, destinationNode);
                    pathFound = true;
                    return;
                }

                var neighbours = grid.GetNeighbours(currentNode);
                var length = neighbours.Count;
                for (var index = 0; index < length; index++)
                {
                    var neighbour = neighbours[index];
                    if (!neighbour.walkable || closedSet.Contains(neighbour)) continue;
                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, destinationNode);
                        neighbour.parent = currentNode;
                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                    }
                }
            }
        }
        
    }

    private void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
        while (currentNode != startNode)
        {
            path.Add(currentNode.parent);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        grid.path = path;
    }

    private int GetDistance(Node a, Node b)
    {
        int distX = Mathf.Abs(a.gridX - b.gridX);
        int distY = Mathf.Abs(a.gridY - b.gridY);
        if (distX > 0)
        {
            return 14 * distY + 10 * (distX - distY);
        }
        
        return 14 * distX + 10 * (distY - distX);
    }
}
