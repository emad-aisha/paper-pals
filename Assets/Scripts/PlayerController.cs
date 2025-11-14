using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour, IInteractable, IDamage
{
    // Unity variables
    [Header("Player Neccesities")]
    [SerializeField] CharacterController controller;
    
	[Header("Layers")]
    [SerializeField] LayerMask IgnoreLayer;
	[SerializeField] LayerMask DialougeLayer;
    [SerializeField] LayerMask TapeLayer;
    [SerializeField] LayerMask AmmoLayer;
    [SerializeField] LayerMask CoinLayer;

	[Header("UI stuffs")]
    [SerializeField] int interactDistance;
	[SerializeField] int HP;
    [SerializeField] int healAmount;

	[Header("Movement")]
	[SerializeField] int speed;
    [SerializeField] int sprintMod;

    [SerializeField] int jumpSpeed;
    [SerializeField] int maxJumps;
    [SerializeField] float gravity;

	[Header("Shooting")]
	[SerializeField] int ShootDamage;
    [SerializeField] int ShootDistance;
    [SerializeField] float FireRate;

    
    // private variables
    // movement
    Vector3 moveDir;
    Vector3 jumpVelocity;
    int OGGravity;
    float maxGravity;

    int jumpCount;
    bool HaveTape;

    // TODO: change this to shooting based on tapping
    float FireTimer;

    // OG stats before boosts
    int MaxHP;
    int OGSpeed;

    // TODO: maybe get rid of this?
    bool isInvincible;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        MaxHP = HP;
        OGGravity = (int)gravity;
        maxGravity = gravity * 1.2f;
        HaveTape = false;
    }

    // Update is called once per frame
    void Update() {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * ShootDistance, Color.blue);

        // interactable icon showing up
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, interactDistance, ~IgnoreLayer)
            && GameManager.instance.isPaused == false) {

            if (hit.collider.gameObject.layer == 7) GameManager.instance.InteractOn();
            else if (GameManager.instance.isInteractOn) GameManager.instance.InteractOff();

        }
        else if (hit.collider == null && GameManager.instance.isPaused == false) {
            GameManager.instance.InteractOff();
        }

        if (Input.GetButtonDown("Heal") && HaveTape && HP < MaxHP) {
            Heal(healAmount);
            HaveTape = false;
            GameManager.instance.TapeImage.SetActive(false);
        }

        FireTimer += Time.deltaTime;
        Movement();
        Sprint();
    }

    void Movement() {
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
        controller.Move(moveDir * speed * Time.deltaTime);

        // jump movement
        Jump();
        controller.Move(jumpVelocity * Time.deltaTime);

        if (Input.GetButton("Fire1") && FireTimer >= FireRate)
        {
            Shoot();
        }
        if (Input.GetButtonDown("Interact")) {
            Interact();
        }
    }

    void Sprint() {
        if (Input.GetButtonDown("Sprint")) {
            speed *= sprintMod;
        }
        else if (Input.GetButtonUp("Sprint")) {
            speed /= sprintMod;
        }
    }

    void Jump() {
        if (Input.GetButtonDown("Jump") && jumpCount < maxJumps) {
            jumpVelocity.y = jumpSpeed;
            jumpCount++;
        }
    }


    void Shoot()
    {
        FireTimer = 0;

        RaycastHit hit;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, ShootDistance, ~IgnoreLayer))
        {
            IDamage dmg = hit.collider.GetComponent<IDamage>();
            if (dmg != null)
            {
                dmg.TakeDamage(ShootDamage);
            }
        }
    }

    void UpdateHealthBar() {
        float healthDecimal = HP / (float)MaxHP;
        float healthBarPercent = healthDecimal * 500;

        float healthBarPosition = healthBarPercent - 500 + 260;
        // the 260 it to counter the origin

        if (healthBarPosition > 260) healthBarPosition = 260;

        GameManager.instance.HealthBar.transform.position = new Vector3(healthBarPosition, 995, 0);
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

    public void Interact() {
        RaycastHit hit;

        // Dialouge
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, interactDistance, DialougeLayer)) {
            // dialouge
            GameManager.instance.Dialouge();
        }
        // Pickup
        else if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, interactDistance, TapeLayer) && !HaveTape) {
            HaveTape = true;
            GameManager.instance.TapeImage.SetActive(true);

            Destroy(hit.collider.gameObject);
        }
        else if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, interactDistance, CoinLayer)) {
            GameManager.instance.UpdateCoinCount(1);

            Destroy(hit.collider.gameObject);
        }
        else if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, interactDistance, AmmoLayer)) {
            GameManager.instance.UpdateAmmoCount(1);

            Destroy(hit.collider.gameObject);
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

    public IEnumerator Flash(float duration) {
        GameManager.instance.flashRed.SetActive(true);
        yield return new WaitForSeconds(duration);
        GameManager.instance.flashRed.SetActive(false);
    }
}
