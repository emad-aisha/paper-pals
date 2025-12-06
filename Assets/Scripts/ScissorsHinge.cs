using UnityEngine;

public class ScissorHinge : MonoBehaviour
{
    [SerializeField] private float speed = 2.0f;
    [SerializeField] private float maxAngle = 45.0f;
    [SerializeField] private bool invert = false;

    private float timer;
    public bool isCutting { get; private set; }

    private void Update()
    {
        timer += Time.deltaTime * speed;
        float angle = Mathf.PingPong(timer, maxAngle);

        float finalAngle = invert ? angle : -angle;
        transform.localRotation = Quaternion.Euler(0, invert ? angle : -angle, 0);

        isCutting = angle > maxAngle * 0.8f;
    }
}

