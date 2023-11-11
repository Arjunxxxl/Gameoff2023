using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayMenu : MonoBehaviour
{
    [Header("Crosshair")]
    [SerializeField] private TweenUtil hitIndicator;

    public static Action<bool> EnableShootMarker;

    private void OnEnable()
    {
        EnableShootMarker += OnEnableHitIndicator;
    }

    private void OnDisable()
    {
        EnableShootMarker -= OnEnableHitIndicator;
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

    public void SetUp()
    {
        OnEnableHitIndicator(false);
    }

    private void OnEnableHitIndicator(bool enable)
    {
        hitIndicator.gameObject.SetActive(enable);
        
        if(enable)
        {
            hitIndicator.PlayTween("Scale Up");
        }
    }
}
