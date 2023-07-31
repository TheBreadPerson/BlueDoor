using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Bouncepad : MonoBehaviour
{
    AudioSource source;
    public float launchForce;

    private void Start()
    {
        source = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.GetComponent<Rigidbody>())
        {
            if(collision.transform.GetComponent<PlayerMovement>())
            {
                collision.transform.GetComponent<PlayerMovement>().exitingSlope = true;
            }

            Rigidbody rb = collision.transform.GetComponent<Rigidbody>();
            rb.AddForce(-collision.GetContact(0).normal * launchForce, ForceMode.Impulse);
            if(source != null) source.Play();
        }
    }
}
