using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    [Header("Time Scale")]
    [SerializeField] private bool slowTime;
    [SerializeField] private float currentTimeScale;
    [SerializeField] private float normalTimeScale;
    [SerializeField] private float stopingTimeScale;
    [SerializeField] private float fixedDeltaTimeDefaultValue;
    [SerializeField] private float timeScaleChangeSpeed;

    [Header("Time Slow Energy")]
    [SerializeField] private bool isNoEnergyLeft;
    [SerializeField] private float maxEnergy;
    [SerializeField] private float energyBurnRate;
    [SerializeField] private float energyGenerateRate;
    [SerializeField] private float energLeft;

    private Player player;

    public static Action<float, float, float> UpdateEnergy;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            return;
        }

        if (!player.IsPlayerActive)
        {
            return;
        }

        ChangeTimeScale();
        BurnEnergy();
        GenerateEnergy();
    }

    public void SetUp(Player player)
    {
        this.player = player;

        currentTimeScale = normalTimeScale;
        Time.timeScale = currentTimeScale;
        Time.fixedDeltaTime = Time.timeScale * fixedDeltaTimeDefaultValue;

        energLeft = maxEnergy;
        isNoEnergyLeft = false;
    }

    #region Time Slow
    private void ChangeTimeScale()
    {
        slowTime = player.userInput.TimeScaleInput;

        currentTimeScale = Mathf.Lerp(currentTimeScale, slowTime && !isNoEnergyLeft ? stopingTimeScale : normalTimeScale, 1f - Mathf.Pow(0.5f, Time.unscaledDeltaTime * timeScaleChangeSpeed));
        
        if(slowTime && !isNoEnergyLeft)
        {
            if(Mathf.Abs(currentTimeScale - stopingTimeScale) < 0.01f)
            {
                currentTimeScale = stopingTimeScale;
            }
        }
        else
        {
            if (Mathf.Abs(currentTimeScale - normalTimeScale) < 0.01f)
            {
                currentTimeScale = normalTimeScale;
            }
        }

        PostProcessingManager.Instance.SetBloom(slowTime, false);
        PostProcessingManager.Instance.SetChromaticAberration(slowTime, false);
        PostProcessingManager.Instance.SetVignette(slowTime, false);

        Time.timeScale = currentTimeScale;
        Time.fixedDeltaTime = Time.timeScale * fixedDeltaTimeDefaultValue;
    }
    #endregion

    #region Time Slow Energy
    private void BurnEnergy()
    {
        if (slowTime && energLeft > 0)
        {
            energLeft -= Time.unscaledDeltaTime * energyBurnRate;

            isNoEnergyLeft = energLeft <= 0;

            if (energLeft <= 0)
            {
                energLeft = 0;
            }

            UpdateEnergy?.Invoke(energLeft, 0, maxEnergy);
        }
    }

    private void GenerateEnergy()
    {
        if (!slowTime && energLeft < maxEnergy)
        {
            energLeft += Time.unscaledDeltaTime * energyGenerateRate;

            isNoEnergyLeft = energLeft <= 0;

            if (energLeft >= maxEnergy)
            {
                energLeft = maxEnergy;
            }

            UpdateEnergy?.Invoke(energLeft, 0, maxEnergy);
        }
    }
    #endregion
}
