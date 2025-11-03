using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] NavMeshAgent AgentAI;
    [SerializeField] Renderer ModelColor;

    [SerializeField] int HP;

    Color ColorOriginally;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ColorOriginally = ModelColor.material.color;
        //gameManager.instance.updateGameGoal();
    }

    // Update is called once per frame
    void Update()
    {
        //will look for player position and move towards it
        AgentAI.SetDestination(gameManager.instance.player.transform.position); 
    }
}
