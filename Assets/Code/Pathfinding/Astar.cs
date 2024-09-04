using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class AstarTileBased
{
    public class NodeRecord
    {
        public Vector3Int Node;
        public Vector3Int? CameFrom;

        public float CostSoFar;
        public float EstimatedTotalCost;
    }

    public static List<Vector3Int> FindPath(IGraph graph, Vector3Int start, Vector3Int end, Func<Vector3Int, Vector3Int, float> heuristic)
    {
        var startRecord = new NodeRecord()
        {
            Node = start,
            CameFrom = null,
            CostSoFar = 0,
            EstimatedTotalCost = heuristic(start, end)
        };

        var openList = new List<NodeRecord>();
        openList.Add(startRecord);
        var closedList = new List<NodeRecord>();

        NodeRecord currentNode = null;

        while (openList.Count > 0)
        {
            currentNode = openList.OrderBy(n => n.EstimatedTotalCost).First();

            if (currentNode.Node == end)
                break;

            var neighbours = graph.GetNeighbours(currentNode.Node);

            foreach (var neighbour in neighbours)
            {
                var toNodeCost = currentNode.CostSoFar + heuristic(currentNode.Node, neighbour);

                var toNodeClosedRecord = closedList.FirstOrDefault(n => n.Node == neighbour);
                var toNodeOpenRecord = openList.FirstOrDefault(n => n.Node == neighbour);

                if (toNodeClosedRecord != null)
                {
                    if (toNodeClosedRecord.CostSoFar <= toNodeCost)
                        continue;

                    closedList.Remove(toNodeClosedRecord);
                }
                else if (toNodeOpenRecord != null)
                {
                    if (toNodeOpenRecord.CostSoFar <= toNodeCost)
                        continue;
                }


                var toNodeRecord = new NodeRecord()
                {
                    Node = neighbour,
                    CameFrom = currentNode.Node,
                    CostSoFar = toNodeCost,
                    EstimatedTotalCost = heuristic(neighbour, end)
                };

                if (toNodeOpenRecord == null)
                {
                    openList.Add(toNodeRecord);
                }
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);
        }

        if (currentNode != null && currentNode.Node == end)
        {
            var path = new List<Vector3Int>();

            while (currentNode.Node != start)
            {
                path.Add(currentNode.Node);
                currentNode = closedList.First(n => n.Node == currentNode.CameFrom.Value);
            }

            path.Reverse();

            return path;
        }

        return null;
    }
}