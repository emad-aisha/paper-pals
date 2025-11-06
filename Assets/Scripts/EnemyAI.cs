using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage
{
    public enum EnemyType { ranged, melee, bull };


    [SerializeField] NavMeshAgent AgentAI;
    [SerializeField] Renderer Model;

    [SerializeField] int HP;

    [SerializeField] int contactDamage;
    [SerializeField] float attackRange;
    [SerializeField] float attackCooldown;

    [SerializeField] EnemyType enemyType = EnemyType.melee;

    //Bull Fields
    [SerializeField] int chargeMaxSpeed = 30;
    [SerializeField] int accelerationTime = 2;
    [SerializeField] int chargeDuration = 3;
    [SerializeField] int chargeCooldown = 5;

    //Ranged fields
    [SerializeField] Transform ShootPos;
    [SerializeField] GameObject Bullet;
    [SerializeField] float ShootRate;

    //power up fields
    [SerializeField] GameObject[] Powerbonusprefab;
    [SerializeField] int dropPowerbonus;

    //Field of View (FOV)
    [SerializeField] Transform HeadPosition;
    [SerializeField] int FOV;
    [SerializeField] int FaceTargetSpeed;
    float AngleToPlayer;
    Vector3 playerDirection;
    float StoppingDistanceOG;

    Color OGColor;

    bool PlayerInTrigger;

    float ShootTimer;

    //bull variables
    float attackTimer = 0f;

    float chargeTimer = 0f;

    bool isCharging = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OGColor = Model.material.color;

        //copying the stopping distance
        StoppingDistanceOG = AgentAI.stoppingDistance;
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

        FaceTarget();
        //if player is in the trigger collider
        if (PlayerInTrigger && CanSeePlayer())
        {

        }
        // Bull charge logic
        if (enemyType == EnemyType.bull && PlayerInTrigger && !isCharging)
        {
            chargeTimer += Time.deltaTime;

            if (chargeTimer >= chargeCooldown)
            {
                StartCoroutine(BullCharge());
            }
        }

    }

    bool CanSeePlayer()
    {
        //calculate direction vector from the enemy to the player
        playerDirection = gameManager.instance.player.transform.position - HeadPosition.position;

        //Calculate the angle between the enemy's forward direction and the direction to the player
        AngleToPlayer = Vector3.Angle(playerDirection, transform.forward);
        Debug.DrawRay(HeadPosition.position, playerDirection, Color.green);

        RaycastHit hit;

        //cast a ray from the enemy to the player to check for obstacles
        if (Physics.Raycast(HeadPosition.position, playerDirection, out hit))
        {
            Debug.Log(hit.collider.name);

            if (AngleToPlayer <= FOV && hit.collider.CompareTag("Player"))
            {

                //will look for player position and move towards it
                AgentAI.SetDestination(gameManager.instance.player.transform.position);

                if (ShootTimer >= ShootRate && enemyType == EnemyType.ranged)
                {
                    Shoot();
                }

                // Update attack timer
                attackTimer += Time.deltaTime;

                // Check distance between enemy and player
                float distance = Vector3.Distance(transform.position, gameManager.instance.player.transform.position);

                // If close enough to attack, and cooldown is ready and EnemyType.melee
                if (distance <= attackRange && attackTimer >= attackCooldown && enemyType == EnemyType.melee || enemyType == EnemyType.bull)
                {
                    AttackPlayer();
                }

                return true;
            }

        }

        return false;
    }

    void FaceTarget()
    {
        //calculate the rotation needed to look at the player
        Quaternion Rotate =
            Quaternion.LookRotation(new Vector3(playerDirection.x, transform.position.y, playerDirection.z));

        //smoothly rotate towards the player
        transform.rotation = Quaternion.Lerp(transform.rotation, Rotate, FaceTargetSpeed * Time.deltaTime);
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;


        if (HP <= 0)
        {
            //will destroy self 
            Destroy(gameObject);
            Debug.Log("Enemy Destroyed");
        }
        else
        {
            //else if it survives will flash white
            StartCoroutine(FlashWhite());
            Debug.Log("Enemy took damage");
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
        Instantiate(Bullet, ShootPos.position, transform.rotation);

        //to see if shoot works
        Debug.Log("Shoot");
    }

    IEnumerator BullCharge()
    {
        isCharging = true;
        chargeTimer = 0f;

        // Direction toward player at start
        Vector3 dir = (gameManager.instance.player.transform.position - transform.position).normalized;
        float timer = 0f;

        // Temporarily stop pathfinding so we can move manually
        AgentAI.isStopped = true;


        while (timer < accelerationTime)
        {
            AgentAI.velocity = dir * Mathf.Lerp(AgentAI.speed, chargeMaxSpeed, timer / accelerationTime);
            timer += Time.deltaTime;
            yield return null;
        }

        // Maintain max speed for charge duration
        float chargeTime = 0f;
        while (chargeTime < chargeDuration)
        {
            AgentAI.velocity = dir * chargeMaxSpeed;
            chargeTime += Time.deltaTime;
            yield return null;
        }

        // Stop and resume normal AI
        AgentAI.velocity = Vector3.zero;
        AgentAI.isStopped = false;
        isCharging = false;
    }
}
