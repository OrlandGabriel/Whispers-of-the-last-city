using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _distanceFromTarget = 5.0f;

    private float _sensitivity = 1000f;
    private float _yaw = 0f;
    private float _pitch = 0f;

    void Update()
    {
        HandleInput();
        Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0);
        RotateCamera(rotation);
    }

    private void HandleInput()
    {
        Vector2 inputDelta = Vector2.zero;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            inputDelta = touch.deltaPosition;
        }
        else if (Input.GetMouseButton(0))
        {
            inputDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        }

        _yaw += inputDelta.x * _sensitivity * Time.deltaTime;
        _pitch -= inputDelta.y * _sensitivity * Time.deltaTime;
    }

    private void RotateCamera(Quaternion rotation)
    {
        Vector3 desiredPosition = _target.position + rotation * new Vector3(0, 0, -_distanceFromTarget);
        transform.position = desiredPosition;
        transform.rotation = rotation;
    }
}
