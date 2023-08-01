using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SlidingMine : MonoBehaviour
{
    float slideF;
    float counterForce;
    public Rigidbody rb;
    float normalDrag;
    float dragSet;
    public bool isSliding;
    public float slidingDrag;
    public float slopeDrag;
    public float slidingForce;
    public float slideFalloff;
    public float velocityToSlide;
    public float velocityToStop;
    public PlayerMovement pm;

    // Start is called before the first frame update
    void Start()
    {
        counterForce = 0f;
        normalDrag = pm.groundDrag;
    }

    // Update is called once per frame
    void Update()
    {
        if(pm.OnSlope())
        {
            dragSet = slopeDrag;
        }
        else
        {
            dragSet = slidingDrag;
        }

        if(rb.velocity.magnitude > velocityToSlide)
        {
            if(pm.crouching && pm.isGrounded)
            {
                StartSliding();
            }
        }

        if(rb.velocity.magnitude != 0f && pm.OnSlope() && pm.crouching)
        {
            StartSliding();
        }

        if(((rb.velocity.magnitude < velocityToStop) || !pm.crouching || !pm.isGrounded) && isSliding)
        {
            StopSlide();
        }

        if(isSliding)
        {
            SlideMovement();
        }
    }

    void StartSliding()
    {
        pm.groundDrag = dragSet;
        isSliding = true;
        slideF = slidingForce;
    }

    void SlideMovement()
    {
        CounterSlide();
        rb.AddForce(pm.orientation.forward * slideF, ForceMode.Force);
        rb.AddForce(-transform.up * 10f, ForceMode.Force);
    }

    void CounterSlide()
    {
        rb.AddForce(-pm.orientation.forward * counterForce, ForceMode.Force);
        counterForce += Time.deltaTime * slideFalloff;
    }

    void StopSlide()
    {
        counterForce = 0f;
        slideF = slidingForce;
        pm.groundDrag = normalDrag;
        isSliding = false;
    }
}
