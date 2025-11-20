using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamage
{
    // Unity variables
    [Header("Player Neccesities")]
    [SerializeField] CharacterController controller;
    [SerializeField] List<WeaponStats> Weapons = new List<WeaponStats>();
    [SerializeField] GameObject WeaponModel;
    [SerializeField] GameObject GunModel;

    [Header("Layers")]
    [SerializeField] LayerMask IgnoreLayer;
    [SerializeField] LayerMask DialogueLayer;
    [SerializeField] LayerMask InteractLayer;

    [Header("UI stuffs")]
    [SerializeField] int interactDistance;
    [SerializeField] int HP;
    [SerializeField] int healAmount;

    [Header("Movement")]
    [SerializeField] float speed;
    [SerializeField] int sprintMod;
    [SerializeField] float sprintDrainRate;
    [SerializeField] float sprintRegenRate;
    [SerializeField] int sprintTimer;
    [SerializeField] float sprintCurrBoost;

    [SerializeField] int jumpSpeed;
    [SerializeField] int maxJumps;
    [SerializeField] float gravity;

    [Header("Combat")]
    [SerializeField] int Damage;
    [SerializeField] int ShootDistance;
    [SerializeField] float FireRate;
    [SerializeField] float MeleeSpeed;
    [SerializeField] int TickDamage;
    [SerializeField] GameObject MeleeHitbox;

    float MeleeRange;
    public bool DamageOverTime;
    [SerializeField] List<IDamage> Enemies = new List<IDamage>();

    [Header("Camera Stuff")]
    [SerializeField] float FOVChange;
    [SerializeField] int FOVChangeSpeed;
    bool isSprinting = false;
    float sprintCurr;
    float OGFOV;

    [Header("Flashlight")]
    public GameObject flashlightSwitch;
    private bool flashlightOn = true;

    [Header("Audio")]
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] audStep;
    [SerializeField] float audStepVol;
    [SerializeField] AudioClip[] audJump;
    [SerializeField] float audJumpVol;
    [SerializeField] AudioClip[] audHurt;
    [SerializeField] float audHurtVol;



    // private variables
    // movement
    Vector3 moveDir;
    Vector3 jumpVelocity;
    int OGGravity;
    float maxGravity;
    int jumpCount;


    // TODO: change this to shooting based on tapping
    // weapon
    GameObject EquippedWeapon;
    int WeaponListPos;
    float FireTimer;
    float MeleeTimer;

    // inventory
    bool HaveTape;   

    // OG stats before boosts
    int MaxHP;
    float OGSpeed;
    bool isInvincible; // TODO: maybe get rid of this?
    float finalSpeed;

    bool isPlayingStep;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OGFOV = GameManager.instance.mainCamera.fieldOfView;
        MaxHP = HP;
        OGGravity = (int)gravity;
        maxGravity = gravity * 1.3f;
        HaveTape = false;
        sprintCurr = sprintTimer;
        OGSpeed = speed;

        RespawnPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.isPaused) {
            // clean up variables
            RaycastHit hit;

           // Debug.DrawRay(GameManager.instance.mainCamera.transform.position, Camera.main.transform.forward * ShootDistance, Color.blue);
            Debug.DrawRay(GameManager.instance.mainCamera.transform.position, Camera.main.transform.forward * MeleeRange, Color.red);

            // interact icon
            if (Physics.Raycast(GameManager.instance.mainCamera.transform.position, Camera.main.transform.forward, out hit, interactDistance, ~IgnoreLayer)) {
                if (hit.collider.gameObject.layer == 6 || hit.collider.gameObject.layer == 7) GameManager.instance.InteractOn();
                else if (GameManager.instance.isInteractOn) GameManager.instance.InteractOff();
            }
            else if (hit.collider == null) {
                GameManager.instance.InteractOff();
            }

            // fov change
            if (isSprinting && GameManager.instance.mainCamera.fieldOfView != OGFOV + FOVChange) {
                GameManager.instance.mainCamera.fieldOfView = Mathf.Lerp(GameManager.instance.mainCamera.fieldOfView, OGFOV + FOVChange, Time.deltaTime * FOVChangeSpeed);
            }
            else if (!isSprinting && GameManager.instance.mainCamera.fieldOfView != OGFOV) {
                GameManager.instance.mainCamera.fieldOfView = Mathf.Lerp(GameManager.instance.mainCamera.fieldOfView, OGFOV, Time.deltaTime * FOVChangeSpeed);
            }

            if (Input.GetButtonDown("Heal") && HaveTape && HP < MaxHP) {
                Heal(healAmount);
                HaveTape = false;
                GameManager.instance.TapeImage.SetActive(false);
            }

            FireTimer += Time.deltaTime;
            MeleeTimer += Time.deltaTime;
            Movement();
        }
        
        Sprint();

    }

    void Movement()
    {
        // jump physics
        if (controller.isGrounded)
        {
            if( moveDir.normalized.magnitude > 0.3f && !isPlayingStep)
            {
                StartCoroutine(playStep());
            }
;

            jumpVelocity = Vector3.zero;
            jumpCount = 0;
            gravity = OGGravity;
        }
        else
        {
            jumpVelocity.y -= (gravity * Time.deltaTime);
            if (gravity < maxGravity) gravity *= 1.005f;
        }

        // movement
        moveDir = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        if (isSprinting)
        {
            speed = finalSpeed;
        }
        else
        {
            speed = OGSpeed;
        }
        controller.Move(moveDir * speed * Time.deltaTime);

        // jump movement
        Jump();
        controller.Move(jumpVelocity * Time.deltaTime);
        if (Weapons.Count > 0)
        {
            if (Input.GetButton("Fire1"))
            {
                if (Weapons[WeaponListPos].type == WeaponType.Gun && FireTimer >= FireRate)
                {
                    Shoot();
                }
                else if (Weapons[WeaponListPos].type == WeaponType.Melee && MeleeTimer >= MeleeSpeed)
                {
                    Swing();
                }
            }
        }

        if (Input.GetButtonDown("Interact"))
        {
            // initial interact
            Interact();
        }
        SelectWeapon();
    }

    IEnumerator playStep()
    {
        isPlayingStep = true;
        aud.pitch = Random.Range(0.9f, 1.1f);
        aud.PlayOneShot(audStep[Random.Range(0, audStep.Length)], audStepVol);

        if (isSprinting)
        {
            yield return new WaitForSeconds(0.3f);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }

        isPlayingStep = false;
    }

    void Sprint()
    {
        bool wantsToSprint = Input.GetButton("Sprint") && sprintCurr > 0;

        if (wantsToSprint)
        {
            isSprinting = true;
            sprintCurr -= sprintDrainRate * Time.deltaTime;

        }
        else
        {
            isSprinting = false;
            sprintCurr += sprintRegenRate * Time.deltaTime;
        }

        if (sprintCurr > sprintTimer) 
            sprintCurr = sprintTimer;

        if (sprintCurr < 0f) 
            sprintCurr = 0f;

        sprintCurrBoost = sprintMod * (sprintCurr / sprintTimer);
        finalSpeed = OGSpeed + sprintCurrBoost;
     
        UpdateSprintBar();
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount == 0)
        {
            jumpVelocity.y = jumpSpeed;
            jumpCount++;
            aud.pitch = Random.Range(0.9f, 1.1f);
            aud.PlayOneShot(audJump[Random.Range(0, audJump.Length)], audJumpVol);
        }
        else if (Input.GetButtonDown("Jump") && jumpCount < maxJumps && GameManager.instance.hasDoubleJump) {
            jumpVelocity.y = jumpSpeed;
            jumpCount++;
        }
    }


    void Shoot()
    {
        FireTimer = 0;

        RaycastHit hit;

        if (Physics.Raycast(GameManager.instance.mainCamera.transform.position, Camera.main.transform.forward, out hit, ShootDistance, ~IgnoreLayer))
        {
            IDamage dmg = hit.collider.GetComponent<IDamage>();
            if (dmg != null)
            {
                Instantiate(Weapons[WeaponListPos].HitFX, hit.point, Quaternion.identity);
                dmg.TakeDamage(Damage);
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy")) 
        {
            
            IDamage Enemy = other.GetComponent<IDamage>();

            if (!Enemies.Contains(Enemy))
            {
                Enemies.Add(Enemy);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
     
            IDamage Enemy = other.GetComponent<IDamage>();

            if (Enemies.Contains(Enemy))
            {
                Enemies.Remove(Enemy);
            }
        }
    }

    void Swing()
    {
        MeleeTimer = 0;
        //Debug.Log("called");
        if (Enemies.Count > 0)
        {

            Debug.Log("swinging");

            RaycastHit hit;

            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, MeleeRange , ~IgnoreLayer))
            {
                IDamage dmg = hit.collider.GetComponent<IDamage>();

                if (dmg != null && Enemies.Contains(dmg))
                {
                    dmg.TakeDamage(Weapons[WeaponListPos].GetDamage());
                    return;
                }
            }
        }
    }



    void UpdateHealthBar()
    {
        GameManager.instance.HealthBar.fillAmount = HP / (float)MaxHP;
    }

    void UpdateSprintBar()
    {
        GameManager.instance.SprintBar.fillAmount = sprintCurr / (float)sprintTimer;
    }


    public void TakeDamage(int amount)
    {
        HP -= amount;

        StartCoroutine(Flash(0.1f));
        UpdateHealthBar();
        aud.pitch = Random.Range(0.9f, 1.1f);
        aud.PlayOneShot(audHurt[Random.Range(0, audHurt.Length)], audHurtVol);

        if (HP <= 0)
        {
            GameManager.instance.Defeat();
        }
    }

    public void Interact()
    {
        RaycastHit hit;

        // Dialogue
        if (Physics.Raycast(GameManager.instance.mainCamera.transform.position, Camera.main.transform.forward, out hit, interactDistance, DialogueLayer)) {
            IDialogue dialogue = hit.collider.GetComponent<IDialogue>();
            dialogue.SetDialogue();
        }
        else if (Physics.Raycast(GameManager.instance.mainCamera.transform.position, Camera.main.transform.forward, out hit, interactDistance, InteractLayer)) {
            IInteractable interact = hit.collider.GetComponent<IInteractable>();
            interact.Interact();
            HaveTape = interact.SetTape();
        }
    }

    public void Heal(int amount)
    {
        HP += amount;
        if (HP > MaxHP) HP = MaxHP;

        UpdateHealthBar();
    }
    public IEnumerator Shield(int duration)
    {
        bool original = isInvincible;
        isInvincible = true;
        yield return new WaitForSeconds(duration);
        isInvincible = original;

    }

    public IEnumerator Flash(float duration)
    {
        GameManager.instance.flashRed.SetActive(true);
        yield return new WaitForSeconds(duration);
        GameManager.instance.flashRed.SetActive(false);
    }

    public void GetWeaponStats(WeaponStats Weapon)
    {

        Weapons.Add(Weapon);
        WeaponListPos = Weapons.Count - 1;
        ChangeItem();
    }

    void ChangeItem()
    {

        WeaponStats Weapon = Weapons[WeaponListPos];
        Damage = Weapon.GetDamage();

        if (Weapon.type == WeaponType.Gun)
        {
            Debug.Log("hi king");
            GunStats Gun = (GunStats)Weapon;

            ShootDistance = Gun.ShootDistance;
            FireRate = Gun.ShootRate;
        }

        else if (Weapon.type == WeaponType.Melee)
        {
            MeleeStats Melee = (MeleeStats)Weapon;

            MeleeSpeed = Melee.SwingSpeed;
            DamageOverTime = Melee.DamageOverTime;
            TickDamage = Melee.TickDamage;
            MeleeRange = Melee.MeleeRange;
        }

        if (Weapons[WeaponListPos].type == WeaponType.Melee) {
            WeaponModel.SetActive(true);
            GunModel.SetActive(false);
            WeaponModel.GetComponent<MeshFilter>().sharedMesh = Weapons[WeaponListPos].Model.GetComponent<MeshFilter>().sharedMesh;
            WeaponModel.GetComponent<MeshRenderer>().sharedMaterial = Weapons[WeaponListPos].Model.GetComponent<MeshRenderer>().sharedMaterial;
            WeaponModel.layer = 10; 
        }
        else if (Weapons[WeaponListPos].type == WeaponType.Gun) {
            GunModel.SetActive(true);
            WeaponModel.SetActive(false);
            GunModel.GetComponent<MeshFilter>().sharedMesh = Weapons[WeaponListPos].Model.GetComponent<MeshFilter>().sharedMesh;
            GunModel.GetComponent<MeshRenderer>().sharedMaterial = Weapons[WeaponListPos].Model.GetComponent<MeshRenderer>().sharedMaterial;
            GunModel.layer = 10;
        }
    }

    void SelectWeapon()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && WeaponListPos < Weapons.Count - 1) {
            WeaponListPos++;
            ChangeItem();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && WeaponListPos > 0) {
            WeaponListPos--;
            ChangeItem();
        }
    }

    internal void FlashlightToggle()
    {
        flashlightOn = !flashlightOn;
        flashlightSwitch.SetActive(flashlightOn);
    }

    public void RespawnPlayer()
    {
        // reset player position to last checkpoint
        controller.transform.position = GameManager.instance.playerSpawnPos.transform.position;

        // resetting and updating player health
        HP = MaxHP;
        UpdateHealthBar();
    }


}