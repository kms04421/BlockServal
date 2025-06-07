using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Weapons/Weapon Data")]
public class WeaponData : ScriptableObject
{ 
     [Header("Basic Stats")]
    public string weaponName;
    public Sprite weaponIcon;
    public GameObject weaponPrefab;
    public WeaponType weaponType;

    [Header("Combat Stats")]
    public float damage = 10f;
    public float fireRate = 1f;
    public float range = 100f;
    
    [Header("Effects")]
    public GameObject muzzleFlashPrefab;
    public GameObject hitEffectPrefab;
    public AudioClip fireSound;
    public AudioClip reloadSound;

    [Header("Ammo Stats")]
    public int maxAmmo = 30;
    public float reloadTime =2f;
}
public enum WeaponType
{
    Melee,
    Range,
}

