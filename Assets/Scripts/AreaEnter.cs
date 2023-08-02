using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
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
    private bool LevelEnd = false;
    public GameObject playerCamera;
    public GameObject cameraPos;
    public GameObject blackThing;
    public GameObject doorKnob;
    public float lerpSpeed = 1;

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

        if (LevelEnd == true)
        {
            playerCamera.transform.parent = null;

            if ((playerCamera.transform.position - cameraPos.transform.position).magnitude > 0.01f )
            {
                gameObject.layer = LayerMask.NameToLayer("GunRender");
                doorKnob.layer = LayerMask.NameToLayer("GunRender");
                blackThing.layer = LayerMask.NameToLayer("GunRender");
                playerCamera.transform.position = Vector3.Lerp(playerCamera.transform.position, cameraPos.transform.position, lerpSpeed * Time.deltaTime);
                playerCamera.transform.rotation = Quaternion.Lerp(playerCamera.transform.rotation, cameraPos.transform.rotation, lerpSpeed * Time.deltaTime);
            }
            else
            {
                blackThing.transform.localScale = Vector3.Lerp(blackThing.transform.localScale, new Vector3(10,0,6), (lerpSpeed / 2) * Time.deltaTime);
            }

            //playerCamera.transform.position = cameraPos.transform.position;
            //playerCamera.transform.rotation = cameraPos.transform.rotation;

            if ((blackThing.transform.localScale - new Vector3(10, 0, 6)).magnitude < 1 )
            {
                SceneManager.LoadScene(nextSceneName);
            }
            
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if(needsAllKilled)
        {
            if (other.transform.CompareTag("Player") && !enemiesInScene)
            {

                LevelEnd = true;
            }
        }
        else
        {
            if (other.transform.CompareTag("Player"))
            {
                LevelEnd = true;
            }
        }
    }
}
