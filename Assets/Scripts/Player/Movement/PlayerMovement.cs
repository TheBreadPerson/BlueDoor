using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [HideInInspector] public Gun gun;
    float timer;
    [Header("Movement")]
    [Space]
    public GameObject Cam;
    bool canJump;
    public bool sliding;
    public float acceleration;
    public float velocityCap;
    private float MoveSpeed;
    public float crouchSpeed;
    public float walkSpeed;
    public float slideSpeed;
    float desiredMoveSpeed;
    float lastDesiredMoveSpeed;
    public float sprintSpeed;
    float cyTimer;
    public float coyoteTime;
    private float startYScale;
    public float crouchHeight;
    public AudioClip footstep;
    public float footstepFrequency;
    public AudioSource footstepSource;
    public float wallrunSpeed;
    public float groundDrag;
    public float standingDrag;

    [Header("Jumping")]
    [Space]
    bool doubledJumped;
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Dash")]
    [Space]
    public AudioClip dashClip;
    float dashFov;
    [HideInInspector] public float currentCamFov;
    float dashTimer;
    [HideInInspector] public bool dashed;
    public KeyCode dashKey;
    public float dashForce;
    public float dashDuration;

    [Header("SlowMo")]
    public KeyCode slowKey = KeyCode.B;
    public float timeSlowMo;


    [Header("Keybinds")]
    [Space]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.C;

    [Header("Ground Check")]
    [Space]
    public float playerHeight;
    public LayerMask ground;
    [HideInInspector] public bool isGrounded;

    [Header("Slope Handling")]
    [Space]
    public float maxSlopeAngle;
    RaycastHit slopeHit;
    [HideInInspector] public bool exitingSlope;

    [Header("Gun Switching")]
    [Space]
    float prevGunIndex;
    public KeyCode[] gunKeys;
    public GameObject[] guns;
    public AudioSource aSource;
    public AudioSource gruntSound;
    public AudioClip gunRustle;


    [Header("Main")]
    [Space]
    public bool playerDead;
    public Pause pauseMan;
    public Slider healthSlider;
    public Transform orientation;
    [HideInInspector] public float playerHealth;
    public float Health;
    public GameObject deathScreen;
    float horizontal, vertical;


    [Header("Sound")]
    [Space]
    public AudioClip[] grunt;

    Vector3 MoveDirection;

    Rigidbody rb;
    [HideInInspector] public int gunIndex = 0;

    public MovementState state;
    public enum MovementState
    { 
        walking,
        sprinting,
        air,
        wallrunning,
        crouching,
        sliding
    }

    public bool wallrunning;
    public bool crouching;

    // Start is called before the first frame update
    void Start()
    {
        playerDead = false;
        healthSlider.maxValue = Health;
        rb = GetComponent<Rigidbody>();
        ResetJump();
        Time.timeScale = 1f;
        startYScale = transform.localScale.y;
        gun = guns[0].GetComponentInChildren<Gun>();
        playerHealth = Health;
        currentCamFov = Cam.GetComponent<Camera>().fieldOfView;
        state = MovementState.walking;
        MoveSpeed = walkSpeed;
        desiredMoveSpeed = walkSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(MoveSpeed + " desired " + desiredMoveSpeed + " " + state);
        dashFov = currentCamFov + 20f;
        // HEALTH 
        healthSlider.value = playerHealth;

        if (playerHealth <= 0f)
        {
            Death();
        }

        // WALLRUNNING STATE
        if (wallrunning)
        {
            state = MovementState.wallrunning;
            MoveSpeed = wallrunSpeed;
        }



        // DASH
        if ((!isGrounded && Input.GetKeyDown(dashKey) && !dashed) && !pauseMan.paused)
        {
            dashed = true;
            aSource.PlayOneShot(dashClip, .5f);
        }

        if ((dashTimer < dashDuration) && dashed && !pauseMan.paused && !wallrunning)
        {
            Dash();
        }

        // WEAPON SWITCHING
        if(!pauseMan.paused)
        {
            SwitchWeapon();
            StartCoroutine(GunRustle());
            if (Input.mouseScrollDelta.y > 0f)
            {
                if (gunIndex < guns.Length - 1)
                {
                    gunIndex++;
                }
            }
            if (Input.mouseScrollDelta.y < 0f)
            {
                if (gunIndex > 0)
                {
                    gunIndex--;
                }
            }

            for (int i = 0; i < guns.Length; i++)
            {
                if (Input.GetKeyDown((i + 1).ToString()))
                {
                    gunIndex = i;
                }
            }
        }
        

        // SPEED FOV
        //if(!wallrunning)
        //{
        //    Cam.GetComponent<CameraMove>().DoFov(currentCamFov + rb.velocity.magnitude/5f);
        //    Debug.Log(currentCamFov);
        //}

        if (wallrunning)
        {
            dashed = false;
            dashTimer = 0f;
        }

        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, ground);

        MyInputs();

        StateHandler();

        if (isGrounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;

        //if(isGrounded && (horizontal == 0f && vertical == 0f))
        //{
        //    rb.drag = standingDrag;
        //}

        //FOOTSTEPS

        if (isGrounded && (horizontal != 0f || vertical != 0f) && timer >= footstepFrequency && !sliding)
        {
            footstepSource.PlayOneShot(footstep);
            timer = 0f;
        }
        if(timer < footstepFrequency)
        {
            timer += Time.deltaTime * 2f;
        }

        if(isGrounded)
        {
            doubledJumped = false;
            cyTimer = coyoteTime;
            dashed = false;
            dashTimer = 0f;
        }
        else
        {
            cyTimer -= Time.deltaTime;
        }

        if(Input.GetKey(jumpKey) && cyTimer > 0f && readyToJump)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // DOUBLE JUMP

        //else if(Input.GetKeyDown(jumpKey) && !doubledJumped && !wallrunning)
        //{
        //    doubledJumped = true;
        //    readyToJump = false;
        //    Jump();
        //    Invoke(nameof(ResetJump), jumpCooldown);
        //}

        if (Input.GetKeyDown(crouchKey) && !crouching)
        {
            crouching = true;
            StartCrouch();
        }
        else if (Input.GetKeyUp(crouchKey) && crouching)
        {
            crouching = false;
            StopCrouch();
        }
        
    }

    IEnumerator GunRustle()
    {
        prevGunIndex = gunIndex;
        yield return new WaitForEndOfFrame();
        if (prevGunIndex != gunIndex)
        {
            guns[gunIndex].GetComponentInParent<AudioSource>().PlayOneShot(gunRustle);
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
        SpeedControl();
    }

    void MyInputs()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        
    }

    void SwitchWeapon()
    {
        int i = 0;
        foreach(GameObject gunO in guns)
        {
            if (i == gunIndex)
            {
                gunO.SetActive(true);
                gun = gunO.GetComponentInChildren<Gun>();
            }
            else
            {
                gunO.SetActive(false);
            }
            i++;
        }
    }

    void StateHandler()
    {
        if(sliding)
        {
            state = MovementState.sliding;
            if(OnSlope() && rb.velocity.y < 0.1f)
            {
                desiredMoveSpeed = slideSpeed;
            }
            else
            {
                desiredMoveSpeed = sprintSpeed;
            }
        }

        else if(isGrounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            MoveSpeed = sprintSpeed;
        }

        else if (crouching && !sliding)
        {
            state = MovementState.crouching;
            MoveSpeed = crouchSpeed;
        }

        else if(isGrounded)
        {
            state = MovementState.walking;
            MoveSpeed = walkSpeed;
        }

        else
        {
            state = MovementState.air;
        }

        if (Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 4f && MoveSpeed != 0)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothlyLerpMoveSpeed());
        }

        else
        {
            MoveSpeed = desiredMoveSpeed;
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;
    }

    public IEnumerator SmoothlyLerpMoveSpeed()
    {
        float time = 0;
        float diff = Mathf.Abs(desiredMoveSpeed - MoveSpeed);
        float startV = MoveSpeed;

        while (time < diff)
        {
            MoveSpeed = Mathf.Lerp(startV, desiredMoveSpeed, time/diff);
            time += Time.deltaTime * acceleration;
            yield return null;
        }

        MoveSpeed = desiredMoveSpeed;
    }


    void MovePlayer()
    {
        MoveDirection = orientation.forward * vertical + orientation.right * horizontal;

        if(OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(MoveDirection) * MoveSpeed * 20f, ForceMode.Force);

            if(rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }
        else if (isGrounded)
        {
            rb.AddForce(MoveDirection.normalized * MoveSpeed * (10f), ForceMode.Force);
        }
        else if(!isGrounded)
        {
            rb.AddForce(MoveDirection.normalized * MoveSpeed * (10f) * airMultiplier, ForceMode.Force);
        }

        if (!wallrunning)
        {
            rb.useGravity = !OnSlope();
        }
    }

    void SpeedControl()
    {
        if(OnSlope() && !exitingSlope)
        {
            if(rb.velocity.magnitude > velocityCap)
            {
                rb.velocity = rb.velocity.normalized * velocityCap;
            }
        }

        else
        {
            Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if ((flatVelocity.magnitude > velocityCap))
            {
                Vector3 limitedVelocity = flatVelocity.normalized * velocityCap;
                rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
            }
            //else if((flatVelocity.magnitude > airVelocityCap) && !isGrounded)
            //{
            //    Vector3 limitedVelocity = flatVelocity.normalized * MoveSpeed;
            //    rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
            //}
        }
    }

    void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        exitingSlope = true;
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    void ResetJump()
    {
        readyToJump = true;
        exitingSlope = false;
    }

    void Dash()
    {
        Cam.GetComponent<CameraMove>().DoFov(dashFov);
        rb.AddForce(MoveDirection.normalized * dashForce, ForceMode.Impulse);
        if(dashTimer < dashDuration)
        {
            dashTimer += Time.deltaTime;
        }
        if(dashTimer >= dashDuration)
        {
            Cam.GetComponent<CameraMove>().DoFov(currentCamFov);
        }
    }

    void StartCrouch()
    {
        transform.localScale = new Vector3(transform.localScale.x, crouchHeight, transform.localScale.z);
        rb.AddForce(-transform.up * 5f, ForceMode.Impulse);
    }

    void StopCrouch()
    {
        transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
    }

    public bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

    public void Damage(float amt)
    {
        playerHealth -= amt;

        if (grunt.Length > 0)
        {
            int rnd = Random.Range(0, grunt.Length);
            gruntSound.PlayOneShot(grunt[rnd]);
        }
    }

    public void Death()
    {
        playerDead = true;
        deathScreen.SetActive(true);
        Cursor.lockState = CursorLockMode.None; Cursor.visible = true;
        Time.timeScale = 0f;
    }


}
