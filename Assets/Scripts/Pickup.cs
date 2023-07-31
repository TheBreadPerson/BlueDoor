using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    AudioSource source;
    public AudioClip clip;
    public bool AmmoP;
    public bool HealthP;
    public float healthAmount = 10f;


    private void Start()
    {
        source = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            other.GetComponent<AudioSource>().PlayOneShot(clip);
            if (AmmoP)
            {
                Ammo(other);
            }
            if(HealthP)
            {
                source.Play();
                Health(other);
            }
        }
    }

    void Ammo(Collider other)
    {
        if (other.GetComponentInParent<PlayerMovement>().gun != null)
        {
            other.GetComponentInParent<PlayerMovement>().gun.currentMags += 5;
            Destroy(gameObject);
        }
    }

    void Health(Collider other)
    {
        if (other.GetComponentInParent<PlayerMovement>() != null)
        {
            other.GetComponentInParent<PlayerMovement>().Health += healthAmount;
            Destroy(gameObject);
        }
    }
}
