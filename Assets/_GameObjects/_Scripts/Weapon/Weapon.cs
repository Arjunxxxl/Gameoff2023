using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Weapon : MonoBehaviour
{
    [SerializeField] protected WeaponType weaponType;

    [Header("Weapon Parent")]
    [SerializeField] protected Transform weaponParentT;
    [SerializeField] protected Player player;

    [Header("Child Transform")]
    [SerializeField] private Transform weaponMesh;
    [SerializeField] private Vector3 meshLocalPos;
    [SerializeField] private Vector3 meshLocalRot;

    [Header("Equip Data")]
    [SerializeField] private int equipLayerMask;
    [SerializeField] private int unEquipLayerMask;
    [SerializeField] private BoxCollider detectionCollider;
    [SerializeField] private BoxCollider collisionCollider;
    [SerializeField] private Rigidbody collisionRigidbody;

    [Header("Ammo Data")]
    [SerializeField] private bool isNoAmmoLeftInGun;
    [SerializeField] private int carryingAmmo;
    [SerializeField] private int ammoLeftInGun;
    [SerializeField] private int maxAmmoInGun;
    [SerializeField] private float reloadTime;
    [SerializeField] private bool isReloading;

    [Header("Attack Time Data")]
    [SerializeField] protected bool userAttackInput;
    [SerializeField] protected bool isAttacking;
    [SerializeField] protected bool attacked;
    [SerializeField] protected float attackDelay;
    [SerializeField] protected float nextAttackDelay;
    [SerializeField] protected float attackTimeElapsed;

    [Header("Gun shot particle")]
    [SerializeField] protected Transform gunshotParticleSpawnT;
    [SerializeField] protected Transform shellEjectParticleSpawnT;

    [Header("Enemies")]

    [Header("Tween util")]
    [SerializeField] protected TweenUtil tweenUtil;

    public static Action<int, bool> UpdateCurrentAmmo;
    public static Action<int, bool> UpdateCarryingAmmo;

    protected ObjectPooler objectPooler;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        objectPooler = ObjectPooler.Instance;

        SetUp();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        UpdateAtackingTime();
    }

    #region Setup/Equip/Unequip
    public void SetUp()
    {
        detectionCollider.enabled = true;
        collisionCollider.enabled = true;
        collisionRigidbody.isKinematic = false;

        attackTimeElapsed = 0;
        attacked = false;
        isAttacking = false;

        SetUnequipLayer();
        SetUpAmmo();
    }

    public void Equip(Player player)
    {
        this.player = player;

        detectionCollider.enabled = false;
        collisionCollider.enabled = false;
        collisionRigidbody.isKinematic = true;

        weaponMesh.transform.localPosition = meshLocalPos;
        weaponMesh.transform.localRotation = Quaternion.Euler(meshLocalRot);

        attackTimeElapsed = 0;
        attacked = false;
        isAttacking = false;

        SetEquipLayer();
    }

    public void Unequip(Vector3 throwForce)
    {
        detectionCollider.enabled = true;
        collisionCollider.enabled = true;
        collisionRigidbody.isKinematic = false;

        collisionRigidbody.AddForce(throwForce, ForceMode.VelocityChange);
        collisionRigidbody.angularVelocity = throwForce * Random.Range(-0.5f, 0.5f);

        attackTimeElapsed = 0;
        attacked = false;
        isAttacking = false;

        if(isReloading)
        {
            isReloading = false;
            StopCoroutine("Reload");
        }

        StartCoroutine(ChangeLayerAfterEnquiping());
    }

    IEnumerator ChangeLayerAfterEnquiping()
    {
        yield return new WaitForSecondsRealtime(2f);

        SetUnequipLayer();
    }

    private void SetEquipLayer()
    {
        gameObject.layer = equipLayerMask;

        List<Transform> allchilds = GetAllNestedChilds(transform);

        for (int i = 0; i < allchilds.Count; i++)
        {
            allchilds[i].gameObject.layer = equipLayerMask;
        }
    }

    private void SetUnequipLayer()
    {
        gameObject.layer = unEquipLayerMask;

        List<Transform> allchilds = GetAllNestedChilds(transform);

        for (int i = 0; i < allchilds.Count; i++)
        {
            allchilds[i].gameObject.layer = unEquipLayerMask;
        }
    }

    private List<Transform> GetAllNestedChilds(Transform t)
    {
        List<Transform> childs = new List<Transform>();

        for (int i = 0; i < t.childCount; i++)
        {
            childs.Add(t.GetChild(i));

            List<Transform> tempChild = GetAllNestedChilds(t.GetChild(i));

            for (int j = 0; j < tempChild.Count; j++)
            {
                childs.Add(tempChild[j]);
            }
        }

        return childs;
    }
    #endregion

    #region Weapon Type / Transfrom
    public WeaponType GetWeaponType()
    {
        return weaponType;
    }

    public void SetNewParent(Transform parent)
    {
        weaponParentT = parent;
        transform.SetParent(parent);
    }
    #endregion

    #region Attack
    public virtual void ActivateWeapon(bool userAttackInput)
    {
        this.userAttackInput = userAttackInput;

        if(this.userAttackInput && ((IsAmmoWeapon() && IsAmmoLeftInGun() && !isReloading) || !IsAmmoWeapon()))
        {
            InvokeBeforeAttack();
            isAttacking = true;
        }
    }

    protected virtual void UpdateAtackingTime()
    {
        if (!isAttacking)
        {
            return;
        }

        attackTimeElapsed += Time.unscaledDeltaTime;

        if(attackTimeElapsed > attackDelay)
        {
            if (!attacked)
            {
                attacked = true;

                if (IsAmmoWeapon() && IsAmmoLeftInGun())
                {
                    UseAmmo();
                }

                InvokeAttack();
            }
        }

        if (attackTimeElapsed > nextAttackDelay)
        {
            attackTimeElapsed = 0;

            attacked = false;
            isAttacking = false;

            InvokeAfterAttack();
        }
    }

    private void InvokeBeforeAttack()
    {
        switch (weaponType)
        {
            case WeaponType.Rifle:
                tweenUtil.PlayTween("Recoil Move Pullback");
                GameplayMenu.EnableShootMarker?.Invoke(true);
                break;
        }
    }

    private void InvokeAttack()
    {
        switch(weaponType)
        {
            case WeaponType.Rifle:
                GetComponent<Rifle>().SpawnBullet();
                break;
        }
    }

    private void InvokeAfterAttack()
    {
        switch (weaponType)
        {
            case WeaponType.Rifle:
                break;
        }
    }
    #endregion

    #region Ammo
    private void SetUpAmmo()
    {
        if(IsAmmoWeapon())
        {
            ammoLeftInGun = maxAmmoInGun;
            carryingAmmo = 90;

            UpdateCurrentAmmo?.Invoke(ammoLeftInGun, false);
            UpdateCarryingAmmo?.Invoke(carryingAmmo, false);
        }
    }

    private bool IsAmmoWeapon()
    {
        if(weaponType == WeaponType.Rifle)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool IsAmmoLeftInGun()
    {
        isNoAmmoLeftInGun = ammoLeftInGun <= 0;

        return !isNoAmmoLeftInGun;
    }

    private void UseAmmo()
    {
        ammoLeftInGun--;
        UpdateCurrentAmmo?.Invoke(ammoLeftInGun, ammoLeftInGun <= maxAmmoInGun / 3);
    }

    public void TryReloading(bool reload)
    {
        if(!reload || isReloading || carryingAmmo <= 0 || ammoLeftInGun == maxAmmoInGun)
        {
            return;
        }

        isReloading = true;
        tweenUtil.StopAllTween();
        tweenUtil.PlayTween("Reload");

        StartCoroutine(Reload());
    }

    IEnumerator Reload()
    {
        yield return new WaitForSecondsRealtime(reloadTime);

        int ammoRequire = maxAmmoInGun - ammoLeftInGun;

        ammoRequire = ammoRequire <= carryingAmmo ? ammoRequire : carryingAmmo;

        ammoLeftInGun += ammoRequire;
        carryingAmmo -= ammoRequire;

        UpdateCarryingAmmo?.Invoke(carryingAmmo, carryingAmmo <= maxAmmoInGun);
        UpdateCurrentAmmo?.Invoke(ammoLeftInGun, ammoLeftInGun <= maxAmmoInGun / 3);

        isReloading = false;
    }

    private void AmmoPickedUp(int amount)
    {
        carryingAmmo += amount;
    }
    #endregion
}
