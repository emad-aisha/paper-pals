using UnityEngine;
using System.Collections;

public class Damage : MonoBehaviour
{
    enum DamageType { moving, stationary, DOT, homing, AOE};

    [Header("Neccesities")]
    [SerializeField] DamageType type;
    [SerializeField] Rigidbody rb;

    [Header("Damage Stats")]
    [SerializeField] int damageAmount;
    [SerializeField] float damageRate;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;

    bool isDamaging;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(type == DamageType.moving || type == DamageType.homing)
        {
           Destroy(gameObject, destroyTime);

            if(type == DamageType.moving)
            {
                rb.linearVelocity = transform.forward * speed;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(type == DamageType.homing)
        {
            if(GameManager.instance.player != null)
            {
                rb.linearVelocity = (GameManager.instance.player.transform.position - transform.position).normalized * speed * Time.deltaTime;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IDamage playerDamage = other.GetComponent<IDamage>();
            if (playerDamage != null)
            {
                playerDamage.TakeDamage(damageAmount); 
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.isTrigger)
            return;

        IDamage dmg = other.GetComponent<IDamage>();

        if (dmg != null && type == DamageType.DOT && isDamaging != true)
        {
            StartCoroutine(DamageOther(dmg));
        }
    }

    IEnumerator DamageOther(IDamage d)
    {
        isDamaging = true;
        d.TakeDamage(damageAmount);
        yield return new WaitForSeconds(damageRate);
        isDamaging = false;
    }
}
