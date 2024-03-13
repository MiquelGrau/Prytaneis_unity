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

    public static List<WorldMapLandPath> DijkstraAlgorithm(
        string startNodeId, string endNodeId, List<WorldMapNode> nodes, List<WorldMapLandPath> paths)
    {
        var distances = new Dictionary<string, double>();
        var previous = new Dictionary<string, WorldMapLandPath>();
        var notVisited = new List<WorldMapNode>(nodes);
        WorldMapNode endNode = nodes.Find(n => n.id == endNodeId); // Troba el node final

        // Inicialitza distàncies
        foreach (var node in nodes)
        {
            distances[node.id] = double.MaxValue;
        }
        distances[startNodeId] = 0;

        while (notVisited.Count != 0)
        {
            notVisited.Sort((x, y) => distances[x.id].CompareTo(distances[y.id]));
            var current = notVisited[0];
            notVisited.RemoveAt(0);

            if (current.id == endNodeId)
            {
                var optimalPath = ReconstructPath(previous, current.id, nodes, paths);
                
                // Aquí afegim el codi per debugar
                DebugLogOptimalPath(optimalPath, nodes);
                
                return optimalPath;
            }   

            foreach (var path in paths.Where(p => p.startNode == current.id))
            {
                WorldMapNode nextNode = nodes.Find(n => n.id == path.endNode);
                if (nextNode == null) continue;

                double tentativeDistance = distances[current.id] + CalculatePathWeight(path, nodes, endNode); // Passa el node final

                if (tentativeDistance < distances[nextNode.id])
                {
                    distances[nextNode.id] = tentativeDistance;
                    previous[nextNode.id] = path;

                    if (!notVisited.Contains(nextNode))
                    {
                        notVisited.Add(nextNode);
                    }
                }
            }
        }
        Debug.Log("Dijkstra Algorithm: No path found.");
        return new List<WorldMapLandPath>(); // Retorna llista buida si no es troba cap camí
    }


    private static List<WorldMapLandPath> ReconstructPath(Dictionary<string, WorldMapLandPath> previous, string currentId, List<WorldMapNode> nodes, List<WorldMapLandPath> paths)
    {
        var totalPath = new List<WorldMapLandPath>();

        while (previous.ContainsKey(currentId))
        {
            WorldMapLandPath path = previous[currentId];
            totalPath.Insert(0, path); // Afegeix al principi per reconstruir el camí en ordre
            currentId = nodes.Find(n => n.id == path.startNode).id;
        }

        return totalPath;
    }

    private static double CalculatePathWeight(WorldMapLandPath path, List<WorldMapNode> nodes, WorldMapNode endNode)
    {
        WorldMapNode startNode = nodes.Find(n => n.id == path.startNode);
        WorldMapNode nextNode = nodes.Find(n => n.id == path.endNode);

        // Converteix els nodes a markers per a calcular la distància física utilitzant Haversine
        WorldMapMarker markerStart = new WorldMapMarker(startNode.latitude, startNode.longitude);
        WorldMapMarker markerNext = new WorldMapMarker(nextNode.latitude, nextNode.longitude);

        // Càlcul de la distància física utilitzant Haversine
        //double distance = HaversineDistance(startNode, nextNode);
        double distance = HaversineDistance(markerStart, markerNext);

        // Potencial basat en la distància en línia recta fins al node final
        //double potentialModifier = Potential(startNode, endNode);
        WorldMapMarker markerEnd = new WorldMapMarker(endNode.latitude, endNode.longitude);
        double potentialModifier = Potential(markerNext, markerEnd);

        // Ajusta el pes basant-te en la distància, la dificultat del camí i el potencial
        double weight = distance * (1 + path.pathDifficulty) + potentialModifier;

        return weight;
    }

    /* private static double Potential(WorldMapNode node, WorldMapNode endNode)
    {
        // Conversió de latitud i longitud a radians per a càlculs
        double nodeLatRad = DegreesToRadians(node.latitude);
        double nodeLonRad = DegreesToRadians(node.longitude);
        double endNodeLatRad = DegreesToRadians(endNode.latitude);
        double endNodeLonRad = DegreesToRadians(endNode.longitude);

        // Càlcul de la "distància" en línia recta utilitzant la fórmula donada
        return Math.Sqrt(Math.Pow(nodeLatRad - endNodeLatRad, 2) + Math.Pow(nodeLonRad - endNodeLonRad, 2));
    } */

    private static double Potential(WorldMapMarker node, WorldMapMarker endNode)
    {
        // Càlcul de la "distància" en línia recta utilitzant la fórmula donada
        return Math.Sqrt(Math.Pow(DegreesToRadians(node.Latitude) - DegreesToRadians(endNode.Latitude), 2) 
        + Math.Pow(DegreesToRadians(node.Longitude) - DegreesToRadians(endNode.Longitude), 2));
    }

    private static void DebugLogOptimalPath(List<WorldMapLandPath> optimalPath, List<WorldMapNode> nodes)
    {
        string pathNames = string.Join(" -> ", optimalPath.Select(path =>
        {
            var startNode = nodes.Find(node => node.id == path.startNode);
            return startNode.name;
        }));

        // Afegeix el nom del node final manualment ja que l'últim path només mostra el node d'inici
        if (optimalPath.Any())
        {
            var lastPath = optimalPath.Last();
            var endNode = nodes.Find(node => node.id == lastPath.endNode);
            pathNames += " -> " + endNode.name;
        }

        Debug.Log("Dijkstra Algorithm: Optimal path is " + pathNames);
    }

}