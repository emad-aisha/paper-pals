using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class playerBullet : MonoBehaviour
{
    public enum BulletType { moving, homing, AOE };

    [SerializeField] BulletType type;
    [SerializeField] Rigidbody rb;
    [SerializeField] int speed;
    [SerializeField] float destroyTime;

    private Vector3 direction;

    //Transform target;          //optional for homing bullets
    void Start()
    {
        Destroy(gameObject, destroyTime);

        if (type == BulletType.moving)
        {
            rb.linearVelocity = transform.forward * speed;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
    }
}
