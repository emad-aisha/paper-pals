using UnityEngine;

public class MapRotationLock : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        Vector3 newPos = GameManager.instance.controller.transform.position;
        newPos.y = transform.position.y;
        transform.position = newPos;
    }
}
