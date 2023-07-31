using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletDamage = 8f;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(other.GetComponentInParent<PlayerMovement>())
            {
                other.GetComponentInParent<PlayerMovement>().Damage(bulletDamage);
            }
            Destroy(gameObject);
        }

        if(!other.CompareTag("Enemy") && !other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
