using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtmosphereSound : MonoBehaviour
{
    public Collider Area;
    public GameObject Player;
   
    // Update is called once per frame
    void Update()
    {
        Vector3 closestPoint = Area.ClosestPoint(Player.transform.position);

        transform.position = closestPoint;

    }
}
