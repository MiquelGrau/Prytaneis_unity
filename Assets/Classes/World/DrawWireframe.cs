using UnityEngine;

[RequireComponent(typeof(Camera))]
public class DrawWireframe : MonoBehaviour
{
    public Material lineMaterial;

    void OnPostRender()
    {
        if (!lineMaterial)
        {
            Debug.LogError("Please Assign a material on the inspector");
            return;
        }

        GL.PushMatrix();
        lineMaterial.SetPass(0);
        GL.Begin(GL.LINES);
        GL.Color(Color.black);

        foreach (var obj in FindObjectsOfType<MeshFilter>())
        {
            var mesh = obj.mesh;
            var vertices = mesh.vertices;
            var triangles = mesh.triangles;
            for (int i = 0; i < triangles.Length; i += 3)
            {
                var v0 = obj.transform.TransformPoint(vertices[triangles[i]]);
                var v1 = obj.transform.TransformPoint(vertices[triangles[i + 1]]);
                var v2 = obj.transform.TransformPoint(vertices[triangles[i + 2]]);
                
                GL.Vertex(v0);
                GL.Vertex(v1);

                GL.Vertex(v1);
                GL.Vertex(v2);

                GL.Vertex(v2);
                GL.Vertex(v0);
            }
        }

        GL.End();
        GL.PopMatrix();
    }
}
