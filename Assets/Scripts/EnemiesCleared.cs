using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesCleared : MonoBehaviour
{
    GameObject[] enemies;
    public GameObject portal;
    public bool killedEnemies;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if(enemies.Length <= 0f)
        {
            Win();
        }
    }

    public void Win()
    {
        killedEnemies = true;
        portal.SetActive(true);
    }
}
