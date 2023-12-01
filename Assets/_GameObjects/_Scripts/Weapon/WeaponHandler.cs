using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    [System.Serializable]
    private class WeaponTransformData
    {
        public WeaponType weaponType;
        [Space]
        public Vector3 equipLocalPos;
        public Vector3 equipLocalRot;
        [Space]
        public Vector3 unequipLocalPos;
        public Vector3 unequipLocalRot;
        [Space]
        public Vector3 unEquipingThrowForce;
    }

    [Header("Weapon Carrying Data")]
    [SerializeField] private WeaponType currentEquipedWeaponType;
    [SerializeField] private Weapon currentEquipedWeapon;
    [SerializeField] private List<Weapon> carryingWeapons;

    [Header("Weapon Equip/Unequip Data")]
    [SerializeField] private Transform weaponEquipParent;
    [SerializeField] private Transform weaponUnequipParent;

    [Header("Weapon Transfrom Data")]
    [SerializeField] private float weaponPosChangeSpeed;
    [SerializeField] private float weaponRotChangeSpeed;
    [SerializeField] private List<WeaponTransformData> weaponTransformData;

    [Header("Grenade")]
    [SerializeField] private Transform grenadeSpawnT;
    [SerializeField] private int carryingGrenade;
    [SerializeField] private int maxGrenade;

    private Player player;

    public static Action<Weapon> EquipWeapon;
    public static Action<int, bool> UpdateGrenadeCount;

    private void OnEnable()
    {
        EnemySpawner.NewWaveStarted += ResetWeapons;

        EquipWeapon += SetWeaponEquiped;
    }

    private void OnDisable()
    {
        EnemySpawner.NewWaveStarted -= ResetWeapons;

        EquipWeapon -= SetWeaponEquiped;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(player == null)
        {
            return;
        }

        if(!player.IsPlayerActive)
        {
            return;
        }

        ThrowGrenade();

        if (currentEquipedWeapon == null)
        {
            return;
        }

        UpdateEquipWeaponTransform();

        ActivateWeapon();
        Reload();

        UnequipWeapon();
    }

    public void SetUp(Player player)
    {
        this.player = player;

        carryingWeapons = new List<Weapon>();

        GameplayMenu.EnablePrimaryWeaponUi(false, true);
        SetUpGrenade();
    }

    #region Equip/Unequip
    private void SetWeaponEquiped(Weapon weapon)
    {
        if(currentEquipedWeapon == weapon)
        {
            return;
        }

        currentEquipedWeaponType = weapon.GetWeaponType();
        currentEquipedWeapon = weapon;

        weapon.Equip(player);
        weapon.SetNewParent(weaponEquipParent);

        if (!carryingWeapons.Contains(weapon))
        {
            carryingWeapons.Add(weapon);
        }

        UpdateEquipWeaponTransform(true);
    }

    private void UnequipWeapon()
    {
        if (player.userInput.UnequipWeapon)
        {
            if (currentEquipedWeapon == null)
            {
                return;
            }

            if (carryingWeapons.Contains(currentEquipedWeapon))
            {
                carryingWeapons.Remove(currentEquipedWeapon);
            }

            currentEquipedWeapon.transform.SetParent(null);

            currentEquipedWeapon.Unequip(currentEquipedWeapon.transform.forward * (GetUnEquipingThrow().z  + player.playerMovement.MoveSpeedFinal / 2)
                                        + currentEquipedWeapon.transform.up * GetUnEquipingThrow().y +
                                        currentEquipedWeapon.transform.right * GetUnEquipingThrow().x);

            GameplayMenu.EnableShootMarker?.Invoke(false);

            GameplayMenu.EnablePrimaryWeaponUi(false, true);

            currentEquipedWeapon = null;
        }
    }

    private void UpdateEquipWeaponTransform(bool isInstantUpdation = false)
    {
        if (currentEquipedWeapon == null)
        {
            return;
        }

        for (int i = 0; i < weaponTransformData.Count; i++)
        {
            if (currentEquipedWeaponType == weaponTransformData[i].weaponType)
            {
                if (isInstantUpdation)
                {
                    currentEquipedWeapon.transform.localPosition = weaponTransformData[i].equipLocalPos;
                    currentEquipedWeapon.transform.localRotation = Quaternion.Euler(weaponTransformData[i].equipLocalRot);
                }
                else
                {
                    currentEquipedWeapon.transform.localPosition = Vector3.MoveTowards(currentEquipedWeapon.transform.localPosition,
                                                                                        weaponTransformData[i].equipLocalPos,
                                                                                        weaponPosChangeSpeed * Time.unscaledDeltaTime);

                    currentEquipedWeapon.transform.localRotation = Quaternion.Lerp(currentEquipedWeapon.transform.localRotation,
                                                                                        Quaternion.Euler(weaponTransformData[i].equipLocalRot),
                                                                                        weaponRotChangeSpeed * Time.unscaledDeltaTime);
                }
            }
        }
    }

    private Vector3 GetUnEquipingThrow()
    {
        for (int i = 0; i < weaponTransformData.Count; i++)
        {
            if (weaponTransformData[i].weaponType == currentEquipedWeapon.GetWeaponType())
            {
                return weaponTransformData[i].unEquipingThrowForce;
            }
        }

        return Vector3.zero;
    }
    #endregion

    #region Attack
    private void ActivateWeapon()
    {
        currentEquipedWeapon.ActivateWeapon(player.userInput.MainAttackInput);

        if(currentEquipedWeapon.GetWeaponType() == WeaponType.Rifle)
        {
            GameplayMenu.EnablePrimaryWeaponUi(true, true);
        }
    }

    private void Reload()
    {
        currentEquipedWeapon.TryReloading(player.userInput.Reload);
    }

    public void AddAmmo(int amt)
    {
        currentEquipedWeapon.AddAmmo(amt);
    }
    #endregion

    #region Grenade
    private void SetUpGrenade()
    {
        carryingGrenade = maxGrenade;

        GameplayMenu.EnableBombUi(carryingGrenade > 0);
        UpdateGrenadeCount?.Invoke(carryingGrenade, carryingGrenade <= 3);
    }

    private void ThrowGrenade()
    {
        if(player.userInput.GrenadeInput && carryingGrenade > 0)
        {
            GameObject obj = player.objectPooler.SpawnFormPool("Grenade", grenadeSpawnT.position, Quaternion.identity);
            obj.GetComponent<Grenade>().ActivateGrenade(player.shootingManager.IsHit, weaponEquipParent.forward, player.shootingManager.HitPoint, player.playerMovement.MoveSpeedFinal);

            carryingGrenade--;

            if(carryingGrenade < 0)
            {
                carryingGrenade = 0;
            }

            UpdateGrenadeCount?.Invoke(carryingGrenade, carryingGrenade <= 3);

            SoundManager.PlayAudio("grenade throw", true, true);
        }
    }

    public void AddGrenade(int amt)
    {
        carryingGrenade += amt;
        UpdateGrenadeCount?.Invoke(carryingGrenade, carryingGrenade <= 3);
    }
    #endregion

    private void ResetWeapons()
    {
        SetUpGrenade();

        if(currentEquipedWeapon)
        {
            currentEquipedWeapon.SetUpAmmo();
        }
    }
}
