using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Vector3 offset = new(0f, 3f, -6f);
    [SerializeField] private float mouseSensitivity = 0.2f;
    [SerializeField] private float maxPitch = 60f;
    [SerializeField] private float minPitch = -20f;

    private Vector2 lookInput = Vector2.zero;
    private float pitch;
    private float yaw;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        yaw += lookInput.x * mouseSensitivity;
        pitch -= lookInput.y * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        var rotation = Quaternion.Euler(pitch, yaw, 0f);
        var rotatedOffset = rotation * offset;

        cameraTransform.position = transform.position + rotatedOffset;
        cameraTransform.LookAt(transform.position);
    }

    private void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }
}