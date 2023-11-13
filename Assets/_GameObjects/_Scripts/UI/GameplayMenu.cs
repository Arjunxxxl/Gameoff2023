using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class GameplayMenu : MonoBehaviour
{
    [Header("Crosshair")]
    [SerializeField] private TweenUtil hitIndicator;

    [Header("Panels")]
    [SerializeField] private GameObject primaryWeaponParent;
    [SerializeField] private GameObject bombParent;

    [Header("Texts")]
    [SerializeField] private TMP_Text currentAmmoTxt;
    [SerializeField] private TMP_Text carryingAmmoTxt;
    [SerializeField] private TMP_Text bombTxt;
    [SerializeField] private TMP_Text hpTxt;

    [Header("Text Color")]
    [SerializeField] private Color ammoNormalColor;
    [SerializeField] private Color ammoLowColor;

    [Header("Energy Slider")]
    [SerializeField] private Slider energySlider;

    [Header("Animations")]
    [SerializeField] private TweenUtil currentAmmoTween;
    [SerializeField] private TweenUtil bombTxtTween;

    public static Action<bool> EnableShootMarker;
    public static Action<bool, bool> EnablePrimaryWeaponUi;
    public static Action<bool> EnableBombUi;

    private void OnEnable()
    {
        Weapon.UpdateCurrentAmmo += UpdateCurrentAmmo;
        Weapon.UpdateCarryingAmmo += UpdateCarryingAmmo;
        WeaponHandler.UpdateGrenadeCount += UpdateGrenadeCount;

        TimeController.UpdateEnergy += UpdateEnergySlider;

        PlyaerHp.UpdateCurrentHp += UpdateHp;

        EnableShootMarker += OnEnableHitIndicator;
        EnablePrimaryWeaponUi += OnEnablePrimaryWeaponUi;
        EnableBombUi += OnEnableBombUi;
    }

    private void OnDisable()
    {
        Weapon.UpdateCurrentAmmo -= UpdateCurrentAmmo;
        Weapon.UpdateCarryingAmmo -= UpdateCarryingAmmo;
        WeaponHandler.UpdateGrenadeCount -= UpdateGrenadeCount;

        TimeController.UpdateEnergy -= UpdateEnergySlider;

        PlyaerHp.UpdateCurrentHp -= UpdateHp;

        EnableShootMarker -= OnEnableHitIndicator;
        EnablePrimaryWeaponUi -= OnEnablePrimaryWeaponUi;
        EnableBombUi -= OnEnableBombUi;
    }

    // Start is called before the first frame update
    void Start()
    {
        SetUp();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region SetUp
    public void SetUp()
    {
        OnEnableHitIndicator(false);
    }
    #endregion

    #region Crosshair
    private void OnEnableHitIndicator(bool enable)
    {
        hitIndicator.gameObject.SetActive(enable);
        
        if(enable)
        {
            hitIndicator.PlayTween("Scale Up");
        }
    }
    #endregion

    #region Panels
    private void OnEnablePrimaryWeaponUi(bool enable, bool isAmmoWeapon)
    {
        primaryWeaponParent.SetActive(enable);

        currentAmmoTxt.gameObject.SetActive(isAmmoWeapon);
        carryingAmmoTxt.gameObject.SetActive(isAmmoWeapon);
    }

    private void OnEnableBombUi(bool enable)
    {
        bombParent.SetActive(enable);
    }
    #endregion

    #region Ammo Txt
    private void UpdateCurrentAmmo(int value, bool isLowAmmo)
    {
        currentAmmoTween.PlayTween("Scale Up");
        currentAmmoTxt.text = value.ToString();

        currentAmmoTxt.color = isLowAmmo ? ammoLowColor : ammoNormalColor;
    }

    private void UpdateCarryingAmmo(int value, bool isLowAmmo)
    {
        carryingAmmoTxt.text = "(" + value.ToString() + ")";

        carryingAmmoTxt.color = isLowAmmo ? ammoLowColor : ammoNormalColor;
    }
    #endregion

    #region Grenade
    private void UpdateGrenadeCount(int value, bool isLowAmmo)
    {
        bombTxtTween.PlayTween("Scale Up");
        bombTxt.text = value.ToString();

        bombTxt.color = isLowAmmo ? ammoLowColor : ammoNormalColor;
    }
    #endregion

    #region Energy Slider
    private void UpdateEnergySlider(float sliderValue, float sliderMinValue, float sliderMaxValue)
    {
        energySlider.minValue = sliderMinValue;
        energySlider.maxValue = sliderMaxValue;
        energySlider.value = sliderValue;
    }
    #endregion

    #region Hp
    private void UpdateHp(int hpLeft)
    {
        hpTxt.text = hpLeft.ToString();
    }
    #endregion
}
