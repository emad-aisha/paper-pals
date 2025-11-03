using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] NavMeshAgent AgentAI;
    [SerializeField] Renderer Model;

    [SerializeField] int HP;

    Color OGColor;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OGColor = Model.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        //will look for player position and move towards it
        AgentAI.SetDestination(gameManager.instance.player.transform.position); 
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;

        if (HP <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(FlashWhite());
        }
    }

    IEnumerator FlashWhite()
    {
        Model.material.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        Model.material.color = OGColor;
    }

}
