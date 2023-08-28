using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public static class WorldMapUtils
{
    private const double EarthRadius = 6371; // Radi de la Terra en km

    public static double HaversineDistance(WorldMapMarker marker1, WorldMapMarker marker2)
    {
        double dLat = DegreesToRadians(marker2.Latitude - marker1.Latitude);
        double dLon = DegreesToRadians(marker2.Longitude - marker1.Longitude);

        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(DegreesToRadians(marker1.Latitude)) * Math.Cos(DegreesToRadians(marker2.Latitude)) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return EarthRadius * c;
    }

    public static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }

    public static List<WorldMapWaterPath> DijkstraAlgorithm(string startNodeId, string endNodeId, List<WorldMapNode> nodes, List<WorldMapWaterPath> paths)
    {
        List<WorldMapNode> queue = new List<WorldMapNode>(nodes);
        Dictionary<string, double> distances = new Dictionary<string, double>();
        Dictionary<string, WorldMapWaterPath> previousPaths = new Dictionary<string, WorldMapWaterPath>();
        HashSet<string> visitedNodes = new HashSet<string>();

        Debug.Log($"Starting Dijkstra's Algorithm from Node {startNodeId} to Node {endNodeId}.");

        foreach (var node in nodes)
        {
            distances[node.id] = node.id == startNodeId ? 0 : double.MaxValue;
            previousPaths[node.id] = null;
        }

        Debug.Log("Initialization complete.");

        int maxIterations = 100;
        int currentIteration = 0;

        while (queue.Count > 0 && currentIteration < maxIterations)
        {
            queue.Sort((a, b) => distances[a.id].CompareTo(distances[b.id]));

            var currentNode = queue.First();
            queue.RemoveAt(0);
            visitedNodes.Add(currentNode.id);

            Debug.Log($"Processing Node {currentNode.id} with current distance {distances[currentNode.id]}.");

            if (currentNode.id == endNodeId)
            {
                Debug.Log("End Node found. Constructing path.");

                List<WorldMapWaterPath> path = new List<WorldMapWaterPath>();
                var previousPath = previousPaths[endNodeId];
                while (previousPath != null)
                {
                    path.Add(previousPath);
                    previousPath = previousPaths[previousPath.startNode == currentNode.id ? previousPath.endNode : previousPath.startNode];
                }
                path.Reverse();

                // Calcular la distància total del camí
                double totalDistance = 0;
                foreach (var segment in path)
                {
                    var startNode = nodes.Find(node => node.id == segment.startNode);
                    var endNode = nodes.Find(node => node.id == segment.endNode);
                    totalDistance += HaversineDistance(new WorldMapMarker(startNode.latitude, startNode.longitude), new WorldMapMarker(endNode.latitude, endNode.longitude));
                }

                // Construir una cadena amb tots els IDs del camí
                string pathIds = string.Join(" -> ", path.Select(p => p.waterpathId));

                // Registrar la informació amb Debug.Log
                Debug.Log($"Total path distance: {totalDistance} km");
                Debug.Log($"Final path IDs: {pathIds}");
                return path;
            }

            if (distances[currentNode.id] == double.MaxValue)
            {
                Debug.Log($"Node {currentNode.id} has max distance. Skipping.");
                continue;
            }

            foreach (var pathObj in paths.Where(path => (path.startNode == currentNode.id) && !visitedNodes.Contains(path.startNode == currentNode.id ? path.endNode : path.startNode)))
            {
                var connectedNodeId = pathObj.startNode == currentNode.id ? pathObj.endNode : pathObj.startNode;

                if (distances.ContainsKey(connectedNodeId))
                {
                    var connectedNode = nodes.Find(node => node.id == connectedNodeId);
                    float distance = (float)(distances[currentNode.id] + HaversineDistance(new WorldMapMarker(currentNode.latitude, currentNode.longitude), new WorldMapMarker(connectedNode.latitude, connectedNode.longitude)) / pathObj.pathSpeed);

                    Debug.Log($"Checking path from Node {currentNode.id} to Node {connectedNodeId}. Distance: {distance}.");

                    if (distance < distances[connectedNodeId])
                    {
                        Debug.Log($"Updating distance for Node {connectedNodeId} to {distance}.");
                        distances[connectedNodeId] = distance;
                        previousPaths[connectedNodeId] = pathObj;
                    }
                }
            }

            currentIteration++;
            Debug.Log($"Iteration {currentIteration} complete.");
        }

        Debug.Log("No path found or max iterations reached. Returning empty path.");
        return new List<WorldMapWaterPath>();
    }
}