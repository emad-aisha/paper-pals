using UnityEngine;
using System.Collections.Generic;

public class CloudLightTrigger : MonoBehaviour
{
    public GameObject Light;  
    private HashSet<GameObject> trackedObjects = new HashSet<GameObject>();
    private readonly HashSet<string> validTags = new HashSet<string> { "Player", "Enemy" };

    private void Start()
    {
        if(Light == null)
        {
            Light.SetActive(false);
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (validTags.Contains(other.tag) && trackedObjects.Add(other.gameObject))
        {
            if(Light != null)
            {
                Light.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (validTags.Contains(other.tag) && trackedObjects.Remove(other.gameObject))
        {
            if(trackedObjects.Count == 0 && Light != null)
            {
                Light.SetActive(false);
            }
        }
    }
}