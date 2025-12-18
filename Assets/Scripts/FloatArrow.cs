using UnityEngine;

public class FloatArrow : MonoBehaviour
{
    public float floatSpeed = 1f;
    public float floatHeight = 0.2f;
    private Vector3 startPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = startPosition + new Vector3(0, Mathf.Sin(Time.time * floatSpeed) * floatHeight, 0);
    }
}
