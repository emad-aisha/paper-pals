using UnityEngine;

public class CameraController : MonoBehaviour
{

    [SerializeField] int Sens;
    [SerializeField] int LockVertMin, LockVertMax;
    [SerializeField] bool invertY;

    float CamX;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        // get input
        float mouseX = Input.GetAxis("Mouse X") * Sens * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * Sens * Time.deltaTime;
        // use the invertY 

        if (invertY)
        {
            CamX += mouseY;
        }
        else
        {
            CamX -= mouseY;
        }

        // clamp camera on x axis 

        CamX = Mathf.Clamp(CamX, LockVertMin, LockVertMax);
        // Rotate the camera on the x-axis 
        transform.localRotation = Quaternion.Euler(CamX, 0, 0);
        // rotate the player on the y axis 
        transform.parent.Rotate(Vector3.up * mouseX);
    }
}
