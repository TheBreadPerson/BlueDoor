using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    bool reloadSound = false;
    public GameObject fractured;
    public GameObject normal;
    Animator anim;
    public bool swapDeadBody;
    bool seperatedGun;
    Transform enemyGun;
    EnemyJump enemyJump;
    float reloadTimer;
    bool hasAmmo, reloading;
    [Header("Enemy Settings")]
    bool canShoot, seePlayer;
    [HideInInspector] public bool isDead;
    Vector3 playerDir;
    [Header("Enemy Type")]
    [Space]
    public EnemyScriptable enemy;
    NavMeshAgent agent;
    RaycastHit hit, shotHit, moveHit;
    PlayerMovement playerM;
    public AudioSource enemyShot;
    AudioClip gunS, reloadS;
    public ParticleSystem enemyFlash;
    float timer;
    float maxAmmo;
    float currentAmmo;
    float reloadTime;
    public GameObject projectilePrefab;
    public float shootRange, noticeRange, bulletSpeed, stopDistance;
    float shotDamage, Health, shotCooldown;
    public LayerMask playerLayer;
    public Transform player;
    public GameObject ammoPickup, healthPickup;
    public float ammoOdds, healthOdds;
    // Start is called before the first frame update
    void Start()
    {
        playerM = player.GetComponent<PlayerMovement>();
        agent = GetComponent<NavMeshAgent>();
        enemyGun = transform.GetChild(0);
        shotDamage = enemy.damage;
        Health = enemy.health;
        shotCooldown = enemy.fireRate;
        gunS = enemy.gunClip;
        reloadS = enemy.reloadSound;
        shootRange = enemy.range;
        maxAmmo = enemy.ammo;
        reloadTime = enemy.reloadTime;
        currentAmmo = maxAmmo;
        bulletSpeed = enemy.bulletSpeed;
        anim = GetComponent<Animator>();
        enemyJump = player.GetComponent<EnemyJump>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Health <= 0f)
        {
            Death();
        }

        float distance = Vector3.Distance(transform.position, player.position);
        playerDir = player.position - transform.position;
        if (seePlayer && !isDead)
        {
            transform.LookAt(player.position);
            if (distance >= stopDistance)
            {
                agent.isStopped = false;
                agent.SetDestination(player.position);
                anim.SetBool("Walking", true);
            }
            else
            {
                agent.isStopped = true;
                anim.SetBool("Walking", false);
            }
        }

        //if (!seePlayer)
        //{
        //    timer = 0f;
        //}
        if (timer >= shotCooldown)
        {
            canShoot = true;
        }
        if (timer <= shotCooldown)
        {
            timer += Time.deltaTime * 2f;
            canShoot = false;
        }

        if (Physics.Raycast(transform.position, playerDir, out hit, shootRange, playerLayer) && canShoot && !isDead)
        {
            if (hit.transform.CompareTag("Player") && hasAmmo && !playerM.playerDead)
            {
                Shoot();
            }
            Physics.Raycast(transform.position, playerDir, out moveHit, noticeRange, playerLayer);
            if (moveHit.transform != null)
            {
                seePlayer = moveHit.transform.CompareTag("Player");
            }

            if (currentAmmo <= 0f)
            {
                StartCoroutine(EnemyReload());
            }

            hasAmmo = currentAmmo > 0f;
        }
    }

    // AI CODE
    void Shoot()
    {
        currentAmmo -= 1;
        enemyFlash.Play();
        enemyShot.PlayOneShot(gunS);
        timer = 0f;
        // Calculate the player's predicted position
        Vector3 playerVelocity = player.GetComponent<Rigidbody>().velocity;
        //float distanceFromPlayer = Vector3.Distance(transform.position, player.position);
        float timeToReachPlayer = Vector3.Distance(transform.position, player.position) / shootRange;
        Vector3 predictedPosition = player.position + playerVelocity / 3f * timeToReachPlayer;

        Vector3 dir = predictedPosition - transform.position;

        //if (Physics.Raycast(transform.position, dir, out shotHit, shootRange, playerLayer))
        //{
        //    shotHit.transform.gameObject.GetComponent<PlayerMovement>().Damage(shotDamage);
        //}
        FireProjectile(predictedPosition);
    }

    void FireProjectile(Vector3 targetPosition)
    {
        // Instantiate and launch the projectile towards the target position
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.transform.GetComponent<Bullet>().bulletDamage = enemy.damage;
        Rigidbody projectileRigidbody = projectile.GetComponent<Rigidbody>();

        // Calculate the direction towards the target position
        Vector3 direction = (targetPosition - transform.position).normalized;

        // Launch the projectile towards the target position
        projectileRigidbody.velocity = direction * bulletSpeed;
        Destroy(projectile, 5f);
    }

    // AI CODE

    public void Damage(float dmg)
    {
        Health -= dmg;
    }

    public void DropPickups()
    {
        int ammoRandom = Random.Range(0, 100);
        int healthRandom = Random.Range(0, 100);
        if(ammoRandom <= ammoOdds)
        {
            Instantiate(ammoPickup, transform.position, Quaternion.identity);
            ammoPickup.GetComponent<Pickup>().magsGained = 2f;
        }
        if(healthRandom <= healthOdds)
        {
            Instantiate(healthPickup, transform.position, Quaternion.identity);
            healthPickup.GetComponent<Pickup>().healthAmount = 5f;
        }
        
    }
    public void Death()
    {
        if(swapDeadBody && fractured != null && normal != null)
        {
            normal.SetActive(false);
            fractured.SetActive(true);
        }
        if (!isDead && !enemyJump.dashed) enemyJump.enemiesKilled++;
        else if (!isDead && !enemyJump.dashed) enemyJump.enemiesKilled -= enemy.jumpCost;
        if (enemyGun != null && !seperatedGun)
        {
            DropPickups();
            enemyGun.gameObject.AddComponent<MeshCollider>().convex = true;
            enemyGun.gameObject.AddComponent<Rigidbody>().AddForce(Vector3.up * 5f, ForceMode.Impulse);
            enemyGun.parent = null;
            Destroy(enemyGun.gameObject, 5f);
            seperatedGun = true;
        }
        isDead = true;
        agent.enabled = false;
        transform.GetComponent<Rigidbody>().freezeRotation = false;
        StartCoroutine(EnemyDissolve());
        //Destroy(gameObject, 3f);
    }

    IEnumerator EnemyDissolve()
    {
        yield return new WaitForSeconds(10f);
        anim.Play("Dissolve");
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }


    IEnumerator EnemyReload()
    {
        reloading = true;
        if(!reloadSound)
        {
            enemyShot.PlayOneShot(reloadS);
            reloadSound = true;
        }
        yield return new WaitForSeconds(reloadTime);
        reloadSound = false;
        reloading = false;
        currentAmmo = maxAmmo;
    }


}