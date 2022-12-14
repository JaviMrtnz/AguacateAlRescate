using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;

public class Pathfinding : MonoBehaviour
{
    // PathRequestManager requestManager;
    public static Grid grid;
    //public Transform target;

    void Awake()
    {

        grid = GetComponent<Grid>();
    }

    public void StartFindPath(Vector3 startPos, Vector3 targetPos)
    {
        //StartCoroutine(FindPath(startPos, targetPos));
    }

    public static List<Vector3> FindPathArchersKing(Vector3 startPos, Vector3 targetPos)
    {
        // Get positions as nodes
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        // Initialize the open and closed set
        Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
        HashSet<Node> closedSet = new HashSet<Node>();

        // Start the open set with the first node
        openSet.Add(startNode);

        // While there are nodes on the open set, keep looking for a route
        while (openSet.Count > 0)
        {
            Node currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);

            // Path found
            if (currentNode == targetNode)
            {
                return RetracePath(startNode, currentNode);
            }

            foreach (Node neighbour in grid.GetNeighbours(currentNode))
            {
                if (neighbour.hasTree || !neighbour.walkable || closedSet.Contains(neighbour) || neighbour.hasUnit)
                    continue;

                float newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.gCost  || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = (int)newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }

        // No path found, return empty list
        return new List<Vector3>();
    }

    public static List<Vector3> FindPathKnightsDragons(Vector3 startPos, Vector3 targetPos)
    {
        // Get positions as nodes
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        // Initialize the open and closed set
        Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
        HashSet<Node> closedSet = new HashSet<Node>();

        // Start the open set with the first node
        openSet.Add(startNode);

        // While there are nodes on the open set, keep looking for a route
        while (openSet.Count > 0)
        {
            Node currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);

            // Path found
            if (currentNode == targetNode)
            {
                return RetracePath(startNode, currentNode);
            }

            foreach (Node neighbour in grid.GetNeighbours(currentNode))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour) || neighbour.hasUnit)
                    continue;

                float newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour) ;
                
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = (int)newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                   

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }

        // No path found, return empty list
        return new List<Vector3>();
    }

    static List<Vector3> RetracePath(Node startNode, Node endNode)
    {
        List<Vector3> path = new List<Vector3>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode.worldPosition);
            currentNode = currentNode.parent;
        }

        path.Reverse();
        return path;

    }

    public static int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
        {
            return 2000 * dstY + 10 * (dstX - dstY);
        }

        return 2000 * dstX + 10 * (dstY - dstX);
    }
}
