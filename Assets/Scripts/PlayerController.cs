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

    [Header("Layers")]
    [SerializeField] LayerMask IgnoreLayer;
    [SerializeField] LayerMask DialogueLayer;
    [SerializeField] LayerMask InteractLayer;

    [Header("UI stuffs")]
    [SerializeField] int interactDistance;
    [SerializeField] int HP;
    [SerializeField] int healAmount;
    [SerializeField] int HPOffset;
    [SerializeField] int SOffset;

    [Header("Movement")]
    [SerializeField] int speed;
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
    public bool DamageOverTime;

    [Header("Camera Stuff")]
    [SerializeField] float FOVChange;
    [SerializeField] int FOVChangeSpeed;
    bool isSprinting = false;
    float sprintCurr;
    float OGFOV;

    [Header("Flashlight")]
    public GameObject flashlightSwitch;
    private bool flashlightOn = true;

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

    // inventory
    bool HaveTape;   

    // OG stats before boosts
    int MaxHP;
    int OGSpeed;
    bool isInvincible; // TODO: maybe get rid of this?

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OGFOV = GameManager.instance.mainCamera.fieldOfView;
        MaxHP = HP;
        OGGravity = (int)gravity;
        maxGravity = gravity * 1.3f;
        HaveTape = false;
        sprintCurr = sprintTimer;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.isPaused) {
            // clean up variables
            RaycastHit hit;

            Debug.DrawRay(GameManager.instance.mainCamera.transform.position, Camera.main.transform.forward * ShootDistance, Color.blue);

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
            Movement();
        }
        
        Sprint();
        
    }

    void Movement()
    {
        // jump physics
        if (controller.isGrounded) {
            jumpVelocity = Vector3.zero;
            jumpCount = 0;
            gravity = OGGravity;
        }
        else {
            jumpVelocity.y -= (gravity * Time.deltaTime);
            if (gravity < maxGravity) gravity *= 1.005f;
        }

        // movement
        moveDir = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        float finalSpeed = speed + sprintCurrBoost;
        controller.Move(moveDir * finalSpeed * Time.deltaTime);

        // jump movement
        Jump();
        controller.Move(jumpVelocity * Time.deltaTime);

        if (Input.GetButton("Fire1") && FireTimer >= FireRate) {
            Shoot();
        }
        if (Input.GetButtonDown("Interact")) {
            // initial interact
            Interact();
        }
    }

    void Sprint()
    {
        bool wantsToSprint = Input.GetButton("Sprint") && sprintCurr > 0;

        if (wantsToSprint) {
            isSprinting = true;
            sprintCurr -= sprintDrainRate * Time.deltaTime;
        }
        else {
            isSprinting = false;
            sprintCurr += sprintRegenRate * Time.deltaTime;
        }

        if (sprintCurr > sprintTimer) 
            sprintCurr = sprintTimer;

        sprintCurrBoost = sprintMod * (sprintCurr / sprintTimer);
        UpdateSprintBar();
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < maxJumps)
        {
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
                dmg.TakeDamage(Damage);
            }
        }
    }

    void UpdateHealthBar()
    {
        float healthDecimal = HP / (float)MaxHP;
        float healthBarPercent = healthDecimal * 500;

        float healthBarPosition = healthBarPercent - 500 + HPOffset;
        if (healthBarPosition > HPOffset) healthBarPosition = HPOffset;

        GameManager.instance.HealthBar.transform.position = new Vector3(healthBarPosition, 995, 0);
    }

    void UpdateSprintBar()
    {
        float staminaDecimal = sprintCurr / sprintTimer;
        float staminaPercent = staminaDecimal * 500;

        float staminaBarPosition = staminaPercent - 500 + SOffset;
        if (staminaBarPosition > SOffset) staminaBarPosition = SOffset;

        GameManager.instance.SprintBar.transform.position = new Vector3(staminaBarPosition, 915, 0);
        
        // float barScaleX = staminaPercent * (sprintCurrBoost / sprintMod);
        // barScaleX = Mathf.Clamp(barScaleX, 0f, 1.5f);
        // Vector3 scale = GameManager.instance.SprintBar.transform.localScale;
        // scale.x = barScaleX;
        // GameManager.instance.SprintBar.transform.localScale = scale;
    }


    public void TakeDamage(int amount)
    {
        HP -= amount;

        StartCoroutine(Flash(0.1f));
        UpdateHealthBar();

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
        }
        WeaponModel.GetComponent<MeshFilter>().sharedMesh = Weapons[WeaponListPos].Model.GetComponent<MeshFilter>().sharedMesh;
        WeaponModel.GetComponent<MeshRenderer>().sharedMaterial = Weapons[WeaponListPos].Model.GetComponent<MeshRenderer>().sharedMaterial;
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
}