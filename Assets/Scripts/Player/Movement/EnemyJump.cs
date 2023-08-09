using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyJump : MonoBehaviour
{

    bool playedReadySound;
    Rigidbody rb;
    Enemy enemyS;
    [Space]
    [Header("Enemy Check")]
    [Space]
    Vector3 enemyDir;
    Collider[] enemyColliders;
    RaycastHit hit;
    GameObject enemy;
    public float checkRadius;
    public float Range;
    public LayerMask enemyLayer;

    [Header("Attack")]
    [Space]
    Vector3 enemyTopPos;
    AudioSource source;
    bool Dash;
    Vector3 latchOffset;
    public KeyCode dashKey = KeyCode.X;
    public AudioClip killSound;
    public AudioClip readySound;
    public float speed;
    public float damage;
    public float bounceForce;

    [Header("Meter")]
    [Space]
    public bool dashed;
    public GameObject xPrompt;
    public GameObject meterGlow;
    public TrailRenderer line;
    public float meterSubtractSlowness;
    public float killsNeeded;
    public float enemiesKilled;
    public bool canDash;
    public Slider meterSlider;

    [Header("Other")]
    [Space]
    public Transform orientation;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        meterGlow.SetActive(canDash);
        if(enemiesKilled > killsNeeded) enemiesKilled = killsNeeded;
        // START DASH TIMER
        if(canDash && dashed)
        {
            canDash = enemiesKilled > 0f;
        }
        else
        {
            canDash = enemiesKilled >= killsNeeded;
        }

        if(canDash && !playedReadySound)
        {
            source.PlayOneShot(readySound);
            playedReadySound = true;
        }

        if (!canDash)
        {
            dashed = false;
            playedReadySound = false;
        }
        meterSlider.value = enemiesKilled/killsNeeded;
        xPrompt.SetActive(canDash && InEnemyRange() && EnemyView());
        line.enabled = canDash;
        if (canDash)
        {
            if(enemiesKilled > 0f && dashed)
            {
                enemiesKilled -= Time.deltaTime/meterSubtractSlowness;
            }
            if (InEnemyRange() && EnemyView())
            {
                if (Input.GetKeyDown(dashKey))
                {
                    Dash = true;
                    dashed = true;
                }
            }

            if (Dash)
            {
                DashPlayer();
            }
        }

        //Debug.Log("Enemy in range " + InEnemyRange() + ", enemy in view " + EnemyView());
    }

    bool InEnemyRange()
    {
        enemyColliders = Physics.OverlapSphere(transform.position, checkRadius, enemyLayer);
        //enemy = enemyColliders.Length > 0 ? enemyColliders[0].transform.gameObject: null;
        foreach (Collider collider in enemyColliders)
        {  
            if(collider.transform.GetComponent<Enemy>())
            {
                if (!collider.transform.GetComponent<Enemy>().isDead)
                {
                    enemy = collider.transform.gameObject;
                }
            }
        }
        if(enemyColliders == null || !enemy)
        {
            return false;
        }
        return enemyColliders.Length > 0f && !enemy.GetComponent<Enemy>().isDead;
    }

    bool EnemyView()
    {
        if(enemy == null)
        {
            return false;
        }
        enemyDir = enemy.transform.position - transform.position;
        return Physics.Raycast(transform.position, enemyDir, out hit, Range, enemyLayer);
    }

    void DashPlayer()
    {
        latchOffset = new Vector3(0f, enemy.transform.localScale.y + 2f, 0f);
        enemyTopPos = enemy.transform.position + latchOffset;
        transform.position = Vector3.Lerp(transform.position, enemyTopPos, Time.deltaTime * speed);

        if(Vector3.Distance(transform.position, enemy.transform.position) < 3.5f)
        {
            Attack();
        }

    }

    void Attack()
    {
        Dash = false;
        if (enemy.GetComponent<Enemy>())
        {
            enemyS = enemy.GetComponent<Enemy>();
            enemyS.Damage(damage);
            source.PlayOneShot(killSound);
            enemy.GetComponent<Rigidbody>().AddForce(orientation.forward * 2f, ForceMode.Impulse);
        }

        Bounce();

        //if (Vector3.Distance(transform.position, enemy.transform.position) < 2f)
        //{
        //    Dash = false;
        //    if (enemy.GetComponent<Enemy>())
        //    {
        //        enemyS = enemy.GetComponent<Enemy>();
        //        enemyS.Damage(damage);
        //        enemy.GetComponent<Rigidbody>().AddForce(transform.forward * 2f, ForceMode.Impulse);
        //    }

        //    Bounce();
        //}
    }

    void Bounce()
    {
        Debug.Log("Bounce");
        rb.AddForce(orientation.forward * bounceForce, ForceMode.Impulse);
        rb.AddForce(Vector3.up * bounceForce, ForceMode.Impulse);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, checkRadius);
        Gizmos.DrawRay(transform.position, enemyDir);
    }
}
