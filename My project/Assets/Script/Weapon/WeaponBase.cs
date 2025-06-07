using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour, IUsable
{
    [SerializeField] protected WeaponData weaponData; // 무기 데이터
    protected int currentAmmo;
    protected bool isReloading;
    protected Camera mainCamera;
    protected virtual void Start()
    {
        currentAmmo = weaponData.maxAmmo;
        mainCamera = Camera.main;
    }

    // 반드시 구현해야 하는 기능
    public abstract void Attack();

    public virtual void Reload()
    {
        if(isReloading)
        {
            StartCoroutine(ReloadRoutine());
        }
    }
    protected virtual IEnumerator ReloadRoutine()
    {
        isReloading = true; 
        yield return new WaitForSeconds(weaponData.reloadTime);
        currentAmmo = weaponData.maxAmmo;
        isReloading = false;
    }

    public float GetDamage()
    {
        return weaponData.damage;
    }
}
