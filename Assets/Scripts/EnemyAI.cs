using System.Collections;
using UnityEngine;
using UnityEngine.AI;


public class EnemyAI : MonoBehaviour, IDamage
{
    public enum EnemyType { ranged, melee, bull };
    [Header("Enemy Type")]
	[SerializeField] EnemyType enemyType;

	[Header("Neccesities")]
	[SerializeField] LayerMask IgnoreLayer;
    [SerializeField] NavMeshAgent AgentAI;
    [SerializeField] SpriteRenderer Sprite;
    

	[Header("Health")]
	[SerializeField] int HP;

	[Header("Melee Type")]
	[SerializeField] int contactDamage;
    [SerializeField] float attackRange;
    [SerializeField] float attackCooldown;

	[Header("Charge")]
	[SerializeField] int chargeMaxSpeed;
    [SerializeField] int accelerationTime;
    [SerializeField] int chargeDuration;
    [SerializeField] int chargeCooldown;

	[Header("Shooter")]
	[SerializeField] Transform ShootPos;
    [SerializeField] GameObject Bullet;
    [SerializeField] float ShootRate;

	[Header("Power Ups")]
	[SerializeField] GameObject[] Powerbonusprefab;
    [SerializeField] int dropPowerbonus;

	[Header("FOV")]
	[SerializeField] Transform HeadPosition;
    [SerializeField] int FOV;
    [SerializeField] int FaceTargetSpeed;
    float AngleToPlayer;
    Vector3 playerDirection;

    [Header("Roam")]
    [SerializeField] int RoamDistance;
    [SerializeField] int RoamPauseTime;
    float RoamTimer;
    float StoppingDistanceOG;
    Vector3 StartPosition;

    // private variables   
    bool PlayerInTrigger;
    float ShootTimer;
    Color OGColor;

    // bull variables
    float attackTimer = 0;
    float chargeTimer = 0;

    float normalSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OGColor = Sprite.color;
        normalSpeed = AgentAI.speed;

        StoppingDistanceOG = AgentAI.stoppingDistance;
        StartPosition = transform.position;

        //calculate direction vector from the enemy to the player
        playerDirection = GameManager.instance.player.transform.position - HeadPosition.position;

    }

    void AttackPlayer()
    {
        attackTimer = 0f; // reset cooldown timer

        // Try to get the player's damage interface
        IDamage dmg = GameManager.instance.player.GetComponent<IDamage>();

        if (dmg != null)
        {
            dmg.TakeDamage(contactDamage);
        }
    }

    // Update is called once per frame
    void Update()
    {
        ShootTimer += Time.deltaTime;
        attackTimer += Time.deltaTime;

        if (AgentAI.remainingDistance < 0.01f)
        {
            //increment the Roam timer
            RoamTimer += Time.deltaTime;
        }

        FaceTarget();

        //if player is in the trigger collider
        if (PlayerInTrigger && !CanSeePlayer() && enemyType == EnemyType.ranged)
        {
            CheckRoam();
        }
        else if(!PlayerInTrigger && enemyType == EnemyType.ranged)
        {
            CheckRoam();
        }


        if (PlayerInTrigger && enemyType == EnemyType.melee)
        {
            AgentAI.SetDestination(GameManager.instance.player.transform.position);

            // Check distance between enemy and player
            float distance = Vector3.Distance(transform.position, GameManager.instance.player.transform.position);

            // If close enough to attack, and cooldown is ready and EnemyType.melee
            if (distance <= attackRange && attackTimer >= attackCooldown)
            {
                AttackPlayer();
            }
        }

        // Bull charge logic
        if (enemyType == EnemyType.bull && PlayerInTrigger)
        {
            chargeTimer += Time.deltaTime;

            float distance = Vector3.Distance(transform.position, GameManager.instance.player.transform.position);

            if (distance <= attackRange && attackTimer >= attackCooldown)
            {
                AttackPlayer();
            }

            if (chargeTimer >= chargeCooldown)
            {
                StartCoroutine(BullCharge());
            }
        }

    }
    
    void CheckRoam()
    {
        if (AgentAI.remainingDistance < 0.01f && RoamTimer >= RoamPauseTime)
        {
            Roam();
        }
    }
    
    void Roam() 
    {
        //setting the Timer to 0
        RoamTimer = 0;
        AgentAI.stoppingDistance = 0;

        //Find a random position within a sphere of radius RoamDistance
        Vector3 RandomPosition = Random.insideUnitSphere * RoamDistance;
        RandomPosition += StartPosition;

        //Check if the Roam Position is within the NavMesh
        NavMeshHit Hit;
        NavMesh.SamplePosition(RandomPosition, out Hit, RoamDistance, 1);

        AgentAI.SetDestination(Hit.position);
    }
        
    bool CanSeePlayer()
    {
        //calculate direction vector from the enemy to the player
        playerDirection = GameManager.instance.player.transform.position - HeadPosition.position;

        //Calculate the angle between the enemy's forward direction and the direction to the player
        AngleToPlayer = Vector3.Angle(playerDirection, transform.forward);
        Debug.DrawRay(HeadPosition.position, playerDirection, Color.green);

        RaycastHit hit;

        //cast a ray from the enemy to the player to check for obstacles
        if (Physics.Raycast(HeadPosition.position, playerDirection, out hit, 100, ~IgnoreLayer))
        {
            if (AngleToPlayer <= FOV && hit.collider.CompareTag("Player"))
            {

                //will look for player position and move towards it
                AgentAI.SetDestination(GameManager.instance.player.transform.position);

                if (ShootTimer >= ShootRate && enemyType == EnemyType.ranged)
                {
                    Shoot();
                }

              

                // Check distance between enemy and player
                float distance = Vector3.Distance(transform.position, GameManager.instance.player.transform.position);

                // If close enough to attack, and cooldown is ready and EnemyType.melee
                if (enemyType == EnemyType.melee && distance <= attackRange && attackTimer >= attackCooldown)
                {
                    AttackPlayer();
                }
                AgentAI.stoppingDistance = StoppingDistanceOG;
                return true;
            }

        }
        AgentAI.stoppingDistance = 0;
        return false;
    }

    void FaceTarget()
    {
        //calculate the rotation needed to look at the player
        Quaternion Rotate =
            Quaternion.LookRotation(new Vector3(playerDirection.x, 0, playerDirection.z));

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
        }
        else
        {
            //else if it survives will flash white
            StartCoroutine(FlashRed());
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

    IEnumerator FlashRed()
    {
        Sprite.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        Sprite.color = OGColor;
    }

    void Shoot()
    {
        ShootTimer = 0f;

        //Will created an object at the shoot pos
        Instantiate(Bullet, ShootPos.position, transform.rotation);
    }

    IEnumerator BullCharge()
    {
        chargeTimer = 0f;

        // Direction toward player at start
        Vector3 dir = (GameManager.instance.player.transform.position - transform.position).normalized;
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
        AgentAI.speed = normalSpeed;
    }
}
