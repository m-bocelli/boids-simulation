using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDrag : MonoBehaviour
{
    public int mouseSensitivity = 10;

    Vector2 rotation;

    void Start()
    {
        Camera.main.transform.parent = transform;
    }

    void Update()
    {
        Vector2 mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * mouseSensitivity;

        rotation.y += mouseInput.x;
        rotation.x += mouseInput.y;
        
        rotation.x = Mathf.Clamp(rotation.x, 0f, 25f);
        //transform.Rotate(Vector3.up * Time.deltaTime * mouseInput.x, Space.World);
        //transform.Rotate(Vector3.right * Time.deltaTime * mouseInput.y, Space.World);
        transform.eulerAngles = new Vector3(rotation.x, rotation.y, 0);
    }
}
