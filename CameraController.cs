using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform player; // The player transform to follow
    public float distance = 5.0f; // Distance from the player
    public float height = 2.0f; // Height offset from the player
    public float mouseSensitivity = 2.0f; // Sensitivity for mouse movement
    public float verticalClamp = 80.0f; // Limit for up/down rotation

    private float yaw = 0.0f; // Yaw rotation
    private float pitch = 0.0f; // Pitch rotation

    void Start()
    {
        // Hide the mouse cursor and lock it to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Get mouse input
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Clamp pitch to prevent flipping
        pitch = Mathf.Clamp(pitch, -verticalClamp, verticalClamp);
    }

    void LateUpdate()
    {
        // Calculate the rotation and position of the camera
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 position = player.position + Vector3.up * height - (rotation * Vector3.forward * distance);

        // Set the camera's position and rotation
        transform.position = position;
        transform.LookAt(player.position + Vector3.up * height);
    }
}
