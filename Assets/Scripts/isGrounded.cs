using UnityEngine;

public class isGrounded : MonoBehaviour
{
    bool grounded;

    private void Start() {
        grounded = false;
    }
    public bool GetGrounded() { return grounded;}

    private void OnTriggerStay(Collider other) {
        if (other.name != "Player" && other.isTrigger == false)
            grounded = true;
    }

    private void OnTriggerExit(Collider other) {
        if (other.name != "Player" && other.isTrigger == false) {
            grounded = false;
        }
    }

}
