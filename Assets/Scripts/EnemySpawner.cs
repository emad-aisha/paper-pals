using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner")]
    [SerializeField] GameObject ObjectToSpawn;
    [SerializeField] Transform[] SpawnPositions;
    [SerializeField] int SpawnAmount;
    [SerializeField] int SpawnRate;

    int SpawnCount;
    float SpawnTimer;
    bool StartSpawning;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Code here, if we wanted to count how many enemies there are...
        //SpawnCount
    }

    // Update is called once per frame
    void Update()
    {
        if (StartSpawning)
        {
            //starts the timer
            SpawnTimer = Time.deltaTime;

            if (SpawnCount < SpawnAmount && SpawnTimer >= SpawnRate)
            {
                Spawn();
            }
        }

    }


    private void OnTriggerEnter(Collider other)
    {
        //when the "Player" enters the collider
        if (other.CompareTag("Player"))
        {
            StartSpawning = true;
        }
    }

    void Spawn()
    {
        //spawns the enemies in on certain positions
        Instantiate(ObjectToSpawn, SpawnPositions[Random.Range(0,
            SpawnPositions.Length)].transform.position, Quaternion.identity);
        
        //Keeps track of how many enemies that spawns in
        SpawnCount++;

        //resets the timer when an enemy spawns in
        SpawnTimer = 0;
    }

}
