using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Fracture : MonoBehaviour
{
    Transform[] fractures;
    public float launchForce;

    private void Start()
    {
        fractures = GetComponentsInChildren<Transform>();
    }

    private void OnEnable()
    {
        Explode();
    }

    void Explode()
    {
        foreach(Transform f in fractures)
        {
            float rndX = Random.Range(-launchForce, launchForce);
            float rndY = Random.Range(-launchForce, launchForce);
            Vector3 launchDir = new Vector3(rndX, rndY, 0f);
            if(f.GetComponent<Rigidbody>())
            {
                f.AddComponent<Rigidbody>();
            }
            Rigidbody rb = f.GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * rndX, ForceMode.Impulse);
            rb.AddForce(transform.up * rndY, ForceMode.Impulse);
            Destroy(f.gameObject, 3f);
        }
    }
}
