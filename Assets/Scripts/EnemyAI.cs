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
    [SerializeField] float chargeWindUp;
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

        // Calculate direction vector from the enemy to the player
        if (HeadPosition != null)
        {
            playerDirection = GameManager.instance.player.transform.position - HeadPosition.position;
        }

        // NavMesh standard setup
        AgentAI.updatePosition = true;
        AgentAI.updateRotation = false;
        AgentAI.updateUpAxis = false;

        if (enemyType == EnemyType.ranged)
        {
            AgentAI.baseOffset = flyHeight; // Bats float up
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
        // --- 1. TIMERS ---
        LeapTimer += Time.deltaTime;
        ShootTimer += Time.deltaTime;
        attackTimer += Time.deltaTime;

        // Always tick the charge timer up (unless reset)
        if (chargeTimer < chargeCooldown) chargeTimer += Time.deltaTime;

        // Distance check
        float distance = Vector3.Distance(transform.position, GameManager.instance.player.transform.position);

        if (AgentAI.remainingDistance < 0.01f)
        {
            RoamTimer += Time.deltaTime;
        }

        // --- 2. FACE TARGET FIX ---
        // Only look at the player if we are NOT charging.
        // If we are charging, we are locked in a straight line.
        if (!isCharging)
        {
            FaceTarget();
        }

        // --- 3. RANGED LOGIC ---
        if (enemyType == EnemyType.ranged)
        {
            canSeePlayer = true;
            PlayerInTrigger = true;
            timeSinceLastSeen = 0;
            FlyingBehavior();
            AgentAI.nextPosition = transform.position;
            return;
        }

        HandleFlashlightDetection();

        // --- 4. MELEE & BOSS LOGIC ---
        if (enemyType == EnemyType.melee || enemyType == EnemyType.boss)
        {
            if (canSeePlayer || CanSeePlayer() || PlayerInTrigger)
            {
                timeSinceLastSeen = 0f;
                AgentAI.SetDestination(GameManager.instance.player.transform.position);

                if (distance <= attackRange && attackTimer >= attackCooldown)
                {
                    AttackPlayer();
                }
            }
            else
            {
                AgentAI.ResetPath();
                timeSinceLastSeen += Time.deltaTime;
            }
            UpdateMovementAnimation();
        }

        // --- 5. BULL LOGIC (Moved OUTSIDE of Melee block) ---
        if (enemyType == EnemyType.bull)
        {
            if (canSeePlayer || CanSeePlayer())
            {
                timeSinceLastSeen = 0f;

                // Only use NavMesh movement if NOT currently charging
                if (!isCharging)
                {
                    AgentAI.SetDestination(GameManager.instance.player.transform.position);
                }

                // Normal Attack (if close)
                if (distance <= attackRange && attackTimer >= attackCooldown && !isCharging)
                {
                    AttackPlayer();
                }

                // --- THE CHARGE TRIGGER ---
                // Trigger if: Close enough (15), Cooldown ready, and NOT already charging
                if (distance < 15 && chargeTimer >= chargeCooldown && !isCharging)
                {
                    // CRITICAL: Save the routine so we can stop it in OnCollisionEnter
                    chargeRoutine = StartCoroutine(BullCharge());
                }
            }
            else
            {
                // Lost sight logic
                if (!isCharging) AgentAI.ResetPath();

                timeSinceLastSeen += Time.deltaTime;
                chargeTimer = 0f; // Optional: reset charge if he loses you? Up to you.
            }
            UpdateMovementAnimation();
        }

        // --- 6. BOSS LEAP ---
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

       
        AgentAI.ResetPath(); // Stop moving
        AgentAI.velocity = Vector3.zero;

        
        if (anim != null) 
            anim.SetTrigger("bullBuild");

       
        yield return new WaitForSeconds(chargeWindUp);


        if (anim != null) 
            anim.SetBool("bullCharge", true);

        // Calculate direction (Aiming at where player is NOW)
        Vector3 rawDir = (GameManager.instance.player.transform.position - transform.position);
        rawDir.y = 0;
        Vector3 dir = rawDir.normalized;
        float timer = 0;

        // Acceleration Loop
        while (timer < accelerationTime)
        {
            AgentAI.velocity = dir * Mathf.Lerp(AgentAI.speed, chargeMaxSpeed, timer / accelerationTime);
            timer += Time.deltaTime;
            yield return null;
        }

        // Max Speed Loop
        float chargeTime = 0;
        while (chargeTime < chargeDuration)
        {
            AgentAI.velocity = dir * chargeMaxSpeed;
            chargeTime += Time.deltaTime;
            yield return null;
        }

       
        AgentAI.velocity = Vector3.zero;
        isCharging = false;

        // Stop animation
        if (anim != null) 
            anim.SetBool("bullCharge", false);

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

            // 2. STOP THE CHARGE
            if (chargeRoutine != null) StopCoroutine(chargeRoutine);

            // 3. RESET PHYSICS/LOGIC
            AgentAI.velocity = Vector3.zero;
            isCharging = false;

            // --- ADD THIS LINE ---
            // Without this, he keeps "running" in place after hitting you
            if (anim != null) anim.SetBool("isCharging", false);
            // ---------------------

            AgentAI.speed = normalSpeed;
            AgentAI.ResetPath();
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


