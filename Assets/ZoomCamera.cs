using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ZoomCamera : MonoBehaviour
{
    public float zoomSpeed = 5.0f;
    public float minZoomDistance = 5.0f;
    public float maxZoomDistance = 30.0f;

    private Camera myCamera;
    public bool HasZoomed { get; private set; } = false;

    private void Start()
    {
        myCamera = GetComponent<Camera>();
    }

    void Update()
    {
        float zoom = Input.GetAxis("Mouse ScrollWheel");
        if (zoom != 0.0f)
        {
            float newSize = myCamera.orthographicSize - zoom * zoomSpeed;
            myCamera.orthographicSize = Mathf.Clamp(newSize, minZoomDistance, maxZoomDistance);
            HasZoomed = true;
        }
        else
        {
            HasZoomed = false;
        }
    }
}
