using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    public PlayerMovement playerMovement;
    bool hasAmmo;
    bool reloading = false;
    public bool aiming;

    [Header("Gun Type")]
    [Space]
    public bool shootAnim;
    [HideInInspector] public float gunWeight;
    public float playerLaunchForce;
    public Guns gun;
    public AudioSource gunShotSource;
    AudioClip gunClip;
    public AudioClip metalHit, bodyImpact;
    public GameObject BulletHole;
    public ParticleSystem muzzleFlash, bloodSplatter;
    public Animator reloadAnimator;
    float fireCooldown;
    float gunDamage;
    float falloff;
    [HideInInspector] public float clipSize, currentammo, currentMags;
    bool Automatic;
    bool canAds;
    Vector3 recoilOffsetHip;
    Vector3 recoilOffsetAim;
    public Vector3 reloadOffest;
    public float reloadSpeed;
    private float currentCooldown;

    public TextMeshProUGUI magText;
    public TextMeshProUGUI clipText;

    [Header("ADS")]
    [Space]
    private Camera mCam;
    private float normalFov;
    public float zoomAmount;
    //public float currentPOV;
    public float adsSpeed;
    public Transform gunHolder;
    public Transform aimPoint;
    public GameObject mainCam;


    [Header("Raycast")]
    [Space]
    public Transform Barrel;
    public Transform shootPoint;
    public LayerMask Shootable;
    float gunRange;
    private RaycastHit hit;
    AudioSource enemySource;
    public GameObject bloodSplat;

    [Header("Recoil")]
    [Space]
    public Transform camRecoilHolder;
    public float rotationSpeed;
    public float returnSpeed;
    private float crossMultiplier = 1f;
    private float crossAddition;
    private Vector3 currentRotation;
    private Vector3 realRecoil, rRecoilRot;
    private Vector3 Rot;

    [Header("Crosshair")]
    [Space]
    public RawImage crossH;
    public Color enemyColor = Color.red;
    private Color deafult;
    private RaycastHit chHit;

    // Start is called before the first frame update
    void Start()
    {
        currentCooldown = fireCooldown;

        gunRange = gun.range;
        gunClip = gun.gunClip;
        fireCooldown = gun.fireRate;
        gunDamage = gun.gunDamage;
        Automatic = gun.automatic;
        recoilOffsetAim = gun.AimRecoil;
        recoilOffsetHip = gun.Recoil;
        canAds = gun.canADS;
        clipSize = gun.ammo;
        gunWeight = gun.weight;
        currentMags = gun.mags;
        currentammo = gun.ammo;
        falloff = gun.gunFalloff;
        hasAmmo = true;
        mCam = mainCam.GetComponent<Camera>();
        

        deafult = crossH.color;

    }

    private void OnDisable()
    {
        QuickStopAim();
        magText.text = "";
        clipText.text = "";
    }

    private void OnEnable()
    {
        reloading = false;
    }

    // Update is called once per frame
    void Update()
    {
        normalFov = playerMovement.currentCamFov;
        if (currentammo > 0f)
        {
            hasAmmo = true;
        }

        if(currentammo <= 0f)
        {
            hasAmmo = false;
        }

        if(Input.GetKeyDown(KeyCode.R) && currentMags > 0 && !reloading && currentammo < clipSize)
        {
            StartCoroutine(Reload());
        }

        magText.text = currentammo + "";
        clipText.text = "" + clipSize * currentMags;

        currentRotation = Vector3.Lerp(currentRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        Rot = Vector3.Slerp(Rot, currentRotation, rotationSpeed * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(Rot);
        camRecoilHolder.localRotation = Quaternion.Euler(Rot);


        realRecoil = Vector3.Lerp(realRecoil, Vector3.zero, returnSpeed * Time.deltaTime);
        rRecoilRot = Vector3.Slerp(rRecoilRot, realRecoil, rotationSpeed * Time.deltaTime);
        shootPoint.localRotation = Quaternion.Euler(rRecoilRot);


        if (Input.GetMouseButton(1) && canAds && !reloading)
        {
            Aim();
        }
        if (!Input.GetMouseButton(1) & canAds && !reloading)
        {
            StopAim();
        }

        if (!canAds || reloading)
        {
            StopAim();
        }

        if (!playerMovement.pauseMan.paused)
        {
            if (Automatic)
            {
                if (Input.GetMouseButton(0) && hasAmmo && !reloading)
                {
                    if (currentCooldown <= 0f)
                    {
                        Shoot();
                        currentCooldown = fireCooldown;
                    }
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0) && hasAmmo && !reloading)
                {
                    if (currentCooldown <= 0f)
                    {
                        Shoot();
                        currentCooldown = fireCooldown;
                    }
                }
            }
        }
        
        if (!Input.GetMouseButton(0))
        {
            crossMultiplier = 1f;
        }

        if(currentCooldown > 0f)
        {
            currentCooldown -= Time.deltaTime;
        }

        reloadAnimator.SetBool("Reloading", reloading);
        //CheckCrosshair();

        if (shootAnim && Input.GetMouseButtonUp(0))
        {
            reloadAnimator.SetBool("Shoot", false);
        }
    }

    private void Shoot()
    {
        if(shootAnim)
        {
            reloadAnimator.Play("Shoot");
        }
        crossMultiplier = crossH.transform.localScale.x + Time.deltaTime;
        if(!playerMovement.isGrounded)
        {
            playerMovement.transform.GetComponent<Rigidbody>().AddForce(-transform.forward * playerLaunchForce, ForceMode.Impulse);
        }
        
        currentammo -= 1;
        gunShotSource.pitch = Random.Range(.95f, 1.1f);
        gunShotSource.PlayOneShot(gunClip);
        muzzleFlash.Play();

        if(Physics.Raycast(shootPoint.position, shootPoint.TransformDirection(Vector3.forward), out hit, gunRange, Shootable) && !gun.hasSpread)
        {
            float shotDistance = Vector3.Distance(hit.point, transform.position);

            GameObject bulletHole;
            
            

            if (!hit.transform.CompareTag("Enemy"))
            {
                bulletHole = Instantiate(BulletHole, hit.point + (hit.normal * 0.01f), Quaternion.FromToRotation(Vector3.up, hit.normal));
                bulletHole.transform.parent = hit.transform;
                //Destroy(bulletHole, 5f);
                if (hit.transform.GetComponent<Rigidbody>())
                    hit.transform.GetComponent<Rigidbody>().AddForce(Barrel.forward * gun.knockback, ForceMode.Impulse);
            }
            if(hit.transform.CompareTag("Enemy"))
            {
                if (hit.transform.GetComponent<AudioSource>())
                {
                    enemySource = hit.transform.GetComponent<AudioSource>();
                }
                GameObject blood = Instantiate(bloodSplatter.gameObject, hit.point + (hit.normal * 0.01f), Quaternion.FromToRotation(Vector3.up, hit.normal));
                GameObject bloodDecal = Instantiate(bloodSplat, hit.point + (hit.normal * 0.01f), Quaternion.FromToRotation(Vector3.up, hit.normal));
                bloodDecal.transform.parent = hit.transform;
                //if (!hit.transform.GetComponent<AudioSource>())
                //{
                //    blood.AddComponent<AudioSource>().PlayOneShot(bodyImpact);
                //}
                //else
                //{
                //    enemySource.PlayOneShot(bodyImpact);
                //}
                enemySource.PlayOneShot(bodyImpact);
                Destroy(blood, 5f);
                Destroy(bloodDecal, 5f);
                float falloffAmt = (shotDistance * falloff)/2f;
                if(hit.transform.GetComponent<Enemy>())
                {
                    hit.transform.GetComponent<Enemy>().Damage(gunDamage - (shotDistance / falloff));
                    hit.transform.GetComponent<Rigidbody>().AddForce(Barrel.forward * gun.knockback, ForceMode.Impulse);
                } 
            }

        }

        else if(Physics.Raycast(shootPoint.position, shootPoint.TransformDirection(Vector3.forward), out hit, gunRange, Shootable) && gun.hasSpread)
        {
            for(int i = 0; i < gun.sprayRays; i++)
            {
                float rndx = Random.Range(-gun.sprayStrength, gun.sprayStrength);
                float rndy = Random.Range(-gun.sprayStrength, gun.sprayStrength);
                float rndz = Random.Range(-gun.sprayStrength, gun.sprayStrength);
                Vector3 offset = new Vector3(rndx/2f, rndy/2f, rndz/2f);
                Vector3 fwd = Vector3.forward + offset;
                if(Physics.Raycast(shootPoint.position, shootPoint.TransformDirection(new Vector3(fwd.x, fwd.y, fwd.z)), out hit, gunRange, Shootable))
                {
                    float shotDistance = Vector3.Distance(hit.point, transform.position);

                    GameObject bulletHole;


                    if (!hit.transform.CompareTag("Enemy"))
                    {
                        bulletHole = Instantiate(BulletHole, hit.point + (hit.normal * 0.01f), Quaternion.FromToRotation(Vector3.up, hit.normal));
                        bulletHole.transform.parent = hit.transform;
                        //Destroy(bulletHole, 5f);
                        if (hit.transform.GetComponent<Rigidbody>())
                            hit.transform.GetComponent<Rigidbody>().AddForce(Barrel.forward * gun.knockback, ForceMode.Impulse);
                    }
                    if (hit.transform.CompareTag("Enemy"))
                    {
                        if (hit.transform.GetComponent<AudioSource>())
                        {
                            enemySource = hit.transform.GetComponent<AudioSource>();
                        }
                        GameObject blood = Instantiate(bloodSplatter.gameObject, hit.point + (hit.normal * 0.01f), Quaternion.FromToRotation(Vector3.up, hit.normal));
                        GameObject bloodDecal = Instantiate(bloodSplat, hit.point + (hit.normal * 0.01f), Quaternion.FromToRotation(Vector3.up, hit.normal));
                        bloodDecal.transform.parent = hit.transform;
                        //if (!hit.transform.GetComponent<AudioSource>())
                        //{
                        //    blood.AddComponent<AudioSource>().PlayOneShot(bodyImpact);
                        //}
                        //else
                        //{
                        //    enemySource.PlayOneShot(bodyImpact);
                        //}
                        enemySource.PlayOneShot(bodyImpact);
                        Destroy(blood, 5f);
                        Destroy(bloodDecal, 5f);
                        float falloffAmt = (shotDistance * falloff) / 2f;
                        hit.transform.GetComponent<Enemy>().Damage(gunDamage - (shotDistance / falloff));
                        hit.transform.GetComponent<Rigidbody>().AddForce(Barrel.forward * gun.knockback, ForceMode.Impulse);

                    }
                }
            }
        }

        if (aiming)
        {
            currentRotation += new Vector3(-recoilOffsetAim.x, Random.Range(-recoilOffsetAim.y, recoilOffsetAim.y), Random.Range(-recoilOffsetAim.z, recoilOffsetAim.z));
            realRecoil += new Vector3(-recoilOffsetAim.x, Random.Range(-recoilOffsetAim.y, recoilOffsetAim.y), Random.Range(-recoilOffsetAim.z, recoilOffsetAim.z));
        }
        else
        {
            currentRotation += new Vector3(-recoilOffsetHip.x, Random.Range(-recoilOffsetHip.y, recoilOffsetHip.y), Random.Range(-recoilOffsetHip.z, recoilOffsetHip.z));
            realRecoil += new Vector3(-recoilOffsetHip.x, Random.Range(-recoilOffsetHip.y, recoilOffsetHip.y), Random.Range(-recoilOffsetHip.z, recoilOffsetHip.z));
        }

    }


    void Aim()
    {
        aiming = true;
        mCam.fieldOfView = Mathf.Lerp(mCam.fieldOfView, zoomAmount, adsSpeed * Time.deltaTime);
        gunHolder.localPosition = Vector3.Lerp(gunHolder.localPosition, aimPoint.localPosition, adsSpeed * Time.deltaTime);
    }
    void StopAim()
    {
        aiming = false;
        mCam.fieldOfView = Mathf.Lerp(mCam.fieldOfView, normalFov, adsSpeed * Time.deltaTime);
        gunHolder.localPosition = Vector3.Lerp(gunHolder.localPosition, Vector3.zero, adsSpeed * Time.deltaTime);
    }

    void QuickStopAim()
    {
        aiming = false;
        mCam.fieldOfView = normalFov;
        gunHolder.localPosition = Vector3.zero;
    }

    IEnumerator Reload()
    {
        reloading = true;
        gunShotSource.PlayOneShot(gun.reloadSound);
        yield return new WaitForSeconds(gun.reloadTime);
        reloading = false;
        hasAmmo = true;
        currentammo = clipSize;
        currentMags -= 1;
    }

    void CheckCrosshair()
    {
        crossH.transform.localScale = new Vector3(crossMultiplier, crossMultiplier, crossMultiplier);

        if(crossMultiplier <= 1f)
        {
            crossMultiplier = 1f;
        }
        
        if (Physics.Raycast(shootPoint.position, shootPoint.TransformDirection(Vector3.forward), out chHit, gunRange, Shootable))
        {
            if (chHit.transform.CompareTag("Enemy"))
            {
                crossH.color = enemyColor;
            }
            else
            {
                crossH.color = deafult;
            }
        }
        else
        {
            crossH.color = deafult;
        }
    }
}
