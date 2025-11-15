using UnityEngine;

public class Billboard : MonoBehaviour
{
    Camera mainCamera;

    private void LateUpdate()
    {
        mainCamera = GameManager.instance.player.GetComponent<Camera>();

        if (mainCamera == null) return;

        // Get camera position
        Vector3 cameraPosition = mainCamera.transform.position;

        // Only rotate on Y-axis
        cameraPosition.y = transform.position.y;

        // Make the sprite face the camera
        transform.LookAt(cameraPosition);

        // Rotate 180 on Y if the sprite appears backwards
        transform.Rotate(0f, 180f, 0f);
    }
}