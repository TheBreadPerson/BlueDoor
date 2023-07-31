using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    AudioSource source;
    public AudioClip clip;
    public bool AmmoP;
    public bool HealthP;
    public float magsGained = 5f;
    public float healthAmount = 10f;


    private void Start()
    {
        source = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            
            if (AmmoP)
            {
                Ammo(other);
            }
            if(HealthP)
            {
                Health(other);
            }
        }
    }

    void Ammo(Collider other)
    {
        if (other.GetComponentInParent<PlayerMovement>().gun != null)
        {
            other.GetComponent<AudioSource>().PlayOneShot(clip);
            other.GetComponentInParent<PlayerMovement>().gun.currentMags += magsGained;
            Destroy(gameObject);
        }
    }

    void Health(Collider other)
    {
        if (other.GetComponentInParent<PlayerMovement>() != null)
        {
            if (other.GetComponentInParent<PlayerMovement>().playerHealth < other.GetComponentInParent<PlayerMovement>().Health)
            {
                other.GetComponent<AudioSource>().PlayOneShot(clip);
                other.GetComponentInParent<PlayerMovement>().playerHealth += healthAmount;
                Destroy(gameObject);
            }
        }
    }
}
