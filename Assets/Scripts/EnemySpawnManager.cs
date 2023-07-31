using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemySpawnManager : MonoBehaviour
{
    float timer;
    GameObject Enemy;
    public GameObject enemyPrefab;
    public GameObject[] enemyTypes;
    public float timeBtwEnemySpawn;
    public Transform player;
    public Transform[] spawnPoints;

    // Start is called before the first frame update
    void Start()
    {
        timer = timeBtwEnemySpawn;
    }

    // Update is called once per frame
    void Update()
    {
        if(timer >= timeBtwEnemySpawn)
        {
            Spawn();
        }
        if(timer <= timeBtwEnemySpawn)
        {
            timer += Time.deltaTime * 2f;
        }
    }

    void Spawn()
    {
        timer = 0f;
        int rnd = Random.Range(0, spawnPoints.Length);
        int enemyRnd = Random.Range(0, enemyTypes.Length);
        Enemy = Instantiate(enemyTypes[enemyRnd], spawnPoints[rnd].position, Quaternion.identity);
        Enemy e = Enemy.GetComponent<Enemy>();
        e.player = player;
    }
}
