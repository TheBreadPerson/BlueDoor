using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AreaEnter : MonoBehaviour
{
    int enemInScene;
    bool enemiesInScene;
    bool checkedEnemy;
    GameObject[] enemies;
    public bool needsAllKilled;
    public string nextSceneName;

    private void Update()
    {
        foreach(Enemy enem in FindObjectsByType<Enemy>(FindObjectsSortMode.None))
        {
            if(enem.isDead && !checkedEnemy)
            {
                enemInScene--;
                checkedEnemy = true;
            }
            if (!enem.isDead && !checkedEnemy)
            {
                enemInScene++;
                checkedEnemy = true;
            }
        }

        if(enemInScene > 0)
        {
            enemiesInScene = true;
        }
        else
        {
            enemiesInScene = false;
        }

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
