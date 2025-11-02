using UnityEngine;

public class PlayerController : MonoBehaviour {
    // Unity variables
    
    [SerializeField] CharacterController controller;
    
    // movement
    [SerializeField] int speed;
    [SerializeField] int sprintMod;

    [SerializeField] int jumpSpeed;
    [SerializeField] int maxJumps;
    [SerializeField] int gravity;

    // personal variables
    

    // movement
    Vector3 moveDir;
    Vector3 jumpVelocity;

    int jumpCount;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {

    }

    // Update is called once per frame
    void Update() {
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
}
