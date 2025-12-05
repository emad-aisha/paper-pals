using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] WaypointPath waypointPath;

    private int targetWaypointIndex;

    private Transform prevWaypoint;
    private Transform targetWaypoint;

    private float timeToWaypoint;
    private float elapsedTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TargetNextWaypoint();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        elapsedTime += Time.deltaTime;

        float elapedPercentage = elapsedTime / timeToWaypoint;
        elapedPercentage = Mathf.SmoothStep(0, 1, elapedPercentage);
        transform.position = Vector3.Lerp(prevWaypoint.position, targetWaypoint.position, elapedPercentage);

        if (elapedPercentage >= 1)
        {
            TargetNextWaypoint();
        }
    }

    private void TargetNextWaypoint()
    {
        prevWaypoint = waypointPath.GetWaypoint(targetWaypointIndex);
        targetWaypointIndex = waypointPath.GetWaypointIndex(targetWaypointIndex);
        targetWaypoint = waypointPath.GetWaypoint(targetWaypointIndex);

        elapsedTime = 0;

        float distanceToWaypoint = Vector3.Distance(prevWaypoint.position, targetWaypoint.position);
        timeToWaypoint = distanceToWaypoint / moveSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        other.transform.SetParent(transform);
    }

    private void OnTriggerExit(Collider other)
    {
        other.transform.SetParent(null);
    }

}
