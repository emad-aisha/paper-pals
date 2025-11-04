using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] NavMeshAgent AgentAI;
    [SerializeField] Renderer Model;

    [SerializeField] int HP;

    [SerializeField] Transform ShootPos;
    [SerializeField] GameObject Bullet;
    [SerializeField] float ShootRate;


    Color OGColor;

    bool PlayerInTrigger;

    float ShootTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OGColor = Model.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        ShootTimer += Time.deltaTime;

        //if player is in the trigger collider
        if (PlayerInTrigger)
        {
            //will look for player position and move towards it
            AgentAI.SetDestination(gameManager.instance.player.transform.position);

            if (ShootTimer >= ShootRate)
            {
                Shoot();
            }
        }

    }

    public void TakeDamage(int amount)
    {
        HP -= amount;

        if (HP <= 0)
        {
            //will destroy self 
            Destroy(gameObject);
        }
        else
        {
            //else if it survives will flash white
            StartCoroutine(FlashWhite());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            PlayerInTrigger = false;
        }
    }

    IEnumerator FlashWhite()
    {
        Model.material.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        Model.material.color = OGColor;
    }

    void Shoot()
    {
        ShootTimer = 0f;
        
        //Will created an object at the shoot pos
        Instantiate(Bullet, ShootPos.position, transform.rotation);

        Debug.Log("Shoot");
    }
}
