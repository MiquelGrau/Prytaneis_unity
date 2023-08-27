using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class WorldMapUtils
{
    private const float EarthRadius = 6371; // Radi de la Terra en km

    public static float HaversineDistance(IWorldMapMarker marker1, IWorldMapMarker marker2)
    {
        float dLat = DegreesToRadians(marker2.Latitude - marker1.Latitude);
        float dLon = DegreesToRadians(marker2.Longitude - marker1.Longitude);

        float a = Mathf.Sin(dLat / 2) * Mathf.Sin(dLat / 2) +
                  Mathf.Cos(DegreesToRadians(marker1.Latitude)) * Mathf.Cos(DegreesToRadians(marker2.Latitude)) *
                  Mathf.Sin(dLon / 2) * Mathf.Sin(dLon / 2);

        float c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));

        return EarthRadius * c;
    }

    public static float DegreesToRadians(float degrees)
    {
        return degrees * Mathf.PI / 180;
    }

    public static List<IWorldMapPath> DijkstraAlgorithm(string startNodeId, string endNodeId, List<IWorldMapNode> nodes, List<IWorldMapPath> paths)
    {
        List<IWorldMapNode> queue = new List<IWorldMapNode>();
        Dictionary<string, float> distances = new Dictionary<string, float>();
        Dictionary<string, IWorldMapPath> previousPaths = new Dictionary<string, IWorldMapPath>();

        Debug.Log($"startNodeId {startNodeId}, endNodeId {endNodeId}");

        foreach (var node in nodes)
        {
            distances[node.Id] = node.Id == startNodeId ? 0 : float.MaxValue;
            queue.Add(node);
            previousPaths[node.Id] = null;
        }

        while (queue.Count > 0)
        {
            queue.Sort((a, b) => distances[a.Id].CompareTo(distances[b.Id]));

            var currentNode = queue.First();
            queue.Remove(currentNode);

            if (currentNode.Id == endNodeId)
            {
                List<IWorldMapPath> path = new List<IWorldMapPath>();
                var previousPath = previousPaths[endNodeId];
                while (previousPath != null)
                {
                    path.Add(previousPath);
                    previousPath = previousPaths[previousPath.StartNodeId == currentNode.Id ? previousPath.EndNodeId : previousPath.StartNodeId];
                }
                path.Reverse();
                return path;
            }

            if (distances[currentNode.Id] == float.MaxValue) continue;

            foreach (var pathObj in paths.Where(path => path.StartNodeId == currentNode.Id || path.EndNodeId == currentNode.Id))
            {
                var connectedNodeId = pathObj.StartNodeId == currentNode.Id ? pathObj.EndNodeId : pathObj.StartNodeId;
                var connectedNode = nodes.Find(node => node.Id == connectedNodeId);
                if (connectedNode != null)
                {
                    float distance = distances[currentNode.Id] + HaversineDistance(currentNode.Marker, connectedNode.Marker) / pathObj.Speed;
                    if (distance < distances[connectedNodeId])
                    {
                        distances[connectedNodeId] = distance;
                        previousPaths[connectedNodeId] = pathObj;
                        queue.Add(connectedNode);
                    }
                }
            }
        }

        return new List<IWorldMapPath>(); // Return empty list if no path found
    }
}
