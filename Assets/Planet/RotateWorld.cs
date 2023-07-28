using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public float rotationSpeed = 60f;
    public Camera observerCamera;
    public bool HasRotated { get; private set; } = false;

    void Awake()
    {
        if (!observerCamera)
        {
            observerCamera = Camera.main;
        }
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            float horizontal = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            float vertical = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

            Vector3 rotationAroundUp = -horizontal * observerCamera.transform.up;
            Vector3 rotationAroundRight = vertical * observerCamera.transform.right;

            transform.Rotate(rotationAroundUp, Space.World);
            transform.Rotate(rotationAroundRight, Space.World);
            HasRotated = true;
        }
        else
        {
            HasRotated = false;
        }
    }
}
