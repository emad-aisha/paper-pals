using UnityEngine;

public class MapRotationLock : MonoBehaviour
{
    

    [SerializeField] private Transform player;
 

    // Update is called once per frame
    void Update()
    {
        Vector3 newPos = player.position;
        newPos.y = transform.position.y;
        transform.position = newPos;
    }
}
