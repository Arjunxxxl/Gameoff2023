using System.Collections;
using System.Collections.Generic;
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
        [Space]
        public Vector3 equipLocalRot;
        [Space]
        public Vector3 unequipLocalPos;
        [Space]
        public Vector3 unequipLocalRot;
    }

    [Header("Weapon Carrying Data")]
    [SerializeField] private WeaponType currentEquipedWeaponType;
    [SerializeField] private Weapon currentEquipedWeapon;
    [SerializeField] private List<WeaponType> carryingWeapons;

    [Header("Weapon Equip/Unequip Data")]
    [SerializeField] private Transform weaponEquipParent;
    [SerializeField] private Transform weaponUnequipParent;

    [Header("Weapon Transfrom Data")]
    [SerializeField] private float weaponPosChangeSpeed;
    [SerializeField] private float weaponRotChangeSpeed;
    [SerializeField] private List<WeaponTransformData> weaponTransformData;

    private UserInput userInput;
    private PlayerCollisionDetection playerCollisionDetection;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        ActivateWeapon();
        UpdateEquipWeaponTransform();
    }

    public void SetUp(Player player, UserInput userInput, PlayerCollisionDetection playerCollisionDetection)
    {
        this.userInput = userInput;
        this.playerCollisionDetection = playerCollisionDetection;

        carryingWeapons = new List<WeaponType>();

        SetWeaponEquiped(player, currentEquipedWeapon);
        UpdateEquipWeaponTransform(true);
    }

    #region Equip/Unequip
    private void SetWeaponEquiped(Player player, Weapon weapon)
    {
        currentEquipedWeaponType = weapon.GetWeaponType();
        currentEquipedWeapon = weapon;

        weapon.SetUp(player);
        weapon.SetNewParent(weaponEquipParent);

        carryingWeapons.Add(weapon.GetWeaponType());
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
                                                                                        weaponPosChangeSpeed * Time.deltaTime);

                    currentEquipedWeapon.transform.localRotation = Quaternion.Lerp(currentEquipedWeapon.transform.localRotation,
                                                                                        Quaternion.Euler(weaponTransformData[i].equipLocalRot),
                                                                                        weaponRotChangeSpeed * Time.deltaTime);
                }
            }
        }
    }
    #endregion

    #region Attack
    private void ActivateWeapon()
    {
        currentEquipedWeapon.ActivateWeapon(userInput.MainAttackInput);
    }
    #endregion
}
