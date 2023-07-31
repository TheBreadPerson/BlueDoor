using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy")]
public class EnemyScriptable : ScriptableObject
{
    public AudioClip gunClip;
    public float fireRate;
    public float damage;
    public float range;
    public float health;
    public float ammo;
    public float reloadTime;
    public float bulletSpeed;
}
