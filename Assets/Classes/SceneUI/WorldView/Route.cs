using UnityEngine;

public class Route : MonoBehaviour
{
    public GameObject planet; // Assigna això des de l'Inspector d'Unity amb el teu objecte Earth

    void Start()
    {
    }

    public void ConnectMarkersWithLine(Marker markerA, Marker markerB)
    {
        GameObject lineObject = new GameObject("MarkersLine");

        // Estableix el marcador A com a pare del GameObject de la línia
        lineObject.transform.SetParent(markerA.transform, false);
        
        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Unlit/Texture"));
        lineRenderer.material.color = Color.red;
        lineRenderer.widthMultiplier = 0.003f;
        lineRenderer.positionCount = 2;

        // Aquesta propietat fa que el LineRenderer utilitzi les coordenades locals del seu pare
        lineRenderer.useWorldSpace = false;

        // Estableix les posicions de la línia respecte al pare (markerA)
        Vector3 startPosition = Vector3.zero; // Inici és la posició local del marcador A
        Vector3 endPosition = markerA.transform.InverseTransformPoint(markerB.transform.position); // Converteix la posició global del marcador B a l'espai local del marcador A
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);
    }

    Vector3 LatLongToPosition(float lat, float lon)
    {
        // Aquí, reutilitza el mètode de conversió de LatLongToPosition com en MarkersManager
        float baseRadius = planet.transform.localScale.x / 2; // Assumeix que l'escala de l'objecte és un diàmetre
        float heightOffset = 0; // Ajusta aquest valor si cal
        float radius = baseRadius + heightOffset;

        lat = Mathf.Deg2Rad * lat;
        lon = Mathf.Deg2Rad * lon;

        Vector3 direction = new Vector3(
            Mathf.Cos(lat) * Mathf.Cos(lon),
            Mathf.Sin(lat),
            Mathf.Cos(lat) * Mathf.Sin(lon)
        ).normalized;

        Vector3 position = planet.transform.position + direction * radius;

        return position;
    }

}
