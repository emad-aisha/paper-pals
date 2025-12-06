using UnityEngine;

public class ScissorsTrap : MonoBehaviour
{
    [SerializeField] private ScissorHinge hinge;
    [SerializeField] private int damageAmount = 10;
    [SerializeField] private float damageCoooldown = 0.5f;

    private float DamageTime = 0f;

    private void OnTriggerEnter(Collider other)
    {
        if (!hinge.isCutting) return;
        if(Time.time < DamageTime + damageCoooldown) return;

        IDamage damageable = other.GetComponent<IDamage>();
           if (damageable != null)
           {
               damageable.TakeDamage(damageAmount);
                DamageTime = Time.time + damageCoooldown;
        }
        
    }
}
