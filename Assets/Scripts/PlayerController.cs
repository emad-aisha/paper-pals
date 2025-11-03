using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    // Unity variables
    
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask IgnoreLayer;

    [SerializeField] int HP;

    // movement
    [SerializeField] int speed;
    [SerializeField] int sprintMod;

    [SerializeField] int jumpSpeed;
    [SerializeField] int maxJumps;
    [SerializeField] int gravity;

    // shooting
    [SerializeField] int ShootDamage;
    [SerializeField] int ShootDistance;
    [SerializeField] float FireRate;


    // Personal Variables

    // movement
    Vector3 moveDir;
    Vector3 jumpVelocity;

    int jumpCount;
    float FireTimer;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * ShootDistance, Color.blue);
        FireTimer += Time.deltaTime;
        Movement();
        Sprint();
    }

    void Movement() {
        // jump physics
        // TODO: add acceleration to the fall
        if (controller.isGrounded) {
            jumpVelocity = Vector3.zero;
            jumpCount = 0;
        }
        else {
            jumpVelocity.y -= (gravity * Time.deltaTime);
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

            Debug.Log(hit.collider.name);

            IDamage dmg = hit.collider.GetComponent<IDamage>();
            if (dmg != null)
            {
                dmg.TakeDamage(ShootDamage);
            }
        }
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;

        if (HP <= 0)
        {
            gameManager.instance.Defeat();
        }
    }

}
