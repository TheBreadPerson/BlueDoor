using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [HideInInspector] public Gun gun;
    float timer;
    [Header("Movement")]
    [Space]
    public float airVelocityCap;
    private float MoveSpeed;
    public float crouchSpeed;
    public float walkSpeed;
    public float sprintSpeed;
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
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("SlowMo")]
    public KeyCode slowKey = KeyCode.B;
    public float timeSlowMo;


    [Header("Keybinds")]
    [Space]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

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
    public Slider healthSlider;
    public Transform orientation;
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
        crouching
    }

    public bool wallrunning;
    public bool crouching;

    // Start is called before the first frame update
    void Start()
    {
        healthSlider.maxValue = Health;
        rb = GetComponent<Rigidbody>();
        ResetJump();
        Time.timeScale = 1f;
        startYScale = transform.localScale.y;
        gun = guns[0].GetComponentInChildren<Gun>();
    }

    // Update is called once per frame
    void Update()
    {
        // WEAPON SWITCHED
        StartCoroutine(GunRustle());
        
        healthSlider.value = Health;
        if(wallrunning)
        {
            state = MovementState.wallrunning;
            MoveSpeed = wallrunSpeed - gun.gunWeight;
        }

        if(Health <= 0f)
        {
            Death();
        }


        SwitchWeapon();
        if (Input.mouseScrollDelta.y > 0f)
        {
            if(gunIndex < guns.Length-1)
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

        if (guns.Length > 0)
        {
            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                gunIndex = 0;
            }
        }

        if (guns.Length > 1)
        {
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                gunIndex = 1;
            }
        }
        if (guns.Length > 2)
        {
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                gunIndex = 2;
            }
        }



        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, ground);

        MyInputs();
        SpeedControl();
        StateHandler();

        if (isGrounded && (horizontal != 0f && vertical != 0f))
            rb.drag = groundDrag;
        else
            rb.drag = 0;

        if(isGrounded && (horizontal == 0f && vertical == 0f))
        {
            rb.drag = standingDrag;
        }

        //FOOTSTEPS

        if(isGrounded && (horizontal != 0f || vertical != 0f) && timer >= footstepFrequency)
        {
            footstepSource.PlayOneShot(footstep);
            timer = 0f;
        }
        if(timer < footstepFrequency)
        {
            timer += Time.deltaTime * 2f;
        }


        if(Input.GetKey(jumpKey) && isGrounded && readyToJump)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

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
            aSource.PlayOneShot(gunRustle);
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
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
        if(isGrounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            MoveSpeed = sprintSpeed - gun.gunWeight;
        }

        else if (crouching)
        {
            state = MovementState.crouching;
            MoveSpeed = crouchSpeed - gun.gunWeight;
        }

        else if(isGrounded)
        {
            state = MovementState.walking;
            MoveSpeed = walkSpeed - gun.gunWeight;
        }

        else
        {
            state = MovementState.air;
        }
    }

    void MovePlayer()
    {
        MoveDirection = orientation.forward * vertical + orientation.right * horizontal;

        if(OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * MoveSpeed * 10f, ForceMode.Force);

            if(rb.velocity.y > 0f)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }

        if(isGrounded)
            rb.AddForce(MoveDirection.normalized * MoveSpeed * 10f, ForceMode.Force);
        else
            rb.AddForce(MoveDirection.normalized * MoveSpeed * 10f * airMultiplier, ForceMode.Force);

        if(!wallrunning) rb.useGravity = !OnSlope();
    }

    void SpeedControl()
    {
        if(OnSlope() && !exitingSlope)
        {
            if(rb.velocity.magnitude > MoveSpeed)
            {
                rb.velocity = rb.velocity.normalized * MoveSpeed;
            }
        }

        else
        {
            Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if ((flatVelocity.magnitude > MoveSpeed) && isGrounded)
            {
                Vector3 limitedVelocity = flatVelocity.normalized * MoveSpeed;
                rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
            }
            else if((flatVelocity.magnitude > airVelocityCap) && !isGrounded)
            {
                Vector3 limitedVelocity = flatVelocity.normalized * MoveSpeed;
                rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
            }
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

    void StartCrouch()
    {
        transform.localScale = new Vector3(transform.localScale.x, crouchHeight, transform.localScale.z);
        rb.AddForce(-transform.up * 5f, ForceMode.Impulse);
    }

    void StopCrouch()
    {
        transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
    }

    bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(MoveDirection, slopeHit.normal).normalized;
    }

    public void Damage(float amt)
    {
        Health -= amt;

        if (grunt.Length > 0)
        {
            int rnd = Random.Range(0, grunt.Length);
            gruntSound.PlayOneShot(grunt[rnd]);
        }
    }

    public void Death()
    {
        deathScreen.SetActive(true);
        Cursor.lockState = CursorLockMode.None; Cursor.visible = true;
        Time.timeScale = 0f;
    }


}
