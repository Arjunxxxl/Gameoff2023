using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    [Header("Canavs")]
    [SerializeField] private Canvas loadingMenu;
    [SerializeField] private Canvas gameplayMenu;
    [SerializeField] private Canvas pauseMenu;

    #region SingleTon
    public static UiManager Instance;
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
        
    }

    public void ShowLoadingScreen()
    {
        Cursor.lockState = CursorLockMode.Locked;

        loadingMenu.enabled = true;
        gameplayMenu.enabled = false;
        pauseMenu.enabled = false;
    }

    public void ShowGameplayScreen()
    {
        Cursor.lockState = CursorLockMode.Locked;

        loadingMenu.enabled = false;
        gameplayMenu.enabled = true;
        pauseMenu.enabled = false;
    }

    public void ShowPauseScreen()
    {
        Cursor.lockState = CursorLockMode.None;

        loadingMenu.enabled = false;
        gameplayMenu.enabled = false;
        pauseMenu.enabled = true;
    }
}
