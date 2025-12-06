using UnityEngine;

public class ScissorBlade : MonoBehaviour
{
    [SerializeField] private float speed = 2.0f;
    [SerializeField] private float maxAngle = 45.0f;
    [SerializeField] private bool invert = false; 

    private float timer;

    private void Update()
    {
        timer += Time.deltaTime * speed;
        float angle = Mathf.PingPong(timer, maxAngle);

        
        float finalAngle = invert ? -angle : angle;
        transform.localRotation = Quaternion.Euler(0, 0, finalAngle);
    }
}