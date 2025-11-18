using UnityEngine;

public class CloudLightTrigger : MonoBehaviour
{
    public GameObject Light;  
    private int objectsInside = 0;

    private void Start()
    {
        Light.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            objectsInside++;
            Light.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            objectsInside--;

            if (objectsInside <= 0)
            {
                Light.SetActive(false);
                objectsInside = 0;
            }
        }
    }
}