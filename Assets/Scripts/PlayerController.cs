using System.Collections;
using System.Collections.Generic;
using System.Linq;

//using System.Diagnostics;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour, IDamage
{
    // Unity variables
    [Header("Player Neccesities")]
    [SerializeField] CharacterController controller;
    [SerializeField] List<WeaponStats> Weapons = new List<WeaponStats>();
    [SerializeField] GameObject WeaponModel;
    [SerializeField] GameObject GunModel;
    [SerializeField] GameObject ThrowPoint;

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

    [SerializeField] float dashForce; //20
    [Range(0, 1)] public float dashDuration; // 0.2
    [SerializeField] float dashCooldown; // 1

    [Header("Combat")]
    [SerializeField] int Damage;
    [SerializeField] int ShootDistance;
    [SerializeField] float FireRate;
    [SerializeField] float MeleeSpeed;
    [SerializeField] int TickDamage;
    [SerializeField] Transform playerShootPos;
    [SerializeField] GameObject playerBullet;
    [SerializeField] float bulletToCamera;  //Changes the the the amount of time it takes for the bullet to get to the center of the camera


    [SerializeField] int ThrowDistance;
    [SerializeField] GameObject MeleeHitbox;

    float MeleeRange;
    public bool DamageOverTime;
    [SerializeField] List<GameObject> Enemies = new List<GameObject>();

    [Header("Camera Stuff")]
    [SerializeField] float FOVChange;
    [SerializeField] int FOVChangeSpeed;
    bool isSprinting = false;
    float sprintCurr;
    float OGFOV;

    [Header("Flashlight")]
    public GameObject flashlightSwitch;
    bool flashlightOn = true;
    public bool FlashlightOn { get { return flashlightOn; } }


    [Header("Map")]
    public GameObject mapSwitch;
    bool mapOn = false;
    [SerializeField] bool hasMap = false;

    [Header("Audio")]
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] audStep;
    [SerializeField] float audStepVol;
    [SerializeField] AudioClip[] audJump;
    [SerializeField] float audJumpVol;
    [SerializeField] AudioClip[] audHurt;
    [SerializeField] float audHurtVol;

    [Header("Animation")]
    [SerializeField] Animator anim;


    // private variables
    // movement
    Vector3 moveDir;
    Vector3 jumpVelocity;
    int OGGravity;
    float maxGravity;
    int jumpCount;



    // weapon
    GameObject EquippedWeapon;
    int WeaponListPos;
    float FireTimer;
    float MeleeTimer;
    float ThrowTimer;


    // TODO: make this from another script I think?
    // inventory
    bool HaveTape;
    bool IsDead = false;
    bool canDash;
    bool isDashing;
    float dashTimer;
    Vector3 dashDirection;


    // OG stats before boosts
    int MaxHP;
    float OGSpeed;

    float finalSpeed;

    bool isPlayingStep;

    bool isInvincible;
    public float IFrames;
    float IFramesTimer;

    int C4AmmoCurr = 10;


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

        if (LoadSave.instance.GetPlayerWeapons().Count > 0)
            Weapons = LoadSave.instance.GetPlayerWeapons();

        if (Weapons.Count > 0)
        {
            ChangeItem();
        }

        flashlightOn = false;

        if (LoadSave.instance.GetHeartsBought() == 0)
        {
            GameManager.instance.ExtraHearts[0].SetActive(false);
            GameManager.instance.ExtraHearts[1].SetActive(false);
        }
        else if (LoadSave.instance.GetHeartsBought() == 1)
        {
            MaxHP++;
            GameManager.instance.ExtraHearts[0].SetActive(true);
            GameManager.instance.ExtraHearts[1].SetActive(false);
        }
        else if (LoadSave.instance.GetHeartsBought() >= 2)
        {
            MaxHP += 2;
            GameManager.instance.ExtraHearts[0].SetActive(true);
            GameManager.instance.ExtraHearts[1].SetActive(true);
        }

        HaveTape = LoadSave.instance.GetPlayerTape();
        if (HaveTape) GameManager.instance.TapeImage.SetActive(true);


        hasMap = LoadSave.instance.GetPlayerMap();
        if (hasMap) GameManager.instance.MapImage.SetActive(true);

        RespawnPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.isPaused)
        {
            SetIFrames();

            if (hasMap && !GameManager.instance.MapImage.activeSelf)
            {
                GameManager.instance.MapImage.SetActive(true);
            }

            // clean up variables
            RaycastHit hit;

            Debug.DrawRay(GameManager.instance.mainCamera.transform.position, Camera.main.transform.forward * MeleeRange, Color.red);
            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * MeleeRange, Color.blue);

            // interact icon
            if (Physics.Raycast(GameManager.instance.mainCamera.transform.position, Camera.main.transform.forward, out hit, interactDistance, ~IgnoreLayer))
            {
                if (hit.collider.gameObject.layer == 6 || hit.collider.gameObject.layer == 7) GameManager.instance.InteractOn();
                else if (GameManager.instance.isInteractOn) GameManager.instance.InteractOff();
            }
            else if (hit.collider == null)
            {
                GameManager.instance.InteractOff();
            }

            // fov change
            if (isSprinting && GameManager.instance.mainCamera.fieldOfView != OGFOV + FOVChange)
            {
                GameManager.instance.mainCamera.fieldOfView = Mathf.Lerp(GameManager.instance.mainCamera.fieldOfView, OGFOV + FOVChange, Time.deltaTime * FOVChangeSpeed);
            }
            else if (!isSprinting && GameManager.instance.mainCamera.fieldOfView != OGFOV)
            {
                GameManager.instance.mainCamera.fieldOfView = Mathf.Lerp(GameManager.instance.mainCamera.fieldOfView, OGFOV, Time.deltaTime * FOVChangeSpeed);
            }

            if (Input.GetButtonDown("Heal") && HaveTape && HP < MaxHP)
            {
                Heal(healAmount);
                HaveTape = false;
                GameManager.instance.TapeImage.SetActive(false);
                LoadSave.instance.SetPlayerTape(HaveTape);
            }



            if (Input.GetButtonUp("Reload"))
            {
                Reload();
            }


            if (mapOn)
            {
                GameManager.instance.crosshair.SetActive(false); // Hide crosshair when map is open
            }
            else
            {
                GameManager.instance.crosshair.SetActive(true); // Show crosshair when map is closed
                FireTimer += Time.deltaTime;
                MeleeTimer += Time.deltaTime;
                ThrowTimer += Time.deltaTime;
                Movement();
                Sprint();
            }

        }

        aud.volume = LoadSave.instance.GetSFXVolume();

    }

    void Movement()
    {
        //dash mechanics
        if (isDashing)
        {
            dashTimer += Time.deltaTime;
            controller.Move(dashDirection * dashForce * Time.deltaTime);

            if (dashTimer >= dashDuration)
            {
                Debug.Log("add a fov change and lines");
                isDashing = false;
                dashDirection = Vector3.zero;
            }
            return;
        }

        // jump physics
        if (controller.isGrounded)
        {
            if (moveDir.normalized.magnitude > 0.3f && !isPlayingStep)
            {
                StartCoroutine(playStep());
            }

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

                if (Weapons[WeaponListPos].Throwable && ThrowTimer >= Weapons[WeaponListPos].ThrowSpeed)
                {
                    Throw();
                }

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
            Interact();
        }
        SelectWeapon();

        if (Input.GetKeyDown(KeyCode.C) && canDash && GameManager.instance.hasDash)
        {
            Dash();
        }
    }
    void Dash()
    {
        canDash = false;
        isDashing = true;
        dashTimer = 0;

        dashDirection = moveDir.normalized;
        if (dashDirection == Vector3.zero)
        {
            dashDirection = transform.forward;
        }
        canDash = true;
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

    void SetIFrames()
    {
        if (isInvincible)
        {
            IFramesTimer += Time.deltaTime;
        }
        if (IFramesTimer >= IFrames)
        {
            IFramesTimer = 0;
            isInvincible = false;
            GameManager.instance.FlashFrames.SetActive(false);
        }
    }

    void DisableIFrames() // Only for turning off UI Element
    {
        GameManager.instance.FlashFrames.SetActive(false);
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
            aud.PlayOneShot(audJump[Random.Range(0, audJump.Length)], audJumpVol);
        }
        else if (Input.GetButtonDown("Jump") && jumpCount < maxJumps && GameManager.instance.hasDoubleJump)
        {
            jumpVelocity.y = jumpSpeed;
            jumpCount++;
        }
    }


    void Shoot()
    {

        if (Weapons[WeaponListPos].type == WeaponType.Gun)
        {
            GunStats Gun = (GunStats)Weapons[WeaponListPos];

            if (Gun.AmmoCurr <= 0)
            {
                // TODO: add a disappointing sound here idk
                return;
            }
            else
            {


                Gun.AmmoCurr -= 1;
                GameManager.instance.UpdateAmmoCount(-1, Gun);

                GameManager.instance.CurrAmmo.text = Gun.AmmoCurr.ToString();
                // inventory top-right (clip + stored)
                GameManager.instance.UpdateTotal(Gun);
                FireTimer = 0;

                aud.PlayOneShot(Weapons[WeaponListPos].GetAudio(), Weapons[WeaponListPos].Volume);

                Vector3 camDir = GameManager.instance.mainCamera.transform.forward;
                Vector3 gunDir = playerShootPos.forward;
                Vector3 shootDir = Vector3.Lerp(gunDir, camDir, bulletToCamera).normalized;

                Instantiate(playerBullet, playerShootPos.position, Quaternion.LookRotation(shootDir)).GetComponent<playerBullet>().SetDirection(shootDir);

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
        }
    }


    void Reload()
    {
        if (Weapons[WeaponListPos].type == WeaponType.Gun)
        {
            GunStats Gun = (GunStats)(Weapons[WeaponListPos]);

            if (Gun.AmmoCurr >= Gun.AmmoMax)
                return;

            int stored = GameManager.instance.TotalAmmoOwned;
            if (stored <= 0)
                return;


            int Needed = Gun.AmmoMax - Gun.AmmoCurr;
            int ToLoad = Mathf.Min(Needed, stored); //  basically takes however much we need

            Gun.AmmoCurr += ToLoad;

            GameManager.instance.UpdateAmmoCount(-ToLoad, Gun);

            GameManager.instance.CurrAmmo.text = Gun.AmmoCurr.ToString();
            GameManager.instance.TotalAmmo.text = Gun.AmmoMax.ToString();
        }
        else if (Weapons[WeaponListPos].type == WeaponType.Explosive)
        {
            ExplosiveStats explosive = (ExplosiveStats)Weapons[WeaponListPos];


            if (C4AmmoCurr >= explosive.AmmoMax)
                return;

            int stored = GameManager.instance.TotalAmmoOwned;
            if (stored < 10)
                return;


            int needed = explosive.AmmoMax - C4AmmoCurr;
            int toLoad = 10;


            C4AmmoCurr += toLoad;
            GameManager.instance.UpdateAmmoCount(-toLoad);

            GameManager.instance.CurrAmmo.text = C4AmmoCurr.ToString();
            GameManager.instance.TotalAmmo.text = explosive.AmmoMax.ToString();
        }
    }


    // idk what this is buttttt
    // if we get rif of that add enemy bs, we could make it like, oooh someone chasing you
    // so play music heh

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;

        GameObject enemy = other.transform.root.gameObject;

        if (!Enemies.Contains(enemy))
        {
            Enemies.Add(enemy);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;

        GameObject enemy = other.transform.root.gameObject;

        if (Enemies.Contains(enemy))
        {
            Enemies.Remove(enemy);
        }
    }

    void ClearEnemies()
    {
        for (int i = Enemies.Count - 1; i >= 0; i--)
        {
            if (Enemies[i] == null)
            {
                Enemies.RemoveAt(i);
            }
        }
    }


    GameObject GetClosestEnemy()
    {
        GameObject Closest = null;
        float ClosestDist = 9999f;

        Vector3 PlayerPos = transform.position;

        for (int i = 0; i < Enemies.Count; i++)
        {
            float dist = Vector3.Distance(PlayerPos, Enemies[i].transform.position);

            if (dist < ClosestDist)
            {
                ClosestDist = dist;
                Closest = Enemies[i];
            }
        }

        return Closest;
    }

    void Swing()
    {
        //Animation: Melee Attack
        if (anim != null)
            anim.SetTrigger("pencilAttack");

        ClearEnemies();
        MeleeTimer = 0;
        aud.PlayOneShot(Weapons[WeaponListPos].GetAudio(), Weapons[WeaponListPos].Volume);
        if (Enemies.Count == 0) return;

        GameObject enemy = GetClosestEnemy();
        if (enemy == null) return;

        float distance = Vector3.Distance(transform.position, enemy.transform.position);

        MeleeStats melee = (MeleeStats)Weapons[WeaponListPos];

        float meleeRange = melee.MeleeRange;

        if (distance > meleeRange)
            return;

        Vector3 EnemyDirection = (enemy.transform.position - transform.position).normalized;

        if (Vector3.Dot(transform.forward, EnemyDirection) < 0.5f) // outside of fov (cant hit enemies u cant see)
            return;

        IDamage dmg = enemy.GetComponent<IDamage>();

        if (dmg != null)
        {
            dmg.TakeDamage(Weapons[WeaponListPos].GetDamage());
            Debug.Log("hit");
        }
    }

    void Throw()
    {
        if (C4AmmoCurr == 0) return;
        C4AmmoCurr = 0;
        GameManager.instance.CurrAmmo.text = C4AmmoCurr.ToString();

        Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, ThrowDistance, ~IgnoreLayer);

        Vector3 Force = Camera.main.transform.forward * Weapons[WeaponListPos].ThrowForce + transform.up * Weapons[WeaponListPos].ThrowUpwardForce;

        GameObject ThrownObject = Instantiate(EquippedWeapon, ThrowPoint.transform.position, Camera.main.transform.rotation);

        ThrownObject.layer = 0;

        Rigidbody rb = ThrownObject.GetComponent<Rigidbody>();

        rb.AddForce(Force, ForceMode.Impulse);

        ThrowTimer = 0;

        if (ThrownObject.GetComponent<C4>())
        {
            ExplosiveStats Info = (ExplosiveStats)Weapons[WeaponListPos];
            C4 c4 = ThrownObject.GetComponent<C4>();
            if (c4 != null)
            {

                c4.OnThrow(Info);
            }
        }
    }


    void UpdateHealthHearts()
    {
        for (int i = 0; i < MaxHP; i++)
        {
            if (i < HP) GameManager.instance.Hearts[i].SetActive(true);
            else GameManager.instance.Hearts[i].SetActive(false);
        }
    }

    void UpdateSprintBar()
    {
        GameManager.instance.SprintBar.fillAmount = sprintCurr / (float)sprintTimer;
    }


    public void TakeDamage(int amount)
    {
        if (!isInvincible && !IsDead)
        {
            HP -= amount;

            StartCoroutine(Flash(0.1f));

            UpdateHealthHearts();
            aud.pitch = Random.Range(0.9f, 1.1f);
            aud.PlayOneShot(audHurt[Random.Range(0, audHurt.Length)], audHurtVol);

            if (HP <= 0)
            {
                IsDead = true;
                StartCoroutine(DeathAnimation());
            }

            isInvincible = true;
            GameManager.instance.FlashFrames.SetActive(true);
        }

    }

    IEnumerator DeathAnimation()
    {
        if (HP <= 0)
        {
            yield return new WaitForSeconds(0.1f); // leting the character anims and flash play before end the game
            GameManager.instance.Defeat();
        }
    }

    public void Interact()
    {
        RaycastHit hit;

        // Dialogue
        if (Physics.Raycast(GameManager.instance.mainCamera.transform.position, Camera.main.transform.forward, out hit, interactDistance, DialogueLayer))
        {
            IDialogue dialogue = hit.collider.GetComponent<IDialogue>();
            dialogue.SetDialogue();
        }
        else if (Physics.Raycast(GameManager.instance.mainCamera.transform.position, Camera.main.transform.forward, out hit, interactDistance, InteractLayer))
        {
            IInteractable interact = hit.collider.GetComponent<IInteractable>();
            interact.Interact();
            HaveTape = interact.SetTape();
            LoadSave.instance.SetPlayerTape(HaveTape);
        }
        else if (Physics.Raycast(GameManager.instance.mainCamera.transform.position, Camera.main.transform.forward, out hit, interactDistance, InteractLayer))
        {
            IInteractable interact = hit.collider.GetComponent<IInteractable>();
            interact.Interact();
            HaveTape = interact.SetTape();
            LoadSave.instance.SetPlayerTape(HaveTape);
        }

        canDash = GameManager.instance.hasDash;
    }

    public void Heal(int amount)
    {
        HP += amount;
        if (HP > MaxHP) HP = MaxHP;

        UpdateHealthHearts();
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

    public WeaponStats GetWeaponStats(WeaponStats Weapon)
    {

        Weapons.Add(Weapon);
        WeaponListPos = Weapons.Count - 1;

        LoadSave.instance.SetPlayerWeapons(Weapons);
        ChangeItem();

        return Weapon;
    }

    void ChangeItem()
    {

        WeaponStats Weapon = Weapons[WeaponListPos];
        EquippedWeapon = Weapon.Model;
        Damage = Weapon.GetDamage();
        ThrowDistance = Weapon.ThrowDistance;

        if (Weapon.type == WeaponType.Gun)
        {
            GunStats Gun = (GunStats)Weapon;

            ShootDistance = Gun.ShootDistance;
            FireRate = Gun.ShootRate;

            // model stuff

            GunModel.SetActive(true);
            WeaponModel.SetActive(false);
            GunModel.GetComponent<MeshFilter>().sharedMesh = Weapons[WeaponListPos].Model.GetComponent<MeshFilter>().sharedMesh;
            GunModel.GetComponent<MeshRenderer>().sharedMaterial = Weapons[WeaponListPos].Model.GetComponent<MeshRenderer>().sharedMaterial;
            GunModel.layer = 10;

            GunStats CurrGun = (GunStats)Weapons[WeaponListPos];
            GameManager.instance.CurrAmmo.text = CurrGun.AmmoCurr.ToString();
            GameManager.instance.TotalAmmo.text = CurrGun.AmmoMax.ToString();
            // GameManager.instance.TotalAmmoOwned = CurrGun.AmmoCurr;
            // GameManager.instance.UpdateAmmoCount(CurrGun.AmmoCurr + GameManager.instance.TotalAmmoOwned);
            GameManager.instance.AmmoMenu.SetActive(true);
        }

        else if (Weapon.type == WeaponType.Melee)
        {
            GameManager.instance.AmmoMenu.SetActive(false);
            MeleeStats Melee = (MeleeStats)Weapon;

            MeleeSpeed = Melee.SwingSpeed;
            DamageOverTime = Melee.DamageOverTime;
            TickDamage = Melee.TickDamage;
            MeleeRange = Melee.MeleeRange;

            // model stuff
            WeaponModel.SetActive(true);
            GunModel.SetActive(false);
            WeaponModel.GetComponent<MeshFilter>().sharedMesh = Weapons[WeaponListPos].Model.GetComponent<MeshFilter>().sharedMesh;
            WeaponModel.GetComponent<MeshRenderer>().sharedMaterial = Weapons[WeaponListPos].Model.GetComponent<MeshRenderer>().sharedMaterial;
            WeaponModel.layer = 10;
        }

        else if (Weapon.type == WeaponType.Explosive)
        {
            ThrowDistance = Weapon.ThrowDistance;
            Weapon.Throwable = true;

            WeaponModel.SetActive(true);
            GunModel.SetActive(false);
            WeaponModel.GetComponent<MeshFilter>().sharedMesh = Weapons[WeaponListPos].Model.GetComponent<MeshFilter>().sharedMesh;
            WeaponModel.GetComponent<MeshRenderer>().sharedMaterial = Weapons[WeaponListPos].Model.GetComponent<MeshRenderer>().sharedMaterial;
            WeaponModel.layer = 10;

            GameManager.instance.CurrAmmo.text = C4AmmoCurr.ToString();
            GameManager.instance.TotalAmmo.text = 10.ToString();
            GameManager.instance.AmmoMenu.SetActive(true);
        }

        // sets inventory image
        for (int i = 0; i < GameManager.instance.Weapons.Count; i++)
        {
            string name = GameManager.instance.Weapons[i].name;

            int index = WeaponListPos;
            if (Weapons.Count == 2)
            {
                if (index + 1 > 1) index = 0;
                else index = 1;
            }

            if (name.ToUpper() == Weapons[index].name.ToUpper())
            {
                GameManager.instance.Weapons[i].SetActive(true);
            }
            else
            {
                GameManager.instance.Weapons[i].SetActive(false);
            }
        }

    }

    void SelectWeapon()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && WeaponListPos < Weapons.Count - 1)
        {
            WeaponListPos++;
            ChangeItem();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && WeaponListPos > 0)
        {
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
        if (GameManager.instance.AllEnemies.Count > 0)
            for (int i = GameManager.instance.AllEnemies.Count; i <= 0; i++)
            {
                Destroy(GameManager.instance.AllEnemies[i]);
            }

        isInvincible = false;
        IsDead = false;
        DisableIFrames();
        // reset player position to last checkpoint
        controller.transform.position = GameManager.instance.playerSpawnPos.transform.position;

        // resetting and updating player health
        HP = MaxHP;
        UpdateHealthHearts();
    }



    public void MapToggle()
    {
        if (hasMap)
        {
            mapOn = !mapOn;
            GameManager.instance.mapMenu.SetActive(mapOn);
        }
    }
    public WeaponStats GetCurrentWeapon()
    {
        return Weapons[WeaponListPos];
    }
}
