using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Gun", menuName = "Guns/Gun")]
public class Guns : ScriptableObject
{
    public AudioClip gunClip;
    public AudioClip reloadSound;
    public float fireRate;
    public float gunDamage;
    public float knockback;
    public float ammo;
    public float range;
    public float gunFalloff;
    public float reloadTime;
    public float sprayRays;
    public float sprayStrength;
    public float weight;
    public int mags;
    public bool automatic;
    public bool canADS;
    public Vector3 Recoil;
    public Vector3 AimRecoil;
    public bool hasSpread;
}
