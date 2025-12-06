using UnityEngine;

public class ScissorsTrap : MonoBehaviour
{
    [SerializeField] private int damageAmount = 10;

    private void OnTriggerEnter(Collider other)
    {
        IDamage damageable = other.GetComponent<IDamage>();
        if (damageable != null)
        {
            damageable.TakeDamage(damageAmount);
        }
    }
}
