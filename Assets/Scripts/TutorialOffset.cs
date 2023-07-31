using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialOffset : MonoBehaviour
{
    public GameObject[] intercoms;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartPlaying());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator StartPlaying()
    {
        yield return new WaitForSeconds(1f);
        foreach(GameObject intercom in intercoms)
        {
            intercom.GetComponent<AudioSource>().enabled = true;
        }
    }
}
