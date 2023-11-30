using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [Header("Time Survived")]
    [SerializeField] private bool startTimer;
    [SerializeField] private float timeSurviving;

    private void OnEnable()
    {
        GameManager.GameSetUp += SetUp;
        GameManager.GameSetCompleted += StartTimer;

        Player.PlayerDied += SaveTimeSurvived;
    }

    private void OnDisable()
    {
        GameManager.GameSetUp -= SetUp;
        GameManager.GameSetCompleted -= StartTimer;

        Player.PlayerDied -= SaveTimeSurvived;
    }

    private void Update()
    {
        UpdateTimeSurvining();
    }

    private void SetUp()
    {
        timeSurviving = 0;
        startTimer = false;

        GameplayMenu.UpdateTimeSurvivingTxt?.Invoke(timeSurviving);
    }

    private void StartTimer()
    {
        timeSurviving = 0;
        startTimer = true;

        GameplayMenu.UpdateTimeSurvivingTxt?.Invoke(timeSurviving);
    }

    private void UpdateTimeSurvining()
    {
        if (startTimer)
        {
            timeSurviving += Time.deltaTime;

            GameplayMenu.UpdateTimeSurvivingTxt?.Invoke(timeSurviving);
        }
    }

    private void SaveTimeSurvived()
    {
        LocalDataManager.Instance.SetTimeSurvived(timeSurviving);
    }
}
