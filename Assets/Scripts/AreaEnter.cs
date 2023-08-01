using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AreaEnter : MonoBehaviour
{
    int enemInScene;
    bool enemiesInScene;
    bool checkedEnemy;
    public Enemy[] enemies;
    public bool needsAllKilled;
    public string nextSceneName;

    private void Update()
    {
        // Check if any enemy is alive
        foreach(Enemy enem in enemies)
        {
            if(enem.isDead)
            {
                Debug.Log("DEAD");
            }
        }

        enemiesInScene = enemInScene > 0;

        Debug.Log(enemInScene);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(needsAllKilled)
        {
            if (other.transform.CompareTag("Player") && !enemiesInScene)
            {
                SceneManager.LoadScene(nextSceneName);
            }
        }
        else
        {
            if (other.transform.CompareTag("Player"))
            {
                SceneManager.LoadScene(nextSceneName);
            }
        }
        
    }
}
