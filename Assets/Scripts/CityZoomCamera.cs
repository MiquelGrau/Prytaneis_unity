using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CityZoomCamera : MonoBehaviour
{
    public float zoomSpeed = 5.0f;
    public float minZoomDistance = 5.0f;
    public float maxZoomDistance = 30.0f;
    public float initialZoom = 10.0f;

    private Camera myCamera;
    public bool HasZoomed { get; private set; } = false;

    private void Start()
    {
        myCamera = GetComponent<Camera>();
        myCamera.orthographicSize = initialZoom;
    }

    void Update()
    {
        if (Input.mousePresent)
        {
            ZoomTowardsMouse();
        }
    }

    void ZoomTowardsMouse()
    {
        float zoom = Input.GetAxis("Mouse ScrollWheel");
        if (zoom == 0) return;

        Vector3 mouseBeforeZoom = myCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, myCamera.nearClipPlane));

        float newSize = myCamera.orthographicSize - zoom * zoomSpeed;
        myCamera.orthographicSize = Mathf.Clamp(newSize, minZoomDistance, maxZoomDistance);

        Vector3 mouseAfterZoom = myCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, myCamera.nearClipPlane));

        Vector3 zoomTowards = mouseAfterZoom - mouseBeforeZoom;
        transform.position -= new Vector3(zoomTowards.x, 0, zoomTowards.y);

        HasZoomed = true;
    }
}
