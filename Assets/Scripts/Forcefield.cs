using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forcefield : MonoBehaviour
{
    public float shieldHealth;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(shieldHealth <= 0f)
        {
            Destroy(gameObject);
        }
    }

    public void Damage(float amt)
    {
        shieldHealth -= amt;
    }
}
