using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalDataManager : MonoBehaviour
{
    [SerializeField] private string MOUSE_SENSITIVITY = "MOUSE_SENSITIVITY";
    [SerializeField] private string TIME_SURVIVED = "TIME_SURVIVED";

    [SerializeField] private float mouseSensitivity;
    [SerializeField] private float timeSurvived;

    public float MouseSensitivity { get { return mouseSensitivity; } }
    public float TimeSurvived { get { return timeSurvived; } }

    #region SingleTon
    public static LocalDataManager Instance;
    private void Awake()
    {
        if(!Instance)
        {
            Instance = this;
        }
        else if(Instance != this)
        {
            Destroy(gameObject);
        }

        LoadAlData();

        DontDestroyOnLoad(gameObject);
    }
    #endregion

    public void LoadAlData()
    {
        GetMouseSensitivity();
        GetTimeSurvived();
    }

    #region Mouse Sensivity
    private void GetMouseSensitivity()
    {
        mouseSensitivity = PlayerPrefs.GetFloat(MOUSE_SENSITIVITY, 1);
    }

    public void SetMouseSensitivity(float val)
    {
        mouseSensitivity = val;
        PlayerPrefs.SetFloat(MOUSE_SENSITIVITY, mouseSensitivity);
    }
    #endregion

    #region Time Survived
    private void GetTimeSurvived()
    {
        timeSurvived = PlayerPrefs.GetFloat(TIME_SURVIVED, 0);
    }

    public void SetTimeSurvived(float val)
    {
        timeSurvived = val;
        PlayerPrefs.SetFloat(TIME_SURVIVED, timeSurvived);
    }
    #endregion
}
