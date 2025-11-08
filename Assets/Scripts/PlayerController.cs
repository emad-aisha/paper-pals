using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour, IInteractable, IDamage
{
    // Unity variables
    
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask IgnoreLayer;

    [SerializeField] LayerMask DialougeLayer;
    [SerializeField] LayerMask TapeLayer;
    [SerializeField] LayerMask AmmoLayer;
    [SerializeField] LayerMask CoinLayer;
    [SerializeField] int interactDistance;

    [SerializeField] int HP;

    // movement
    [SerializeField] int speed;
    [SerializeField] int sprintMod;

    [SerializeField] int jumpSpeed;
    [SerializeField] int maxJumps;
    [SerializeField] float gravity;

    // shooting
    [SerializeField] int ShootDamage;
    [SerializeField] int ShootDistance;
    [SerializeField] float FireRate;

    [SerializeField] int healAmount;


    // Personal Variables

    // movement
    Vector3 moveDir;
    Vector3 jumpVelocity;
    int OGGravity;
    float maxGravity;

    bool HaveTape;

    int jumpCount;
    float FireTimer;

    int MaxHP;
    int OGSpeed;

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
            && gameManager.instance.isPaused == false) {

            if (hit.collider.gameObject.layer == 7) gameManager.instance.InteractOn();
            else if (gameManager.instance.isInteractOn) gameManager.instance.InteractOff();

        }
        else if (hit.collider == null && gameManager.instance.isPaused == false) {
            gameManager.instance.InteractOff();
        }

        // TODO: update heal amount with health amount
        if (Input.GetButtonDown("Heal") && HaveTape && HP < MaxHP) {
            Heal(healAmount);
            HaveTape = false;
            gameManager.instance.TapeImage.SetActive(false);
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
            if (gravity < maxGravity) gravity = gravity * 1.005f;
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
        if (Input.GetButtonDown("Jump") && jumpCount <= maxJumps) {
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

        float healthBarPosition = healthBarPercent - 500 + 280;
        // the 280 it to counter the origin

        if (healthBarPosition > 280) healthBarPosition = 280;

        gameManager.instance.HealthBar.transform.position = new Vector3(healthBarPosition, 1000, 0);
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;

        StartCoroutine(Flash(0.1f));

        UpdateHealthBar();

        if (HP <= 0)
        {
            gameManager.instance.Defeat();
        }
    }

    public void Interact() {
        RaycastHit hit;

        // Dialouge
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, interactDistance, DialougeLayer)) {
            // dialouge
            gameManager.instance.Dialouge();
        }
        // Pickup
        else if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, interactDistance, TapeLayer) && !HaveTape) {
            HaveTape = true;
            gameManager.instance.TapeImage.SetActive(true);

            Destroy(hit.collider.gameObject);
        }
        else if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, interactDistance, CoinLayer)) {
            gameManager.instance.UpdateCoinCount(1);

            Destroy(hit.collider.gameObject);
        }
        else if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, interactDistance, AmmoLayer)) {
            gameManager.instance.UpdateAmmoCount(1);

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
        gameManager.instance.flashRed.SetActive(true);
        yield return new WaitForSeconds(duration);
        gameManager.instance.flashRed.SetActive(false);
    }
}
