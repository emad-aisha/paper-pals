using UnityEngine;

public class JumpHintTrigger : MonoBehaviour 
{
    [SerializeField] GameObject jumpHint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jumpHint.SetActive(true);
            GameManager.instance.StartHideHint(jumpHint);
        }

    }

}
