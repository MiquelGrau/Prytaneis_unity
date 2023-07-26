using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class WireframeOverlay : MonoBehaviour
{
    public Material WireframeMaterial;

    private void OnDrawGizmos()
    {
        if (WireframeMaterial == null) return;

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null) return;

        Mesh mesh = meshFilter.sharedMesh;
        if (mesh == null) return;

        Matrix4x4 transformMatrix = transform.localToWorldMatrix;
        WireframeMaterial.SetPass(0);
        Graphics.DrawMeshNow(mesh, transformMatrix);
    }
}
