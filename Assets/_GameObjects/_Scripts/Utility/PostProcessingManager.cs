using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using static Unity.Burst.Intrinsics.X86.Avx;

public class PostProcessingManager : MonoBehaviour
{
    [Header("Lens Distortion")]
    LensDistortion lensDistortion;
    [SerializeField] private float currentLensDistortion;
    [SerializeField] private float dashLensDistortion;
    [SerializeField] private float normalLensDistortion;
    [SerializeField] private float lensDistortionChangeSpeed;

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

        SetLensDistortion(false, true);
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
}
