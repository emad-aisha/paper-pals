using UnityEngine;

public class isGrounded : MonoBehaviour
{
    bool grounded;

    private void Start() {
        grounded = false;
    }

    public bool GetGrounded() { return grounded;}

    private void OnTriggerEnter(Collider other) { 
        if (other != GameManager.instance.player) {
            grounded = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other != GameManager.instance.player) {
            grounded = false;
        }
    }

}
