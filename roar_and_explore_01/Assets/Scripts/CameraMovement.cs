using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Panning Settings")]
    public bool allowPanning = true; // zsenu: opening shop/pause menu should disable panning 
    
    public float panSpeed = 20f; // Speed of camera movement
    public float panBorderThickness = 10f; // Distance from screen edge to trigger panning

    [Header("Camera Movement Limits")]
    public Vector2 panLimitX = new Vector2(-30, 30);
    public Vector2 panLimitY = new Vector2(0, 30);

    private Vector3 initialPosition; // Store initial position to prevent resets

    void Start()
    {
        // Store the initial camera position
        initialPosition = transform.position;
        initialPosition.z = -10; // Ensure correct depth for 2D games

        // Set camera to initial position
        transform.position = initialPosition;
    }

    void Update()
    {
        if (!allowPanning) return;

        Vector3 pos = transform.position;

        // Get mouse position
        Vector3 mousePos = Input.mousePosition;

        // Edge panning logic
        if (mousePos.x >= Screen.width - panBorderThickness) pos.x += panSpeed * Time.deltaTime;
        if (mousePos.x <= panBorderThickness) pos.x -= panSpeed * Time.deltaTime;
        if (mousePos.y >= Screen.height - panBorderThickness) pos.y += panSpeed * Time.deltaTime;
        if (mousePos.y <= panBorderThickness) pos.y -= panSpeed * Time.deltaTime;

        // Clamp position within limits
        pos.x = Mathf.Clamp(pos.x, panLimitX.x, panLimitX.y);
        pos.y = Mathf.Clamp(pos.y, panLimitY.x, panLimitY.y);

        // Apply position update
        transform.position = new Vector3(pos.x, pos.y, initialPosition.z);
    }
}