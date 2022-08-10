using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Speeds")]
    public float movementSpeed = 5;
    public float rotationSpeed = 10;
    public Vector2 minMaxRotation = new Vector2(-90f, 90f);

    Vector2 rotation;
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        rotation.y += rotationSpeed * Input.GetAxis("Mouse X");
        rotation.x -= rotationSpeed * Input.GetAxis("Mouse Y");

        // Rotiation about the x axis is clamped
        rotation.x = Mathf.Clamp(rotation.x, minMaxRotation.x, minMaxRotation.y);

        // Rotate camera
        transform.eulerAngles = new Vector3(rotation.x, rotation.y, 0);
        
        Vector3 direction = (transform.TransformDirection(Vector3.right) * input.x) + (transform.TransformDirection(Vector3.forward) * input.y);
        Vector3 velocity = direction * movementSpeed;

        transform.position += velocity * Time.deltaTime;
    }
}
