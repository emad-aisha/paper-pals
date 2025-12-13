using NUnit.Framework;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;


public class EnemyAI : MonoBehaviour, IDamage
{
    public enum EnemyType { ranged, melee, bull, boss };
    [Header("Enemy Type")]
    [SerializeField] EnemyType enemyType;
    
    [Header("Boss")]
    [SerializeField] float LeapDuration;
    [SerializeField] float SlamVisibility;
    [SerializeField] GameObject SlamArea;

    [Header("Loot Drops")]
    [SerializeField] GameObject LootDrops;

    [Header("Neccesities")]
    [SerializeField] LayerMask IgnoreLayer;
    [SerializeField] NavMeshAgent AgentAI;
    [SerializeField] SpriteRenderer Sprite;

    [Header("Bat")]
    [SerializeField] float flyHeight;
    [SerializeField] float flySpeed;
    [SerializeField] float flyAmplitude;
    [SerializeField] float flyFrequency;
    [SerializeField] float flyDistance;
    [SerializeField] private GameObject swoopTrigger;
    private bool PlayerInSwoopZone;
    [SerializeField] private float swoopSpeed;
    public bool isSwooping = false;

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

    [Header("Detection")]
    [SerializeField] float baseDetectionDistance;
    [SerializeField] float flashlightMultiplier;
    [SerializeField] float lookMultiplier;
    [SerializeField] float awayMultiplier;
    [SerializeField] float loseSightDelay;
    [SerializeField] float lookDotThreshold;

    [Header("Animation")]
    [SerializeField] Animator anim;

    //[SerializeField] string walkBoolName = "catWalking";

    // private variables   
    bool PlayerInTrigger;
    float ShootTimer;
    Color OGColor;

    float timeSinceLastSeen;
    bool canSeePlayer;

    // bull variables
    float attackTimer = 0;
    float chargeTimer = 0;

    float normalSpeed;

    //boss variables
    float LeapTimer = 0f;
    float TravelTime = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.instance.AllEnemies.Add(this.gameObject);
        anim = GetComponentInChildren<Animator>();

        OGColor = Sprite.material.color;
        normalSpeed = AgentAI.speed;

        StoppingDistanceOG = AgentAI.stoppingDistance;
        StartPosition = transform.position;

        //calculate direction vector from the enemy to the player
        playerDirection = GameManager.instance.player.transform.position - HeadPosition.position;

        AgentAI.updateRotation = false;
        //SlamArea.SetActive(false);
    }

    void AttackPlayer()
    {
        attackTimer = 0f; // reset cooldown timer

        //Animation: Cat Attack
        if (anim != null && enemyType == EnemyType.melee)
            anim.SetTrigger("catAttack");

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
        LeapTimer += Time.deltaTime;//Boss Leap Attack Timer
        ShootTimer += Time.deltaTime;
        attackTimer += Time.deltaTime;

        if (AgentAI.remainingDistance < 0.01f)
        {
            //increment the Roam timer
            RoamTimer += Time.deltaTime;
        }

        FaceTarget();

        HandleFlashlightDetection();

        if (enemyType == EnemyType.ranged)
        {
            if (canSeePlayer || CanSeePlayer())
            {
                timeSinceLastSeen = 0f;
                AgentAI.SetDestination(GameManager.instance.player.transform.position);
            }
            else
            {
                timeSinceLastSeen += Time.deltaTime;
                if (timeSinceLastSeen >= loseSightDelay)
                {
                    CheckRoam();
                }
            }
            FlyingBehavior();
        }


        if (enemyType == EnemyType.melee)
        {
            if (canSeePlayer || CanSeePlayer())
            {
                timeSinceLastSeen = 0f;

                AgentAI.SetDestination(GameManager.instance.player.transform.position);

                // Check distance between enemy and player
                float distance = Vector3.Distance(transform.position, GameManager.instance.player.transform.position);

                // If close enough to attack, and cooldown is ready and EnemyType.melee
                if (distance <= attackRange && attackTimer >= attackCooldown)
                {
                    AttackPlayer();
                }
            }
            else
            {
                AgentAI.ResetPath();
                timeSinceLastSeen += Time.deltaTime;

                if (timeSinceLastSeen >= loseSightDelay)
                {
                    //CheckRoam();
                }
            }

            // Bull charge logic
            if (enemyType == EnemyType.bull)
            {
                if (canSeePlayer || CanSeePlayer())
                {
                    timeSinceLastSeen = 0f;

                    AgentAI.SetDestination(GameManager.instance.player.transform.position);

                    float distance = Vector3.Distance(transform.position, GameManager.instance.player.transform.position);
                    if (distance <= attackRange && attackTimer >= attackCooldown)
                    {
                        AttackPlayer();
                    }

                    chargeTimer += Time.deltaTime;
                    if (chargeTimer >= chargeCooldown)
                    {
                        StartCoroutine(BullCharge());
                        chargeTimer = 0f;
                    }


                }
                else
                {
                    AgentAI.ResetPath();
                    timeSinceLastSeen += Time.deltaTime;
                    if (timeSinceLastSeen >= loseSightDelay)
                    {
                        //CheckRoam();
                    }
                    chargeTimer = 0f;
                }


            }


            UpdateMovementAnimation();
        }

        //boss leap attack
        if (PlayerInTrigger && LeapTimer >= LeapDuration && enemyType == EnemyType.boss)
        {
            LeapFog();
        }

    }

    //flashlight detection methods

    bool IsPlayerLookingAtEnemy(Transform player)
    {
        Vector3 toEnemy = (transform.position - player.position).normalized;
        float dot = Vector3.Dot(player.forward, toEnemy);
        return dot >= lookDotThreshold;
    }

    void HandleFlashlightDetection()
    {
        Transform player = GameManager.instance.player.transform;
        bool flashlightActive = GameManager.instance.hasFlashlight && GameManager.instance.controller.FlashlightOn;
        bool flashlightSeesPlayer = flashlightActive && IsPlayerLookingAtEnemy(player);
        bool withinTrigger = PlayerInTrigger;

        if (flashlightSeesPlayer && withinTrigger)
        {
            if (CanSeePlayer())
            {
                canSeePlayer = true;
                timeSinceLastSeen = 0f;
            }
            else
            {
                canSeePlayer = false;
                timeSinceLastSeen += Time.deltaTime;
            }
        }
        else
        {
            canSeePlayer = false;
            timeSinceLastSeen += Time.deltaTime;
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
        Debug.Log("Roam");
        //setting the Timer to 0
        RoamTimer = 0;
        AgentAI.stoppingDistance = 0;

        //Find a random position within a sphere of radius RoamDistance
        Vector3 RandomPosition = Random.insideUnitSphere * RoamDistance;
        RandomPosition += StartPosition;

        //Check if the Roam Position is within the NavMesh
        NavMeshHit Hit;
        NavMesh.SamplePosition(RandomPosition, out Hit, RoamDistance, 1);

        if (Hit.position.x != float.PositiveInfinity)
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
        if (Physics.Raycast(HeadPosition.position, playerDirection, out hit, 30, ~IgnoreLayer))
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
        // Update direction EVERY frame
        Vector3 targetPos = GameManager.instance.player.transform.position;
        playerDirection = targetPos - HeadPosition.position;

        if (playerDirection.sqrMagnitude < 0.001f)
            return;

        Vector3 flatDir = new Vector3(playerDirection.x, 0, playerDirection.z);

        Quaternion targetRot = Quaternion.LookRotation(flatDir);

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, FaceTargetSpeed * Time.deltaTime);
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;

        AgentAI.SetDestination(GameManager.instance.player.transform.position);
        LeapFog();
        if (HP <= 0)
        {
            Instantiate(LootDrops, transform.position, transform.rotation);

            Destroy(gameObject);
            GameManager.instance.gameGoalCounter++;
            GameManager.instance.UpdateKeysLeft();
        }
        else
        {
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
        Sprite.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        Sprite.material.color = OGColor;
    }

    void Shoot()
    {
        ShootTimer = 0;

        RaycastHit hit;
        float shootDistance = 100f; // or whatever range you want

        // Raycast from the shoot position forward
        if (Physics.Raycast(ShootPos.position, transform.forward, out hit, shootDistance, ~IgnoreLayer))
        {
            // Damage player if hit
            IDamage dmg = hit.collider.GetComponent<IDamage>();
            if (dmg != null)
            {
                dmg.TakeDamage(contactDamage);
            }

        }
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
        AgentAI.ResetPath();
        AgentAI.SetDestination(GameManager.instance.player.transform.position);
    }


    [SerializeField] float BatDetectDistance;
    void FlyingBehavior()
    {
        Transform player = GameManager.instance.player.transform;
        Vector3 target = player.position;

        // to not let the bat jump with the player
        float hoverBase = 1.07f;


        // Hover offset
        target.y = hoverBase + flyHeight;
        float hover = Mathf.Sin(Time.time * flyFrequency) * flyAmplitude;
        target.y += hover;

        Vector3 flatDirection = new Vector3(target.x - transform.position.x, 0, target.z - transform.position.z);
        float flatDistance = flatDirection.magnitude;


        //Move with NavMeshAgent if farther than flyDistance
        if (flatDistance > flyDistance && flatDistance < BatDetectDistance)
        {
            AgentAI.SetDestination(new Vector3(player.position.x, transform.position.y, player.position.z));
        }
        else if (flatDistance < BatDetectDistance)
        {
            CheckRoam();
        }
        else
        {
            //Stop agent near the player
            AgentAI.ResetPath();
        }

        // Adjust Y manually for hovering
        Vector3 pos = transform.position;
        pos.y = target.y;
        transform.position = pos;



        if (ShootTimer >= ShootRate)
        {
            Shoot();
        }
    }


    public IEnumerator SwoopAttack()
    {
        isSwooping = true;

        Transform player = GameManager.instance.player.transform;

        float originalShootTimer = ShootTimer;
        ShootTimer = 0; // Reset shoot timer to prevent shooting during swoop

        Vector3 startPosition = transform.position;
        Vector3 direction = (player.position - transform.position).normalized;

        AttackPlayer();
        yield return new WaitForSeconds(1); // Pause briefly after attack

        isSwooping = false;
        ShootTimer = originalShootTimer;
    }


    //Walk Animation
    void UpdateMovementAnimation()
    {
        if (anim == null || AgentAI == null)
            return;

        float speed = AgentAI.velocity.sqrMagnitude;
        bool walking = speed > 0.05f;

        //anim.SetBool("catWalking", walking);
    }

    void LeapFog()
    {
        //TravelTime for the Leap Attack
        TravelTime += Time.deltaTime;

        float Duration = 0.5f;
        float ZeroToOne = TravelTime / Duration;

        int HoldsFive = 5;

        //move to target
        Vector3 A = transform.position;//from the boss position
        Vector3 B = GameManager.instance.player.transform.position;//to the player position
        Vector3 Pos = Vector3.Lerp(A, B, ZeroToOne); //position between A and B

        //moves the boss in an arc
        Vector3 ArcMotion = Vector3.up * HoldsFive * Mathf.Sin(ZeroToOne * 3.14f); //move in an arc

        //make the boss leap
        transform.position = Pos + ArcMotion;

        //when the boss reaches the player position or "B"
        if (ZeroToOne >= 1)
        {
            //reset timers 
            LeapTimer = 0f;
            TravelTime = 0f;

            StartCoroutine(Slam()); 
        }
    }
    IEnumerator Slam()
    {
        //Activate slam area and deactivate it after 1 second
        SlamArea.SetActive(true);
        yield return new WaitForSeconds(SlamVisibility);
        SlamArea.SetActive(false);
    }
}


