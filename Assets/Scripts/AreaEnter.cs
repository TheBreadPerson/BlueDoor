using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AreaEnter : MonoBehaviour
{
    bool enemiesInScene;
    GameObject[] enemies;
    public bool needsAllKilled;
    public string nextSceneName;

    private void Update()
    {
        if(GameObject.FindGameObjectsWithTag("Enemy") != null)
        {
            enemies = GameObject.FindGameObjectsWithTag("Enemy");
        }
        if(enemies.Length > 0)
        {
            enemiesInScene = true;
        }
        else
        {
            enemiesInScene = false;
        }

        Debug.Log(enemies.Length);
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
