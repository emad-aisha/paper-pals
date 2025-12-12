using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner")]
    [SerializeField] GameObject ObjectToSpawn;
    [SerializeField] Transform[] SpawnPositions;
     int SpawnAmount;
    [SerializeField] float SpawnRate;

    int SpawnCount;
    float SpawnTimer;
    bool StartSpawning;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Code here, if we wanted to count how many enemies there are...
        //SpawnCount

        // thank you so much for these comments mat ily

        // please get a room aisha and mat, thank you... - marcellus

        // what the kaka - a
        //GameManager.instance.gameGoalCount = SpawnAmount;
        SpawnAmount = SpawnPositions.Length - 1;
        SpawnCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (StartSpawning)
        {
            //starts the timer
            SpawnTimer += Time.deltaTime;

            if (SpawnCount < SpawnAmount && SpawnTimer >= SpawnRate)
            {
                Spawn();
                SpawnTimer = 0;
            }
            else if (SpawnCount >= SpawnAmount) {
                StartSpawning = false;
            }
        }
        else if (!StartSpawning && SpawnCount >= SpawnAmount) {
            Destroy(this.gameObject);
        }

    }


    private void OnTriggerEnter(Collider other)
    {
        StartSpawning = true;
    }

    void Spawn()
    {
        //spawns the enemies in on certain positions
        Instantiate(ObjectToSpawn, SpawnPositions[SpawnCount].transform.position, Quaternion.identity);
        
        //Keeps track of how many enemies that spawns in
        SpawnCount++;

        //resets the timer when an enemy spawns in
        SpawnTimer = 0;
    }

}
