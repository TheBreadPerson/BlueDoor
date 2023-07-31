using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AreaEnter : MonoBehaviour
{
    public string nextSceneName;
    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.CompareTag("Player"))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
