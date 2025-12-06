using UnityEngine;

public class BatSwoopTrigger : MonoBehaviour
{
    public EnemyAI batAI; // Drag the parent EnemyAI in the inspector

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!batAI.isSwooping) // optional check if you make a getter
            {
                batAI.StartCoroutine(batAI.SwoopAttack());
            }
        }
    }
}