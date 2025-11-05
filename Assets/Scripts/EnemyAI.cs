using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] NavMeshAgent AgentAI;
    [SerializeField] Renderer Model;

    [SerializeField] int HP;

    [SerializeField] int contactDamage;
    [SerializeField] float attackRange;
    [SerializeField] float attackCooldown;

    //[SerializeField] Transform ShootPos;
    //[SerializeField] GameObject Bullet;
    //[SerializeField] float ShootRate;


    Color OGColor;

    bool PlayerInTrigger;

    float ShootTimer;

    float attackTimer = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OGColor = Model.material.color;
    }

    void AttackPlayer()
    {
        attackTimer = 0f; // reset cooldown timer

        // Try to get the player's damage interface
        IDamage dmg = gameManager.instance.player.GetComponent<IDamage>();

        if (dmg != null)
        {
            dmg.TakeDamage(contactDamage);
            Debug.Log("Enemy attacked the player!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        ShootTimer += Time.deltaTime;

        //if player is in the trigger collider
        if (PlayerInTrigger)
        {
            //will look for player position and move towards it
            AgentAI.SetDestination(gameManager.instance.player.transform.position);

            AgentAI.speed = 15;

            //  if (ShootTimer >= ShootRate)
            {
                Shoot();
            }

            // Update attack timer
            attackTimer += Time.deltaTime;

            // Check distance between enemy and player
            float distance = Vector3.Distance(transform.position, gameManager.instance.player.transform.position);

            // If close enough to attack, and cooldown is ready
            if (distance <= attackRange && attackTimer >= attackCooldown)
            {
                AttackPlayer();
            }
        }



    }

    public void TakeDamage(int amount)
    {
        HP -= amount;

        if (HP <= 0)
        {
            //will destroy self 
            Destroy(gameObject);
        }
        else
        {
            //else if it survives will flash white
            StartCoroutine(FlashWhite());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInTrigger = false;
        }
    }

    IEnumerator FlashWhite()
    {
        Model.material.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        Model.material.color = OGColor;
    }

    void Shoot()
    {
        ShootTimer = 0f;

        //Will created an object at the shoot pos
        //  Instantiate(Bullet, ShootPos.position, transform.rotation);

        Debug.Log("Shoot");
    }
}
