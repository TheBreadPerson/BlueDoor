using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public float beforeClipEndTime;
    public Collider collider;
    public MeshRenderer meshRenderer;
    public AudioSource portalSource;
    public AudioClip portalClip, intercomClip;
    public Animator portal;

    private void Start()
    {
        meshRenderer.enabled = false;
        collider.enabled = false;
        portal.enabled = false;
        StartCoroutine(Wait());
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            StartCoroutine(Teleport());
        }
    }

    IEnumerator Teleport()
    {
        yield return new WaitForSeconds(.1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    IEnumerator Wait()
    {
        collider.enabled = false;
        meshRenderer.enabled = false;
        yield return new WaitForSeconds(intercomClip.length - beforeClipEndTime);
        meshRenderer.enabled = true;
        portal.enabled = true;
        collider.enabled = true;
    }
}
