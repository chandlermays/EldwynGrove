/*-------------------------
File: Pathfinder.cs
Author: Chandler Mays
-------------------------*/
using System.Collections.Generic;
using UnityEngine;
//---------------------------------

namespace EldwynGrove.Core
{
    public static class Pathfinder
    {
        /*----------------------------------------------------------------------------------------------
        | --- FindPath: Uses A* algorithm to find the shortest path between two points on the grid --- |
        ----------------------------------------------------------------------------------------------*/
        public static List<Vector3> FindPath(Vector3 start, Vector3 target)
        {
            Node startNode = GridManager.Instance.GetNodeFromWorld(start);
            Node targetNode = GridManager.Instance.GetNodeFromWorld(target);

            if (startNode == null || targetNode == null || !targetNode.Walkable)
                return null;

            List<Node> openSet = new List<Node>();
            HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();

            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = GetLowestFCost(openSet);

                if (currentNode.GridCoord == targetNode.GridCoord)
                    return RetracePath(startNode, targetNode);

                openSet.Remove(currentNode);
                closedSet.Add(currentNode.GridCoord);

                foreach (Node neighbor in GridManager.Instance.GetNeighbors(currentNode))
                {
                    if (!neighbor.Walkable || closedSet.Contains(neighbor.GridCoord))
                        continue;

                    int tentativeG = currentNode.GCost + GetDistance(currentNode, neighbor);

                    if (tentativeG < neighbor.GCost || !openSet.Contains(neighbor))
                    {
                        neighbor.GCost = tentativeG;
                        neighbor.HCost = GetDistance(neighbor, targetNode);
                        neighbor.Parent = currentNode;

                        if (!openSet.Contains(neighbor))
                            openSet.Add(neighbor);
                    }
                }
            }

            return null;
        }

        /*------------------------------------------------------------------------
        | --- RetracePath: Backtracks from target to start to build the path --- |
        ------------------------------------------------------------------------*/
        private static List<Vector3> RetracePath(Node start, Node target)
        {
            List<Vector3> path = new();
            Node currentNode = target;

            while (currentNode.GridCoord != start.GridCoord)
            {
                path.Add(currentNode.WorldPosition);
                currentNode = currentNode.Parent;
            }

            path.Reverse();
            return path;
        }

        /*---------------------------------------------------------------------------
        | --- GetLowestFCost: Finds the node with the lowest F cost in the list --- |
        ---------------------------------------------------------------------------*/
        private static Node GetLowestFCost(List<Node> nodes)
        {
            Node lowest = nodes[0];

            foreach (Node node in nodes)
            {
                if (node.FCost < lowest.FCost || (node.FCost == lowest.FCost && node.HCost < lowest.HCost))
                    lowest = node;
            }

            return lowest;
        }

        /*----------------------------------------------------------------------
        | --- GetDistance: Calculates Manhattan distance between two nodes --- |
        ----------------------------------------------------------------------*/
        private static int GetDistance(Node a, Node b)
        {
            int dx = Mathf.Abs(a.GridCoord.x - b.GridCoord.x);
            int dy = Mathf.Abs(a.GridCoord.y - b.GridCoord.y);
            return dx + dy;
        }
    }
}