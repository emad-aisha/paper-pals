using System.Collections;
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
    [SerializeField] Transform visual;
    [SerializeField] float flyHeight;
    [SerializeField] float flySpeed;
    [SerializeField] float flyAmplitude;
    [SerializeField] float flyFrequency;
    [SerializeField] float flyDistance;
    [SerializeField] private GameObject swoopTrigger;
    private bool PlayerInSwoopZone;
    [SerializeField] private float swoopSpeed;
    public bool isSwooping = false;
    [Header("Bat Ranges")]
    [SerializeField] float followDistance;
    [SerializeField] float shootDistance;
    [SerializeField] float swoopDistance;
    [SerializeField] float retreatDistance;

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
    private bool isCharging = false;
    private Coroutine chargeRoutine;

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


        //SlamArea.SetActive(false);

        if (enemyType == EnemyType.ranged) // Bat
        {
            AgentAI.updatePosition = true;    // Agent moves in XZ
            AgentAI.updateRotation = false;   // Rotate manually with FaceTarget()
            AgentAI.updateUpAxis = false;     // Disable automatic Y alignment
            AgentAI.baseOffset = flyHeight;   // Hover height
        }
        else if (enemyType == EnemyType.bull || enemyType == EnemyType.melee || enemyType == EnemyType.boss)
        {
            AgentAI.updatePosition = true;
            AgentAI.updateRotation = true;
            AgentAI.updateUpAxis = true;
        }
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

        // Check distance between enemy and player
        float distance = Vector3.Distance(transform.position, GameManager.instance.player.transform.position);

        if (AgentAI.remainingDistance < 0.01f)
        {
            //increment the Roam timer
            RoamTimer += Time.deltaTime;
        }

        FaceTarget();

        if (enemyType == EnemyType.ranged)
        {
            // Force the bat to always "see" the player
            canSeePlayer = true;
            PlayerInTrigger = true;

            // Reset last-seen timer
            timeSinceLastSeen = 0;

            // Handle flying movement and attacks
            FlyingBehavior();

            // Skip flashlight/detection logic entirely
            AgentAI.nextPosition = transform.position;

            return; // exit Update early for ranged enemy
        }

        HandleFlashlightDetection();



        if (enemyType == EnemyType.melee || enemyType == EnemyType.boss || enemyType == EnemyType.bull)
        {

            if (canSeePlayer || CanSeePlayer() || PlayerInTrigger)
            {
                timeSinceLastSeen = 0;

                AgentAI.SetDestination(GameManager.instance.player.transform.position);


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
                    CheckRoam();
                }
            }

            // Bull charge logic
            if (enemyType == EnemyType.bull)
            {
               
                if (canSeePlayer || CanSeePlayer())
                {
                    chargeTimer += Time.deltaTime;
                    if (chargeTimer >= chargeCooldown)
                    {
                        StartCoroutine(BullCharge());
                        chargeTimer = 0;
                    }
                 
                    if (distance <= attackRange && attackTimer >= attackCooldown)
                    {
                        AttackPlayer();
                    }
                }
                else
                {
                    chargeTimer = 0;
                }
            }
            UpdateMovementAnimation();
        }

        //boss leap attack
        if (PlayerInTrigger && LeapTimer >= LeapDuration && enemyType == EnemyType.boss)
        {
            LeapFrog();
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
        playerDirection = GameManager.instance.player.transform.position - HeadPosition.position;
        AngleToPlayer = Vector3.Angle(playerDirection, transform.forward);

        RaycastHit hit;
        if (Physics.Raycast(HeadPosition.position, playerDirection, out hit, 30, ~IgnoreLayer))
        {
            if (AngleToPlayer <= FOV && hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }

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

        if (HP <= 0)
        {
            Instantiate(LootDrops, transform.position, transform.rotation);

            Destroy(gameObject);
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


        Transform player = GameManager.instance.player.transform;
        Vector3 dir = (player.position - ShootPos.position).normalized;

        RaycastHit hit;
        
        if (Physics.Raycast(ShootPos.position, dir, out hit, shootDistance, ~IgnoreLayer))
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
        isCharging = true;
        chargeTimer = 0;

        // Direction toward player at start
        Vector3 rawDir = (GameManager.instance.player.transform.position - transform.position);
        rawDir.y = 0;
        Vector3 dir = rawDir.normalized;
        float timer = 0;

        AgentAI.ResetPath();

        while (timer < accelerationTime)
        {
            AgentAI.velocity = dir * Mathf.Lerp(AgentAI.speed, chargeMaxSpeed, timer / accelerationTime);
            timer += Time.deltaTime;
            yield return null;
        }

        // Maintain max speed for charge duration
        float chargeTime = 0;
        while (chargeTime < chargeDuration)
        {
            AgentAI.velocity = dir * chargeMaxSpeed;
            chargeTime += Time.deltaTime;
            yield return null;
        }

        // Stop and resume normal AI
        AgentAI.velocity = Vector3.zero;
        isCharging = false;
        AgentAI.speed = normalSpeed;
        AgentAI.SetDestination(GameManager.instance.player.transform.position);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if we hit the player AND we are currently charging
        if (enemyType == EnemyType.bull && isCharging && collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("BULL HIT PLAYER!");

            IDamage dmg = collision.gameObject.GetComponentInParent<IDamage>();

            if (dmg != null)
            {
                dmg.TakeDamage(contactDamage);
            }


            // 2. STOP THE CHARGE (Optional but recommended)
            // This stops the Bull from sliding through the player after hitting them
            if (chargeRoutine != null) StopCoroutine(chargeRoutine);

            // 3. RESET PHYSICS/LOGIC
            AgentAI.velocity = Vector3.zero;
            isCharging = false;
            AgentAI.speed = normalSpeed;
            AgentAI.ResetPath(); // Stop moving for a moment
        }
    }





    void FlyingBehavior()
    {
        if (isSwooping)
            return;

        Transform player = GameManager.instance.player.transform;
        float distance = Vector3.Distance(transform.position, player.position);

      
        if (distance <= swoopDistance && !isSwooping)
        {
            StartCoroutine(SwoopAttack());
            return;
        }

       
        if (distance <= followDistance)
        {
            FollowPlayer(player);

            // Only shoot if not too close
            if (distance <= shootDistance && ShootTimer >= ShootRate)
            {
                Shoot();
            }
            return;
        }
        CheckRoam();
    }

    void FollowPlayer(Transform player)
    {
        // Let the NavMeshAgent handle horizontal movement
       
        AgentAI.SetDestination(player.position);

        // Apply hover only on Y-axis
        Vector3 pos = transform.position;
        pos.y = flyHeight + Mathf.Sin(Time.time * flyFrequency) * flyAmplitude;
        transform.position = pos;
    }
    public IEnumerator SwoopAttack()
    {
      
        isSwooping = true;
        AgentAI.isStopped = true;

        //Animation: Bat Attack
        if (anim != null)
            anim.SetTrigger("batAttack");

        Transform player = GameManager.instance.player.transform;

        Vector3 start = transform.position;
        Vector3 end = new Vector3(player.position.x,start.y,player.position.z);

        float duration = 1f;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = t / duration;

            Vector3 pos = Vector3.Lerp(start, end, lerp);
            pos.y += Mathf.Sin(lerp * Mathf.PI) * 2f;

            transform.position = pos;
            yield return null;
        }

        AttackPlayer();

        yield return new WaitForSeconds(0.3f);

        Vector3 retreatDir = (start - end).normalized;
        transform.position += retreatDir * retreatDistance;

        AgentAI.ResetPath();
        AgentAI.isStopped = false;
        isSwooping = false;
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

    void LeapFrog()
    {
        //TravelTime for the Leap Attack
        TravelTime += Time.deltaTime;

        float Duration = 0.5f;//0.5f;
        float ZeroToOne = TravelTime / Duration;

        int JumpHeight = 5;

        //move to target
        Vector3 A = transform.position;//from the boss position
        Vector3 B = GameManager.instance.player.transform.position;//to the player position
        Vector3 Pos = Vector3.Lerp(A, B, ZeroToOne); //position between A and B

        //moves the boss in an arc
        Vector3 ArcMotion = Vector3.up * JumpHeight * Mathf.Sin(ZeroToOne * 3.14f);

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
        //Activate slam area and deactivate it 
        SlamArea.SetActive(true);
        yield return new WaitForSeconds(SlamVisibility);
        SlamArea.SetActive(false);
    }
}


