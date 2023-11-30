using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using static Unity.Burst.Intrinsics.X86.Avx;

public class PostProcessingManager : MonoBehaviour
{
    [Header("Lens Distortion")]
    [SerializeField] private float currentLensDistortion;
    [SerializeField] private float dashLensDistortion;
    [SerializeField] private float normalLensDistortion;
    [SerializeField] private float lensDistortionChangeSpeed;
    LensDistortion lensDistortion;

    [Header("Bloom")]
    [SerializeField] private float currentBloom;
    [SerializeField] private float normalBloom;
    [SerializeField] private float timeSlowBloom;
    [SerializeField] private float bloomChangeSpeed;
    Bloom bloom;

    [Header("Chromatic Aberration")]
    [SerializeField] private float currentChromaticAberration;
    [SerializeField] private float normalChromaticAberration;
    [SerializeField] private float timeSlowChromaticAberration;
    [SerializeField] private float chromaticAberrationChangeSpeed;
    ChromaticAberration chromaticAberration;

    [Header("Vignette")]
    [SerializeField] private float currentVignette;
    [SerializeField] private float normalVignette;
    [SerializeField] private float timeSlowCVignette;
    [SerializeField] private float vignetteChangeSpeed;
    Vignette vignette;

    private Volume volume;

    #region SingleTon
    public static PostProcessingManager Instance;
    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        volume = GetComponent<Volume>();

        volume.profile.TryGet<LensDistortion>(out lensDistortion);
        volume.profile.TryGet<Bloom>(out bloom);
        volume.profile.TryGet<ChromaticAberration>(out chromaticAberration);
        volume.profile.TryGet<Vignette>(out vignette);

        SetLensDistortion(false, true);
        SetBloom(false, true);
        SetChromaticAberration(false, true);
        SetVignette(false, true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Lens distortion
    public void SetLensDistortion(bool isDash, bool isInstant)
    {
        if(isInstant)
        {
            currentLensDistortion = isDash ? dashLensDistortion : normalLensDistortion;
        }
        else
        {
            currentLensDistortion = Mathf.Lerp(currentLensDistortion, isDash ? dashLensDistortion : normalLensDistortion, 1 - Mathf.Pow(0.5f, Time.unscaledDeltaTime * lensDistortionChangeSpeed));
        }

        lensDistortion.intensity.value = currentLensDistortion;
    }
    #endregion

    #region Bloom
    public void SetBloom(bool isTimeSlowed, bool isInstant)
    {
        if(isInstant)
        {
            currentBloom = isTimeSlowed ? timeSlowBloom : normalBloom;
        }
        else
        {
            currentBloom = Mathf.Lerp(currentBloom, isTimeSlowed ? timeSlowBloom : normalBloom, 1 - Mathf.Pow(0.5f, Time.unscaledDeltaTime * bloomChangeSpeed));
        }

        bloom.intensity.value = currentBloom;
    }
    #endregion

    #region Chromatic Aberration
    public void SetChromaticAberration(bool isTimeSlowed, bool isInstant)
    {
        if(isInstant)
        {
            currentChromaticAberration = isTimeSlowed ? timeSlowChromaticAberration : normalChromaticAberration;
        }
        else
        {
            currentChromaticAberration = Mathf.Lerp(currentChromaticAberration, isTimeSlowed ? timeSlowChromaticAberration : normalChromaticAberration, 1 - Mathf.Pow(0.5f, Time.unscaledDeltaTime * chromaticAberrationChangeSpeed));
        }

        chromaticAberration.intensity.value = currentChromaticAberration;
    }
    #endregion

    #region Vignette
    public void SetVignette(bool isTimeSlowed, bool isInstant)
    {
        if(isInstant)
        {
            currentVignette = isTimeSlowed ? timeSlowCVignette : normalVignette;
        }
        else
        {
            currentVignette = Mathf.Lerp(currentVignette, isTimeSlowed ? timeSlowCVignette : normalVignette, 1 - Mathf.Pow(0.5f, Time.unscaledDeltaTime * vignetteChangeSpeed));
        }

        vignette.intensity.value = currentVignette;
    }
    #endregion
}
