using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private Transform cameraT;

    [Header("Shake Data")]
    [SerializeField] private float shakeDuration;
    [SerializeField] private float positionStrength;
    [SerializeField] private float rotationStrength;
    [SerializeField] private int vibrato;
    [SerializeField] private float randomness;
    [SerializeField] private bool snapping;
    [SerializeField] private bool fadeout;
    [SerializeField] private ShakeRandomnessMode shakeRandomnessMode;

    public static Action<float, float> ShakeCamera;

    private void OnEnable()
    {
        ShakeCamera += OnShakeCamera;
    }

    private void OnDisable()
    {
        ShakeCamera -= OnShakeCamera;
    }

    private void OnShakeCamera(float strengthMultiplyer, float durationMultiplyer)
    {
        cameraT.DOComplete();
        cameraT.DOShakePosition(shakeDuration * durationMultiplyer, positionStrength * strengthMultiplyer, vibrato, randomness, snapping, fadeout, shakeRandomnessMode).SetUpdate(true);
        cameraT.DOShakeRotation(shakeDuration * durationMultiplyer, rotationStrength * strengthMultiplyer, vibrato, randomness, fadeout, shakeRandomnessMode).SetUpdate(true);
    }
}
