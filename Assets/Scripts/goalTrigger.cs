using UnityEngine;

public class goalTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player reached goal!");
            gameManager.instance.winTrophy(1);
        }
    }
  
}
