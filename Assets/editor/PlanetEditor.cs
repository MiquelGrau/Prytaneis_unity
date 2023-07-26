using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Planet))]
public class PlanetEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();  // Dibuixa l'inspector per defecte

        Planet planet = (Planet)target;  // Obtenim la referència a l'script del planeta
        if (GUILayout.Button("Update Planet"))  // Si es prem el botó...
        {
            planet.ManuallyUpdatePlanet();  // Cridem a la funció d'actualització
        }
    }
}
